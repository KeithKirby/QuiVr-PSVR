using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToGround : MonoBehaviour {

    public bool InvertDir;
    public bool OverrideDir;
    public Vector3 OverrideVect;

    void Start()
    {
        RaycastHit hit;
        int layer = 13;
        int mask = ~(1 << layer);
        if (Physics.Raycast(new Ray(transform.position + (transform.TransformDirection(Vector3.up) * 0.1f), transform.TransformDirection(Vector3.down)), out hit, 2f, mask))
        {
            Vector3 norm = hit.normal;
            transform.position += norm * 0.05f;
            int i = -1;
            if (InvertDir)
                i = 1;
            transform.rotation = Quaternion.LookRotation(norm * i);
            transform.Rotate(Vector3.left, 90);
        }
        if (OverrideDir)
            transform.eulerAngles = OverrideVect;
    }
}
