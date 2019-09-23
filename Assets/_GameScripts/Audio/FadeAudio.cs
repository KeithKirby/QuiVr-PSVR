using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudio : MonoBehaviour {

    AudioSource src;
    public AnimationCurve FadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float fadeTime = 1;
    public bool fadeStart;
    float maxVol = 1;
    void Awake()
    {
        src = GetComponent<AudioSource>();
        if (src == null)
            src = GetComponentInChildren<AudioSource>();
        if (src != null)
            maxVol = src.volume;
        src.volume = 0;
    }

    void Start()
    {
        if (fadeStart)
            FadeIn();
    }

    float t = 0;
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine("FadeInTimed");
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine("FadeOutTimed");
    }

    IEnumerator FadeInTimed()
    {
        if (!src.isPlaying)
            src.Play();
        while(t < 1)
        {
            t += Time.deltaTime / fadeTime;
            src.volume = maxVol * FadeCurve.Evaluate(t);
            yield return true;
        }
        src.volume = maxVol;
    }

    IEnumerator FadeOutTimed()
    {
        while (t > 0)
        {
            t -= Time.deltaTime / fadeTime;
            src.volume = maxVol * FadeCurve.Evaluate(t);
            yield return true;
        }
        src.volume = 0;
        src.Stop();
    }




}
