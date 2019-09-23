using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTornado : MonoBehaviour {

    public List<Rigidbody> Inside;
    AoEEffect e;

    public float RotateForce;
    public float PullForce;
    public float TorqueMult;

    void Awake()
    {
        Inside = new List<Rigidbody>();
        e = GetComponent<AoEEffect>();
    }

    void OnTriggerStay(Collider col)
    {
        Rigidbody r = col.GetComponent<Rigidbody>();
        if (r != null && !r.isKinematic && !Inside.Contains(r))
        {
            Inside.Add(r);
        }
    }

    void OnTriggerExit(Collider col)
    {
        Rigidbody r = col.GetComponent<Rigidbody>();
        if (r != null && Inside.Contains(r))
        {
            Remove(r);
        }
    }

    void Remove(Rigidbody r)
    {
        Inside.Remove(r);
    }

    void FixedUpdate()
    {
        foreach (var v in Inside)
        {
            if (v != null)
            {
                if(v.useGravity)
                    v.AddForce((-0.95f * Physics.gravity), ForceMode.Acceleration);
                Vector3 dir = ((transform.position+transform.up) - v.position).normalized;
                Vector3 pull = Time.fixedDeltaTime * dir * PullForce * (1 / Mathf.Max(Vector3.Distance(v.transform.position, transform.position + transform.up), 0.9f));
                Vector3 rot = Vector3.Cross(transform.up, dir) * RotateForce * Time.fixedDeltaTime;
                Debug.DrawRay(v.transform.position, pull/50f, Color.blue);
                Debug.DrawRay(v.transform.position, rot/50f, Color.red);
                v.AddForce(pull);
                v.AddForce(rot);
                v.AddTorque(rot* TorqueMult * Random.Range(1.05f, 0.95f));
                v.velocity *= 0.95f;
            } 
        }
        if (e != null && !e.enabled)
        {
            RemoveAll();
            this.enabled = false;
        }
    }

    public void RemoveAll()
    {
        for (int i = Inside.Count - 1; i >= 0; i--)
        {
            var r = Inside[i];
            if (r != null)
                Remove(r);
        }
        Inside = new List<Rigidbody>();
    }

}
