using UnityEngine;
using System.Collections;

public class PBubbleEffect : MonoBehaviour {

    public MeshRenderer Bubble;
    public AnimationCurve curve;
    public float OutSpeed = 5f;
    Material m;
    Transform t;
    bool pulsing;

    float origDist;
    float origStr;

    void Awake()
    {
        m = Bubble.material;
        t = Bubble.transform;
        if (m.HasProperty("_BumpAmt"))
            origDist = m.GetFloat("_BumpAmt");
        if (m.HasProperty("_ColorStrength"))
            origStr = m.GetFloat("_ColorStrength");
    }

    void Update()
    {
        if(pulsing)
        {
            t.localScale += Vector3.one * Time.unscaledDeltaTime * OutSpeed;
            float percent = t.localScale.x/3f;
            if (m.HasProperty("_BumpAmt"))
                m.SetFloat("_BumpAmt", origDist*curve.Evaluate(percent));
            if (m.HasProperty("_ColorStrength"))
                m.SetFloat("_ColorStrength", origStr*curve.Evaluate(percent));
            if (t.localScale.x > 3f)
            {
                pulsing = false;
                Bubble.gameObject.SetActive(false);
            }
        }
    }

    public void PulseBubble()
    {
        Bubble.gameObject.SetActive(true);
        t.localScale = Vector3.zero;
        t.position = transform.position;
        pulsing = true;
    }
}
