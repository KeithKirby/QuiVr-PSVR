using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MuteMic : MonoBehaviour {

    public Text buttonText;
    public string mutedText;
    public string unMutedText;

    public MuteControl mctrl;

    void Start()
    {
        if(PhotonNetwork.inRoom)
        {
            if (Settings.HasKey("MicOn") && Settings.GetBool("MicOn"))
                DoToggleMute();
        }
    }

	// Update is called once per frame
	public void DoToggleMute ()
    {
        bool muted;
        muted = mctrl.toggleFullMute();
        if (muted)
            Settings.Set("MicOn", false);
        else
            Settings.Set("MicOn", true);
        if (muted)
            buttonText.text = mutedText;
        else
            buttonText.text = unMutedText;
    }
}
