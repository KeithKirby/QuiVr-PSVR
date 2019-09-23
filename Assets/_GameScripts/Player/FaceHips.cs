using UnityEngine;
using System.Collections;

public class FaceHips : MonoBehaviour {

    public float ForwardOffset;
    public bool active;

    void Update()
    {
        if(active && Hips.instance != null)
        {
            transform.LookAt(Hips.instance.transform.position + (Hips.instance.transform.forward * ForwardOffset));
        }
    }
}
