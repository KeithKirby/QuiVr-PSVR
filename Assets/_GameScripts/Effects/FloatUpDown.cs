using UnityEngine;
using System.Collections;

public class FloatUpDown : MonoBehaviour {

    Vector3 startPos;
    public AnimationCurve FloatCurve;
    public float PosDelta;
    public float LoopTime;
    float t = 0;

    void Awake()
    {
        startPos = transform.position;
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t > LoopTime)
            t = 0;

        transform.position = startPos + (Vector3.up * FloatCurve.Evaluate(t / LoopTime) * PosDelta);
    }
}
