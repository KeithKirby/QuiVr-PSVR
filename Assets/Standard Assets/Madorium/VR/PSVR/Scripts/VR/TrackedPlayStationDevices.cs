using UnityEngine;
using UnityEngine.VR;
using System.Collections;
#if UNITY_PS4
using UnityEngine.PS4;
using UnityEngine.PS4.VR;
#endif

public class TrackedPlayStationDevices : MonoBehaviour
{
    public enum TrackedDevicesType {DualShock4 = 1, Aim = 2, Move = 4};
    public TrackedDevicesType trackedDevicesType = TrackedDevicesType.DualShock4;

#if UNITY_PS4
    public PlayStationVRTrackingQuality moveTrackingA = PlayStationVRTrackingQuality.None;
    public PlayStationVRTrackingQuality moveTrackingB = PlayStationVRTrackingQuality.None;

    public PlayStationVRTrackingType trackingType = PlayStationVRTrackingType.Absolute;
    public PlayStationVRTrackerUsage trackerUsageType = PlayStationVRTrackerUsage.OptimizedForHmdUser;
#endif

    public TrackedDevice deviceDualShock4;
    public TrackedDevice deviceAim;
    public TrackedDevice deviceMovePrimary;
    public TrackedDevice deviceMoveSecondary;
    int _numMoves = 0;

    [System.Serializable]
    public class TrackedDevice
    {
        public int handle = -1;
        public Transform transform;
        public Renderer light;
        public Vector3 position = Vector3.zero;
        public Quaternion orientation = Quaternion.identity;
    }

    IEnumerator Start()
    {
        if(deviceDualShock4.transform)
            deviceDualShock4.transform.gameObject.SetActive(false);

        if(deviceAim.transform)
            deviceAim.transform.gameObject.SetActive(false);

        if(deviceMovePrimary.transform)
            deviceMovePrimary.transform.gameObject.SetActive(false);

        if(deviceMoveSecondary.transform)
            deviceMoveSecondary.transform.gameObject.SetActive(false);

        // Keep waiting until we have a VR Device available
        while (!UnityEngine.XR.XRDevice.isPresent)
            yield return new WaitForSeconds(1.0f);

#if UNITY_PS4
        // Register the callbacks needed to detect resetting the HMD
		Utility.onSystemServiceEvent += OnSystemServiceEvent;
#endif

        // Make sure the device we now have is PlayStation VR
        if (UnityEngine.XR.XRSettings.loadedDeviceName != VRDeviceNames.PlayStationVR)
        {
            Debug.LogWarning("Tracking only works for PS4!");
            this.enabled = false;
        }
        else
        {
            DetectMovesChanged();            
        }
    }

    void DetectMovesChanged()
    {
        var numMoves = 0;
#if UNITY_PS4
        if (UnityEngine.PS4.PS4Input.MoveIsConnected(0, 0))
            numMoves++;
        if (UnityEngine.PS4.PS4Input.MoveIsConnected(0, 1))
            numMoves++;
#endif
        if(_numMoves != numMoves)
        {
            _numMoves = numMoves;
            ResetDeviceTracking();
        }
    }

    void Update()
    {
        if (UnityEngine.XR.XRDevice.isPresent && UnityEngine.XR.XRSettings.loadedDeviceName == VRDeviceNames.PlayStationVR)
        {
            if ((trackedDevicesType & TrackedDevicesType.DualShock4) == TrackedDevicesType.DualShock4)
                UpdateDualShock4Transforms();

            if ((trackedDevicesType & TrackedDevicesType.Aim) == TrackedDevicesType.Aim)
                UpdateAimTransforms();

            if ((trackedDevicesType & TrackedDevicesType.Move) == TrackedDevicesType.Move)
            {
                DetectMovesChanged();
                UpdateMoveTransforms();
            }
        }
    }

    void OnDisable()
    {
#if UNITY_PS4
        Utility.onSystemServiceEvent -= OnSystemServiceEvent;
#endif
        UnregisterDevices();
    }

    void OnDestroy()
    {
#if UNITY_PS4
        Utility.onSystemServiceEvent -= OnSystemServiceEvent;
#endif
        UnregisterDevices();
    }

    // Unregister and re-register the controllers to reset them
    public void ResetDeviceTracking()
    {
        UnregisterDevices();
        RegisterDevices();
    }

