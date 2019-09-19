using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackedDeviceDebug : MonoBehaviour
{
    public Text TxtOut;
	
	// Update is called once per frame
	void Update ()
    {
#if UNITY_PS4
        TxtOut.text = string.Format("{0}", PS4InputEx.GrabRight());
        /*var td = GameObject.FindObjectOfType<TrackedPlayStationDevices>();
        if(null!=td)
        {
            TxtOut.text = string.Format( "{0}\n{1}\n",  td.moveTrackingA, td.moveTrackingB );
        }
        else
        {
            TxtOut.text = "No controllers detected";
        }*/
#endif
    }
}
