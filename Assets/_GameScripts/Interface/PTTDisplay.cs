using UnityEngine;
using System.Collections;

public class PTTDisplay : MonoBehaviour {

    public GameObject Display;

    void Start()
    {
        if(PhotonNetwork.inRoom && Settings.GetBool("PushToTalk"))
        {
            Display.SetActive(true);
            Invoke("TurnOff", 10f);
        }
    }

    void TurnOff()
    {
        Display.SetActive(false);
    }
}
