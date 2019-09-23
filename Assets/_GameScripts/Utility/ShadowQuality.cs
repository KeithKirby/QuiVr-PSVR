using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowQuality : MonoBehaviour {

	public void SetQuality(float f)
    {
        if (f <= 0)
        {
            QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
            QualitySettings.shadowResolution = ShadowResolution.Low;
        }
        else
        {
            QualitySettings.shadows = UnityEngine.ShadowQuality.All;
            if (f <= 0.2f)
                QualitySettings.shadowResolution = ShadowResolution.Low;
            else if (f <= 0.4f)
                QualitySettings.shadowResolution = ShadowResolution.Medium;
            else if (f <= 0.8f)
                QualitySettings.shadowResolution = ShadowResolution.High;
            else
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
        }
    }
}
