using UnityEngine;
using System.Collections;

public class RotateTimed : MonoBehaviour {

    public Vector3 Rotation;
    public bool world;
    Renderer r;
    public bool paused;
    public bool needRenderer = true;

    void Start()
    {
        r = GetComponent<Renderer>();
    }
	
	void Update () {
        if (paused)
            return;
        if(needRenderer)
        {
            if (r == null || !r.enabled || !r.isVisible)
                return;
        }
        if (world)
            transform.eulerAngles += Rotation * Time.deltaTime;
        else
            transform.localEulerAngles += Rotation * Time.deltaTime;
       
	}
}
