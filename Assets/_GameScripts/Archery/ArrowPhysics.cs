using UnityEngine;
using System.Collections;

public class ArrowPhysics : MonoBehaviour {

    public Transform Tail;
    Rigidbody rb;
    TrailRenderer tr;

    public float ExtraGravity;

    [HideInInspector]
    public bool IgnoreExtraGravity ;

    public void Init()
    {
        Start();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(rb != null && !rb.isKinematic && rb.velocity.magnitude > 0.25f)
        {
            if(!IgnoreExtraGravity)
                rb.AddForce(Vector3.down * ExtraGravity * Time.fixedDeltaTime, ForceMode.Acceleration);
            Vector3 direction = rb.velocity.normalized;
            Quaternion toRotation = Quaternion.LookRotation(rb.velocity.normalized, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rb.velocity.magnitude * Time.deltaTime);
        }
	}
}
