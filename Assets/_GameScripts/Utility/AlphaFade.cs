using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaFade : MonoBehaviour {

    public float FadeDur;
    public AnimationCurve FadeCurve;
    public string ColorField;
    public Renderer rend;
    bool startsOn = false;

    Material m;

    void Awake()
    {
        m = rend.material;
        if (startsOn)
            curDur = 1;
    }

    public void ChangeBrightness(float brightness)
    {
        if (m.HasProperty(ColorField))
        {
            Color c = m.GetColor(ColorField);
            float biggestValue = c.r;
            if (c.g > biggestValue) biggestValue = c.g;
            if (c.b > biggestValue) biggestValue = c.b;
            if (biggestValue > 1f)
            {
                c =
                    new Color(
                        c.r / biggestValue,
                        c.g / biggestValue,
                        c.b / biggestValue
                        );
            }
            m.SetColor(ColorField, c * brightness);
        }
    }

    public float curDur;

    public void FadeOutImmediate()
    {
        if (m.HasProperty(ColorField))
        {
            Color c = m.GetColor(ColorField);
            c.a = 0;
            m.SetColor(ColorField, c);
        }
    }

    public void FadeInImmediate()
    {
        if (m.HasProperty(ColorField))
        {
            Color c = m.GetColor(ColorField);
            c.a = 0;
            m.SetColor(ColorField, c);
        }
    }

	public void FadeIn()
    {
        if(m.HasProperty(ColorField) && curDur < 1)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(true));
        }
    }

    public void FadeOut()
    {
        if (m.HasProperty(ColorField) && curDur > 0)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(false));
        }
    }

    IEnumerator Fade(bool fadeIn)
    {
        float targ = fadeIn ? 1f : 0f;
        Color c = m.GetColor(ColorField);
        while((fadeIn && curDur < targ) || (!fadeIn && curDur > targ))
        {
            yield return true;
            curDur += fadeIn ? Time.deltaTime/FadeDur : -1 * Time.deltaTime/FadeDur;
            c.a = FadeCurve.Evaluate(curDur);
            m.SetColor(ColorField, c);
        }
        c.a = fadeIn ? 1 : 0;
        m.SetColor(ColorField, c);
    }
}
