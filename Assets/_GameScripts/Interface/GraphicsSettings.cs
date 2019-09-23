using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour {

    public Text txt;
    bool onHigh;

    public GameObject[] HighObjects;

	// Use this for initialization
	public void Toggle () {
	    if(onHigh)
        {
            txt.text = "High Settings";
            onHigh = false;
            ToggleObjects(false);
        }
        else
        {
            txt.text = "Low Settings";
            onHigh = true;
            ToggleObjects(true);
        }
	}

    void ToggleObjects(bool val)
    {
        foreach(var v in HighObjects)
        {
            v.SetActive(val);
        }
    }
}
