using UnityEngine;
using System.Collections;

public class FancyParticles : MonoBehaviour {

	public void ToggleValue(bool val)
    {
        if(val)
        {
            //QualitySettings.billboardsFaceCameraPosition = true;
            //QualitySettings.softParticles = true;
        }
        else
        {
            //QualitySettings.billboardsFaceCameraPosition = false;
            //QualitySettings.softParticles = false;
        }
    }
}
