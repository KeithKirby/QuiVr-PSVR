using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.UI;
using U3D.SteamVR.UI;
using UnityEngine.Events;

public class ToggleMenu : MonoBehaviour {

    public Transform MCamera;

    public static bool MenuDisabled = false;

    public VRTK_ControllerEvents controller;
    bool didToggle;
    bool open;

    public float menuDistance = 2.2f;
    public EMOpenCloseMotion Menu;
    public GameObject Pointer;
    

    GameObject mainPointer;
    public static VRTK_InteractTouch mtouch;
    public static VRTK_InteractGrab mgrab;
    public static VRTK_InteractUse muse;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;

    public static ToggleMenu instance;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    IEnumerator Start () {
        yield return new WaitForEndOfFrame();
        Menu.enabled = true;
        SteamVR_ControllerManager mgr = GetComponentInParent<SteamVR_ControllerManager>();
        mgr.left.GetComponent<VRTK_ControllerEvents>().ApplicationMenuPressed += new ControllerInteractionEventHandler(Toggle);
        mgr.right.GetComponent<VRTK_ControllerEvents>().ApplicationMenuPressed += new ControllerInteractionEventHandler(Toggle);
        while(SteamVR_ControllerManager.freeHand == null)
        {
            yield return true;
        }
        mainPointer = SteamVR_ControllerManager.freeHand;
        mtouch = mainPointer.GetComponent<VRTK_InteractTouch>();
        mgrab = mainPointer.GetComponent<VRTK_InteractGrab>();
        muse = mainPointer.GetComponent<VRTK_InteractUse>();
    }

    public bool isOpen()
    {
        return open;
    }

    Vector3 rotatePoint;
    float WantRotation = 0;

    public void Close()
    {
        if (Menu.motionState != EMBaseMotion.MotionState.Closed)
        {
            /*
                foreach(var v in Pointer.GetComponents<SteamVRPointer>())
                {
                    if(v != null)
                        v.SetEnabled(false);
                }
                */
            if (VRTK_UIPointer.Pointers == null)
                VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
            foreach (var v in VRTK_UIPointer.Pointers)
            {
                v.On = false;
            }
            mainPointer = SteamVR_ControllerManager.freeHand;
            if (mainPointer != null)
            {
                mainPointer.GetComponent<VRTK_InteractTouch>().enabled = true;
                mainPointer.GetComponent<VRTK_InteractGrab>().enabled = true;
                mainPointer.GetComponent<VRTK_InteractUse>().enabled = true;
            }
            open = false;
            Menu.Close(true);
            Time.timeScale = 1f;
            SetupPublisher.IsOptionsScreenActive = false;
        }
    }

    public void Toggle()
    {
        if (MenuDisabled)
            return;
        if (Menu.motionState != EMBaseMotion.MotionState.Closed)
        {
            Close();
        }
        else
        {
            /*
            foreach (var v in Pointer.GetComponents<SteamVRPointer>())
            {
                v.SetEnabled(true);
            }
            */
            if (VRTK_UIPointer.Pointers == null)
                VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
            foreach (var v in VRTK_UIPointer.Pointers)
            {
                v.On = true;
            }
            WantRotation = 0;
            curRot = 0;
            Menu.Open(false);
            open = true;
            mainPointer = SteamVR_ControllerManager.freeHand;
            if (mainPointer != null)
            {
                mainPointer.GetComponent<VRTK_InteractTouch>().enabled = false;
                mainPointer.GetComponent<VRTK_InteractGrab>().enabled = false;
                mainPointer.GetComponent<VRTK_InteractUse>().enabled = false;
            }

            Menu.transform.position = MCamera.position + new Vector3(MCamera.TransformDirection(Vector3.forward).x, 0, MCamera.TransformDirection(Vector3.forward).z) * menuDistance;
            Vector3 offset = (Menu.transform.position - MCamera.position).normalized;
            rotatePoint = MCamera.position;
            Menu.transform.LookAt(Menu.transform.position + offset);
            if (!PhotonNetwork.inRoom)
                Time.timeScale = 0.01f;
            SetupPublisher.IsOptionsScreenActive = true;
        }
    }

    public void Toggle(object sender, ControllerInteractionEventArgs e)
    {
        Toggle();
    }

    public void TogglePointers(bool on)
    {
        if (VRTK_UIPointer.Pointers == null)
            VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
        foreach (var v in VRTK_UIPointer.Pointers)
        {
            v.On = on;
        }
    }

    float cd = 0;
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetButtonDown("Options")) &&
            !DevConsole.DevConsoleUI.isActive())
        {
            Toggle();
        }

        if (isOpen())
        {
            UpdateRotation();
            cd += Time.unscaledDeltaTime;
            if (cd > 3.73153f)
            {
                foreach (var v in Pointer.GetComponents<SteamVRPointer>())
                {
                    if(!v.enabled)
                        v.SetEnabled(true);
                }
                cd = 0;
            }
        }
        else
            cd = 0;
    }

    float curRot;
    void UpdateRotation()
    {
        if(Mathf.Abs(WantRotation-curRot) > 2)
        {
            float lastRot = curRot;
            curRot = Mathf.Lerp(curRot, WantRotation, 3*Time.unscaledDeltaTime);
            float dif = curRot - lastRot;
            Menu.transform.RotateAround(rotatePoint, Vector3.up, dif);
        }
    }

    public void AdjustRotation(float val)
    {
        WantRotation = val;
    }
}
