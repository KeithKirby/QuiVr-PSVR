using UnityEngine;
using System;
using ObserVR;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

public class ObserVR_360Video : MonoBehaviour {
    private static ObserVR_ProjectSettings settings;
    private static Camera cam;
    [HideInInspector]
    public float sphereRadius;
#if UNITY_5_6_OR_NEWER && OBSERVR_AVPRO
    [HideInInspector]
    public ObserVR_TrackedVideo.VideoPlayerType playerType;
    [HideInInspector]
    public VideoPlayer nativePlayer;
    [HideInInspector]
    public RenderHeads.Media.AVProVideo.MediaPlayer AVProPlayer;
#elif UNITY_5_6_OR_NEWER
    [HideInInspector]
    public VideoPlayer nativePlayer;
#elif OBSERVR_AVPRO
    [HideInInspector]
    public RenderHeads.Media.AVProVideo.MediaPlayer AVProPlayer;
#endif

    void Start() {
        settings = Resources.Load<ObserVR_ProjectSettings>("ObserVR_ProjectSettings");
        cam = Camera.main ?? FindObjectOfType<Camera>();

        if (settings == null || string.IsNullOrEmpty(settings.applicationID)) {
            ObserVR_Analytics.AnalyticsEnabled = false;
            enabled = false;
        }
        else {
#if UNITY_5_6_OR_NEWER && OBSERVR_AVPRO
            if (playerType == ObserVR_TrackedVideo.VideoPlayerType.NativeVideoPlayer) {
                if (nativePlayer.source == VideoSource.Url) {
                    ObserVR_Events.CustomEvent("VIDEO_URL").AddParameter("url", nativePlayer.url).EndParameters();
                }
                InvokeRepeating("SendFOVPointNative", 0.1f, settings.generalFOVInterval);
            }
            else if (playerType == ObserVR_TrackedVideo.VideoPlayerType.AVProVideoPlayer) {
                if (AVProPlayer.m_VideoLocation == RenderHeads.Media.AVProVideo.MediaPlayer.FileLocation
                        .AbsolutePathOrURL) {
                    ObserVR_Events.CustomEvent("VIDEO_URL").AddParameter("url", AVProPlayer.m_VideoPath)
                        .EndParameters();
                }
                InvokeRepeating("SendFOVPointAVPro", 0.1f, settings.generalFOVInterval);
            }
#elif UNITY_5_6_OR_NEWER
            InvokeRepeating("SendFOVPointNative", 0.1f, settings.generalFOVInterval);
#elif OBSERVR_AVPRO
            InvokeRepeating("SendFOVPointAVPro", 0.1f, settings.generalFOVInterval);
#endif
        }
    }

#if OBSERVR_AVPRO
    private void SendFOVPointAVPro() {
        RaycastHit hit;
        Ray cursorRay = new Ray(transform.position, -cam.transform.forward);
        if (Physics.Raycast(cursorRay, out hit, 200)) {
            if (AVProPlayer.Control.IsPlaying()) {
                double lat = Math.Round(90 - (float)Math.Acos(hit.point.y / sphereRadius) * 180 / Math.PI);
                double lon = Math.Round(((180 + (float)Math.Atan2(hit.point.x, hit.point.z) * 180 / Math.PI) % 360) - 180);
#if UNITY_EDITOR
                if (settings.debugMode) {
                    Debug.DrawLine(hit.point, cam.transform.position, Color.red);
                }
#endif
                ObserVR_Events.CustomEvent("GENERAL_FOV").AddParameter("videoTime", (Math.Round(AVProPlayer.Control.GetCurrentTimeMs() / 100d, 0) * 100)).AddParameter("hitLat", lat).AddParameter("hitLon", lon).EndParameters();
            }
        }
    }
#endif

#if UNITY_5_6_OR_NEWER
    private void SendFOVPointNative() {
        RaycastHit hit;
        Ray cursorRay = new Ray(transform.position, -cam.transform.forward);
        if (Physics.Raycast(cursorRay, out hit, 200)) {
            if (nativePlayer.isPlaying) {
                double lat = Math.Round(90 - (float)Math.Acos(hit.point.y / sphereRadius) * 180 / Math.PI);
                double lon = Math.Round(((180 + (float)Math.Atan2(hit.point.x, hit.point.z) * 180 / Math.PI) % 360) - 180);
#if UNITY_EDITOR
                if (settings.debugMode) {
                    Debug.DrawLine(hit.point, cam.transform.position, Color.red);
                }
#endif
                ObserVR_Events.CustomEvent("GENERAL_FOV").AddParameter("videoTime", (Math.Round((nativePlayer.time * 1000) / 100d, 0) * 100)).AddParameter("hitLat", lat).AddParameter("hitLon", lon).EndParameters();
            }
        }
    }
#endif
}
