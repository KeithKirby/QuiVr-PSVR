using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureQuality : MonoBehaviour {

    public void SetQuality(float f)
    {
        if (f <= 0.25f)
            QualitySettings.masterTextureLimit = 3;
        else if (f <= 0.5f)
            QualitySettings.masterTextureLimit = 2;
        else if (f <= 0.75f)
            QualitySettings.masterTextureLimit = 1;
        else
            QualitySettings.masterTextureLimit = 0;
    }

    public void ToggleSoftParticles(bool val)
    {
        QualitySettings.softParticles = val;
    }
}
