using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FriendUI : MonoBehaviour {

    public Text playerName;
    public Text playerStatus;
    public Button joinButton;
    Friend player;

    public Color inRoomColor;
    public Color onlineColor;
    public Color offlineColor;

    public void Init(Friend p)
    {
        UpdateUI(p);
    }

    void UpdateUI()
    {
        playerName.text = player.Name;
        playerStatus.text = player.GetStatus();
        if(player.inRoom)
        {
            playerName.color = inRoomColor;
#if STEAM
            if (player.ID == new Steamworks.CSteamID(76561198025035958))
            {
                playerName.color = new Color(1, 0.63f, 0);
                playerName.text = player.Name + " <Developer>";
            }
#endif
            playerStatus.color = inRoomColor;
            if(PhotonNetwork.insideLobby)
            {
                var rooms = PhotonNetwork.GetRoomList();
                foreach(var v in rooms)
                {
                    if(v.name == player.GetRoom() && v.playerCount < v.maxPlayers)
                    {
                        joinButton.gameObject.SetActive(true);
                    }
                    else if(v.name == player.GetRoom())
                    {
                        joinButton.interactable = false;
                        Text t = joinButton.GetComponentInChildren<Text>();
                        t.text = "Full";
                        t.color = Color.red;
                    }
                }
            }
        }
        else
        {
            joinButton.gameObject.SetActive(false);
            if(player.online)
            {
                playerName.color = onlineColor;
                playerStatus.color = onlineColor;
            }
            if (!player.shouldShow)
            {
                playerName.color = offlineColor;
                playerStatus.color = offlineColor;
            }
        }  
    }

    public void UpdateUI(Friend f)
    {
        player = f;
        UpdateUI();
    }

    public void JoinPlayerRoom()
    {
        string type = "";
        if (MPSphere.instance != null)
            type = MPSphere.instance.MPType;
        JoinMultiplayer.JoinSpecific(player.GetRoom(), type);
    }
}