    void UpdateDualShock4Transforms()
    {
#if UNITY_PS4
        if (deviceDualShock4.handle >= 0)
        {
            if (Tracker.GetTrackedDevicePosition(deviceDualShock4.handle, out deviceDualShock4.position) == PlayStationVRResult.Ok)
                deviceDualShock4.transform.localPosition = deviceDualShock4.position;

            if (Tracker.GetTrackedDeviceOrientation(deviceDualShock4.handle, out deviceDualShock4.orientation) == PlayStationVRResult.Ok)
                deviceDualShock4.transform.localRotation = deviceDualShock4.orientation;
        }
#endif
    }

    void UpdateAimTransforms()
    {
#if UNITY_PS4
        if (deviceAim.handle >= 0)
        {
            if (Tracker.GetTrackedDevicePosition(deviceAim.handle, out deviceAim.position) == PlayStationVRResult.Ok)
                deviceAim.transform.localPosition = deviceAim.position;

            if (Tracker.GetTrackedDeviceOrientation(deviceAim.handle, out deviceAim.orientation) == PlayStationVRResult.Ok)
                deviceAim.transform.localRotation = deviceAim.orientation;
        }
#endif
    }

    void UpdateMoveTransforms()
    {
#if UNITY_PS4
        // Perform tracking for the primary controller, if we've got a handle
        if (deviceMovePrimary.transform && deviceMovePrimary.handle >= 0)
        {
            Tracker.GetTrackedDevicePositionQuality(deviceMovePrimary.handle, out moveTrackingA);
            if (Tracker.GetTrackedDevicePosition(deviceMovePrimary.handle, out deviceMovePrimary.position) == PlayStationVRResult.Ok)
                deviceMovePrimary.transform.localPosition = deviceMovePrimary.position;

            if (Tracker.GetTrackedDeviceOrientation(deviceMovePrimary.handle, out deviceMovePrimary.orientation) == PlayStationVRResult.Ok)
                deviceMovePrimary.transform.localRotation = deviceMovePrimary.orientation;

            var move0 = PS4InputEx.GetMove(0);
            move0.UpdatePosition(deviceMovePrimary.transform.position, Time.deltaTime);            
        }

        // Perform tracking for the secondary controller, if we've got a handle
        if (deviceMoveSecondary.transform && deviceMoveSecondary.handle >= 0)
        {
            Tracker.GetTrackedDevicePositionQuality(deviceMovePrimary.handle, out moveTrackingB);
            if (Tracker.GetTrackedDevicePosition(deviceMoveSecondary.handle, out deviceMoveSecondary.position) == PlayStationVRResult.Ok)
                deviceMoveSecondary.transform.localPosition = deviceMoveSecondary.position;

            if (Tracker.GetTrackedDeviceOrientation(deviceMoveSecondary.handle, out deviceMoveSecondary.orientation) == PlayStationVRResult.Ok)
                deviceMoveSecondary.transform.localRotation = deviceMoveSecondary.orientation;

            var move1 = PS4InputEx.GetMove(1);
            move1.UpdatePosition(deviceMoveSecondary.transform.position, Time.deltaTime);
        }
#endif
    }

    public void RegisterDevices()
    {
        if ((trackedDevicesType & TrackedDevicesType.DualShock4) == TrackedDevicesType.DualShock4)
            StartCoroutine(RegisterDualShock4Controller());

        if ((trackedDevicesType & TrackedDevicesType.Aim) == TrackedDevicesType.Aim)
            StartCoroutine(RegisterAimController());

        if ((trackedDevicesType & TrackedDevicesType.Move) == TrackedDevicesType.Move)
            StartCoroutine(RegisterMoveControllers());
    }

    // Register DualShock 4 to track
    IEnumerator RegisterDualShock4Controller()
    {
#if UNITY_PS4
        if (PS4Input.PadIsConnected(0) == false)
        {
            Debug.LogError("Trying to register a DualShock 4 controller, but none are connected!");
            yield break;
        }

        int[] handles = new int[2];
        PS4Input.PadGetUsersHandles(2, handles);
        PS4Input.DeviceType deviceType;
        PlayStationVRResult result;

        foreach (int handle in handles)
        {
            deviceType = (PS4Input.DeviceType)PS4Input.GetDeviceTypeForHandle(handle);

            if (deviceType == PS4Input.DeviceType.DeviceDS4)
            {
                deviceDualShock4.handle = handle;
                break;
            }
            else if (deviceType == PS4Input.DeviceType.DeviceUnknown)
            {
                Debug.LogError("Bad handle!");
                break;
            }
        }

        if (deviceDualShock4.handle < 0)
            yield break;

        result = Tracker.RegisterTrackedDevice(PlayStationVRTrackedDevice.DeviceDualShock4, deviceDualShock4.handle, trackingType, trackerUsageType);

        if (result == PlayStationVRResult.Ok)
        {
            // Get the tracking, and wait for it to start
            PlayStationVRTrackingStatus trackingStatus = new PlayStationVRTrackingStatus();
            while (trackingStatus == PlayStationVRTrackingStatus.NotStarted)
            {
                Tracker.GetTrackedDeviceStatus(deviceDualShock4.handle, out trackingStatus);
                yield return null;
            }

            // Get the color of the now tracked device
            PlayStationVRTrackingColor trackedColor;
            Tracker.GetTrackedDeviceLedColor(deviceDualShock4.handle, out trackedColor);

            // Apply the color to the relevant mesh component
            deviceDualShock4.light.material.color = GetUnityColor(trackedColor);
            deviceDualShock4.transform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Tracking failed for DeviceDualShock4! This may be because you're trying to register too many devices at once.");
        }
#else
        yield return null;
#endif
    }

