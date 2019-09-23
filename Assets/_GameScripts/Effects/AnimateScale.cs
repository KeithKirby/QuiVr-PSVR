using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateScale : MonoBehaviour {

    public AnimationCurve XScale;
    public AnimationCurve YScale;
    public AnimationCurve ZScale;
    public float AnimationLength = 1f;
    public bool loop;
    public bool playAtStart;
    public bool ignoreTimescale;
    public bool absoluteScale;

    Vector3 startScale;

    void Start()
    {
        if (playAtStart)
            Play();
        SetScale();
    }

    public void SetScale()
    {
        startScale = transform.localScale;
    }

    public void Play()
    {
        StopCoroutine("DoPlay");
        StartCoroutine("DoPlay");
    }

    IEnumerator DoPlay()
    {
        float t = 0;
        while(t < 1)
        {
            Vector3 v = Vector3.one;
            if (!absoluteScale)
                v = startScale;
            v.x = v.x * XScale.Evaluate(t);
            v.y= v.y * YScale.Evaluate(t);
            v.z = v.z * ZScale.Evaluate(t);
            transform.localScale = v;
            if (ignoreTimescale)
                t += Time.unscaledDeltaTime / AnimationLength;
            else
                t += Time.deltaTime / AnimationLength;
            yield return true;
        }
    }
}
