using UnityEngine;
using System.Collections;
//using Parse;
using System.Collections.Generic;
using UnityEngine.XR;
using SimpleJSON;
using UnityEngine.Networking;
using Sony.NP;
#if UNITY_PS4
#else
using Valve.VR;
#endif

#if STEAM
using Steamworks;
#endif

using System.Text;

public class PlatformSetup : MonoBehaviour
{
    public string UserName; 
    public string UserID;
    public string DisplayName;

    public static bool SetupStarted = false;
    public static bool Initialized = false;

    public List<Friend> Friends;
#if STEAM
    Callback<GameServerChangeRequested_t> m_GameServerChangeRequested;
#endif

    public static PlatformSetup instance;
    public bool serverchecked;

    //public PlayerProfile Version;

    public bool CompletedSetup;
#if UNITY_PS4
    string usedPlatform = "PSVR";
#else
    string usedPlatform = "Steam";
#endif

    public string TestCMD;

    void Awake()
    {
        if (instance == null)
        {
            transform.parent = null;
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (PhotonNetwork.SendMonoMessageTargets == null)
                PhotonNetwork.SendMonoMessageTargets = new HashSet<GameObject>();
            PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        }
        else
            Destroy(gameObject);
#if UNITY_WSA
        usedPlatform = "UWP";
#endif
    }
    
    IEnumerator CMDArgs()
    {
        string[] cli = System.Environment.GetCommandLineArgs();
#if UNITY_EDITOR
        cli = TestCMD.Split(' ');
#endif
        List<string> commands = new List<string>();
        foreach(var v in cli)
        {
            commands.Add(v);
        }
        string SettingCommands = Settings.Get("ExtraCommands");
        if (SettingCommands != null &&  SettingCommands.Length > 3)
        {
            SettingCommands = SettingCommands.Replace("\"", "");
            foreach (var v in SettingCommands.Split(' '))
            {
                commands.Add(v);
            }
        }
        string ProfileUN = null;
        string ProfilePW = null;
        string vgToken = null;
        bool ArcadeMode = false;
        float EnemyDensity = 1;
        float diffMult = 1;
        string s = "";
        if (commands.Count > 1)
        {
            s = commands[0];
            for (int i = 1; i < commands.Count; i++)
            {
                s += " " + commands[i];
                if (i + 1 < commands.Count)
                {
                    string cmd = commands[i];
                    string arg = commands[i + 1];
                    if (cmd == "+login")
                    {
                        Debug.Log("[CLI] Login UN entered");
                        ProfileUN = arg;
                    }
                    else if (cmd == "+pin")
                    {
                        ProfilePW = arg;
                        Debug.Log("[CLI] Login PW entered");
                    }
                    else if (cmd == "-vg_token")
                    {
                        AppBase.v.platform = Platform.VirtualGate;
                        vgToken = arg;
#if (UNITY_STANDALONE || UNITY_WSA) && STEAM
                        VGUserToken = vgToken;
#endif
                        Debug.Log("[CLI] Virtualgate Mode Activated");
                    }
                    else if(cmd == "+Omni")
                    {
                        usedPlatform = "Omniverse";
                        AppBase.v.platform = Platform.Omni;
                        ToggleMenu.MenuDisabled = true;
                        Debug.Log("[CLI] Omniverse Mode Activated");
                    }
                    else if ((cmd == "+arcade" || cmd == "+ArcadeMode") && arg == "1")
                    {
                        ArcadeMode = true;
                        AppBase.v.ArcadeMode = true;
                        Debug.Log("[CLI] Arcade Mode Activated");
                    }
                    else if(cmd == "+enemyDensity")
                    {
                        float x = 0;
                        float.TryParse(arg, out x);
                        if (x > 0.1f)
                            EnemyDensity = Mathf.Clamp(x, 0.1f, 10f);
                        Debug.Log("[CLI] Enemy Density set to " + EnemyDensity);
                    }
                    else if (cmd == "+difficultyMult")
                    {
                        float x = 0;
                        float.TryParse(arg, out x);
                        if (x > 0.1f)
                            diffMult = Mathf.Clamp(x, 0.1f, 10f);
                        Debug.Log("[CLI] Difficulty Multiplier set to " + diffMult);
                    }
                    else if(cmd == "+language")
                    {
                        string lang = arg;
                        if (I2.Loc.LocalizationManager.GetAllLanguagesCode().Contains(lang))
                            I2.Loc.LocalizationManager.CurrentLanguageCode = lang;
                        Debug.Log("[CLI] Language Set to " + I2.Loc.LocalizationManager.CurrentLanguageCode);
                    }
                    else if(cmd == "+replace_settings")
                    {
                        Settings.ReplaceSettings(arg);
                        Debug.Log("[CLI] Replacement Setting Used (Check settings.ini for current settings values)");
                    }
                    else if(cmd == "+leaderboardID")
                    {
                        AppBase.v.LeaderboardID += arg;
                        Debug.Log("[CLI] Custom Leaderboard ID: " + AppBase.v.LeaderboardID);
                    }
                    else if (cmd == "+AutoStart")
                    {
                        AppBase.v.AutoStart = true;
                        Debug.Log("[CLI] Auto-Start Activated");
                    }
                    else if (cmd == "+NoLobby")
                    {
                        AppBase.v.AutoStart = true;
                        AppBase.v.NoLobby = true;
                        Debug.Log("[CLI] Auto-Start & Lobby-Skip Activated");
                    }
                    else if (cmd == "+OfflineMode" && arg == "1")
                    {
                        AppBase.v.OfflineMode = true;
#if OBSERVR_SVR
                        ObserVR_Analytics.AnalyticsEnabled = false;
#endif
                        Debug.Log("[CLI] Offline Mode Activated");
                    }
                }
            }
            Debug.Log("Full command list: " + s);
        }
        if (vgToken != null)
            ConnectFove();
        if(ArcadeMode)
        {
            EnemyDB.v.EnemyDensity = EnemyDensity;
            EnemyDB.v.DifficultyTimeMult = diffMult;
        }

        //Wait for Login
        while (!AppBase.v.networkConnected || !serverchecked // || ParseUser.CurrentUser == null
            )
        {
            yield return new WaitForSeconds(0.5f);
        }
        if(ProfileUN != null && ProfilePW != null)
        {
            User.Login(ProfileUN, ProfilePW, null);
        }
    }

