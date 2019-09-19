using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOutColorMulti : MonoBehaviour {

    public Renderer r;
    public string[] ColorProps;
    Material[] Mats;
    float[] BaseAlphas;
    [Header("Fade In")]
    public bool FadeInStart;
    public AnimationCurve FadeInCurve;
    public float FadeInTime;
    [Header("Fade Out")]
    public bool DestroyOnFadeOut;
    public AnimationCurve FadeOutCurve;
    public float FadeOutTime;
    float t = 0;
    void Awake()
    {
        r = GetComponent<Renderer>();
        Mats = r.materials;
        BaseAlphas = new float[Mats.Length];
        for(int i=0; i< ColorProps.Length; i++)
        {
            if(Mats[i].HasProperty(ColorProps[i]))
                BaseAlphas[i] = Mats[i].GetColor(ColorProps[i]).a;
        }
    }

    void Start()
    {
        if (FadeInStart)
            FadeIn();
    }

	public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine("Fade", true);
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine("Fade", false);
    }

    IEnumerator Fade(bool fadeIn)
    {
        if(fadeIn)
        {
            while(t < 1)
            {
                t += Time.deltaTime / FadeInTime;
                for (int i=0; i<Mats.Length; i++)
                {
                    if(Mats[i].HasProperty(ColorProps[i]))
                    {
                        Color c = Mats[i].GetColor(ColorProps[i]);
                        c.a = BaseAlphas[i] * FadeInCurve.Evaluate(t);
                        Mats[i].SetColor(ColorProps[i], c);
                    }
                }
                yield return true;
            }
        }
        else
        {
            while (t > 0)
            {
                t -= Time.deltaTime / FadeInTime;
                for (int i = 0; i < Mats.Length; i++)
                {
                    if (Mats[i].HasProperty(ColorProps[i]))
                    {
                        Color c = Mats[i].GetColor(ColorProps[i]);
                        c.a = BaseAlphas[i] * FadeOutCurve.Evaluate(t);
                        Mats[i].SetColor(ColorProps[i], c);
                    }
                }
                yield return true;
            }
            if (DestroyOnFadeOut)
                Destroy(gameObject);
        }
    }
}
