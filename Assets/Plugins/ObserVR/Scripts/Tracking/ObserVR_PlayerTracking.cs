using UnityEngine;
using ObserVR;
#if OBSERVR_VRTK
using VRTK;
#endif
#if OBSERVR_SVR
using Valve.VR;
#endif

[DisallowMultipleComponent]
public class ObserVR_PlayerTracking : MonoBehaviour {
    private static ObserVR_ProjectSettings settings;
    private static Camera cam;
    private static bool lostTrackingGlobal;
    private static float _timeCounterPosition;
#if OBSERVR_SVR
    private static float _timeCounterPoses;
#endif

#if OBSERVR_VRTK
    private VRTK_BasicTeleport teleporter;
#endif

    void Start() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");

        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            Debug.LogError("Couldn't find settings file or Application ID is missing...stopping analytics");
            ObserVR_Analytics.AnalyticsEnabled = false;
        }
        else {
            cam = Camera.main ?? FindObjectOfType<Camera>();
        }

#if OBSERVR_VRTK
        if (settings != null && settings.useTeleportation) {
            teleporter = FindObjectOfType<VRTK_BasicTeleport>();
            if (teleporter != null) {
                teleporter.Teleported += SendTeleportLocation;
            }
        }
#endif
    }

    void Update() {
        if (cam == null) {
            cam = Camera.main ?? FindObjectOfType<Camera>();
        }
        else {
            if (_timeCounterPosition < settings.positionalTrackingInterval) {
                _timeCounterPosition += Time.unscaledDeltaTime;
            }
            else {
                if (!lostTrackingGlobal) {
                    var rotX = cam.transform.rotation.eulerAngles.x <= 180.0f ? -cam.transform.rotation.eulerAngles.x : 360.0f - cam.transform.rotation.eulerAngles.x;
                    var rotY = cam.transform.rotation.eulerAngles.y <= 180.0f ? -cam.transform.rotation.eulerAngles.y : 360.0f - cam.transform.rotation.eulerAngles.y;
                    var rotZ = cam.transform.rotation.eulerAngles.z <= 180.0f ? -cam.transform.rotation.eulerAngles.z : 360.0f - cam.transform.rotation.eulerAngles.z;

                    ObserVR_Events.CustomEvent("POSITION")
                    .AddParameter("x", cam.transform.position.x)
                    .AddParameter("y", cam.transform.position.y)
                    .AddParameter("z", cam.transform.position.z)
                    .AddParameter("rotX", -rotX)
                    .AddParameter("rotY", -rotY)
                    .AddParameter("rotZ", -rotZ)
                    .EndParameters();
                }
#if OBSERVR_OVR && OBSERVR_SVR
                if (SteamVR.instance == null && OVRPlugin.hmdPresent) {
                    var rotX = cam.transform.ToOVRPose().orientation.eulerAngles.x <= 180.0f ? -cam.transform.ToOVRPose().orientation.eulerAngles.x : 360.0f - cam.transform.ToOVRPose().orientation.eulerAngles.x;
                    var rotY = cam.transform.ToOVRPose().orientation.eulerAngles.y <= 180.0f ? -cam.transform.ToOVRPose().orientation.eulerAngles.y : 360.0f - cam.transform.ToOVRPose().orientation.eulerAngles.y;
                    var rotZ = cam.transform.ToOVRPose().orientation.eulerAngles.z <= 180.0f ? -cam.transform.ToOVRPose().orientation.eulerAngles.z : 360.0f - cam.transform.ToOVRPose().orientation.eulerAngles.z;

                    ObserVR_Events.CustomEvent("PHYSICAL_POSITION")
                    .AddParameter("x", cam.transform.ToOVRPose().position.x)
                    .AddParameter("y", cam.transform.ToOVRPose().position.y)
                    .AddParameter("z", cam.transform.ToOVRPose().position.z)
                    .AddParameter("rotX", -rotX)
                    .AddParameter("rotY", -rotY)
                    .AddParameter("rotZ", -rotZ)
                    .EndParameters();
                }
#elif OBSERVR_OVR
                if (OVRPlugin.hmdPresent) {
                    var rotX = cam.transform.ToOVRPose().orientation.eulerAngles.x <= 180.0f ? -cam.transform.ToOVRPose().orientation.eulerAngles.x : 360.0f - cam.transform.ToOVRPose().orientation.eulerAngles.x;
                    var rotY = cam.transform.ToOVRPose().orientation.eulerAngles.y <= 180.0f ? -cam.transform.ToOVRPose().orientation.eulerAngles.y : 360.0f - cam.transform.ToOVRPose().orientation.eulerAngles.y;
                    var rotZ = cam.transform.ToOVRPose().orientation.eulerAngles.z <= 180.0f ? -cam.transform.ToOVRPose().orientation.eulerAngles.z : 360.0f - cam.transform.ToOVRPose().orientation.eulerAngles.z;

                    ObserVR_Events.CustomEvent("PHYSICAL_POSITION")
                        .AddParameter("x", cam.transform.ToOVRPose().position.x)
                        .AddParameter("y", cam.transform.ToOVRPose().position.y)
                        .AddParameter("z", cam.transform.ToOVRPose().position.z)
                        .AddParameter("rotX", -rotX)
                        .AddParameter("rotY", -rotY)
                        .AddParameter("rotZ", -rotZ)
                        .EndParameters();
                }
#endif
                _timeCounterPosition = 0.0f;
            }
        }
    }

