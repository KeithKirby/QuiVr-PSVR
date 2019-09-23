using UnityEngine;
using System.Collections;

public class MoveTimed : MonoBehaviour {

    public bool onStart;
    public bool loop;
    public float Length = 1f;
    public AnimationCurve Curve;
    public Vector3 Direction;
    public bool worldSpace;

    float curTime;
    bool moving;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        if (!worldSpace)
            startPos = transform.localPosition;
        if (onStart)
            StartMove();
    }

    void StartMove()
    {
        moving = true;
        curTime = 0;
    }

    void EndMove()
    {
        if (loop)
            StartMove();
        else
            StartMove();
    }

    void StopMove()
    {
        moving = false;
    }

    void Update()
    {
        if(moving)
        {
            curTime += Time.deltaTime;
            if (worldSpace)
                transform.position = startPos + (Direction * Curve.Evaluate(curTime / Length));
            else
                transform.localPosition = startPos + (Direction * Curve.Evaluate(curTime / Length));
            if (curTime >= Length)
                EndMove();
        }
    }
}
