using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using System;
#if UNITY_PS4
using UnityEngine.PS4.VR;
using UnityEngine.PS4;
#endif

// Handles setup for PSVR, will flag itself as do not destroy, if one already exists it will self destruct
public class PSVRManager : MonoBehaviour
{
    public enum VRState
    {
        Inactive,
        Starting,
        Active,
        ShuttingDown
    }

    public delegate void OnHMDShutdown();
    public delegate void OnHMDConnected();
    public event OnHMDShutdown HMDDisconnected;
    public event OnHMDConnected HMDConnected;

    public delegate void OnRecenter();
    static public event OnRecenter Recenter;

    VRState _state = VRState.Inactive;

    int _deviceEventRegisterCount = 0;

    public float renderScale = 1.4f; // 1.4 is Sony's recommended scale for PlayStation VR
    public bool AllowSocialScreen = false; // Set this to 'false' to use the monitor/display as the Social Screen
    bool _showSocialScreen = false;

    static public float PlayerHeight = 1.65f;
    //static public float PlayerScale = 1;

    private static PSVRManager _instance;

    public static PSVRManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PSVRManager>();
                //DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void SetState(VRState state)
    {
        //Debug.Log("SetState:" + state);
        _state = state;
    }

    void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;            
            DontDestroyOnLoad(this);
        }
        else if (this != _instance)
        {
            // There can be only one!
            DestroyImmediate(gameObject);
        }
    }

    IEnumerator Start()
    {
        Debug.Log("PSVRManager.Start()");
        //OnAppReady();

        //yield return new WaitForSeconds(4);
        //UnityEngine.XR.InputTracking.Recenter();
        //PublishRecenter();
        yield return true;
    }

    //void OnAppReady()
    //{
//#if UNITY_PS4
        //BeginVRSetup();
//#endif
    //}

    public void BeginVRSetup()
    {
        if (VRState.Inactive == _state)
        {
            Debug.Log("BeginVRSetup ok");
            SetState(VRState.Starting);
            StartCoroutine(SetupVR());
        }
        else
        {
            Debug.Log("BeginVRSetup called but vr is not inactive:" + _state);
        }
    }

    public void BeginShutdownVR()
    {
        if (VRState.Active == _state ||
            VRState.Starting == _state)
        {
            SetState(VRState.ShuttingDown);
            StartCoroutine(ShutdownVR());
        }
        else
        {
            Debug.Log("BeginShutdownVR called but vr is in unexpected state:" + _state);
        }        
    }

    IEnumerator SetupVR()
    {
#if UNITY_PS4
        Debug.Log("SetupVR");

        // Register the callbacks needed to detect resetting the HMD
        Utility.onSystemServiceEvent += OnSystemServiceEvent;

        if (_deviceEventRegisterCount > 0)
            Debug.Log("Double registered");

        PlayStationVR.onDeviceEvent += OnDeviceEvent;
        ++_deviceEventRegisterCount;
                
        ShowHMDOnMonitor(true);        

        // Not in sample, needed?
        while (null == Camera.main)
            yield return null;
        // Post-reproject for camera locked items, in this case the reticle. Must be
        // set before we change the VR Device. See VRPostReprojection.cs for more info
        if (Camera.main.actualRenderingPath == RenderingPath.Forward)
        {
            if (FindObjectOfType<VRPostReprojection>())
            {
#if !UNITY_EDITOR
                PlayStationVRSettings.postReprojectionType = PlayStationVRPostReprojectionType.PerEye;                
                Debug.Log("Post reprojection enabled");
#else
                Debug.Log("Post reprojection disabled because in editor mode");
#endif

            }
            else
            {
                Debug.Log("Post reprojection is disabled because no script found");
            }
        }
        else
        {
            Debug.LogError("Post reprojection is not yet fully supported in non-Forward Rendering Paths.");
        }
#endif
        UnityEngine.XR.XRSettings.LoadDeviceByName(VRDeviceNames.PlayStationVR);

        // WORKAROUND: At the moment the device is created at the end of the frame so
        // changing almost any VR settings needs to be delayed until the next frame
        yield return null;

        UnityEngine.XR.XRSettings.enabled = true;
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = renderScale;

        if (null != HMDConnected)
            HMDConnected();

        SetState(VRState.Active);
        UpdateMonitorMode();
    }

    IEnumerator ShutdownVR()
    {
        Debug.Log("ShutdownVR called " + _deviceEventRegisterCount);

        UnityEngine.XR.XRSettings.LoadDeviceByName(VRDeviceNames.None);

        // WORKAROUND: At the moment the device is created at the end of the frame so
        // we need to wait a frame until the VR device is changed back to 'None', and
        // then reset the Main Camera's FOV and Aspect
        yield return null;

        UnityEngine.XR.XRSettings.enabled = false;
        UnityEngine.XR.XRSettings.showDeviceView = false;

#if UNITY_PS4
        // Unregister the callbacks needed to detect resetting the HMD
        Utility.onSystemServiceEvent -= OnSystemServiceEvent;
        PlayStationVR.onDeviceEvent -= OnDeviceEvent;
        --_deviceEventRegisterCount;
        PlayStationVR.SetOutputModeHMD(false, 60);
#endif

        Camera.main.fieldOfView = 60.0f;
        Camera.main.ResetAspect();

        SetState(VRState.Inactive);

        if (null != HMDDisconnected)
            HMDDisconnected();

        UpdateMonitorMode();
    }

    private void CameraRigs_RpgCameraEnabledEvt(bool enabled)
    {
        ShowHMDOnMonitor(!enabled);
    }

    IEnumerator OpenHmdSetupDialogDelayed()
    {
        // Add a delay before showing dialog. See https://ps4.scedev.net/forums/thread/172565/
        yield return new WaitForSeconds(1);
        HmdSetupDialog.OpenAsync(0, null);
    }

    public void SetupHmdDevice()
    {
#if UNITY_PS4

        Debug.Log("SetupHmdDevice()");
        // The HMD Setup Dialog is not displayed on the social screen in separate
        // mode, so we'll force it to mirror-mode first
        UnityEngine.XR.XRSettings.showDeviceView = true;

        // Show the HMD Setup Dialog, and specify the callback for when it's finished
        HmdSetupDialog.OpenAsync(0, OnHmdSetupDialogCompleted);
#endif
    }

    public void ShowHMDOnMonitor(bool showOnMonitor)
    {
        _showSocialScreen = !showOnMonitor;
        UpdateMonitorMode();        
    }


    public void UpdateMonitorMode()
    {
        // Only show if social screen allowed, wanted by game and VR is active
        bool showDeviceView = (AllowSocialScreen && _showSocialScreen) == false || VRState.Active != _state;
        if (showDeviceView != UnityEngine.XR.XRSettings.showDeviceView)
        {
            UnityEngine.XR.XRSettings.showDeviceView = showDeviceView;
            //Debug.LogFormat("ShowHMDOnMonitor showDeviceView = {0}", UnityEngine.XR.XRSettings.showDeviceView);
        }
    }

    public void ChangeRenderScale(float scale)
    {
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = scale;
    }

