using UnityEngine;
using System.Collections;

public class GPUTest : MonoBehaviour {

    public GameObject[] FancyObjects;
    public GameObject[] NormalObjects;
    bool isOn;

	public void Toggle ()
    {
	    if(!isOn)
        {
            isOn = true;
            foreach (var v in FancyObjects)
                v.SetActive(true);
            foreach (var v in NormalObjects)
                v.SetActive(false);
        }
        else
        {
            isOn = false;
            foreach (var v in FancyObjects)
                v.SetActive(false);
            foreach (var v in NormalObjects)
                v.SetActive(true);
        }
	}
	
}
