using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using U3D.SteamVR.UI;

public class PVPMatchUI : MonoBehaviour {

    public Text TitleText;
    public Text DetailText;
    public EMOpenCloseMotion motion;
    public Transform HeadHolder;
    public Text AcceptText;
    public Text CancelText;
    public static PVPMatchUI instance;
    bool open;

    #region Lifecycle

    void Awake()
    {
        instance = this;
    }

    float cd = 0;
    Vector3 currentOffset;
    void Update()
    {
        if (open && LocalPlayer.instance != null)
        {
            transform.position = LocalPlayer.instance.PlayArea.position + currentOffset;
            cd += Time.unscaledDeltaTime;
            DetailText.text = "Joining Game: " + (int)(5 - cd) + "";
            if (cd >= 5)
            {
                Accept();
            }
            EnablePointers();
            if (NVR_Player.isThirdPerson())
            {        
                if (CancelText.text != null)
                    CancelText.text = "Cancel (N)";
                else if (Input.GetKeyDown(KeyCode.N))
                    Decline();
            }
        }
        else
            cd = 0;
    }

    #endregion

    #region Static
    public static void OpenUI(string titleText, string detailText)
    {
        if(instance != null)
        {
            PVPQButton.ToggleButton(false);
            instance.open = true;
            if (instance.motion.motionState == EMBaseMotion.MotionState.Closed || instance.motion.motionState == EMBaseMotion.MotionState.Closing)
                instance.motion.Open(true);
            instance.TitleText.text = titleText;
            instance.DetailText.text = detailText;
            if (PlayerHead.instance != null)
            {
                instance.transform.position = PlayerHead.instance.transform.position + (PlayerHead.instance.transform.TransformDirection(Vector3.forward) * 2f);
                instance.transform.position = new Vector3(instance.transform.position.x, PlayerHead.instance.transform.position.y, instance.transform.position.z);
                instance.transform.LookAt(instance.transform.position + (instance.transform.position - PlayerHead.instance.transform.position).normalized);
                instance.currentOffset = instance.transform.position - LocalPlayer.instance.PlayArea.transform.position;
            }
            GameObject mainPointer = SteamVR_ControllerManager.freeHand;
            if (mainPointer != null)
            {
                mainPointer.GetComponent<VRTK_InteractTouch>().enabled = false;
                mainPointer.GetComponent<VRTK_InteractGrab>().enabled = false;
                mainPointer.GetComponent<VRTK_InteractUse>().enabled = false;
            }
            if (VRTK_UIPointer.Pointers == null)
                VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
            foreach (var v in VRTK_UIPointer.Pointers)
            {
                v.On = true;
            }
        }
    }

    public static void CloseUI()
    {
        if (instance != null)
        {
            PVPQButton.ToggleButton(true);
            instance.open = false;
            if(instance.motion.motionState == EMBaseMotion.MotionState.Open || instance.motion.motionState == EMBaseMotion.MotionState.Opening)
                instance.motion.Close(true);
            GameObject mainPointer = SteamVR_ControllerManager.freeHand;
            if (mainPointer != null)
            {
                mainPointer.GetComponent<VRTK_InteractTouch>().enabled = true;
                mainPointer.GetComponent<VRTK_InteractGrab>().enabled = true;
                mainPointer.GetComponent<VRTK_InteractUse>().enabled = true;
            }
            if (VRTK_UIPointer.Pointers == null)
                VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
            foreach (var v in VRTK_UIPointer.Pointers)
            {
                v.On = false;
            }
        }

    }

    #endregion

    #region Interaction

    public void Accept()
    {
        PvPMatchmaking.AcceptMatch();
        CloseUI();
    }

    public void Decline()
    {
        PvPMatchmaking.CancelMatch();
        CloseUI();
    }

    #endregion

    #region Utility

    void EnablePointers()
    {
        foreach (var v in SteamVRPointer.AllPointers())
        {
            if (v != null && !v.menuUp)
                v.SetEnabled(true);
        }
    }

    #endregion
}
