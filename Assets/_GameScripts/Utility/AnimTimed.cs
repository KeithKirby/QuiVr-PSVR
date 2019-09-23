using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class AnimTimed : MonoBehaviour {

    public bool AtStart;
    public bool Loop;
    public bool CanCancel;
    public Vector3 Motion;
    public AnimationCurve Curve;
    public float Duration;
    public UnityEvent OnStart;
    public UnityEvent OnComplete;
    bool inMotion;
    float curTime;

    Vector3 startPos;
    void Start()
    {
        startPos = transform.localPosition;
        if (AtStart)
            StartMotion();
        if (Duration <= 0)
            Duration = 1;
    }

    public void StartMotion()
    {
        if (inMotion && !CanCancel)
            return;
        OnStart.Invoke();
        inMotion = true;
        curTime = 0;
    }

    void Update()
    {
        if (inMotion)
        {
            transform.localPosition = startPos + Motion * Curve.Evaluate(curTime / Duration);
            curTime += Time.deltaTime;
            if (curTime > Duration)
                EndMotion();
        }
    }

    public void EndMotion()
    {
        inMotion = false;
        OnComplete.Invoke();
        if (Loop)
            StartMotion();
    }
}