#if UNITY_PS4
    // HMD recenter happens in this event
    void OnSystemServiceEvent(Utility.sceSystemServiceEventType eventType)
    {
        Debug.LogFormat("OnSystemServiceEvent: {0}", eventType);

        if (eventType == Utility.sceSystemServiceEventType.ResetVrPosition)
        {
            UnityEngine.XR.InputTracking.Recenter();
            PublishRecenter();
        }
    }
#endif

    static public void PublishRecenter()
    {
        if (null != Recenter)
            Recenter();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            UnityEngine.XR.InputTracking.Recenter();
            PublishRecenter();
        }
    }

#if UNITY_PS4
    // Detect completion of the HMD dialog and either proceed to setup VR, or throw a warning
    void OnHmdSetupDialogCompleted(DialogStatus status, DialogResult result)
    {
        Debug.LogFormat("OnHmdSetupDialogCompleted: {0}, {1}", status, result);

        switch (result)
        {
            case DialogResult.OK:
                BeginVRSetup();
                break;
            case DialogResult.UserCanceled:
                Debug.LogWarning("User Cancelled HMD Setup!");
                BeginShutdownVR();
                break;
        }
    }
#endif

#if UNITY_PS4
    // This handles disabling VR in the event that the HMD has been disconnected
    bool OnDeviceEvent(PlayStationVR.deviceEventType eventType, int value)
    {
        Debug.LogFormat("### onDeviceEvent: {0}, {1}", eventType, value);
        var handledEvent = false;

        switch (eventType)
        {
            case PlayStationVR.deviceEventType.deviceStarted:
                Debug.LogFormat("### OnDeviceEvent: deviceStarted: {0}", value);
                break;
            case PlayStationVR.deviceEventType.deviceStopped:
                BeginShutdownVR();
                handledEvent = true;
                break;
            case PlayStationVR.deviceEventType.StatusChanged: // e.g. HMD unplugged
                var devstatus = (VRDeviceStatus)value;
                Debug.LogFormat("### OnDeviceEvent: VRDeviceStatus: {0}", devstatus);
                if (devstatus != VRDeviceStatus.Ready)
                {
                    // TRC R4026 suggests showing the HMD Setup Dialog if the device status becomes non-ready
                    if (UnityEngine.XR.XRSettings.loadedDeviceName == VRDeviceNames.None)
                    {
                        StartCoroutine(OpenHmdSetupDialogDelayed());
                    }
                    else
                    {
                        BeginShutdownVR();
                    }
                }
                handledEvent = true;
                break;
            case PlayStationVR.deviceEventType.MountChanged:
                var status = (VRHmdMountStatus)value;
                Debug.LogFormat("### OnDeviceEvent: VRHmdMountStatus: {0}", status);
                handledEvent = true;
                break;
            case PlayStationVR.deviceEventType.CameraChanged:
                // If the event is for the camera and the value is 0, the camera has been disconnected
                Debug.LogFormat("### OnDeviceEvent: CameraChanged: {0}", value);
                if (value == 0)
                    StartCoroutine(OpenHmdSetupDialogDelayed());
                handledEvent = true;
                break;
            case PlayStationVR.deviceEventType.HmdHandleInvalid:
                // Unity will handle this automatically, please see API documentation
                Debug.LogFormat("### OnDeviceEvent: HmdHandleInvalid: {0}", value);
                break;
            case PlayStationVR.deviceEventType.DeviceRestarted:
                // Unity will handle this automatically, please see API documentation
                Debug.LogFormat("### OnDeviceEvent: DeviceRestarted: {0}", value);
                break;
            case PlayStationVR.deviceEventType.DeviceStartedError:
                //_state = VRState.Error;
                break;
            default:
                throw new ArgumentOutOfRangeException("eventType", eventType, null);
        }

        return handledEvent;
    }
#endif
}