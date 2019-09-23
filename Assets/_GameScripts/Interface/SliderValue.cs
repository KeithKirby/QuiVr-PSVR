using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Slider))]
public class SliderValue : MonoBehaviour {

    Slider s;
    public Text txt;
    public float minVal;
    public float maxVal;

    public float interval;

	// Use this for initialization
	void Start ()
    {
        s = GetComponent<Slider>();
        s.minValue = minVal;
        s.maxValue = maxVal;
        s.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(ValChange));
	}
	
	// Update is called once per frame
	void ValChange(float val) {
        float mult = 1f / interval;
        float newVal = (int)Mathf.Round(val * mult);
        newVal /= mult;
        if(newVal != val)
        {
            s.value = newVal;
            return;
        }
        if (interval >= 1)
            txt.text = string.Format("{0:F0}", newVal);
        else if(interval >= 0.1f)
            txt.text = string.Format("{0:F1}", newVal);
        else
            txt.text = string.Format("{0:F2}", newVal);
    }
}
