using UnityEngine;
using System.Collections;

public class ReloadScene : MonoBehaviour {

	
	// Update is called once per frame
	public void Reload ()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
