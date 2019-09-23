using UnityEngine;
using System.Collections;

public class JoinMultiplayer : MonoBehaviour {

    public string RoomPrefix;

    public static void Click(string prefix="", int maxPlrs = 4, bool isPrivate = false, bool forceHost=false)
    {
        GameState.JoinOrCreateRandomRoom();
    }

    static void MobileJoin(string prefix = "", int maxPlrs = 4, bool isPrivate = false)
    {

    }

    static void VRJoin(string prefix = "", int maxPlrs = 4, bool isPrivate = false)
    {

    }

    [BitStrap.Button]
    public void OnClick()
    {
        Click(RoomPrefix);
    }

    public static void JoinSpecific(string roomName, string gameType)
    {
        bool validRoom = false;
        bool inEvent = false;
        foreach (var v in PhotonNetwork.GetRoomList())
        {
            if (v.customProperties.ContainsKey("type") && v.name == roomName)
            {
                object type = "";
                object ie = false;
                v.customProperties.TryGetValue("type", out type);
                v.customProperties.TryGetValue("InEvent", out ie);
                inEvent = (bool)ie;
                string t = type.ToString();
                if (t.Contains("PRIVATE_"))
                    t = t.Remove(0, 8);
                if(gameType == t && !inEvent)
                {
                    validRoom = true;
                }
            }
        }
        if(validRoom)
        {
            TeleportPlayer plr = FindObjectOfType<TeleportPlayer>();
            if (plr != null)
                plr.SetAllActive();
            PlayerPositions[] nodes = FindObjectsOfType<PlayerPositions>();
            foreach (var n in nodes)
            {
                n.ResetAll();
            }
            Statistics.SaveStatistics();
            try
            {
                FindObjectOfType<Scoreboard>().ResetScores();
            }
            catch
            {
                //Debug.Log("Error finding game components");
            }
            FindObjectOfType<SingleplayerSpawn>().DevCam.SetActive(true);
            PhotonNetwork.JoinRoom(roomName);
            Destroy(LocalPlayer.instance.gameObject);
        }
        else
        {
            Note note = new Note();
            note.title = "Can't Join Room";
            if(PhotonNetwork.inRoom)
                note.body = "Can't join room while in a multiplayer game";
            else if(inEvent)
                note.body = "Can't join room while Event in progress";
            else
                note.body = "You are in the wrong mode to join " + roomName;
            Notification.Notify(note);
        }
    }
}
