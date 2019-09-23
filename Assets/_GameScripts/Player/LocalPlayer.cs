using UnityEngine;
using System.Collections;

public class LocalPlayer : MonoBehaviour {

    public static LocalPlayer instance;
    public Transform PlayArea;

	void Awake()
    {
        if(!PhotonNetwork.inRoom || GetComponent<PhotonView>().isMine)
        {
            instance = this;
        }
    }
}
