using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public GameObject MP_panel;
    public GameObject SP_panel;

	// Use this for initialization
	public void SP () {
        SP_panel.SetActive(true);
	}
	
    public void MP ()
    {
        MP_panel.SetActive(true);
    }
}
