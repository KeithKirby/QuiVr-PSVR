using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Analytics;
using System.Collections.Generic;

public enum BowOrientation
{
    Vertical,
    Horizontal
}

public class BowAim : MonoBehaviour {
    public float powerMultiplier;
	public AnimationCurve powerCurve;
    public AnimationCurve oldPowerCurve;
	float currentPower;
    public float pullMultiplier;
    public float pullOffset;
    public float maxPullDistance = 1.1f;
    public float AnimMult;
    public int bowVibration = 250;
    public int stringVibration = 350;

    public SmoothFollow follow;
    public bool WSASnap = true;

    public GameObject LeftHand;
    public GameObject RightHand;

    public BowOrientation BowOrient;

    private BowAnimation bowAnimation;
    private BowHandle handle;
    private BowAudio Baudio;

    private VRTK_InteractableObject interact;

    private SteamVR_ControllerManager controllers;
    private VRTK_ControllerEvents holdControl;
    private VRTK_ControllerEvents stringControl;

    public VRTK_ControllerActions stringActions;
    [HideInInspector]
    public VRTK_ControllerActions holdActions;

    public Quaternion releaseRotation;
    public Quaternion baseRotation = Quaternion.identity;
    private bool fired;
    private float fireOffset;
    private float currentPull;
    private float previousPull;

    public Transform Display;
    public Vector3 DisplayOffsetLeft;
    public Vector3 DisplayOffsetRight;
    public Vector3 DOff;
    public Vector3 DrawOffset;

    GameObject DrawHand;
    Vector3 lastHandPos;

    Transform drawAim;

    [HideInInspector]
    public static BowAim instance;

    Queue<float> ArrowsSecond;

    float timeBowMult = 0;

    [Header("Runtime Values")]
    public GameObject currentArrow;

    void Awake()
    {
        instance = this;
        bowAnimation = GetComponent<BowAnimation>();
        Baudio = GetComponent<BowAudio>();
        handle = GetComponentInChildren<BowHandle>();
        controllers = SteamVR_ControllerManager.instance;//
        if(controllers == null)
            controllers = FindObjectOfType<SteamVR_ControllerManager>();
        interact = GetComponent<VRTK_InteractableObject>();
        interact.InteractableObjectGrabbed += new InteractableObjectEventHandler(DoObjectGrab);
        drawAim = new GameObject().transform;
        drawAim.name = "AimObject";
        drawAim.transform.SetParent(transform.parent);
        ArrowsSecond = new Queue<float>();
    }

    public VRTK_ControllerEvents GetPullHand()
    {
        return stringControl;
    }

    public bool IsHeld()
    {
        return interact.IsGrabbed();
    }

    [HideInInspector]
    public bool OverrideHasArrow;
    public bool HasArrow()
    {
        return currentArrow != null || OverrideHasArrow;
    }

    public GameObject GetArrow()
    {
        return currentArrow;
    }

    public void SetArrow(GameObject arrow)
    {
        currentArrow = arrow;
        Quiver.instance.ResetCD();
        lastBowPos = transform.position;
        arrow.GetComponent<ArrowEffects>().SetNocked();
        Baudio.PlayNock();
        if (Settings.GetBool("expertNock", false))
            timeBowMult = 1;
        else
            timeBowMult = 0;
        AnimMult = Settings.GetFloat("BowPullMult", 4);
        if (stringControl != null)
            stringControl.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse(2200);
        fullTrigger = stringControl.GetTriggerAxis() >= .15f;
    }