    void ConnectFove()
    {
        Debug.Log("Changing Platform: VGArcade"); 
        usedPlatform = "VGArcade";
    }

    //Initialization of Parse and Steam
    IEnumerator Start()
    {
        if (!SetupStarted)
        {
            while (!PlayerProfile.Ready)
                yield return null;

            SetupStarted = true;
            yield return true;
            Debug.Log("Game Version: " + Application.version);
            if (!AppBase.v.OfflineMode)
                yield return StartCoroutine("checkInternetConnection");

#if UNITY_PS4
            AppBase.v.platform = Platform.PS4;
#endif

            StartCoroutine("CMDArgs");
            CheckVRUnit();
            if (!AppBase.v.OfflineMode)
                StartCoroutine("checkServerOnline");
            Friends = new List<Friend>();
            UserName = "Archer " + Random.Range(0, 100);
            PhotonNetwork.playerName = UserName;
            yield return true;
            yield return StartCoroutine(AttachPlatform());
            PhotonNetwork.playerName = UserName;
            DisplayName = UserName;
            Debug.LogFormat("Set PhotonNetwork.playerName to {0}", DisplayName);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props.Add("UserID", UserID);
            string accountId = SonyNpUserProfiles.GetLocalAccountId(int.Parse(UserID)).Id.ToString();
            props.Add("AccountID", accountId);
            props.Add("DisplayName", DisplayName);
            PhotonNetwork.player.SetCustomProperties(props);
            /*
            ObserVR.ObserVR_Events.CustomEvent("VERSION_INFO")
                .AddParameter("version", AppBase.v.NetworkVersion)
                .AddParameter("beta", AppBase.v.isBeta)
                .EndParameters();
            */
            yield return InitUser();

            while (Armory.instance == null)
            {
                yield return true;
            }
#if UNITY_PS4
#else
        QueryVersion();
#endif

            if (!AppBase.v.OfflineMode)
            {
                Armory.instance.FetchFromServer();
                Cosmetics.FetchFromServer();
            }
            CompletedSetup = true;
            Initialized = true;
            while (!Armory.ValidFetch || !Cosmetics.Fetched)
            { yield return true; }
            Achievement.CheckAllAchievements();
        }
    }

