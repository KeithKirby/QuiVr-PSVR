using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimulatedLob : MonoBehaviour {

    public UnityEvent OnLand;
    public UnityEvent OnBreak;
    public float distTreshold;
    public GameObject LandEffect;
    public bool canBreak;

    bool launched;
    Vector3 globalVelocity;
    Vector3 target;

    public float baseForce = 8;
    public bool Arc = true;
    float force;
    int iterations = 0;

    public void Launch(Vector3 targ)
    {
        Vector3 pos = transform.position;
        target = targ;
        transform.rotation = Quaternion.identity;
        force = baseForce;
        float yRotation = GetYRotation();
        if (Arc)
        {
            float barrelAngle = CalculateProjectileFiringSolution(force, (target.y - transform.position.y));
            Vector3 fVector = new Vector3(force, 0, 0); // fixed vector (image a 2d trajectory graph)          
            globalVelocity = Quaternion.Euler(0, yRotation, barrelAngle) * fVector; // transformed to 3d to point at target
        }
        else
            globalVelocity = (targ - transform.position).normalized * force;
        launched = true;
        Destroy(gameObject, 30f); //Sanity Check for Missed Rocks
    }

    void Update()
    {
        if(launched)
        {
            transform.position += globalVelocity * Time.deltaTime;
            if(Arc)
                globalVelocity += Physics.gravity * Time.deltaTime;
            if (Vector3.Distance(transform.position, target) < distTreshold)
            {
                OnLand.Invoke();
                Explode();
            }
        }
    }

    public void Break()
    {
        if(canBreak)
        {
            OnBreak.Invoke();
            Explode();
        }
    }

    public void Explode()
    {
        launched = false;
        if(LandEffect != null)
        {
            GameObject o = Instantiate(LandEffect, transform.position, Quaternion.identity);
            o.SetActive(true);
            AudioSource a = o.GetComponent<AudioSource>();
            if (a != null)
                a.pitch = Random.Range(0.9f, 1.15f);
        }
        try { Destroy(gameObject, 0.15f); } catch { }  
    }

    float CalculateProjectileFiringSolution(float vel, float alt)
    {
        float g = Mathf.Abs(Physics.gravity.y);

        Vector2 a = new Vector2(transform.position.x, transform.position.z);
        Vector2 b = new Vector2(target.x, target.z);
        float dis = Vector2.Distance(a, b);


        float dis2 = dis * dis;
        float vel2 = vel * vel;
        float vel4 = vel * vel * vel * vel;
        float calc = (vel4 - g * ((g * dis2) + (2 * alt * vel2)));
        if (calc <= 0 && iterations < 100)
        {
            force *= 1.1f;
            iterations++;
            return CalculateProjectileFiringSolution(force, alt);
        }
        float num = vel2 + Mathf.Sqrt(calc);
        float dom = g * dis;
        float angle = Mathf.Atan(num / dom);

        return angle * Mathf.Rad2Deg;
    }

    float GetYRotation()
    {
        Vector3 relativePos = transform.InverseTransformPoint(target);
        float x = (relativePos.x);
        float z = (relativePos.z);

        float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;

        return angle - 90;
    }
}
