using UnityEngine;
using System.Collections;

public class PullAnchor : MonoBehaviour {

    Vector3 spos;
    Quaternion srot;

    void Awake()
    {
        spos = transform.localPosition;
        srot = transform.localRotation;
        InvokeRepeating("FixPose", 5f, 1.5f);
    }

    void FixPose()
    {
        transform.localPosition = spos;
        transform.localRotation = srot;
    }

}