    IEnumerator InitUser()
    {
        bool userSetup = false;
        bool invalidSession = false;
        if (!AppBase.v.OfflineMode)
        {
            //var conf = ParseConfig.GetAsync();
            //while (!conf.IsCompleted)
            //yield return null;
            //if (conf.IsFaulted || conf.IsCanceled)
            //{
            //foreach (var e in conf.Exception.InnerExceptions)
            //{
            //ParseException parseException = (ParseException)e;
            //if (parseException.Code == ParseException.ErrorCode.InvalidSessionToken)
            //{
            //Debug.Log("Invalid Session Token: Relogging");
            //invalidSession = true;
            //}
            //}
            //}
            /*
            if (Parse.ParseUser.CurrentUser == null || ParseUser.CurrentUser.Username != UserID || invalidSession)
            {
                if (ParseUser.CurrentUser != null)
                {
                    var task = ParseUser.LogOutAsync();
                    while (ParseUser.CurrentUser != null)
                        yield return null;
                }
                ParseUser.LogInAsync(UserID, UserID).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        var user = new ParseUser()
                        {
                            Username = UserID,
                            Password = UserID
                        };
                        user["name"] = UserName;
                        user["TimePlayed"] = 10;
                        user.SignUpAsync().ContinueWith(x =>
                        {
                            if (x.IsFaulted || x.IsCanceled)
                            {
                                //Signup Failed
                                //TODO: Handle signup failure
                                Debug.Log("-- User Signup Failed --");
                                Debug.Log(x.Exception.InnerException.Message);
                                userSetup = true;
                            }
                            else
                            {
                                userSetup = true;
                                Debug.Log("-- New User Created --");
                                //User created
                            }
                        });
                    }
                    else
                    {
                        userSetup = true;
                        Debug.Log("-- User Login Successful --");
                        //User logged in
                    }
                });
            }
            else
            {
                var task = ParseUser.CurrentUser.FetchAsync();
                while (!task.IsCompleted)
                {
                    yield return null;
                }
                userSetup = true;
                if (task.IsFaulted || task.IsCanceled)
                {
                    foreach (var e in task.Exception.InnerExceptions)
                    {
                        ParseException parseException = (ParseException)e;
                        Debug.Log("Error updating user: " + parseException.Message);
                    }
                }
                else
                    Debug.Log("-- Existing User Update Successful --");
            }
            while (!userSetup)
            {
                yield return true;
            }
            */
            /*
            if (ParseUser.CurrentUser != null)
            {
                if (ParseUser.CurrentUser.Get<string>("name") != UserName)
                    ParseUser.CurrentUser["name"] = UserName;
                ParseUser.CurrentUser["LatestVersion"] = Application.version;
                if (!AppBase.v.isDemo)
                    ParseUser.CurrentUser["FullVersion"] = true;
            }
            */
            PlayerProfile.Profile.Username = UserName;
            Statistics.SetUser();
        }
        Statistics.AddValue("Sessions", 1f);
        //InvokeRepeating("TimeCounter", 10f, 10f);
#if STEAM
        m_GameServerChangeRequested = Callback<GameServerChangeRequested_t>.Create(OnGameServerChangeRequested);
#endif
        yield return null;
    }


    IEnumerator AttachPlatform()
    {
        if (AppBase.v.platform == Platform.Steam)//usedPlatform == "Steam")
        {
            Debug.Log("Attaching Steam Account");
            yield return StartCoroutine(AttachSteam());
        }
        else if (AppBase.v.platform == Platform.VirtualGate)//usedPlatform == "VGArcade")
        {
            Debug.Log("Starting VGArcade Connection Coroutine");
            yield return StartCoroutine(AttachVGArcade());
        }
        else if(AppBase.v.platform == Platform.UWP)//usedPlatform == "UWP")
        {
            Debug.Log("Attaching Windows Live Account");
            yield return StartCoroutine(AttachWSA());
        }
        else if(AppBase.v.platform == Platform.Omni)//usedPlatform == "Omniverse")
        {
            Debug.Log("Attaching Omniverse SDK");
            yield return StartCoroutine(AttachOmni());
        }
        else if (AppBase.v.platform == Platform.PS4)//usedPlatform == "Omniverse")
        {
            Debug.Log("Attaching Omniverse SDK");
            yield return StartCoroutine(AttachPS4());
        }
        else
            Debug.LogError("No Platform Found");
    }

#region Platforms
    IEnumerator AttachSteam()
    {
#if STEAM
        while (!SteamManager.Initialized)
        {
            yield return true;
        }
        Achievement.Setup(new CGameID(SteamUtils.GetAppID()));
        UserName = SteamFriends.GetPersonaName();
        CSteamID myID = SteamUser.GetSteamID();
        UserID = myID.ToString();
        PopulateFriendsList();
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("SteamID", UserID);
        Debug.Log("Username: " + UserName + " - SteamID: " + UserID);
        PhotonNetwork.player.SetCustomProperties(props);
#else
        throw new System.Exception("Tried to attach steam, but steam is not defined!");
#endif
    }

