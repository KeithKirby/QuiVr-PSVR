using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsSlow : MonoBehaviour {

    [Range(0.01f, 0.9f)]
    public float SpeedPerc;
    public List<Rigidbody> Inside;
    List<Rigidbody> EverInside;
    AoEEffect e;

    void Awake()
    {
        Inside = new List<Rigidbody>();
        EverInside = new List<Rigidbody>();
        e = GetComponent<AoEEffect>();
    }

    void OnTriggerEnter(Collider col)
    {
        Rigidbody r = col.GetComponent<Rigidbody>();
        if (r != null && !r.isKinematic && !Inside.Contains(r) && !EverInside.Contains(r) )
        {
            r.velocity *= SpeedPerc;
            r.angularVelocity *= SpeedPerc;
            if (r.GetComponent<ArrowPhysics>() != null)
                r.GetComponent<ArrowPhysics>().IgnoreExtraGravity = true;
            Inside.Add(r);
            EverInside.Add(r);
        }
    }

    void OnTriggerExit(Collider col)
    {
        Rigidbody r = col.GetComponent<Rigidbody>();
        if(r != null && Inside.Contains(r))
        {
            if (r.GetComponent<ArrowPhysics>() != null)
                r.GetComponent<ArrowPhysics>().IgnoreExtraGravity = false;
            Remove(r);
        }
    }

    public void RemoveAll()
    {
        for(int i=Inside.Count-1; i>=0; i--)
        {
            var r = Inside[i];
            if (r != null)
                Remove(r);
        }
        Inside = new List<Rigidbody>();
    }

    void Remove(Rigidbody r)
    {
        r.velocity *= 1f / SpeedPerc;
        r.angularVelocity *= 1f / SpeedPerc;
        if (r.GetComponentInParent<Creature>() != null)
        {
            while (r.velocity.magnitude > 25)
            {
                r.velocity *= 0.75f;
            }
            while (r.angularVelocity.magnitude > 25)
            {
                r.angularVelocity *= 0.75f;
            }
        }
        Inside.Remove(r);
    }

    void FixedUpdate()
    {
        for(int i=Inside.Count-1; i>= 0; i--)
        {
            if (Inside[i] == null)
                Inside.RemoveAt(i);
        }
        foreach(var v in Inside)
        {
            if(v != null && v.useGravity)
                v.AddForce((-1f*Physics.gravity));
        }
        if (e != null && !e.enabled)
        {
            RemoveAll();
            this.enabled = false;
        } 
    }
}
