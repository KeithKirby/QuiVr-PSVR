using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
/// <summary>
/// This script automatically connects to Photon (using the settings file),
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class PS4Photon : Photon.MonoBehaviour
{
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = false;

    public byte Version = 1;

    public bool InLobby
    {
        get;
        private set;
    }

    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;

    public static PS4Photon instance;

    public bool inRoom;
    public PS4RoomDisplay rinfo;

    bool everConnected;

    enum DisconnectState
    {
        None,
        Requested,
        UserDisconnect,
        HardDisconnect
    }

    DisconnectState _disconnectState = DisconnectState.None;


    public static bool ignoreAFK;

    void Awake()
    {
        instance = this;   
        if (PhotonNetwork.SendMonoMessageTargets == null)
            PhotonNetwork.SendMonoMessageTargets = new HashSet<GameObject>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        //PvPMatchmaking.Initialize();
    }

    public bool AutoJoinRoom;
    public bool AutoHost;
    IEnumerator Start()
    {
        while (!PhotonPs4Auth.Ready)
            yield return null;

        string[] cli = System.Environment.GetCommandLineArgs();
#if UNITY_EDITOR
    #if STEAM
            cli = PlatformSetup.instance.TestCMD.Split(' ');
    #else
    #endif
#endif
        string NetVersion = AppBase.v.NetworkVersion;
        bool overrideAFK = false;
        if (cli.Length > 1)
        {
            for (int i = 1; i < cli.Length; i++)
            {
                if (i + 1 < cli.Length)
                {
                    if (cli[i] == "+connect")
                    {
                        var roomAddress = cli[i + 1];
                        Debug.LogError("[CLI] Join Game not currently supported: " + roomAddress + " won't be joined");
                    }
                    else if (cli[i] == "+versionAdd")
                    {
                        string toAdd = cli[i + 1];
                        if (toAdd.Length > 0)
                            ignoreAFK = true;
                        NetVersion += toAdd;
                        Debug.Log("[CLI] Custom Network version set to: " + NetVersion);
                    }
                    else if (cli[i] == "+mpjoin" && cli[i + 1] == "1")
                    {
                        AutoJoinRoom = true;
                        Debug.Log("[CLI] AutoJoining room...");
                    }
                    else if (cli[i] == "+mphost" && cli[i + 1] == "1")
                    {
                        AutoJoinRoom = true;
                        AutoHost = true;
                        Debug.Log("[CLI] Autohosting room...");
                    }
                    else if (cli[i] == "+overrideAFK" && cli[i + 1] == "1")
                    {
                        overrideAFK = true;
                        Debug.Log("[CLI] AFK Override true...");
                    }
                }
            }
            if (ignoreAFK && overrideAFK)
                ignoreAFK = false;
            //+connect < ip:port >
        }

        PhotonNetwork.autoJoinLobby = true;
        yield return new WaitForSeconds(1f);
        if (AutoConnect && !AppBase.v.isDemo && !AppBase.v.OfflineMode)
        {
            Debug.Log("PhotonNetwork.ConnectUsingSettings");
            PhotonNetwork.ConnectUsingSettings(NetVersion);
        }
        else
        {
            Debug.Log("Do not want autoconnect");
        }
    }

    bool wantConnect;
    string type;
    int maxPlrs;
    bool forceHost;

    public void Connect(string prefix = "", int mp = 4, bool forceHostRoom = false)
    {
        wantConnect = true;
        type = prefix;
        maxPlrs = mp;
        forceHost = forceHostRoom;
    }

    public void DoConnect(Action<bool> connectCB)
    {
        joinRandom = false;
        wantConnect = false;
        jRoom.network = true;

        if (PhotonNetwork.connected)
        {
            Debug.Log("DoConnect already connected");
            connectCB(true); // Wait for onconnectedtomaster
        }
        else
        {
            PhotonPs4Auth.Inst.RequestAuthCode(
                (ok) =>
                {
                    if (ok)
                    {
                        if (PhotonNetwork.ConnectUsingSettings(AppBase.v.NetworkVersion))
                        {
                            Debug.Log("ConnectUsingSettings ok");
                            connectCB(true); // Wait for onconnectedtomaster
                    }
                        else
                        {
                            Debug.Log("ConnectUsingSettings failed");
                            connectCB(false);
                        }
                    }
                    else
                    {
                        Debug.Log("Auth failed");
                        connectCB(false);
                    }
                });
        }
    }

    public bool CreatePrivate(string prefix = "", int mp = 4, string nameOverride = "")
    {
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = (byte)mp };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("type", "PRIVATE_" + prefix);
        roomOptions.CustomRoomProperties.Add("visibility", "PRIVATE");
        roomOptions.CustomRoomProperties.Add("MapSeed", "0");
        roomOptions.CustomRoomProperties.Add("InEvent", false);
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "type", "MapSeed", "InEvent" };
        roomOptions.PublishUserId = true;
        string roomName = PhotonNetwork.player.NickName;
        if (nameOverride.Length > 0)
            roomName = nameOverride;
        return PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public bool MakeRoomPublic()
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            var table = PhotonNetwork.room.CustomProperties;
            string roomType = table["type"].ToString();
            if (roomType.Contains("PRIVATE_"))
            {
                roomType = roomType.Replace("PRIVATE_", "");
                Debug.Log("Changed room type to: " + roomType);
                table["type"] = roomType;
                table["visibility"] = "PUBLIC";
                PhotonNetwork.room.SetCustomProperties(table);
                return true;
            }
        }
        return false;
    }

    public bool MakeRoomPrivate()
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            var table = PhotonNetwork.room.CustomProperties;
            string roomType = table["type"].ToString();
            if (!roomType.Contains("PRIVATE_"))
            {
                roomType = "PRIVATE_" + roomType;
                Debug.Log("Changed room type to: " + roomType);
                table["type"] = roomType;
                table["visibility"] = "PRIVATE";
                PhotonNetwork.room.SetCustomProperties(table);
                return true;
            }
        }
        return false;
    }

    bool joinRandom = false;
    [HideInInspector]
    public bool InRoom;
    [HideInInspector]
    public bool Connected;
    bool FinalizingDC;
    void Update()
    {
        if (!PhotonNetwork.connected && wantConnect)
        {
            joinRandom = true;
            wantConnect = false;
            jRoom.network = true;
            PhotonNetwork.ConnectUsingSettings(AppBase.v.NetworkVersion);
        }
        else if (PhotonNetwork.connected && wantConnect)
        {
            wantConnect = false;
            ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", type }, { "InEvent", false } };
            if (!forceHost)
                PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, (byte)maxPlrs);
            else
                OnPhotonRandomJoinFailed();
            jRoom.network = true;
        }
        /*
        if (Connected && !PhotonNetwork.connected)
        {
            if (PhotonNetwork.connectionState != ConnectionState.Disconnected && !FinalizingDC)
            {
                PhotonNetwork.Disconnect();
                FinalizingDC = true;
            }
            else if (InRoom && PhotonNetwork.connectionState == ConnectionState.Disconnected && PhotonNetwork.connectionState != ConnectionState.Disconnecting)
            {
                SingleplayerSpawn.instance.DevCam.SetActive(true);
                Invoke("CheckRejoined", 5f);
                Debug.Log("Disconnected from Server: attempting to rejoin room...");
                PhotonNetwork.ReconnectAndRejoin();
                reconnecting = true;
            }
            else
            {
                Debug.Log("Reconnecting");
                PhotonNetwork.Reconnect();
                reconnecting = true;
            }
        }
        else
        {
            Connected = PhotonNetwork.connected;
            InRoom = PhotonNetwork.inRoom;
            FinalizingDC = false;
        }
        */
    }

    void CheckRejoined()
    {
        if (!PhotonNetwork.inRoom && SingleplayerSpawn.instance != null)
        {
            SingleplayerSpawn.instance.SpawnSinglePlayer();
            Note n = new Note();
            n.title = "Could not Rejoin";
            n.body = "Could not rejoin room in progress.";
            n.icon = AppBase.v.ErrorIcon;
            Notification.NotifyDelayed(n, 1.5f);
        }
    }

    public JoiningRoom jRoom;

    public void DoDisconnect()
    {
        Debug.Log("DoDisconnect");
        if (_disconnectState == DisconnectState.None)
        {
            _disconnectState = DisconnectState.Requested;
            PhotonNetwork.Disconnect();
            PhotonVoiceNetwork.Disconnect(); // Kill voice as well
        }
    }

    void OnDisconnectedFromPhoton()
    {
        // None is a hard disconnect, requested is when leaving lobby
        if(_disconnectState == DisconnectState.None)
        {
            _disconnectState = DisconnectState.HardDisconnect;
            PhotonShutdown();
        }
        else if (_disconnectState == DisconnectState.Requested)
        {
            _disconnectState = DisconnectState.UserDisconnect;
            PhotonShutdown();
        }
        else
        {
            Debug.LogFormat("Ignoring dup shutdown callback {0}", _disconnectState);
        }
    }

    void PhotonShutdown()
    {
        Debug.Log("OnDisconnectedFromPhoton");
        InLobby = false;
        if (null != jRoom)
            jRoom.network = false;
        if (everConnected)
            SingleplayerSpawn.instance.SpawnSinglePlayer();
        GameState.LeaveMultiplayer(
            () =>
            {
                if (_disconnectState == DisconnectState.HardDisconnect) // Only show this if disconnect was forced
                {
                    Note n = new Note();
                    n.title = "Disconnected";
                    n.body = "Lost connection.";
                    n.icon = AppBase.v.ErrorIcon;
                    Notification.NotifyDelayed(n, 0);
                }
                _disconnectState = DisconnectState.None;
            });
    }

    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        everConnected = true;
        if (joinRandom)
        {
            PhotonNetwork.JoinRandomRoom();
            joinRandom = false;
        }
        else
            PhotonNetwork.JoinLobby();
    }

    bool reconnecting;
    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
        PlayerCustomProps["score"] = 0;
        PhotonNetwork.player.SetCustomProperties(PlayerCustomProps);
        if (reconnecting)
        {
            Note n = new Note();
            n.title = "Reconnected";
            n.body = "Reconnected to Server";
            n.icon = AppBase.v.SuccessIcon;
            Notification.NotifyDelayed(n, 1.5f);
            reconnecting = false;
        }
        if (AutoJoinRoom)
            StartCoroutine("JoinRoomOnStart");

        InLobby = true;
    }

    IEnumerator JoinRoomOnStart()
    {
        AutoJoinRoom = false;
        while (PlayerHead.instance == null || !PhotonNetwork.insideLobby)
        {
            yield return true;
        }
        if (!AutoHost)
        {
            bool validJoin = false;
            while (!validJoin)
            {
                yield return new WaitForSeconds(0.25f);
                RoomInfo[] rooms = PhotonNetwork.GetRoomList();
                foreach (var v in rooms)
                {
                    if (v.PlayerCount < v.MaxPlayers)
                    {
                        if (MPSphere.instance != null && v.CustomProperties.ContainsKey("type"))
                        {
                            if (v.CustomProperties["type"].ToString() == MPSphere.instance.MPType)
                                validJoin = true;
                        }
                    }
                }
            }
            if (MPSphere.instance != null)
                MPSphere.instance.Click();
        }
        else if (MPSphere.instance != null)
        {
            MPSphere.instance.Click(true);
        }

    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = (byte)maxPlrs };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("type", type);
        roomOptions.CustomRoomProperties.Add("InEvent", false);
        roomOptions.CustomRoomProperties.Add("MapSeed", "0");
        roomOptions.CustomRoomProperties.Add("visibility", "PUBLIC");
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "type", "InEvent", "MapSeed" };
        roomOptions.PublishUserId = true;
        PhotonNetwork.CreateRoom(PhotonNetwork.player.NickName, roomOptions, TypedLobby.Default);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom()
    {
        OnScreenLog.Add("OnJoinedRoom()");
        inRoom = true;
        rinfo = new PS4RoomDisplay();
        rinfo.name = PhotonNetwork.room.Name;
        rinfo.maxPlayers = PhotonNetwork.room.MaxPlayers;
        foreach (var v in PhotonNetwork.room.CustomProperties)
        {
            RoomKeyPair p = new RoomKeyPair();
            p.Key = v.Key.ToString();
            p.Value = v.Value.ToString();
            rinfo.Props.Add(p);
        }
        //rinfo.Properties = PhotonNetwork.room.customProperties;
        if (PhotonNetwork.room.CustomProperties.ContainsKey("visibility") && PvPMatchmaking.LookingForMatch)
        {
            string visibility = PhotonNetwork.room.CustomProperties["visibility"].ToString();
            if (visibility == "PRIVATE")
                PvPMatchmaking.ToggleWantMatch(false);
        }
        if (TileManager.instance != null && PhotonNetwork.room.CustomProperties.ContainsKey("MapSeed"))
        {
            string seed = PhotonNetwork.room.CustomProperties["MapSeed"].ToString();
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.Log("Joined game with MapSeed: " + seed);
                TileManager.instance.CreateNew(seed);
            }
            else
                Debug.Log("Created new Room with MapSeed: " + seed);
        }
        ExitGames.Client.Photon.Hashtable PlayerCustomProps = new ExitGames.Client.Photon.Hashtable();
        PlayerCustomProps["Ht"] = 0;
        PlayerCustomProps["Hd"] = 0;
        PhotonNetwork.player.SetCustomProperties(PlayerCustomProps);
    }

    void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom()");
        inRoom = false;
        rinfo = null;
    }

    // Failed to join room
    public virtual void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        OnScreenLog.Add(codeAndMsg.ToString());
    }

    [BitStrap.Button]
    void ForceDisconnect()
    {
        PhotonNetwork.Disconnect();
    }
}

[System.Serializable]
public class PS4RoomDisplay
{
    public string name;
    public int maxPlayers;
    public List<RoomKeyPair> Props;

    public PS4RoomDisplay()
    {
        Props = new List<RoomKeyPair>();
    }
}

[System.Serializable]
public class RoomKeyPair
{
    public string Key;
    public string Value;
}
