using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CinematicCamera : MonoBehaviour {

    public bool AtStart;
    public bool Loop;
    public bool CanCancel;
    public bool LocalVector;
    public Vector3 Motion;
    public AnimationCurve Curve;
    public float Duration;
    public UnityEvent OnStart;
    public UnityEvent OnComplete;
    bool inMotion;
    float curTime;

    bool paused;

    Vector3 startPos;
    void Start()
    {
        startPos = transform.localPosition;
        if (AtStart)
            StartMotion();
        if (Duration <= 0)
            Duration = 1;
    }

    [AdvancedInspector.Inspect]
    public void Reset()
    {
        Start();
    }

    [AdvancedInspector.Inspect]
    public void TogglePause()
    {
        paused = !paused;
    }

    public void StartMotion()
    {
        if (inMotion && !CanCancel)
            return;
        OnStart.Invoke();
        inMotion = true;
        curTime = 0;
    }

    [BitStrap.Button]
    void ResetTimer()
    {
        curTime = 0;
    }

    void Update()
    {
        if (inMotion && !paused)
        {
            Vector3 dir = Motion;
            if (LocalVector)
                dir = transform.TransformDirection(dir);
            transform.localPosition = startPos + dir * Curve.Evaluate(curTime / Duration);
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