#if OBSERVR_VRTK
    private void SendTeleportLocation(object sender, DestinationMarkerEventArgs e) {
        string button = "unknown";
        string hand = "unknown";
#if VRTK_VERSION_3_2_0_OR_NEWER
        button = e.controllerReference != null &&
                   e.controllerReference.actual.GetComponentInChildren<VRTK_Pointer>()
          ? e.controllerReference.actual.GetComponentInChildren<VRTK_Pointer>().activationButton.ToString()
          : "unknown";
        hand = e.controllerReference != null ? e.controllerReference.hand.ToString() : "unknown";
#endif
        ObserVR_Events.CustomEvent("TELEPORT")
            .AddParameter("x", e.destinationPosition.x)
            .AddParameter("y", e.destinationPosition.y)
            .AddParameter("z", e.destinationPosition.z)
            .AddParameter("buttonUsed", button)
            .AddParameter("hand", hand)
            .EndParameters();
    }
#endif

    void OnEnable() {
#if OBSERVR_SVR
        SteamVR_Events.OutOfRange.Listen(LostTracking);
        SteamVR_Events.NewPoses.Listen(NewPosesMethod);
#endif
#if OBSERVR_OVR
        OVRManager.HMDMounted += HeadsetPutOn;
        OVRManager.HMDUnmounted += HeadetTakenOff;
        OVRManager.TrackingAcquired += LostOculusTracking;
        OVRManager.TrackingLost += RegainedOculusTracking;
#endif
    }

    void OnDisable() {
#if OBSERVR_SVR
        SteamVR_Events.OutOfRange.Remove(LostTracking);
        SteamVR_Events.NewPoses.Remove(NewPosesMethod);
#endif
#if OBSERVR_OVR
        OVRManager.HMDMounted -= HeadsetPutOn;
        OVRManager.HMDUnmounted -= HeadetTakenOff;
        OVRManager.TrackingAcquired -= LostOculusTracking;
        OVRManager.TrackingLost -= RegainedOculusTracking;
#endif
    }

#if OBSERVR_OVR
    private void RegainedOculusTracking() {
        ObserVR_Events.CustomEvent("REGAINED_TRACKING").NoParameters();
    }

    private void LostOculusTracking() {
        ObserVR_Events.CustomEvent("LOST_TRACKING").NoParameters();
    }

    private void HeadetTakenOff() {
        ObserVR_Events.CustomEvent("HEADSET_OFF").NoParameters();
    }

    private void HeadsetPutOn() {
        ObserVR_Events.CustomEvent("HEADSET_ON").NoParameters();
    }
#endif

#if OBSERVR_SVR
    private void NewPosesMethod(TrackedDevicePose_t[] arg0) {
        if (_timeCounterPoses < settings.positionalTrackingInterval) {
            _timeCounterPoses += Time.unscaledDeltaTime;
        }
        else {
            if (!lostTrackingGlobal) {
                ObserVR_Events.CustomEvent("PHYSICAL_POSITION")
                    .AddParameter("x", ConvertHmdMatrix34ToVector3(arg0[0].mDeviceToAbsoluteTracking).x)
                    .AddParameter("y", ConvertHmdMatrix34ToVector3(arg0[0].mDeviceToAbsoluteTracking).y)
                    .AddParameter("z", ConvertHmdMatrix34ToVector3(arg0[0].mDeviceToAbsoluteTracking).z)
                    .AddParameter("rotX", PoseMatrix2RotationEuler(arg0[0].mDeviceToAbsoluteTracking).x)
                    .AddParameter("rotY", PoseMatrix2RotationEuler(arg0[0].mDeviceToAbsoluteTracking).y)
                    .AddParameter("rotZ", PoseMatrix2RotationEuler(arg0[0].mDeviceToAbsoluteTracking).z)
                    .EndParameters();
            }
            _timeCounterPoses = 0.0f;
        }
    }

    private static Vector3 ConvertHmdMatrix34ToVector3(HmdMatrix34_t matrix) {
        return new Vector3(matrix.m3, matrix.m7, -matrix.m11);
    }

    private static Vector3 PoseMatrix2RotationEuler(HmdMatrix34_t poseMatrix) {
        Vector2 vector_r32_r33 = new Vector2(poseMatrix.m9, poseMatrix.m10);
        float eulerPitch = Mathf.Atan2(poseMatrix.m9, poseMatrix.m10) * Mathf.Rad2Deg;
        float eulerYaw = Mathf.Atan2(-poseMatrix.m8, vector_r32_r33.magnitude) * Mathf.Rad2Deg;
        float eulerRoll = Mathf.Atan2(poseMatrix.m4, poseMatrix.m0) * Mathf.Rad2Deg;
        return new Vector3(-eulerPitch, -eulerYaw, eulerRoll);
    }

    private static void LostTracking(bool lostTracking) {
        lostTrackingGlobal = lostTracking;
        if (lostTracking) {
            ObserVR_Events.CustomEvent("LOST_TRACKING").NoParameters();
        }
        else {
            ObserVR_Events.CustomEvent("REGAINED_TRACKING").NoParameters();
        }
    }
#endif
}
