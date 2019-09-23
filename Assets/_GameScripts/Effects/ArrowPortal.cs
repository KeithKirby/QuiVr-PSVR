using UnityEngine;
using System.Collections;

public class ArrowPortal : MonoBehaviour {

    public Transform PortalOutPosition;
    public Vector3 InNormal;
    public Vector3 OutNormal;

    GameObject lastArrow;

    public Transform dummy;

    void Update()
    {
        Vector3 worldOffset = dummy.transform.position - transform.position;
        Vector3 offset = PortalOutPosition.TransformDirection(worldOffset);
        Vector3 pos = PortalOutPosition.position;
        Vector3 reflect = Vector3.Reflect(transform.TransformDirection(dummy.forward), transform.TransformDirection(InNormal));
        Vector3 vel = PortalOutPosition.TransformDirection(reflect);
    }

	void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<Arrow>() != null && lastArrow != col.gameObject)
        {
            lastArrow = col.gameObject;
            Rigidbody r = col.GetComponent<Rigidbody>();
            Vector3 reflect = Vector3.Reflect(transform.TransformDirection(r.velocity), transform.TransformDirection(InNormal));
            reflect.x *= -1;
            Vector3 vel = PortalOutPosition.TransformDirection(reflect);
            r.velocity = vel.normalized*r.velocity.magnitude;
            r.transform.LookAt(transform.position + vel.normalized);
            Vector3 offset = transform.TransformDirection(transform.position - r.transform.position);
            r.transform.position = PortalOutPosition.position;
            TrailRenderer tr = lastArrow.GetComponentInChildren<TrailRenderer>();
            if(tr != null)
            {
                Material m = tr.material;
                float sw = tr.startWidth;
                float ew = tr.endWidth;
                float tm = tr.time;
                Destroy(tr);
                tr = lastArrow.AddComponent<TrailRenderer>();
                tr.material = m;
                tr.startWidth = sw;
                tr.endWidth = ew;
                tr.time = tm;
            }
            
        }
    }
}