    string VGUserToken;
    IEnumerator AttachVGArcade()
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post("https://game.virtual-gate.co.kr/v0/check", form);
        www.SetRequestHeader("x-user-token", VGUserToken);
        www.SetRequestHeader("x-contents-key", "5ef14d6e-179d-4119-b200-7293ce05b458");
        www.SetRequestHeader("x-developer-key", "JP_00006");
        yield return www.Send();
        JSONNode resp = JSON.Parse(www.downloadHandler.text);
        if (resp["member"] != null && resp["member"]["name"] != null)
        {
            Debug.Log("Virtual Gate User Authenticated: " + resp["member"]["name"]);
            UserName = resp["member"]["name"];
            UserID = "VGArcade_" + UserName;
        }
        else if(resp["name"] != null && resp["name"] != null)
        {
            Debug.Log("Virtual Gate User Authenticated: " + resp["name"] + "( " + resp["user_id"] + " )");
            UserName = resp["name"];
            UserID = "VGArcade_" + resp["user_id"];
        }
        else if(resp["message"] != null)
        {
            Debug.LogError("Virtual Gate User Authentication Failed: " + resp["message"]);
            Note n = new Note();
            n.title = "Error";
            n.body = resp["message"];
            n.icon = AppBase.v.ErrorIcon;
            Notification.Notify(n);
        }
        //https://api.ipify.org/ - Get Public IP
    }

    IEnumerator AttachPS4()
    {
#if UNITY_EDITOR
        yield return true;
#elif UNITY_PS4
        /*
        ToggleMenu.MenuDisabled = true;
        Cursor.visible = false;
        OVSDK.Init(1104, "9dd4c31147eaae553635cbff592f45f2", "omni=1");
        while (!OVSDK.HasInitialized())
        {
            yield return true;
        }
        Debug.Log("-- OVSDK Initialized --");
        OVSDK.UserInfo OmniUser = OVSDK.GetUserInfo();
        Debug.Log("Omniverse User: " + UserName);
        */

        UnityEngine.PS4.PS4Input.LoggedInUser loggedInUser = UnityEngine.PS4.PS4Input.RefreshUsersDetails(0);
        UserID = loggedInUser.userId.ToString();
        UserName = loggedInUser.userName;
        yield return true;
#else
        throw new System.Exception("Tried to attach ps4 when not built for ps4");
#endif        
    }    

    IEnumerator AttachOmni()
    {
        ToggleMenu.MenuDisabled = true;
        Cursor.visible = false;
        OVSDK.Init(1104, "9dd4c31147eaae553635cbff592f45f2", "omni=1");
        while(!OVSDK.HasInitialized())
        {
            yield return true;
        }
        Debug.Log("-- OVSDK Initialized --");
        OVSDK.UserInfo OmniUser = OVSDK.GetUserInfo();
        UserID = "Omniverse_" + OmniUser.sUserName;
        UserName = OmniUser.sUserName;
        Debug.Log("Omniverse User: " + UserName);
        /*
        Screen.fullScreen = true;
        Cursor.visible = false;
        */
    }

    IEnumerator AttachWSA()
    {
        yield return true;
    }
