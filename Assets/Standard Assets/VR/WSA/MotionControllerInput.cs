using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
#if UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif
public class MotionControllerInput : MonoBehaviour
{

    public GameObject DevicePrefab;

    public Transform Head;
    public Transform HandLeft;
    public Transform HandRight;
    public float LeftLoss;
    public float RightLoss;

    static MotionControllerInput instance;
#if UNITY_WSA
    static InteractionManager.SourceEventArgs LeftSrc;
    static InteractionManager.SourceEventArgs RightSrc;
#endif
    //public Transform ControllersRoot;
    //public ConsoleManager console;

    private HashSet<string> trackedConsoleProperties = new HashSet<string>();

    // Interaction Manager Input
    private Dictionary<uint, Transform> imDevices = new Dictionary<uint, Transform>();

    // Unity Input
    private Dictionary<string, UnityEngine.XR.XRNode> trackedControllers = new Dictionary<string, UnityEngine.XR.XRNode>();
    private Dictionary<UnityEngine.XR.XRNode, Transform> uDevices = new Dictionary<UnityEngine.XR.XRNode, Transform>();

    public float lossRisk;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        this.SetInputMode();
    }

    private void SetInputMode()
    {
        this.ClearTrackedConsoleProperties();
        TearDownInteractionManagerEventInput();
        SetupInteractionManagerEventInput();
    }

#region InteractionManagerPollingInput
    /*
    private void SetupInteractionManagerPollingInput()
    {
        foreach (var sourceState in InteractionManager.GetCurrentReading())
        {
            uint id = sourceState.source.id;

            if (id != 0 && sourceState.source.supportsPointing)
            {
                this.AddDevice(id);
            }
        }
    }

    private void UpdateInteractionManagerPollingInput()
    {
        foreach(var sourceState in InteractionManager.GetCurrentReading())
        {
            uint id = sourceState.source.id;
            var handedness = sourceState.source.handedness;

            if (!this.imDevices.ContainsKey(id))
            {
                if (id !=0 && sourceState.source.supportsPointing)
                {
                    this.AddDevice(id);
                }
            }

            this.UpdateInteractionSourceState(sourceState);

            // Buttons
            if (sourceState.selectPressed)
            {
                Debug.Log(handedness + " select pressed");
            }
            if (sourceState.menuPressed)
            {
                Debug.Log(handedness + " menu pressed");
            }
            if (sourceState.grasped)
            {
                Debug.Log(handedness + " grasped");
            }
            if (sourceState.controllerProperties.touchpadTouched)
            {
                Debug.Log(handedness + " touchpad touched");
            }
            if (sourceState.controllerProperties.touchpadPressed)
            {
                Debug.Log(handedness + " touchpad pressed");
            }
            if (sourceState.controllerProperties.thumbstickPressed)
            {
                Debug.Log(handedness + " thumbstick pressed");
            }
        }
    }

    private void TearDownInteractionManagerPollingInput()
    {
        // Remove all devices from the list
        foreach (var sourceState in InteractionManager.GetCurrentReading())
        {
            uint id = sourceState.source.id;
            this.RemoveDevice(id);
        }
    }
    */
#endregion InteractionManagerPollingInput

#region InteractionManagerEventInput

    public void SetupInteractionManagerEventInput()
    {
#if UNITY_WSA
        InteractionManager.SourceDetected += InteractionManager_SourceDetected;
        InteractionManager.SourceLost += InteractionManager_SourceLost;
        InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
        InteractionManager.SourcePressed += InteractionManager_SourcePressed;
        InteractionManager.SourceReleased += InteractionManager_SourceReleased;
        // Add any already detected devices to the list
        foreach (var sourceState in InteractionManager.GetCurrentReading())
        {
            uint id = sourceState.source.id;

            if (sourceState.source.supportsPointing)
            {
                this.AddDevice(id);
            }
        }
#endif
    }

