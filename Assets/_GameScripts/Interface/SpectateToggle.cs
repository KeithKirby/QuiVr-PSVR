using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpectateToggle : MonoBehaviour {

    public NetworkPlrMgr PlrManager;

    public bool doSpectate;

    Button btn;
	// Use this for initialization
	void Start () {
        btn = GetComponent<Button>();
        if(!PhotonNetwork.inRoom)
        {
            btn.interactable = false;
            this.enabled = false;
        }
	}

    public void Click()
    {
        if(PhotonNetwork.inRoom)
        {
            GetComponentInParent<ToggleMenu>().Toggle();
            if(doSpectate)
            {
               // PlrManager.MakeSpectator();
            }
            else
            {
               // PlrManager.MakePlayer();
            }
        }
    }
}
