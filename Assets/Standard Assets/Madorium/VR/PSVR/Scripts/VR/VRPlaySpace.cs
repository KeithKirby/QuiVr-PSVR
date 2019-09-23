using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4.VR;
#endif

public class VRPlaySpace : MonoBehaviour
{
    public Transform frustumTransform;
    public Renderer[] frustumRenderers;
    public float safeDistance = 0.1f;
    public float fadeSpeed = 3f;
    public Color showColor, hideColor;

    bool showFrustum = false;
    Vector3 camAcceleration;
    Vector3 hmdPositionRaw;
    Quaternion hmdRotationUnity, hmdRotationRaw;
#if UNITY_PS4
    PlayStationVRPlayAreaWarningInfo info;
    PlayStationVRTrackingStatus status;
#endif

    void Start()
    {
        foreach (Renderer fR in frustumRenderers)
        {
            fR.material.color = hideColor;
        }
    }

    void Update()
    {
#if UNITY_PS4
        if (UnityEngine.XR.XRSettings.enabled)
        {
            PlayStationVR.GetPlayAreaWarningInfo(out info);

            // Show/hide the frustum if the HMD is too close to the edge of the play space
            if (info.distanceFromHorizontalBoundary < safeDistance || info.distanceFromVerticalBoundary < safeDistance)
            {
                if (showFrustum == false)
                {
                    UpdateFrustumTransform();
                    showFrustum = true;
                }
            }
            else if (showFrustum == true)
            {
                showFrustum = false;
            }

            UpdateFrustumDisplay();
        }
#endif
    }

    void UpdateFrustumTransform()
    {
#if UNITY_PS4
        int hmdHandle = PlayStationVR.GetHmdHandle();

        Tracker.GetTrackedDevicePosition(hmdHandle, PlayStationVRSpace.Raw, out hmdPositionRaw);

        // Convert from RAW device space into Unity Left handed space.
        hmdPositionRaw.z = -hmdPositionRaw.z;
        Tracker.GetTrackedDeviceOrientation(hmdHandle, PlayStationVRSpace.Unity, out hmdRotationUnity);
        Tracker.GetTrackedDeviceOrientation(hmdHandle, PlayStationVRSpace.Raw, out hmdRotationRaw);

        Quaternion hmdRotationRawInUnity = hmdRotationRaw;
        hmdRotationRawInUnity.z = -hmdRotationRawInUnity.z;
        hmdRotationRawInUnity.w = -hmdRotationRawInUnity.w;
        Quaternion q = Quaternion.Inverse(hmdRotationRawInUnity * Quaternion.Inverse(hmdRotationUnity));

        frustumTransform.position = (Camera.main.transform.position + (q * (-hmdPositionRaw)));

        PlayStationVR.GetCameraAccelerationVector(out camAcceleration);
        Quaternion cameraOrientation = Quaternion.FromToRotation(new Vector3(-camAcceleration.x, camAcceleration.y, -camAcceleration.z), new Vector3(0, 1, 0));
        frustumTransform.rotation = q * cameraOrientation;
#endif
    }

    void UpdateFrustumDisplay()
    {
        foreach(Renderer fR in frustumRenderers)
        {
            if (showFrustum)
                fR.material.color = Color.Lerp(fR.material.color, showColor, Time.deltaTime * fadeSpeed);
            else
                fR.material.color = Color.Lerp(fR.material.color, hideColor, Time.deltaTime * fadeSpeed * 2);
        }
    }
}
