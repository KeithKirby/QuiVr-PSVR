using UnityEngine;
using System.Collections;

public class Shockwave : MonoBehaviour {

    public float Force;
    public float Radius;
    public float Lifetime;
    public AnimationCurve fCurve;
    public UnityEngine.AI.NavMeshObstacle obst;

    float t = 0;
    void Start()
    {
        Collider[] objects = UnityEngine.Physics.OverlapSphere(transform.position, Radius * 1.25f);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null && !r.isKinematic)
            {
                r.AddExplosionForce(Force, transform.position, Radius * 2);
            }
        }
    }

    void Update()
    {
        t += Time.deltaTime;
        if(obst != null)
        {
            obst.radius = Radius * fCurve.Evaluate(t/Lifetime);
            if (t > Lifetime)
                obst.enabled = false;
        }
    }
}
