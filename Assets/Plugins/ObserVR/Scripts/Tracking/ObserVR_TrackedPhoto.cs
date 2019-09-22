using UnityEngine;

public class ObserVR_TrackedPhoto : MonoBehaviour {
    private GameObject sphere;
    private GameObject g;
    private static Camera cam;

    void Start() {
        cam = Camera.main ?? FindObjectOfType<Camera>();
        sphere = GetComponentInChildren<SphereCollider>().gameObject;
        g = new GameObject();
        g.transform.SetParent(cam.transform);
        g.transform.localPosition = new Vector3(0, 0, sphere.transform.localScale.z * 2);
        g.name = "ObserVR_360PhotoTracking";
        g.AddComponent<ObserVR_360Photo>().sphereRadius = sphere.transform.localScale.z;
    }
}