    // Register PS VR Aim Controller to track
    IEnumerator RegisterAimController()
    {
#if UNITY_PS4
        if (PS4Input.AimIsConnected(0) == false)
        {
            Debug.LogError("Trying to register an Aim Controller, but none are connected!");
            yield break;
        }

        int[] handles = new int[2];
        PS4Input.PadGetUsersHandles(2, handles);
        PS4Input.DeviceType deviceType;
        PlayStationVRResult result;

        foreach (int handle in handles)
        {
            deviceType = (PS4Input.DeviceType)PS4Input.GetDeviceTypeForHandle(handle);

            if (deviceType == PS4Input.DeviceType.DeviceAim)
            {
                deviceAim.handle = handle;
                break;
            }
            else if (deviceType == PS4Input.DeviceType.DeviceUnknown)
            {
                Debug.LogError("Bad handle!");
                break;
            }
        }

        if (deviceAim.handle < 0)
            yield break;

        result = Tracker.RegisterTrackedDevice(PlayStationVRTrackedDevice.DeviceAim, deviceAim.handle, trackingType, trackerUsageType);

        if (result == PlayStationVRResult.Ok)
        {
            // Get the tracking, and wait for it to start
            PlayStationVRTrackingStatus trackingStatus = new PlayStationVRTrackingStatus();
            while (trackingStatus == PlayStationVRTrackingStatus.NotStarted)
            {
                Tracker.GetTrackedDeviceStatus(deviceAim.handle, out trackingStatus);
                yield return null;
            }

            // Get the color of the now tracked device
            PlayStationVRTrackingColor trackedColor;
            Tracker.GetTrackedDeviceLedColor(deviceAim.handle, out trackedColor);

            // Apply the color to the relevant mesh component
            deviceAim.light.material.color = GetUnityColor(trackedColor);
            deviceAim.transform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Tracking failed for DeviceAim! This may be because you're trying to register too many devices at once.");
        }
#else
        yield return null;
#endif
    }

    // Register Move device(s) to track
    IEnumerator RegisterMoveControllers()
    {
#if UNITY_PS4
        if (PS4Input.MoveIsConnected(0, 0) == false)
        {
            Debug.LogError("Trying to register the primary Move device, but it is not connected!");
            yield break;
        }

        if (PS4Input.MoveIsConnected(0, 1) == false)
        {
            Debug.LogError("Trying to register the secondary Move device, but it is not connected!");
            yield break;
        }

        int[] primaryHandles = new int[1];
        int[] secondaryHandles = new int[1];
        PS4Input.MoveGetUsersMoveHandles(1, primaryHandles, secondaryHandles);
        deviceMovePrimary.handle = primaryHandles[0];
        deviceMoveSecondary.handle = secondaryHandles[0];
        PlayStationVRResult result;
        PlayStationVRTrackingColor trackedColor;

        // Get the tracking for the primary Move device, and wait for it to start
        result = Tracker.RegisterTrackedDevice(PlayStationVRTrackedDevice.DeviceMove, deviceMovePrimary.handle, trackingType, trackerUsageType);

        if (result == PlayStationVRResult.Ok)
        {
            PlayStationVRTrackingStatus trackingStatusPrimary = new PlayStationVRTrackingStatus();

            while (trackingStatusPrimary == PlayStationVRTrackingStatus.NotStarted)
            {
                Tracker.GetTrackedDeviceStatus(deviceMovePrimary.handle, out trackingStatusPrimary);
                yield return null;
            }

            Tracker.GetTrackedDeviceLedColor(deviceMovePrimary.handle, out trackedColor);
            deviceMovePrimary.light.material.color = GetUnityColor(trackedColor);
            deviceMovePrimary.transform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Tracking failed for DeviceMove! This may be because you're trying to register too many devices at once.");
        }

        // Get the tracking for the secondary Move device, if needed, and wait for it to start
        if (deviceMoveSecondary.transform)
        {
            result = Tracker.RegisterTrackedDevice(PlayStationVRTrackedDevice.DeviceMove, deviceMoveSecondary.handle, trackingType, trackerUsageType);

            if (result == PlayStationVRResult.Ok)
            {
                PlayStationVRTrackingStatus trackingStatusSecondary = new PlayStationVRTrackingStatus();

                while (trackingStatusSecondary == PlayStationVRTrackingStatus.NotStarted)
                {
                    Tracker.GetTrackedDeviceStatus(deviceMoveSecondary.handle, out trackingStatusSecondary);
                    yield return null;
                }

                Tracker.GetTrackedDeviceLedColor(deviceMoveSecondary.handle, out trackedColor);
                deviceMoveSecondary.light.material.color = GetUnityColor(trackedColor);
                deviceMoveSecondary.transform.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Tracking failed for DeviceMove! This may be because you're trying to register too many devices at once.");
            }
        }
#else
        yield return null;
#endif
    }

