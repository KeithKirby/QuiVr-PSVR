using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreatePrivate : MonoBehaviour {

    Button b;
    public Text txt;
    bool isPublic;
    static CreatePrivate instance;

    void Awake()
    {
        b = GetComponent<Button>();
        instance = this;
    }

	void Start()
    {
        if(PhotonNetwork.inRoom)
        {
            txt.text = "Make Game Public";
        }
        else
        {
            txt.text = "Create Private Game";
        }
    }

    public static bool ClickStatic(string nameOverride="")
    {
        if (PhotonNetwork.connected && PhotonNetwork.insideLobby)
        {
            PlayerPositions[] nodes = FindObjectsOfType<PlayerPositions>();
            foreach (var n in nodes)
            {
                n.ResetAll();
            }
            Statistics.SaveStatistics();
            if (SingleplayerSpawn.instance != null)
                SingleplayerSpawn.instance.DevCam.SetActive(true);
            string prefix = "";
            int max = 4;
            if (MPSphere.instance != null)
            {
                prefix = MPSphere.instance.MPType;
                max = MPSphere.instance.MaxPlrs;
            }
            
            ConnectAndJoinRandom.instance.CreatePrivate(prefix, max, nameOverride);
            Destroy(LocalPlayer.instance.gameObject);
            return true;
        }
        else if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            bool success = false;

            if (!instance.isPublic)
                success = ConnectAndJoinRandom.instance.MakeRoomPublic();
            else
                success = ConnectAndJoinRandom.instance.MakeRoomPrivate();
            if (success)
            {
                instance.isPublic = !instance.isPublic;
                return success;
            }
        }
        return false;
    }

    public void Click()
    {
        bool success = ClickStatic();
        if (success && isPublic)
        {
            txt.text = "Make Game Private";
        }
    }
}
