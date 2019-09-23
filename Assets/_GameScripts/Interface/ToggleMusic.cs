using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleMusic : MonoBehaviour {

    public Text buttonText; 

    void Start()
    {
        if(Settings.HasKey("MusicOff") && Settings.GetBool("MusicOff"))
        {
            Click();
        }
        else
        {
            Settings.Set("MusicOff", false);
        }
    }

	public void Click()
    {
        if(FindObjectOfType<infinite_fantasy_pro>() != null)
        {
            bool muted = FindObjectOfType<infinite_fantasy_pro>().toggleMute();
            if (muted)
            {
                buttonText.text = "Turn on Music";
                Settings.Set("MusicOff", true);
            }   
            else
            {
                buttonText.text = "Turn off Music";
                Settings.Set("MusicOff", false);
            }
                
        }
    }
}