#endregion

    static public void CheckVRUnit()
    {
        string Device = XRDevice.model.ToLower();
#if UNITY_PS4
        if (true)
        {
            Debug.Log("VR Headset: PSVR");
            AppBase.v.controls = ControllerType.PSVR;
        }
#else
        if (Device.Contains("oculus"))
        {
            Debug.Log("VR Headset: Oculus");
            AppBase.v.controls = ControllerType.OculusTouch;
        }
#if !UNITY_WSA
        else if (Device.Contains("vive"))
        {
            Debug.Log("VR Headset: Vive");
            AppBase.v.controls = ControllerType.ViveWand;
            /*
            try {
                CVRSystem system = OpenVR.System;
                if (system != null)
                {
                    ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
                    for (int j = 1; j < 5; j++)
                    {
                        uint capacity = system.GetStringTrackedDeviceProperty((uint)j, ETrackedDeviceProperty.Prop_RenderModelName_String, null, 0, ref error);
                        if (capacity > 1)
                        {
                            StringBuilder buffer = new StringBuilder((int)capacity);
                            for (int i = 0; i < capacity; i++)
                            {
                                system.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_RenderModelName_String, buffer, capacity, ref error);
                                string trackedObj = buffer.ToString().ToLower();
                                if (trackedObj.Length > 0)
                                {
                                    //Debug.Log("Controller Type [" +j+"-"+ i + "]: " + trackedObj);
                                    if (trackedObj.Contains("knuckles"))
                                        AppBase.v.controls = ControllerType.SteamVRKnuckles;
                                }
                            }
                        }
                    }
                }
            }
            catch { }    
            */
        }
#endif
        else
        {
            Debug.Log("VR Headset: None");
            AppBase.v.controls = ControllerType.MouseAndKeyboard;
        }
#endif
    }

    void QueryVersion()
    {
        string v = Application.version;
        /*
        var query = ParseObject.GetQuery("VersionInfo").WhereEqualTo("Version", v);
        query.FirstAsync().ContinueWith(t =>
        {
            if(t.IsCanceled || t.IsFaulted)
            {
                Version = new ParseObject("VersionInfo");
                Version["Version"] = v;
                Version["Sessions"] = 1;
                Version["TimePlayed"] = 10;
                Version.SaveAsync();
            }
            else
            {
                Version = t.Result;
                Version.Increment("Sessions");
            }
        });
        */
        PlayerProfile.Profile.Version.Version = v;
        PlayerProfile.Profile.Version.Sessions++;
        PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
    }

    void TimeCounter()
    {
        /*
        if(ParseUser.CurrentUser != null && ParseUser.CurrentUser.ContainsKey("TimePlayed"))
        {
            Statistics.AddValue("TimePlayed", 30f);
            if (User.ArcadeMode && User.ArcadeUser != null)
            {
                if (User.hasSaved())
                    User.ArcadeUser.Increment("TimePlayed", 30f);
                else
                    Statistics.AddValue("TempArcadeTime", 30f);
            }
            Statistics.SaveStatistics();
            IncrementVersionValue("TimePlayed", 30);
        }
        */

        Statistics.AddValue("TimePlayed", 30f);
        PlayerProfile.Profile.Version.TimePlayed += 30f;
        PlayerProfile.Profile.Version.Sessions++;
        PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Analytics);

        Statistics.SaveStatistics();
        //IncrementVersionValue("TimePlayed", 30);
    }

    //REWRITE FOR NON STEAM
    public void ResetName()
    {
        if(SteamManager.Initialized)
        {
#if STEAM
            UserName = SteamFriends.GetPersonaName();
            CSteamID myID = SteamUser.GetSteamID();
            if(PhotonNetwork.connected)
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("SteamID", myID.ToString());
                PhotonNetwork.player.SetCustomProperties(props);
                PhotonNetwork.player.name = UserName;
            }
#endif
        }
    }

    public void IncrementVersionValue(string key, int value)
    {
        var verVal = PlayerProfile.Profile.VersionValue;
        if (!verVal.ContainsKey(key))
            verVal[key] = value;
        else
            verVal[key] += value;
        PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
        //if(Version != null)
        //{
            //Version.Increment(key, value);
            //Version.SaveAsync();
        //}
    }

#region Friends

    void PopulateFriendsList()
    {
        if (!SteamManager.Initialized)
            return;
#if STEAM
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; ++i)
        {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
            EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendSteamId);
            Friend newFriend = new Friend(friendName, friendSteamId, friendState);
            Friends.Add(newFriend);
        }
        CSteamID bid = new CSteamID(76561198025035958);
        Friend blueteak = new Friend("Blueteak", bid, SteamFriends.GetFriendPersonaState(bid));
        Friends.Add(blueteak);
