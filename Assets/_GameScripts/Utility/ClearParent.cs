using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearParent : MonoBehaviour {

    Vector3 startPos;
    Quaternion startRot;
    Transform startParent;

    void Awake()
    {
        if(transform.parent != null)
        {
            startParent = transform.parent;
            startPos = transform.localPosition;
            startRot = transform.localRotation;
        }
    }

	public void Activate()
    {
        transform.SetParent(null);
    }

    public void ResetParent()
    {
        if(startParent != null)
        {
            transform.SetParent(startParent);
            transform.localPosition = startPos;
            transform.localRotation = startRot;
        }
    }

    public void DestroyTimed(float t)
    {
        Destroy(gameObject, t);
    }
}
