using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;
using UnityEngine.PS4.VR;
#endif

namespace Madorium
{
    public class PSVRCamTracker : MonoBehaviour
    {
        public Transform frustumTransform;
        //public Renderer[] frustumRenderers;
        public float safeDistance = 0.1f;
        public float fadeSpeed = 3f;
        public Color showColor, hideColor;

        bool m_ShowFrustum = false;
        Vector3 m_CamAcceleration;
        Vector3 m_HmdPositionRaw;
        Quaternion m_HmdRotationUnity, m_HmdRotationRaw;
#if UNITY_PS4
        PlayStationVRPlayAreaWarningInfo m_Info;
        PlayStationVRTrackingStatus m_Status;
#endif

        void Start()
        {
            /*foreach (var fR in frustumRenderers)
            {
                fR.material.color = hideColor;
            }*/
        }

        void Update()
        {
#if UNITY_PS4
            if (UnityEngine.XR.XRSettings.enabled)
            {
                UpdateFrustumTransform();
                /*
                PlayStationVR.GetPlayAreaWarningInfo(out m_Info);

                // Show/hide the frustum if the HMD is too close to the edge of the play space
                if (m_Info.distanceFromHorizontalBoundary < safeDistance || m_Info.distanceFromVerticalBoundary < safeDistance)
                {
                    if (m_ShowFrustum == false)
                    {
                        UpdateFrustumTransform();
                        m_ShowFrustum = true;
                    }
                }
                else if (m_ShowFrustum == true)
                {
                    m_ShowFrustum = false;
                }
                */

                //UpdateFrustumDisplay();
            }
#endif
        }

        void UpdateFrustumTransform()
        {
#if UNITY_PS4
            var hmdHandle = PlayStationVR.GetHmdHandle();

            Tracker.GetTrackedDevicePosition(hmdHandle, PlayStationVRSpace.Raw, out m_HmdPositionRaw);

            // Convert from RAW device space into Unity Left handed space.
            m_HmdPositionRaw.z = -m_HmdPositionRaw.z;
            Tracker.GetTrackedDeviceOrientation(hmdHandle, PlayStationVRSpace.Unity, out m_HmdRotationUnity);
            Tracker.GetTrackedDeviceOrientation(hmdHandle, PlayStationVRSpace.Raw, out m_HmdRotationRaw);

            var hmdRotationRawInUnity = m_HmdRotationRaw;
            hmdRotationRawInUnity.z = -hmdRotationRawInUnity.z;
            hmdRotationRawInUnity.w = -hmdRotationRawInUnity.w;
            var q = Quaternion.Inverse(hmdRotationRawInUnity * Quaternion.Inverse(m_HmdRotationUnity));

            //var toCamera = q * (-m_HmdPositionRaw);
            //var flippedToCamera = Quaternion.Euler(0, 180, 0) * toCamera;
            //frustumTransform.position = Camera.main.transform.position + flippedToCamera;

            var toCamera = q * (-m_HmdPositionRaw);

            // Take player rotation into account
            //toCamera = Quaternion.Euler(0, rotateSource.localEulerAngles.y, 0) * toCamera;

            //frustumTransform.localPosition = Camera.main.transform.position + toCamera;
            frustumTransform.localPosition = toCamera;

            //PlayStationVR.GetCameraAccelerationVector(out m_CamAcceleration);
            //var cameraOrientation = Quaternion.FromToRotation(new Vector3(-m_CamAcceleration.x, m_CamAcceleration.y, -m_CamAcceleration.z), new Vector3(0, 1, 0));
            //var cameraOrientation = Quaternion.FromToRotation(new Vector3(m_CamAcceleration.x, m_CamAcceleration.y, m_CamAcceleration.z), new Vector3(0, 1, 0));
            //cameraOrientation = Quaternion.Euler(0, -rotateSource.localEulerAngles.y, 0) * cameraOrientation;
            //frustumTransform.rotation = q * cameraOrientation;
#endif
        }
        /*
        void UpdateFrustumDisplay()
        {
            foreach (var fR in frustumRenderers)
            {
                if (m_ShowFrustum)
                    fR.material.color = Color.Lerp(fR.material.color, showColor, Time.deltaTime * fadeSpeed);
                else
                    fR.material.color = Color.Lerp(fR.material.color, hideColor, Time.deltaTime * fadeSpeed * 2);
            }
        }
        */
    }
}