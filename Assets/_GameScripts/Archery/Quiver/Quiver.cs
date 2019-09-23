using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Events;
public class Quiver : MonoBehaviour {

    public Transform Hips;
    SphereCollider[] TriggerPoints;
    public GameObject ArrowPrefab;
    GameObject snapPoint;
    SteamVR_ControllerManager mgr;

    public bool isMounted;

    bool triggersEnabled;
    Collider[] triggers;

    EtherialSwap ether;

    public static Quiver instance;
    public static float DrawCD = 0.75f;

    public UnityEvent OnGrabbedArrow;

    VRTK_InteractableObject obj;

    void Awake()
    {
        obj = GetComponent<VRTK_InteractableObject>();
    }

    float cd;
    IEnumerator Start()
    {
        instance = this;
        triggers = GetComponentsInChildren<Collider>();
        ether = GetComponent<EtherialSwap>();
        TriggerPoints = Hips.gameObject.GetComponentsInChildren<SphereCollider>();
        triggersEnabled = true;
        mgr = SteamVR_ControllerManager.instance;//FindObjectOfType<SteamVR_ControllerManager>();
        GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += new InteractableObjectEventHandler(GArrow);
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(OnGrab);
        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(TrySnap);
        yield return true;
        yield return true;
    }

    public void OnGrab(object sender, InteractableObjectEventArgs e)
    {
        isMounted = false;
        TogglePointVisibility(true, e.interactingObject);
        
    }

    public void ResetCD()
    {
        cd = 0;
    }

