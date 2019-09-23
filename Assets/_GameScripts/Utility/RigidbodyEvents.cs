using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RigidbodyEvents : MonoBehaviour {

    public float CollisionVelThreshold;
    public class FloatEvent : UnityEvent<float> { };
    public  FloatEvent OnCollision;

    void OnCollisionEnter(Collision col)
    {
        OnCollision.Invoke(col.relativeVelocity.magnitude);
    }
}
