using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class SuperSampling : MonoBehaviour {

    public VRTK.VRTK_AdaptiveQuality[] Cameras;
    public SliderSetting setting;
	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForEndOfFrame();
        ChangeValue(setting.getValue());
    }
	
	public void ChangeValue(float v)
    {
        StopCoroutine("ChangeCameraValues");
        StartCoroutine("ChangeCameraValues", v);
    }

    IEnumerator ChangeCameraValues(float f)
    {
        foreach (var c in Cameras)
        {
            c.minimumRenderScale = f;
            c.maximumRenderScale = f;
            c.enabled = false;
            yield return true;
            c.enabled = true;
        }
    }
}
