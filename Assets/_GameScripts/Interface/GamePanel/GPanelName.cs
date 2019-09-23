using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GPanelName : MonoBehaviour {

    public Text PName;
    public Text Val;
    public bool mine;

    public GameObject MuteButton;

    public Sprite muteOn;
    public Sprite muteOff;
    public Sprite userSpeaking;

    GamePanel.PlayerTuple t;

    public void Setup( GamePanel.PlayerTuple tpl, string val )
    {
        t = tpl;
        PName.text = tpl.name;
        Val.text = val;
        Image img = MuteButton.GetComponent<Image>();
        mine = (!PhotonNetwork.inRoom || tpl.pid == PhotonNetwork.player.ID);
        if ( !PhotonNetwork.inRoom )
            MuteButton.SetActive( false );

        if ( t.muted )
        {
            ToggleMute( false );
        }
        else
        {
            if ( t.isTalking )
                img.sprite = userSpeaking;
            else
                img.sprite = muteOff;
        }
    }

    public void ToggleMute(bool change=true)
    {
        Image img = MuteButton.GetComponent<Image>();
        if (img.sprite == muteOff)
            img.sprite = muteOn;
        else
            img.sprite = muteOff;
        if(change && PhotonNetwork.inRoom)
        {
            if (mine)
                PlayerSync.myInstance.selfMute = (img.sprite == muteOn);
            else
            {
                foreach(var v in PlayerSync.Others)
                {
                    if(v.GetComponent<PhotonView>().ownerId == t.pid)
                    {
                        v.muted = (img.sprite == muteOn);
                    }
                }
            }
        }
    }
}