    bool isLeft; 
    private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
    {
#if !UNITY_WSA
        if (e.interactingObject == controllers.left)
        {
            holdControl = controllers.left.GetComponent<VRTK_ControllerEvents>();
            stringControl = controllers.right.GetComponent<VRTK_ControllerEvents>();

            holdActions = controllers.left.GetComponent<VRTK_ControllerActions>();
            stringActions = controllers.right.GetComponent<VRTK_ControllerActions>();

            Display.transform.localScale = new Vector3(1, 1, 1);
            Display.transform.localPosition = DisplayOffsetLeft;
        }
        else if (e.interactingObject == controllers.right)
        {
            stringControl = controllers.left.GetComponent<VRTK_ControllerEvents>();
            holdControl = controllers.right.GetComponent<VRTK_ControllerEvents>();

            stringActions = controllers.left.GetComponent<VRTK_ControllerActions>();
            holdActions = controllers.right.GetComponent<VRTK_ControllerActions>();

            Display.transform.localScale = new Vector3(-1, 1, 1);
            Display.transform.localPosition = DisplayOffsetRight;
        }
        else
            return;
#else
        if (e.interactingObject == LeftHand)
        {
            holdControl = LeftHand.GetComponent<VRTK_ControllerEvents>();
            stringControl =RightHand.GetComponent<VRTK_ControllerEvents>();

            holdActions = LeftHand.GetComponent<VRTK_ControllerActions>();
            stringActions = RightHand.GetComponent<VRTK_ControllerActions>();

            Display.transform.localScale = new Vector3(1, 1, 1);
            Display.transform.localPosition = DisplayOffsetLeft;
        }
        else if (e.interactingObject == RightHand)
        {
            stringControl = LeftHand.GetComponent<VRTK_ControllerEvents>();
            holdControl = RightHand.GetComponent<VRTK_ControllerEvents>();

            stringActions = LeftHand.GetComponent<VRTK_ControllerActions>();
            holdActions = RightHand.GetComponent<VRTK_ControllerActions>();

            Display.transform.localScale = new Vector3(-1, 1, 1);
            Display.transform.localPosition = DisplayOffsetRight;
        }
        else
            return;
#endif
        SteamVR_ControllerManager.freeHand = stringControl.gameObject;
        SteamVR_ControllerManager.bowHand = holdControl.gameObject;
        drawHandObj = stringControl.GetComponentInChildren<PullAnchor>().transform;
        DrawHand = stringActions.gameObject.GetComponentInChildren<HandAnim>().gameObject;
        StartCoroutine("GetBaseRotation");
    }