    // Remove the registered devices from tracking and reset the transform
    public void UnregisterDevices()
    {
        // We can only unregister tracked devices while in VR, or else a crash may occur
        if (UnityEngine.XR.XRSettings.enabled)
        {
            if ((trackedDevicesType & TrackedDevicesType.DualShock4) == TrackedDevicesType.DualShock4 && deviceDualShock4.handle >= 0)
                UnregisterDualShock4();

            if ((trackedDevicesType & TrackedDevicesType.Aim) == TrackedDevicesType.Aim && deviceAim.handle >= 0)
                UnregisterAimController();

            if ((trackedDevicesType & TrackedDevicesType.Move) == TrackedDevicesType.Move && deviceMovePrimary.handle >= 0)
                UnregisterMoveControllers();
        }
    }

    // Remove the registered DualShock 4 device from tracking and reset the transform
    void UnregisterDualShock4()
    {
#if UNITY_PS4
        Tracker.UnregisterTrackedDevice(deviceDualShock4.handle);

        deviceDualShock4.handle = -1;
        deviceDualShock4.transform.gameObject.SetActive(false);
        deviceDualShock4.transform.localPosition = Vector3.zero;
        deviceDualShock4.transform.localRotation = Quaternion.identity;
#endif
    }

    // Remove the registered Aim device from tracking and reset the transform
    void UnregisterAimController()
    {
#if UNITY_PS4
        Tracker.UnregisterTrackedDevice(deviceAim.handle);
        deviceAim.handle = -1;

        deviceAim.transform.gameObject.SetActive(false);
        deviceAim.transform.localPosition = Vector3.zero;
        deviceAim.transform.localRotation = Quaternion.identity;
#endif
    }

    // Remove the registered Move devices from tracking and reset the transform
    void UnregisterMoveControllers()
    {
#if UNITY_PS4
        Tracker.UnregisterTrackedDevice(deviceMovePrimary.handle);

        deviceMovePrimary.handle = -1;
        deviceMovePrimary.transform.gameObject.SetActive(false);
        deviceMovePrimary.transform.localPosition = Vector3.zero;
        deviceMovePrimary.transform.localRotation = Quaternion.identity;

        if (deviceMoveSecondary.handle >= 0)
        {
            Tracker.UnregisterTrackedDevice(deviceMoveSecondary.handle);

            deviceMoveSecondary.handle = -1;
            deviceMovePrimary.transform.gameObject.SetActive(false);
            deviceMoveSecondary.transform.localPosition = Vector3.zero;
            deviceMoveSecondary.transform.localRotation = Quaternion.identity;
        }
#endif
    }

#if UNITY_PS4
    Color GetUnityColor(PlayStationVRTrackingColor trackingColor)
    {
        switch (trackingColor)
        {
            case PlayStationVRTrackingColor.Blue:
                return Color.blue;
            case PlayStationVRTrackingColor.Green:
                return Color.green;
            case PlayStationVRTrackingColor.Magenta:
                return Color.magenta;
            case PlayStationVRTrackingColor.Red:
                return Color.red;
            case PlayStationVRTrackingColor.Yellow:
                return Color.yellow;
            default:
                return Color.black;
        }
    }

    // HMD recenter happens in this event, which we will also use for tracked devices reset
    void OnSystemServiceEvent(Utility.sceSystemServiceEventType eventType)
    {
        switch (eventType)
        {
            case Utility.sceSystemServiceEventType.ResetVrPosition:
                ResetDeviceTracking();
                break;
        }
    }
#endif
}
