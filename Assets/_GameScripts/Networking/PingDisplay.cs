using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PingDisplay : MonoBehaviour {

    int ping;
    public Text PingText;

    void Start()
    {
        InvokeRepeating("GetPing", 1f, 1f);
    }

	void GetPing()
    {
        ping = -1;
        if(PhotonNetwork.connected)
        {
            ping = PhotonNetwork.GetPing();
        }
        if (ping >= 0)
            PingText.text = "Ping: " + ping;
        else
            PingText.text = "Not Connected";
    }
}