    bool FingersOff()
    {
        KnucklesHandControl pos = stringControl.GetComponent<KnucklesHandControl>();
        if (pos != null)
        {
            KnucklesHandPose pose = pos.handPose;
            if (!pos.touchingTrigger() || pose.middle_curl > 0.4f)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator GetBaseRotation()
    {
        yield return new WaitForEndOfFrame();
        releaseRotation = transform.localRotation;
        baseRotation = transform.localRotation;
        wantBowPos = transform.localPosition;
        follow.Target = transform.parent;
    }
    Vector3 pOffset;
    float lastShot;
	Vector3 prevPos;
	Vector3 addVel;
    bool fullTrigger;
    private void Update()
    {
        CheckHandAnim();
		addVel = (transform.position - prevPos) / Time.deltaTime;
        if (!fullTrigger && currentArrow != null && stringControl.GetTriggerAxis() > 0.15f)
            fullTrigger = true;
        if (currentArrow != null && IsHeld())
        {
            timeBowMult += Time.unscaledDeltaTime * 3.5f;
            if (timeBowMult > 1)
                timeBowMult = 1;
#if !UNITY_WSA
            AimArrow();
            AimBow();
#endif
            PullString();
            DrawHand.GetComponent<HandAnim>().drawing = true;
            if ((stringControl.GetTriggerAxis() < 0.15f && fullTrigger) || (!fullTrigger && KnucklesHandControl.UsingFingers && FingersOff()))
            {
                currentArrow.GetComponent<Arrow>().Fired();
                fired = true;
                releaseRotation = transform.localRotation;
                Release();
            }
        }
        else if (IsHeld())
        {
            if (fired)
            {
                transform.localPosition = wantBowPos;
                fired = false;
                fireOffset = Time.time;
                holdTime = 0;
                powerdArrow = false;
            }
            if(baseRotation != Quaternion.identity)
                transform.localRotation = Quaternion.Lerp(releaseRotation, baseRotation, (Time.time - fireOffset) * 8);
        }
        if (!IsHeld() && currentArrow != null)
        {
            Release();
        }
		prevPos = transform.position;
        if (ArrowsSecond.Count > 0 && ArrowsSecond.Peek() + 1 < Time.time)
            ArrowsSecond.Dequeue();
        if (ArrowsSecond.Count >= 8)
            Achievement.EarnAchievement("RAPID_FIRE");
    }

    private bool WSASnapping;

    private void LateUpdate()
    {
#if UNITY_WSA
        if (currentArrow != null && IsHeld())
        {
            uint cid = stringControl.controllerIndex;
            float stringLoss = MotionControllerInput.GetLoss(cid);
            WSASnapping = false;
            if (stringLoss > 0.5f && WSASnap)
            {
                Vector3 cheekPos = WSA_Player.NockPosition(cid);
                stringControl.transform.position = cheekPos;
                WSASnapping = true;
            }
            AimArrow();
            AimBow();
        }
#endif
    }


    void CheckHandAnim()
    {
        if(interact.IsGrabbed() && stringActions != null && DrawHand == null)
            DrawHand = stringActions.gameObject.GetComponentInChildren<HandAnim>().gameObject;  
    }

    public float lastShotDelta()
    {
        return Time.time-lastShot;
    }

    private void Release()
    {
        fullTrigger = false;
        lastShot = Time.time;
        Statistics.AddValue("ArrowsFired", 1f);
        bowAnimation.SetFrame(0);
        Baudio.SetVolume(0);
        currentArrow.transform.SetParent(null);
        currentArrow.GetComponent<Arrow>().isMine = true;
        Collider[] arrowCols = currentArrow.GetComponentsInChildren<Collider>();
        Collider[] BowCols = GetComponentsInChildren<Collider>();
        foreach (var c in arrowCols)
        {
            c.enabled = true;
            foreach (var C in BowCols)
            {
                Physics.IgnoreCollision(c, C);
            }
        }
        currentArrow.GetComponent<Rigidbody>().isKinematic = false;
		float pullPower = powerCurve.Evaluate (currentPull/(maxPullDistance*SettingsMaxPull));
        if (WSASnapping)
            pullPower = 1f;
		Vector3 velocity = addVel + ((pullPower * (1 + pullPower)) * powerMultiplier * currentArrow.transform.TransformDirection(Vector3.forward));
        currentArrow.GetComponent<Rigidbody>().velocity = velocity;
        string disp = "";
        if (Armory.currentOutfit.Arrow != null)
            disp = Armory.currentOutfit.Arrow.ToString();
        if (GetComponentInParent<PlayerSync>() != null)
        {
            bool onFire = currentArrow.GetComponentInChildren<ArrowFire>().OnFire();
            EffectInstance[] effects = currentArrow.GetComponent<ArrowEffects>().effects.ToArray();
            GetComponentInParent<PlayerSync>().ShootArrow(currentArrow.transform.position, currentArrow.transform.rotation, currentArrow.GetComponent<Rigidbody>().velocity, onFire, disp, effects);
        }
        //currentArrow.GetComponent<ArrowDisplay>().ChangeDisplay(new ArmorOption(disp));
        Baudio.PlayTwang(pullPower*0.8f);
        currentArrow.GetComponent<Arrow>().inFlight = true;
        currentArrow.GetComponent<ArrowEffects>().StopAllCoroutines();
        currentArrow = null;
        if (holdControl != null)
            holdControl.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse((ushort)(ushort.MaxValue *pullPower), 0.2f, 0.1f);
        if (stringControl != null)
            stringControl.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse((ushort)(ushort.MaxValue * pullPower), 0.1f, 0.1f);
        currentPull = 0;
        arrowsShot++;
        ReleaseArrow();
        if (stringControl != null)
        {
            stringControl.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
            DrawHand.GetComponent<HandAnim>().drawing = false;
            //DrawHand.GetComponent<HandAnim>().ResetPosition();
        }
        DrawHand.GetComponentInParent<CalibrateHand>().transform.localPosition = DrawHand.GetComponentInParent<CalibrateHand>().GetBasePos();
        DrawHand.GetComponentInParent<CalibrateHand>().transform.localRotation = DrawHand.GetComponentInParent<CalibrateHand>().GetBaseRot();
        ArrowsSecond.Enqueue(Time.time);
    }

    ArrowFire fire;
    public bool arrowOnFire()
    {
        if(fire == null && currentArrow != null)
        {
            fire = currentArrow.GetComponentInChildren<ArrowFire>();
        }
        if(fire != null)
        {
            return fire.OnFire();
        }
        return false;
    }

    int arrowsShot = 0;
    public int GetArrowsShot()
    {
        return arrowsShot;
    }

    void SendArrowNetwork(Vector3 p, Quaternion r, Vector3 v)
    {

    }

    private void ReleaseArrow()
    {
        if (stringControl.gameObject.GetComponent<VRTK_InteractGrab>())
        {
            stringControl.gameObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
        }
    }

    private void AimArrow()
    {
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.LookAt(handle.nockSide.position);
    }

    public void PowerupArrow()
    {
        if(currentArrow != null)
        {
            powerdArrow = true;
            currentArrow.GetComponent<Arrow>().Powerup();
        }
    }

    public void Depower()
    {
        if (currentArrow != null)
        {
            holdTime = 0;
            powerdArrow = false;
            currentArrow.GetComponent<Arrow>().Depower();
        }
    }

    Transform drawHandObj;
    Vector3 offset;

    public Transform DrawHandCalib;
    private void AimBow()
    {
        if (DrawHand == null)
        {
            DrawHand = SteamVR_ControllerManager.freeHand.GetComponentInChildren<HandAnim>().gameObject;
#if UNITY_WSA
            DrawHand = WSA_Player.DrawHand();
#endif
        }
        CalibrateHand ch = DrawHand.GetComponentInParent<CalibrateHand>();
        if (DrawHand == null || ch == null)
            return;
        Transform toMove = ch.transform; // Was DrawHand.transform;
        Vector3 drawPos = Vector3.zero;
        if (drawHandObj != null)
            drawPos = drawHandObj.transform.position;
        else if (DrawHand != null)
            drawPos = toMove.position; //DrawHand.transform.position;
        if (holdControl == null || stringControl == null)
            return;
        Vector3 aimDir = (holdControl.transform.position - drawPos).normalized;
        offset = Vector3.zero;
        if(!Settings.HasKey("drawOffset") || Settings.GetBool("drawOffset"))
        {
            offset = DrawOffset;
            if (holdControl.gameObject != controllers.left.gameObject)
            {
                offset.x *= -1f;
            }
            float yval = Mathf.Abs(aimDir.y);
            if (yval > 0.95f)
            {
                offset *= 1f - Mathf.Abs(((0.95f - yval) * 20f));
            }
        }
        Vector3 sPos = drawPos + drawAim.transform.TransformDirection(offset)*Mathf.Clamp(currentPull/(maxPullDistance*SettingsMaxPull), 0, 1f);
        drawAim.position = drawPos;
        drawAim.rotation = Quaternion.LookRotation(holdControl.transform.position - drawPos, PlayerHolder.Up()); ;

        //Filtering

        //String Hand
        //Quaternion stringRotation = Quaternion.LookRotation(holdControl.transform.position - sPos, holdControl.transform.TransformDirection(-1 * Vector3.forward));
        Quaternion stringRotation;
        if (BowOrientation.Vertical == BowOrient)
            stringRotation = Quaternion.LookRotation(holdControl.transform.position - sPos, holdControl.transform.TransformDirection(-1 * Vector3.forward));        
        else
            stringRotation = Quaternion.LookRotation(holdControl.transform.position - sPos, holdControl.transform.TransformDirection(Vector3.right));
        Vector3 stringPosition = handle.arrowNockingPoint.transform.position - toMove.transform.TransformDirection(DOff);

        transform.rotation = stringRotation;//Quaternion.Lerp(transform.rotation, stringRotation, (Time.deltaTime/Time.timeScale)*10f);
        toMove.position = stringPosition;//Vector3.SmoothDamp(toMove.position, stringPosition, ref stringVel, .05f);
        DrawHandCalib = toMove;
    }
    private Vector3 stringVel;
    private Vector3 bowVel;
    private Vector3 wantBowPos;
    private Vector3 lastBowPos;

    public AnimationCurve PullCurve;
    float SettingsMaxPull = 1;
    float holdTime;
    bool powerdArrow;
    private void PullString()
    {
        SettingsMaxPull = 1;
        if (Settings.HasKey("DrawLength"))
            SettingsMaxPull = Settings.GetFloat("DrawLength");
        currentPull = Mathf.Clamp((Vector3.Distance(holdControl.transform.position, stringControl.transform.position)-pullOffset) * pullMultiplier * timeBowMult, 0, maxPullDistance*SettingsMaxPull);
        float perc = PullCurve.Evaluate(currentPull / maxPullDistance);
        float reqTime = TelePlayer.instance.PoweredUp() ? 0.8f: 1.2f;//0.5f : 1f;
        if (currentPull >= 0.5f)
        {
            holdTime += Time.deltaTime;
            if(!powerdArrow)
                currentArrow.GetComponent<Arrow>().ChargeProgress(holdTime / reqTime);
        }
        else if(powerdArrow)
            Depower();
        if (holdTime >= reqTime && !powerdArrow)
        {
            holdTime = 0;
            PowerupArrow();
        }
        bowAnimation.SetFrame(currentPull*AnimMult);
        if (Mathf.Abs(currentPull - previousPull) > 0.05f)
        {
            previousPull = currentPull;
            holdActions.TriggerHapticPulse((ushort)(bowVibration*perc));
            stringActions.TriggerHapticPulse((ushort)(stringVibration*perc));
            Baudio.Play(perc/2.4f, (0.7f+(0.2f*perc)));
        }
    }

    public void Destroy()
    {
        if(GameAnalytics.instance != null)
        {
            GameAnalytics.instance.SendArrowEvent(GetArrowsShot());
            Debug.Log("Sending arrows fired: " + GetArrowsShot());
        }
    }
}