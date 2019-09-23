using UnityEngine;
using System.Collections;

public class StreamDisplay : MonoBehaviour {

    public bool on;
    Light l;
    float startRange;
    public float addRange;
    public float flashTime;
    public AnimationCurve lCurve;

    void Awake()
    {
        l = GetComponent<Light>();
        if (l != null)
            startRange = l.range;
    }

    [BitStrap.Button]
    public void TurnOn()
    {
        if(!on)
        {
            on = true;
            if (l != null)
            {
                StopCoroutine("FadeOff");
                StopCoroutine("FlashLight");
                StartCoroutine("FlashLight");
            }
        }
    }

    [BitStrap.Button]
    public void TurnOff()
    {
        if (on)
        {
            on = false;
            if(l != null)
            {
                StopCoroutine("FlashLight");
                StopCoroutine("FadeOff");
                StartCoroutine("FadeOff");
            }
        }
    }

    IEnumerator FlashLight()
    {
        l.enabled = true;
        l.range = startRange;
        float t = 0;
        while(t < flashTime)
        {
            l.range = startRange + (lCurve.Evaluate(t / flashTime) * addRange);
            t += Time.deltaTime;
            yield return true;
        }
    }

    IEnumerator FadeOff()
    {
        float r = l.range;
        while(l.range > 0)
        {
            l.range -= Time.deltaTime*r;
            yield return true;
        }
        l.enabled = false;
    }
}
