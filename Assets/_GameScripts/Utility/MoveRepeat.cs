using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveRepeat : MonoBehaviour {

    public AnimationCurve curve;
    public float LoopTime;
    public Vector3 EndPos;
    Vector3 StartPos;

    void Start () {
        StartPos = transform.localPosition;
	}

    float t = 0;
    bool pr;
    void Update()
    {
        t += (Time.deltaTime / Time.timeScale);
        if (t > LoopTime)
        {
            t = 0;
            pr = false;
        }
        transform.localPosition = Vector3.Lerp(StartPos, EndPos, curve.Evaluate(t / LoopTime));
    }
}
