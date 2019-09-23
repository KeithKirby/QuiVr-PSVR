using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Breakable : MonoBehaviour {

    Vector3 spos;
    Quaternion srot;
    MeshRenderer Base;
    Collider col;
    Rigidbody rb;

    public float AutoResetTime;
    public float DissolveTime = 5f;

    public GameObject Broken;
    public Transform[] Pieces;
    List<Vector3> PiecePositions;
    List<Quaternion> PieceRots;
    bool startKM;
    bool broken;

    public UnityEvent OnBreak;

    public float ArrowBreakVelocity = 20f;
    public float CollisionBreakVelocity = 10f;

    void Awake()
    {
        Base = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        startKM = rb.isKinematic;
        spos = transform.position;
        srot = transform.rotation;
        PiecePositions = new List<Vector3>();
        PieceRots = new List<Quaternion>();
        foreach(var p in Pieces)
        {
            PiecePositions.Add(p.localPosition);
            PieceRots.Add(p.localRotation);
        }
    }

    void Start()
    {
        if (GameBase.instance != null)
            GameBase.instance.OnStartGame.AddListener(Reset);
    }

    public void OnArrow(ArrowCollision c)
    {
        if (c.velocityMag >= ArrowBreakVelocity)
            Break(c.impactPos, c.velocityMag*10f);
    }

    void OnCollisionEnter(Collision c)
    {
        if(c.relativeVelocity.magnitude >= CollisionBreakVelocity)
        {
            Break(c.contacts[0].point, c.relativeVelocity.magnitude*10f);
        }
    }

    public void Break(Vector3 point, float force)
    {
        if(!broken)
        {
            Base.enabled = false;
            col.enabled = false;
            rb.isKinematic = true;
            broken = true;
            Broken.SetActive(true);
            foreach(var v in Pieces)
            {
                Rigidbody r = v.GetComponent<Rigidbody>();
                r.AddExplosionForce(force, point, 2f);
            }
            OnBreak.Invoke();
            Invoke("Dissolve", DissolveTime);
            if(AutoResetTime > DissolveTime + 2f)
                Invoke("Reset", AutoResetTime);
        }
    }

    void Dissolve()
    {
        foreach (var v in Pieces)
        {
            BeautifulDissolves.Dissolve d = v.GetComponent<BeautifulDissolves.Dissolve>();
            if(d != null)
                d.TriggerDissolve();
        }
        Invoke("DissableRBs", 2f);
    }

    void DissableRBs()
    {
        foreach(var v in Pieces)
        {
            BeautifulDissolves.Dissolve d = v.GetComponent<BeautifulDissolves.Dissolve>();
            if (d != null)
                d.ResetDissolve();
        }
        Broken.SetActive(false);
    }

    public void Reset()
    {
        if(broken)
        {
            broken = false;
            Base.enabled = true;
            col.enabled = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = startKM;
            transform.position = spos;
            transform.rotation = srot;
            Broken.SetActive(false);
            for (int i = 0; i < Pieces.Length; i++)
            {
                Transform t = Pieces[i];
                t.localPosition = PiecePositions[i];
                t.localRotation = PieceRots[i];
            }
        }
    }
	
}
