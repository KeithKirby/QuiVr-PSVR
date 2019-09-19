using UnityEngine;
using System.Collections;

public class ExplosionAnim : MonoBehaviour {

    MeshRenderer rend;
    public AnimationCurve scalecurve;
    public AnimationCurve clipCurve;

    IEnumerator Start () {
        Destroy(gameObject, 5f);
        rend = GetComponent<MeshRenderer>();
        transform.localEulerAngles = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
        transform.localScale = Vector3.zero;
        float t = 0;
        while(t < 1)
        {
            transform.localScale = Vector3.one * scalecurve.Evaluate(t)*3;
            float r = Mathf.Sin((t / 1f) * (2 * Mathf.PI)) * 0.5f + 0.25f;
            float g = Mathf.Sin((t / 1f + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
            float b = Mathf.Sin((t / 1f + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
            float correction = 1 / (r + g + b);
            r *= correction;
            g *= correction;
            b *= correction;
            rend.material.SetVector("_ChannelFactor", new Vector4(r, g, b, 0));

            rend.material.SetVector("_Range", new Vector4(clipCurve.Evaluate(t), 0.5f, 0, 0));
            t += Time.deltaTime;
            yield return true;
        }
        transform.localScale = Vector3.zero;

    }
}
