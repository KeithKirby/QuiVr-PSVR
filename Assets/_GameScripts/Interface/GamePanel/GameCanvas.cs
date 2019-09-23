using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Events;

public class GameCanvas : MonoBehaviour {

    public EMOpenCloseMotion motion;
    public float ToHeadDot;
    public float parallelDot;

    public static GameCanvas instance;

    public bool menuOpen;

    public UnityEvent OnPanelOpen;
    public UnityEvent OnPanelClose;

    public GMenuRoot rootOverride;
    public Transform HeadOverride;
    public Transform HandOverride;

    Transform head;
    Transform hand;
    Transform root;
    Transform freeHand;

    void Awake()
    {
        instance = this;
        motion = GetComponentInChildren<EMOpenCloseMotion>();
    }

    void Update()
    {
        SetTransforms();
        if (hand != null && head != null && root != null)
        {
            transform.SetPositionAndRotation(root.position, root.rotation);
            if (BowAim.instance != null)
            {
                Vector3 toHead = (transform.position - Hips.instance.transform.position).normalized;
                ToHeadDot = Vector3.Dot(PlayerHolder.Down(), hand.forward);//Vector3.Dot(toHead, SteamVR_ControllerManager.bowHand.transform.forward);
                parallelDot = Vector3.Dot(PlayerHolder.Up(), hand.forward);
                bool above = hand.position.y > Hips.instance.transform.position.y - 0.2f;
                bool handUsed = false;
                if(SteamVR_ControllerManager.freeHand != null)
                    handUsed = SteamVR_ControllerManager.freeHand.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null;
                bool shootDelayed = BowAim.instance.lastShotDelta() > 0.25f;
                if ( ToHeadDot > 0.4f && !BowAim.instance.HasArrow() && above && !handUsed && shootDelayed) //Mathf.Abs(parallelDot) < 0.6f &&  ToHeadDot < 0.1f
                {
                    menuOpen = true;
                    if (!motion.gameObject.activeSelf)
                        motion.gameObject.SetActive(true);
                    if (motion.motionState != EMBaseMotion.MotionState.Open && motion.motionState != EMBaseMotion.MotionState.Opening)
                    {
                        motion.Open();
                        OnPanelOpen.Invoke();
                    }
                }
                else
                {
                    menuOpen = false;
                    if (motion.motionState != EMBaseMotion.MotionState.Closed && motion.motionState != EMBaseMotion.MotionState.Closing)
                    {
                        OnPanelClose.Invoke();
                        motion.Close();
                    }
                    if (motion.motionState == EMBaseMotion.MotionState.Closed)
                        motion.gameObject.SetActive(false);
                        
                }
            }
        }
    }

    void SetTransforms()
    {
        if(HeadOverride != null)
            head = HeadOverride.transform;
        if (HandOverride != null)
            hand = HandOverride.transform;
        if (rootOverride != null)
            root = rootOverride.transform;
        if (head == null && PlayerHead.instance != null)
            head = PlayerHead.instance.transform;
        if (hand != SteamVR_ControllerManager.bowHand && SteamVR_ControllerManager.bowHand != null)
        {
            hand = SteamVR_ControllerManager.bowHand.transform;
            if (rootOverride == null)
                root = null;
        }
        if (root == null && hand != null)
        {
            GMenuRoot r = hand.GetComponentInChildren<GMenuRoot>();
            if(r != null)
                root = r.transform;
            if(PhotonNetwork.inRoom)
            {
                RadialMenuController rc = GetComponentInChildren<RadialMenuController>();
                rc.UpdateEvents(root);
                rc.transform.SetPositionAndRotation(r.RadialRoot.position, r.RadialRoot.rotation);
            }
        }
            
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponentInParent<HandAnim>() != null && menuOpen && col.tag != "Pointer")
        {
            col.GetComponentInParent<HandAnim>().inMenu = 1;
        }     
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponentInParent<HandAnim>() != null && col.tag != "Pointer")
        {
            col.GetComponentInParent<HandAnim>().inMenu = 0;
        }
            
    }
}