#endif
        UpdateFriendsList();
    }

    public bool hasUpdatedFriends()
    {
        return updatedFriends;
    }

    bool updatedFriends;
    public bool UpdateFriendsList()
    {
        updatedFriends = false;
        if (!PhotonNetwork.connected || !PhotonNetwork.insideLobby //|| !SteamManager.Initialized
            || Friends.Count == 0)
            return false;
        string[] names = new string[Friends.Count];
        for(int i=0; i<names.Length; i++)
        {
            names[i] = Friends[i].Name;
        }
        PhotonNetwork.FindFriends(names);
        return true;
    }


    public void OnUpdatedFriendList()
    {
        if(PhotonNetwork.Friends == null)
        {
            updatedFriends = true;
            return;
        }
        foreach(FriendInfo f in PhotonNetwork.Friends)
        {
            foreach(var v in Friends)
            {
                if(f.Name == v.Name)
                {
                    v.inRoom = f.IsInRoom;
                    v.currentRoom = f.Room;
                    v.online = f.IsOnline;
                }
            }
        }
        SortFriends();
        updatedFriends = true;
    }

    void SortFriends()
    {
        Friends.Sort((x, y) =>
        {
            int a = 0;
            int b = 0;

            a += (x.GetStatus() == "Online") ? 10 : 0;
            b += (y.GetStatus() == "Online") ? 10 : 0;

            a += x.isOnline() ? 100 : 0;
            b += y.isOnline() ? 100 : 0;

            a += x.inRoom ? 1000 : 0;
            b += y.inRoom ? 1000 : 0;

            a += x.Name == "Blueteak" ? 1 : 0;
            b += y.Name == "Blueteak" ? 1 : 0;

            if (a != b)
                return b.CompareTo(a);
            return y.Name.CompareTo(x.Name);
        });
    }

#endregion

#if STEAM
    void OnGameServerChangeRequested(GameServerChangeRequested_t pCallback)
    {
        Debug.LogError("Private matches not yet supported: " + pCallback.m_rgchServer);
    }
#endif

    float t = 0;
    void LateUpdate()
    {
#if STEAM
        if (!SteamManager.Initialized)
            return;
        SteamAPI.RunCallbacks();
#endif
        t += Time.unscaledDeltaTime;
        if(t >= 30f)
        {
            t = 0;
            TimeCounter();
        }
    }

    void OnApplicationQuit()
    {
        Statistics.SaveStatistics();
    }

    IEnumerator checkInternetConnection()
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        bool connected = (www.error == null);
        if(AppBase.v != null)
            AppBase.v.networkConnected = connected;
    }

    IEnumerator checkServerOnline()
    {
        if (AppBase.v != null)
        {
            WWW www = new WWW(AppBase.v.AppServer);
            yield return www;
            if(www.isDone)
            {
                if (www.text.Equals("{\"error\":\"unauthorized\"}"))
                    AppBase.v.serverOnline = true;
                else
                    AppBase.v.serverOnline = false;
            }
            else
            {
                AppBase.v.serverOnline = false;
            }
            serverchecked = true;
        }
    }

    void OnJoinedRoom()
    {
        Statistics.ClearCurrent();
    }

    void OnLeftRoom()
    {
        Statistics.ClearCurrent();
    }
}


[System.Serializable]
public class Friend
{
    public string Name;    
    public bool online;
    public bool inRoom;
    public string currentRoom;
    public string State;
#if STEAM
    public CSteamID ID;
    EPersonaState currentState;
#endif

    public bool shouldShow;

#if STEAM
    public Friend(string name, CSteamID id, EPersonaState state)
    {
        ID = id;
        Name = name;
        currentState = state;
        State = GetStatus();
        shouldShow = (isInRoom() || isOnline() || GetStatus() == "Online");
    }
#endif

    public bool isOnline()
    {
        return online;
    }

    void UpdateState(bool Online, bool InRoom, string curRoom)
    {
#if STEAM
        currentState = SteamFriends.GetFriendPersonaState(ID);
#endif
        online = Online;
        inRoom = InRoom;
        currentRoom = curRoom;
        shouldShow = (inRoom || online || GetStatus() == "Online");
        State = GetStatus();
    }

    public bool isInRoom()
    {
        return inRoom;
    }

    public string GetRoom()
    {
        return currentRoom;
    }

    public string GetStatus()
    {
        if (inRoom)
            return "In Room";
        if (online)
            return "In Game";
#if STEAM
        if (currentState == EPersonaState.k_EPersonaStateOnline)
            return "Online";
        if (currentState == EPersonaState.k_EPersonaStateOffline)
            return "Offline";
        if (currentState == EPersonaState.k_EPersonaStateAway || currentState == EPersonaState.k_EPersonaStateBusy
            || currentState == EPersonaState.k_EPersonaStateSnooze)
            return "Unavailable";
#endif
        return "Unknown";
    }
}