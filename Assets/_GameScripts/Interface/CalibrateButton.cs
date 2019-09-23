using UnityEngine;
using System.Collections;

public class CalibrateButton : MonoBehaviour {

    public CalibrateHand calib;

	public void Click()
    {
        if (calib != null)
            calib.StartCalibration();
    }
}