#if UNITY_WSA
    private void InteractionManager_SourcePressed(InteractionManager.SourceEventArgs eventArgs)
    {
        //Debug.Log(eventArgs.state.source.handedness + " controller " + eventArgs.pressKind + " pressed");
        if (eventArgs.state.source.handedness == InteractionSourceHandedness.Left)
            LeftSrc = eventArgs;
        else if (eventArgs.state.source.handedness == InteractionSourceHandedness.Right)
            RightSrc = eventArgs;
    }

    private void InteractionManager_SourceReleased(InteractionManager.SourceEventArgs eventArgs)
    {
        //Debug.Log(eventArgs.state.source.handedness + " controller " + eventArgs.pressKind + " Released");
        if (eventArgs.state.source.handedness == InteractionSourceHandedness.Left)
            LeftSrc = eventArgs;
        else if (eventArgs.state.source.handedness == InteractionSourceHandedness.Right)
            RightSrc = eventArgs;
    }

    private void InteractionManager_SourceUpdated(InteractionManager.SourceEventArgs eventArgs)
    {
        this.UpdateInteractionSourceState(eventArgs.state);
        if (eventArgs.state.source.handedness == InteractionSourceHandedness.Left)
            LeftSrc = eventArgs;
        else if (eventArgs.state.source.handedness == InteractionSourceHandedness.Right)
            RightSrc = eventArgs;
    }

    private void InteractionManager_SourceLost(InteractionManager.SourceEventArgs eventArgs)
    {
        uint id = eventArgs.state.source.id;
        if (eventArgs.state.source.handedness == InteractionSourceHandedness.Left)
            LeftSrc = eventArgs;
        else if (eventArgs.state.source.handedness == InteractionSourceHandedness.Right)
            RightSrc = eventArgs;
        this.RemoveDevice(id);
    }

    private void InteractionManager_SourceDetected(InteractionManager.SourceEventArgs eventArgs)
    {
        uint id = eventArgs.state.source.id;

        if (eventArgs.state.source.supportsPointing)
        {
            this.AddDevice(id);
        }
        if (eventArgs.state.source.handedness == InteractionSourceHandedness.Left)
            LeftSrc = eventArgs;
        else if (eventArgs.state.source.handedness == InteractionSourceHandedness.Right)
            RightSrc = eventArgs;
    }
#endif

    private void TearDownInteractionManagerEventInput()
    {
#if UNITY_WSA
        InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
        InteractionManager.SourceLost -= InteractionManager_SourceLost;
        InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
        InteractionManager.SourcePressed -= InteractionManager_SourcePressed;

        // Remove all devices from the list
        foreach (var sourceState in InteractionManager.GetCurrentReading())
        {
            uint id = sourceState.source.id;
            this.RemoveDevice(id);
        }
#endif
    }

    #endregion InteractionManagerEventInput

    #region InteractionManagerCommon
#if UNITY_WSA
    private void UpdateInteractionSourceState(InteractionSourceState sourceState)
    {

        uint id = sourceState.source.id;
        var handedness = sourceState.source.handedness;

        if (imDevices.ContainsKey(id))
        {
            Transform t = imDevices[id];

            if(t == null)
            {
                if (handedness == InteractionSourceHandedness.Left)
                    imDevices[id] = HandLeft;
                else if (handedness == InteractionSourceHandedness.Right)
                    imDevices[id] = HandRight;
            }
            if (HandLeft != null && !HandLeft.gameObject.activeSelf)
                HandLeft.gameObject.SetActive(true);
            if (HandRight != null && !HandRight.gameObject.activeSelf)
                HandRight.gameObject.SetActive(true);
            var sourcePose = sourceState.sourcePose;
            Vector3 position;
            Quaternion rotation;

            if (sourcePose.TryGetPosition(out position) &&
                sourcePose.TryGetRotation(out rotation) && imDevices[id] != null)
            {
                SetTransform(imDevices[id], position, rotation, handedness);
            }
        }

        // Update properties
        this.UpdateTrackedConsoleProperty(handedness + " Select Value", string.Format("{0:0.000}", sourceState.selectPressedValue));
        this.UpdateTrackedConsoleProperty(handedness + " Touchpad", string.Format("{0:0.000},{1:0.000}", sourceState.controllerProperties.touchpadX, sourceState.controllerProperties.touchpadY));
        this.UpdateTrackedConsoleProperty(handedness + " Thumbstick", string.Format("{0:0.000},{1:0.000}", sourceState.controllerProperties.thumbstickX, sourceState.controllerProperties.thumbstickY));
    }
#endif
    private void AddDevice(uint id)
    {
        if (!imDevices.ContainsKey(id))
        {
            //GameObject go = Instantiate(DevicePrefab, ControllersRoot) as GameObject;
            //go.name = "Controller " + id;
            imDevices[id] = null;//go.transform;
        }

        //this.console.UpdateProperty("Detected Motion Controllers", string.Format("{0}", this.imDevices.Keys.Count));
    }

    private void RemoveDevice(uint id)
    {
        if (imDevices.ContainsKey(id))
        {
            //Destroy(imDevices[id].gameObject);
            imDevices[id].gameObject.SetActive(false);
            imDevices.Remove(id);
        }

        //this.console.UpdateProperty("Detected Motion Controllers", string.Format("{0}", this.imDevices.Keys.Count));
    }

