using UnityEngine;
using System.Collections;

public class CloseMenu : MonoBehaviour {

    public EMOpenCloseMotion em;
   
	public void Close () {
        Debug.Log("Closing Menu");
        em.SetStateToOpen();
        em.Close(true);
	}

}
