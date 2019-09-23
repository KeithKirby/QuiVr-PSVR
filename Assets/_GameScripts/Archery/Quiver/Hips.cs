using UnityEngine;
using System.Collections;

public class Hips : MonoBehaviour {

    public Transform Head;
    public static Hips instance;

    void Awake()
    {
        instance = this;
    }

	void Update()
    {
        Vector3 forward = Head.forward;
        Vector3 forwardLeveled1 = forward;

        forwardLeveled1 = PlayerHolder.InverseTransformDirection(forwardLeveled1);
        forwardLeveled1.y = 0;
        forwardLeveled1 = PlayerHolder.TransformDirection(forwardLeveled1);
        forwardLeveled1.Normalize();

        Vector3 mixedInLocalForward = Vector3.zero;
        forward  = PlayerHolder.InverseTransformDirection(forward);
        if (forward.y > 0)
        {
            mixedInLocalForward = -Head.up;
        }
        else
        {
            mixedInLocalForward = Head.up;
        }
        forward = PlayerHolder.TransformDirection(forward);

        mixedInLocalForward = PlayerHolder.InverseTransformDirection(mixedInLocalForward);
        mixedInLocalForward.y = 0;
        mixedInLocalForward = PlayerHolder.TransformDirection(mixedInLocalForward);

        mixedInLocalForward.Normalize();
        float dot = Mathf.Clamp(Vector3.Dot(forwardLeveled1, forward), 0f, 1f);

        Vector3 finalForward = Vector3.Lerp(mixedInLocalForward, forwardLeveled1, dot * dot);

        transform.position = Head.position + PlayerHolder.Down() * 0.35f;
        transform.rotation = Quaternion.LookRotation(finalForward, PlayerHolder.Up());
    }
}
