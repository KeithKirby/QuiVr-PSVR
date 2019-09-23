using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TextFadeDistance : MonoBehaviour {

    public bool fadeIn;
    public float upperDistance;
    public float lowerDistance;
    Color startC;
    Text txt;
    void Awake()
    {
        txt = GetComponent<Text>();
        startC = txt.color;
    }

    void Update()
    {
        if(PlayerHead.instance != null && txt.enabled)
        {
            float d = Vector3.Distance(transform.position, PlayerHead.instance.transform.position);
            if(d < upperDistance)
            {
                float val = Mathf.Clamp((d - lowerDistance) / (upperDistance - lowerDistance), 0f, 1f);
                if (fadeIn)
                    val = 1 - val;
                Color c = startC;
                c.a = c.a * val;
                txt.color = c;
            }
        }
    }
}