    void TogglePointVisibility(bool on, GameObject holder)
    {
        bool isLeft = (holder == mgr.left.gameObject);
        foreach(var v in TriggerPoints)
        {
            if(v != null)
            {
                v.gameObject.GetComponent<Collider>().enabled = true;
                v.gameObject.GetComponent<MeshRenderer>().enabled = true;
                v.GetComponent<MeshRenderer>().enabled = on;
                if (isLeft && v.transform.localPosition.x > 0)
                {
                    v.gameObject.GetComponent<Collider>().enabled = false;
                    v.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Mathf.Abs(transform.localScale.z));
                }
                else if (!isLeft && v.transform.localPosition.x < 0)
                {
                    v.gameObject.GetComponent<Collider>().enabled = false;
                    v.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -1 * Mathf.Abs(transform.localScale.z));
                }
            }   
        }
    }

    public void ReleaseSnap()
    {
        isMounted = false;
        transform.SetParent(null);
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void TrySnap(object sender, InteractableObjectEventArgs e)
    {
        StartCoroutine("Tsnap", e.interactingObject);
    }

    public void Mount(GameObject m)
    {
        isMounted = true;
        transform.SetParent(m.transform);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    IEnumerator Tsnap(GameObject holder)
    {
        yield return true;
        if (snapPoint != null)
        {
            isMounted = true;
            Settings.Set("HipMount", isHip(snapPoint));
            transform.SetParent(snapPoint.transform);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true;
            if(holder != null)
                holder.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse((ushort)1000);
        }
        else
            ReleaseSnap();
        TogglePointVisibility(false, holder);
    }

    bool isHip(GameObject o)
    {
        return o.name.Contains("Hip");
    }

    void FixedUpdate()
    {
        if(obj.IsTouched() && KnucklesHandControl.UsingFingers)
        {
            GameObject hand = obj.GetTouching();
            KnucklesHandControl pos = hand.GetComponent<KnucklesHandControl>();
            VRTK_InteractGrab grab = hand.GetComponent<VRTK_InteractGrab>();
            if(grab != null && grab.GetGrabbedObject() == null && pos != null)
            {
                KnucklesHandPose pose = pos.handPose;
                if (pos.touchingTrigger() && pose.middle_curl <= 0.2f )
                {
                    InteractableObjectEventArgs e = new InteractableObjectEventArgs();
                    e.interactingObject = grab.gameObject;
                    GArrow(grab, e);
                }
            }
        }
        if(triggersEnabled == false && arrowHand != null && !Disabled)
        {
            if(arrowHand.GetGrabbedObject() == null)
            {
                triggersEnabled = true;
                foreach(var v in triggers)
                {
                    v.gameObject.SetActive(true);
                }
            }
        }
        cd -= Time.fixedDeltaTime;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "QuiverHolder")
        {
            snapPoint = null;
        }
            
    }

    VRTK_InteractGrab arrowHand;

    ToggleMenu menu;

    private bool disabled;
    public bool Disabled 
    {
        get
        {
            return disabled;
        }
        set
        {
            VRTK_InteractableObject obj = GetComponent<VRTK_InteractableObject>();
            if(obj != null)
            {
                obj.isUsable = !value;
            }
            foreach (var v in triggers)
            {
                v.gameObject.SetActive(!value);
            }
            disabled = value;
        }
    }

    public void AllowHand(bool left)
    {
        if (left)
            GetComponent<VRTK_InteractableObject>().allowedTouchControllers = VRTK_InteractableObject.AllowedController.Left_Only;
        else
            GetComponent<VRTK_InteractableObject>().allowedTouchControllers = VRTK_InteractableObject.AllowedController.Right_Only;
    }

    public void TryGiveArrow(int[] effects=null)
    {
        if (BowAim.instance != null && BowAim.instance.HasArrow())
            return;
        if (SteamVR_ControllerManager.freeHand != null)
        {
            VRTK_InteractGrab grab = SteamVR_ControllerManager.freeHand.GetComponent<VRTK_InteractGrab>();
            if (grab != null && grab.GetGrabbedObject() == null)
            {
                VRTK_InteractTouch touch = grab.gameObject.GetComponent<VRTK_InteractTouch>();
                VRTK_InteractUse use = grab.gameObject.GetComponent<VRTK_InteractUse>();
                use.ForceStopUsing();
                touch.ForceStopTouching();
                GameObject arrow = CreateArrow(SteamVR_ControllerManager.freeHand);
                if (effects != null && effects.Length > 0 && !ArrowEffects.EffectsDisabled)
                {
                    ArrowEffects e = arrow.GetComponentInChildren<ArrowEffects>();
                    if(e != null)
                    {
                        foreach (int i in effects)
                        {
                            ItemEffect eff = ItemDatabase.GetEffect(i);
                            e.AddEffect(i, eff.Range.y, false, true);
                        }
                    }
                }
            }
        }
    }

    public void GArrow(object sender, InteractableObjectEventArgs e)
    { 
        if (BowAim.instance != null && BowAim.instance.HasArrow())
            return;
        if (cd > 0 || disabled)
            return;
        if(PhotonNetwork.inRoom)
            cd = DrawCD;
        StopCoroutine("GiveArrow");
        StartCoroutine("GiveArrow", e.interactingObject);
    }

    public IEnumerator GiveArrow(GameObject obj)
    {
        if (menu == null)
            menu = ToggleMenu.instance;
        if (menu != null && menu.isOpen())
        { }
        else
        {
            if (isMounted)
            {
                triggersEnabled = false;
                foreach (var v in triggers)
                {
                    v.gameObject.SetActive(false);
                }
            }
            GetComponent<VRTK_InteractableObject>().StopTouching(obj);
            arrowHand = obj.GetComponent<VRTK_InteractGrab>();
            obj.GetComponent<VRTK_InteractUse>().ForceStopUsing();
            yield return true;
            obj.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
            if (obj.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() == null)
            {
                GameObject newArrow = CreateArrow(obj);
                if (ether.isEtherial())
                {
                    yield return true;
					if(newArrow != null && newArrow.GetComponent<EtherialSwap>() != null)
                    	newArrow.GetComponent<EtherialSwap>().MakeEtherial();
                }
            }
        }
    }

    GameObject CreateArrow(GameObject hand)
    {
        GameObject newArrow;
        if (PhotonNetwork.inRoom)
            newArrow = (GameObject)PhotonNetwork.Instantiate("Base/Arrow", hand.transform.position, Quaternion.identity, 0);
        else
            newArrow = (GameObject)Instantiate(ArrowPrefab, hand.transform.position, Quaternion.identity);
        if(arrowHand == null)
            arrowHand = hand.GetComponent<VRTK_InteractGrab>();
        newArrow.GetComponentInChildren<ArrowDisplay>().Change();
        DrawChance(newArrow);
        hand.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse((ushort)2200);
        hand.GetComponent<VRTK_InteractTouch>().ForceTouch(newArrow);
        arrowHand.AttemptGrab();
        OnGrabbedArrow.Invoke();
        return newArrow;
    }

    public static void DrawChance(GameObject arrow)
    {
        if (ArrowEffects.EffectsDisabled)
            return;
        ArrowEffects e = arrow.GetComponentInChildren<ArrowEffects>();
        EffectInstance[] on = Armory.ArmorEffects().ToArray();
        var db = ItemDatabase.a.Effects;
        foreach(var v in on)
        {
            ItemEffect eff = ItemDatabase.GetEffect(v.EffectID);
            float chance = eff.StaticValue;
            if (eff.randomType == RandomType.RandomChance)
                chance = eff.VariableValue + v.EffectValue;
            if (ItemDatabase.GetEffect(v.EffectID).type == EffectType.DrawChance && Random.Range(0,100) < chance)
            {
                e.AddEffect(v.EffectID, v.EffectValue, false, true);
            }
            else if(ItemDatabase.GetEffect(v.EffectID).type == EffectType.Passive || ItemDatabase.GetEffect(v.EffectID).type == EffectType.MissChance)
            {
                e.AddEffect(v.EffectID, v.EffectValue, false, true);
            }
        }
        if(PowerupManager.powerID > 0)
        {
            PowerupManager.instance.numArrows--;
            e.AddEffect(PowerupManager.powerID, 0, false, true);
        }
        e.ApplyDrawEffects();
        e.ApplyArrowMods();
    }
}