#endregion InteractionManagerCommon
    
    struct ControllerPose
    {
        public Transform t;
        public Vector3 pose;
        public Quaternion rot;
    }

    ControllerPose pLeft;
    ControllerPose pRight;
    void Update()
    {
        // Show keyboard input
        /*
        if (!String.IsNullOrEmpty(Input.inputString))
        {   
            this.console.UpdateProperty("Last Key(s) Pressed", string.Format("{0}", Input.inputString));
        }
        */
        //Set Hand Positions
        if(pLeft.t != null)
        {
            pLeft.t.localPosition = pLeft.pose;
            pLeft.t.localRotation = pLeft.rot;
        }
        if(pRight.t != null)
        {
            pRight.t.localPosition = pRight.pose;
            pRight.t.localRotation = pRight.rot;
        }
#if UNITY_WSA
        LeftLoss = (float)LeftSrc.state.properties.sourceLossRisk;
        RightLoss = (float)RightSrc.state.properties.sourceLossRisk;
#endif
    }

#if UNITY_WSA
    private void SetTransform(Transform t, Vector3 position, Quaternion rotation, InteractionSourceHandedness hand)
    {

        if(hand == InteractionSourceHandedness.Left)
        {
            pLeft.t = t;
            pLeft.pose = position;
            pLeft.rot = rotation;
        }
        else if(hand == InteractionSourceHandedness.Right)
        {
            pRight.t = t;
            pRight.pose = position;
            pRight.rot = rotation;
        }

    //t.localPosition = position;
    //t.localRotation = rotation;
    }
#endif

    private void UpdateTrackedConsoleProperty(string name, string value)
    {
        /*
        if (console != null)
        {
            this.trackedConsoleProperties.Add(name);
            this.console.UpdateProperty(name, value);
        }
        */
    }

    private void ClearTrackedConsoleProperties()
    {
        /*
        if (this.console != null)
        {
            foreach (var property in this.trackedConsoleProperties)
            {
                console.RemoveProperty(property);
            }

            this.trackedConsoleProperties = new HashSet<string>();
        }
        */
    }

    private void OnDestroy()
    {
        this.ClearTrackedConsoleProperties();
        TearDownInteractionManagerEventInput();
        
    }

#region Static Methods
    public static GameObject LeftHand()
    {
        if (instance != null)
            return instance.HandLeft.gameObject;
        return null;
    }

    public static GameObject RightHand()
    {
        if (instance != null)
            return instance.HandRight.gameObject;
        return null;
    }

    public static Transform GetHead()
    {
        if (instance != null)
            return instance.Head;
        return null;
    }

    public static void TriggerHapticPulse(uint id, float strength)
    {
#if UNITY_WSA
        InteractionController controller;
        bool hasController = false;
        if(id == 0)
            hasController = RightSrc.state.source.TryGetController(out controller);
        else
            hasController = LeftSrc.state.source.TryGetController(out controller);
        if (hasController)
        {
            controller.StartHapticBuzz(strength, Time.unscaledDeltaTime);
        }
#endif
    }
    
    public static float GetLoss(uint id)
    {
#if UNITY_WSA
        if (id == 1)
            return (float)LeftSrc.state.properties.sourceLossRisk;
        return (float)RightSrc.state.properties.sourceLossRisk;
#endif
        return 0;
    }

#if UNITY_WSA
    public static InteractionManager.SourceEventArgs Controller(uint id)
    {
        if (id == 0)
            return RightSrc;
        return LeftSrc;
    }
#endif

    public static Vector3 GetControllerVel(uint id)
    {
        Vector3 v = Vector3.zero;
#if UNITY_WSA
        if (id == 0)
            RightSrc.state.sourcePose.TryGetVelocity(out v);
        else
            LeftSrc.state.sourcePose.TryGetVelocity(out v);
#endif
        return -1*v;
    }

    public static Vector3 GetControllerAngularVel(uint id)
    {
        Vector3 v = Vector3.zero;
#if UNITY_WSA
        if (id == 0)
            RightSrc.state.sourcePose.TryGetAngularVelocity(out v);
        else
            LeftSrc.state.sourcePose.TryGetAngularVelocity(out v);
#endif
        return v;
    }
#endregion

    public enum Handed
    {
        Left,
        Right
    }
}
