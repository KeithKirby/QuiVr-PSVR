using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class RotateRepeat : MonoBehaviour {

    public AnimationCurve curve;
    public float LoopTime;
    public Vector3 Angle;
    public float percentInvoke;

    public UnityEvent OnPercentReach;
    public UnityEvent OnReset;

    float t = 0;
    bool pr;
    void Update()
    {
        t += (Time.deltaTime / Time.timeScale);
        if (t > LoopTime)
        {
            t = 0;
            pr = false;
            OnReset.Invoke();
        }
        if(t > percentInvoke*LoopTime && !pr)
        {
            pr = true;
            OnPercentReach.Invoke();
        }
        transform.localEulerAngles = Angle * curve.Evaluate(t / LoopTime);
    }
}
