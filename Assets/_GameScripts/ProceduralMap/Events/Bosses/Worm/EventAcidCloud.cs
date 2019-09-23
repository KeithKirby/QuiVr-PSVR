using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAcidCloud : MonoBehaviour {

    Collider col;
    ParticleSystem particles;
    Vector3[] pts;
    public bool active;
    int pid;
    float moveSpeed;
    public bool localPos;

    RFX4_ShaderFloatCurve fcurve;

    void Awake()
    {
        fcurve = GetComponentInChildren<RFX4_ShaderFloatCurve>();
        col = GetComponent<Collider>();
        particles = GetComponent<ParticleSystem>();
        if (particles == null)
            particles = GetComponentInChildren<ParticleSystem>();
        Deactivate();
    }

    public void Activate(Vector3[] points, float speed)
    {
        active = true;
        if (particles != null)
            particles.Play();
        pts = points;
        if (pts.Length > 0)
        {
            if(localPos)
                transform.localPosition = pts[0];
            else
                transform.position = pts[0];
        }            
        pid = 0;
        moveSpeed = speed;
        if(col != null)
            col.enabled = true;
        if (fcurve != null)
            fcurve.Play();
    }

    public void Deactivate()
    {
        active = false;
        if (particles != null)
            particles.Stop();
        if (col != null)
            col.enabled = false;
        if (fcurve != null)
            fcurve.Reverse();
    }

    void Update()
    {
        if(active && pts.Length > 1)
        {
            if(localPos)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, pts[pid], moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.localPosition, pts[pid]) < 0.05f)
                {
                    pid++;
                    if (pid >= pts.Length)
                        pid = 0;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pts[pid], moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, pts[pid]) < 0.05f)
                {
                    pid++;
                    if (pid >= pts.Length)
                        pid = 0;
                }
            }
            
        }
    }

    void OnTriggerEnter(Collider col)
    {
        PlayerLife p = col.gameObject.GetComponentInParent<PlayerLife>();
        if (p != null && p == PlayerLife.myInstance)
            p.Die();
    }
}
