using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileMPToggle : MonoBehaviour
{
    Button btn;
    public Text txt;

    void Awake()
    {
        btn = GetComponent<Button>();
        if(!PhotonNetwork.connected)
            btn.interactable = false;
        if (PhotonNetwork.inRoom)
            txt.text = "Leave Game";
        else
            txt.text = "Join Multiplayer";
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    void OnJoinedLobby()
    {
        btn.interactable = true;
    }

    public void Click()
    {
        if(!PhotonNetwork.inRoom && PhotonNetwork.connected)
        {
            if (MPSphere.instance != null)
            {
                MPSphere.instance.Click();
            }
            else
                JoinMultiplayer.Click();
        }
        else
        {
            LeaveMultiplayer.Click();
        }
    }
}
