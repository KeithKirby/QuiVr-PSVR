using UnityEngine;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

public class ObserVR_TrackedVideo : MonoBehaviour {
    private GameObject sphere;
    private GameObject g;
    private static Camera cam;

    public enum VideoPlayerType {
        NativeVideoPlayer,
        AVProVideoPlayer
    }

#if UNITY_5_6_OR_NEWER && OBSERVR_AVPRO
    public VideoPlayerType playerType;
#endif

    void Start() {
        cam = Camera.main ?? FindObjectOfType<Camera>();
        sphere = GetComponentInChildren<SphereCollider>().gameObject;
        g = new GameObject();
        g.transform.SetParent(cam.transform);
        g.transform.localPosition = new Vector3(0, 0, sphere.transform.localScale.z * 2);
        g.name = "ObserVR_360VideoTracking";
        g.AddComponent<ObserVR_360Video>().sphereRadius = sphere.transform.localScale.z;
#if UNITY_5_6_OR_NEWER && OBSERVR_AVPRO
        g.GetComponent<ObserVR_360Video>().playerType = playerType;
        if (playerType == VideoPlayerType.NativeVideoPlayer) {
            g.GetComponent<ObserVR_360Video>().nativePlayer = GetComponentInChildren<VideoPlayer>();
        }
        else if (playerType == VideoPlayerType.AVProVideoPlayer) {
            g.GetComponent<ObserVR_360Video>().AVProPlayer = GetComponentInChildren<RenderHeads.Media.AVProVideo.MediaPlayer>();
        }
#elif UNITY_5_6_OR_NEWER
        g.GetComponent<ObserVR_360Video>().nativePlayer = GetComponentInChildren<VideoPlayer>();
#elif OBSERVR_AVPRO
        g.GetComponent<ObserVR_360Video>().AVProPlayer = GetComponentInChildren<RenderHeads.Media.AVProVideo.MediaPlayer>();
#endif
    }
}
