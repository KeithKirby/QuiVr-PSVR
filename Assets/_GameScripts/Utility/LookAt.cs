using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

    public Transform target;
    public bool isCamera;
    public bool invert;
    public bool visible = true;
    public bool ignoreZ;
    public bool alwaysVisible;

    public Vector3 AngleOffset;

    Renderer r;

    void Awake()
    {
        r = GetComponent<Renderer>();
        if (r == null)
            r = GetComponentInChildren<Renderer>();

        if (target != null)
        {
            if (invert)
                transform.LookAt(transform.position - (target.position - transform.position).normalized);
            else
                transform.LookAt(target);
        }
    }

	void LateUpdate ()
    {
        if(!alwaysVisible)
        {
            if (!visible)
                return;
        }
        if(isCamera)
        {
            if(PlayerHead.instance != null)
            {
                target = PlayerHead.instance.transform;
                if (invert)
                    transform.LookAt(transform.position - (target.position - transform.position).normalized);
                else
                    transform.LookAt(target);
                if(AngleOffset != Vector3.zero)
                {
                    Vector3 r = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(r.x + AngleOffset.x, r.y + AngleOffset.y, r.z + AngleOffset.z);
                }
                return;
            }
        }
        else if(target != null && target.gameObject.activeSelf)
        {
            if (invert)
                transform.LookAt(transform.position - (target.position - transform.position).normalized);
            else
                transform.LookAt(target);
        }
        if(ignoreZ)
        {
            Vector3 v = transform.localEulerAngles;
            v.z = 0;
            transform.localEulerAngles = v;
        }
        Vector3 q = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(q.x + AngleOffset.x, q.y + AngleOffset.y, q.z + AngleOffset.z);
    }
}
