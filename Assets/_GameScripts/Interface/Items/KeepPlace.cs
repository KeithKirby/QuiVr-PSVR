using UnityEngine;
using System.Collections;

public class KeepPlace : MonoBehaviour {

    Transform prnt;
    Vector3 pos;
    Quaternion rot;
    Vector3 scl;

    public bool KeepPos = true;
    public bool KeepRot = true;
    public bool DestroyRB = true;
    public bool KeepScl = true;


    void Awake () {
        prnt = transform.parent;
        pos = transform.localPosition;
        rot = transform.localRotation;
        scl = transform.localScale;
    }

    void Update()
    {
        if (transform.parent != prnt)
        {
            if (GetComponent<Rigidbody>() != null && DestroyRB)
                Destroy(GetComponent<Rigidbody>());
            transform.SetParent(prnt);
            if(KeepScl)
                transform.localScale = scl;
            if(KeepRot)
                transform.localRotation = rot;
            if(KeepPos)
                transform.localPosition = pos;
        }
    }
}
