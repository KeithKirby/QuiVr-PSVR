using UnityEngine;
using ExitGames.Client.Photon;

public class BestPingMaster : MonoBehaviour {

    public string CurrentMaster;
    public int ping;
    int timesLowest;
    void Start()
    {
        InvokeRepeating("CheckPing", 1.5f, 2.5f);
    }

    void CheckPing()
    {
        if(PhotonNetwork.connected)
            ping = PhotonNetwork.GetPing();
        if (PhotonNetwork.inRoom)
        {
            Hashtable PlayerCustomProps = new Hashtable();
            PlayerCustomProps["ping"] = ping;
            PhotonNetwork.player.SetCustomProperties(PlayerCustomProps);
            bool lowestPing = true;
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                if(player.customProperties.ContainsKey("ping"))
                {
                    float p = float.Parse(player.customProperties["ping"].ToString());
                    if (p <= ping - 25 && p > 0)
                        lowestPing = false;
                }
            }
            if (lowestPing && !PhotonNetwork.isMasterClient)
            {
                timesLowest++;
                if (timesLowest > 5)
                {
                    timesLowest = 0;
                    PhotonNetwork.SetMasterClient(PhotonNetwork.player);
                }
            }
            else
                timesLowest = 0;
                
            CurrentMaster = PhotonNetwork.masterClient.name;
        }
    }

}
