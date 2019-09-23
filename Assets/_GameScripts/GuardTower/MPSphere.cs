using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class MPSphere : MonoBehaviour {

    public static MPSphere instance;

    public Text OrbText;
    public string MPType = "Normal";
    public int MaxPlrs = 4;

    void Awake()
    {
        instance = this;
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        OrbText.text = I2.Loc.ScriptLocalization.Get("WorldUI/JoinMP");
    }

    void Update()
    {
        if (NVR_Player.isThirdPerson() && Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.LeftControl))
            Click();
    }

    [AdvancedInspector.Inspect]
	public void Click()
    {
        if (PhotonNetwork.inRoom)
            LeaveMultiplayer.Click();
        else
            JoinMultiplayer.Click(MPType, MaxPlrs);
    }

    public void Click(bool host)
    {
        if (PhotonNetwork.inRoom)
            LeaveMultiplayer.Click();
        else
            JoinMultiplayer.Click(MPType, MaxPlrs, false, host);
    }

    [AdvancedInspector.Inspect]
    public void PrivateGame()
    {
        if(!PhotonNetwork.inRoom)
        {
            //JoinMultiplayer.Click(MPType, MaxPlrs, true, true);
            CreatePrivate.ClickStatic();
        }
    }

    public ExitGames.Client.Photon.Hashtable ExpectedRoomProps()
    {
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", MPType }, { "InEvent", false } };
        return expectedCustomRoomProperties;
    }

    void OnJoinedRoom()
    {
        OrbText.text = I2.Loc.ScriptLocalization.Get("WorldUI/LeaveMP");
    }

    void OnLeftRoom()
    {
        OrbText.text = I2.Loc.ScriptLocalization.Get("WorldUI/JoinMP");
    }
}
