using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class JoiningRoom : MonoBehaviour {

    public Text startText;
    public GameObject loadingCamera;
    public Teleporter StartNode;
    public bool NonVive;
    public bool isSpectator;

    PhotonVoiceRecorder rec;
    public bool network;
    public Text connText;

    public GameObject[] SPObjects;
    public GameObject[] MPObjects;

    public GameObject MPGem;

    public UnityEvent OnRoomJoined;

    int players;

    void Awake()
    {
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    void Update()
    {
        if (connText != null)
        {
            if (network && PhotonNetwork.connectionStateDetailed != ClientState.Joined && PhotonNetwork.connectionStateDetailed != ClientState.JoinedLobby)
                connText.text = PhotonNetwork.connectionStateDetailed.ToString();
            else if (connText.text != "")
                connText.text = "";
            if (PhotonNetwork.inRoom && PhotonNetwork.playerList.Length != players)
            {
                connText.text += "\nPlayers: " + PhotonNetwork.playerList.Length + "/4";
                players = PhotonNetwork.playerList.Length;
            }
        }
        if (rec == null && PlayerHead.instance != null)
            rec = PlayerHead.instance.GetComponentInParent<PhotonVoiceRecorder>();
    }

    [BitStrap.Button]
    public void ForceTransferMaster()
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            PhotonNetwork.SetMasterClient(PhotonNetwork.player);
    }

    GameObject myPlayer;
    bool isMaster;

    void OnJoinedRoom()
    {
        Debug.Log("Joined Multiplayer Room - Current Players: " + (PhotonNetwork.otherPlayers.Length+1));
        Time.timeScale = 1f;
        OnRoomJoined.Invoke();
        SetupObjectsInRoom();
        if (PhotonNetwork.isMasterClient)
        {
            isMaster = true;
        }
        else
        {
            try
            {
                if (SteamManager.Initialized)
                {
                    foreach (var v in PhotonNetwork.otherPlayers)
                    {
                        object sid = "";
                        v.customProperties.TryGetValue("SteamID", out sid);
                        string SteamID = "";
                        if (sid != null)
                            SteamID = sid.ToString();
                        ulong i = 0;
                        ulong.TryParse(SteamID, out i);
                        if (i > 0)
                        {
                            Steamworks.SteamFriends.SetPlayedWith(new Steamworks.CSteamID(i));
                        }
                    }
                }
            }
            catch { }
        }
        if (NonVive)
            return;
        int num = PhotonNetwork.player.ID - 1;

        myPlayer = null;
        StopLoadingCam();
        ArrowEffects.EffectsDisabled = false;
#if UNITY_STANDALONE || UNITY_WSA || UNITY_PS4
        if (!isSpectator) 
        {
            CreateArcher();
        }
        else
        {
            CreateSpectator();
        }
#else
        CreateMobilePlayer();
#endif
        AudioListener lstn = loadingCamera.GetComponentInChildren<AudioListener>();
        if (lstn != null)
            lstn.enabled = false;
    }

    void SetupObjectsInRoom()
    {
        foreach (var v in SPObjects) {
            v.SetActive(false);
        }
        foreach (var v in MPObjects) {
            v.SetActive(true);
        }
    }

    public void CreateArcher()
    {
        if (myPlayer != null)
            PhotonNetwork.Destroy(myPlayer);
#if UNITY_PS4
        myPlayer = PhotonNetwork.Instantiate("Base/PsvrPlayer", StartNode.getPosition(0).position, Quaternion.identity, 0);
#else
        myPlayer = PhotonNetwork.Instantiate("Base/GAME_PLAYER", StartNode.getPosition(0).position, Quaternion.identity, 0);
#endif
        myPlayer.transform.eulerAngles = new Vector3(0, 180, 0);
        myPlayer.GetComponent<PlayerSync>().InitPlayer();
        Invoke("SetPosition", 1.0f);
    }

    public void CreateSpectator()
    {
        if (myPlayer != null)
            PhotonNetwork.Destroy(myPlayer);
        myPlayer = PhotonNetwork.Instantiate("Base/SPECTATOR", new Vector3(60, 0, 60), Quaternion.identity, 0);
        myPlayer.GetComponent<FullSpectatorSync>().InitPlayer();
    }

    public void CreateMobilePlayer()
    {
        if (myPlayer != null)
            PhotonNetwork.Destroy(myPlayer);
        myPlayer = PhotonNetwork.Instantiate("Base/MobilePlayer", StartNode.getPosition(0).position, Quaternion.identity, 0);
        myPlayer.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    void SetPosition()
    {
        StartNode.ForceUse();
    }

    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        Debug.Log("New Player Joined: " + plr.name);
        if (SteamManager.Initialized)
        {
            object sid = "";
            plr.customProperties.TryGetValue("SteamID", out sid);
            if (sid == null)
                return;
            string SteamID = sid.ToString();
            ulong i = 0;
            ulong.TryParse(SteamID, out i);
            if (i > 0)
                Steamworks.SteamFriends.SetPlayedWith(new Steamworks.CSteamID(i));
        }
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer plr)
    {
        Debug.Log("Player Disconnected: " + plr.name);
        if (PhotonNetwork.isMasterClient && !isMaster)
        {
            isMaster = true;
            if (startText != null)
                startText.text = "Host left - you are the new host!";
        }
    }

    void StopLoadingCam()
    {
        loadingCamera.SetActive(false);
    }

    void OnLeftRoom()
    {
        Debug.Log("Player Left Network");
        isSpectator = false;
        foreach (var v in SPObjects) {
            v.SetActive(true);
        }
        foreach (var v in MPObjects) {
            v.SetActive(false);
        }
        AudioListener lsn = loadingCamera.GetComponentInChildren<AudioListener>();
        if(lsn != null)
            lsn.enabled = true;
    }

    public string kickName;
    [AdvancedInspector.Inspect]
    void KickPlayer()
    {
        if (PhotonNetwork.inRoom)
        {
            foreach (var v in PhotonNetwork.otherPlayers)
            {
                if (v.name == kickName)
                {
                    PhotonNetwork.CloseConnection(v);
                }
            }
        }
    }

    [AdvancedInspector.Inspect]
    void CloseRoom()
    {
        PhotonNetwork.room.open = false;
    }

    [AdvancedInspector.Inspect]
    void ForceMaster()
    {
        PhotonNetwork.SetMasterClient(PhotonNetwork.player);
    }
}
