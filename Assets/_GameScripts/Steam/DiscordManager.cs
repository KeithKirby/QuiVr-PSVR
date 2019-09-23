using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiscordJoinEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordSpectateEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordJoinRequestEvent : UnityEngine.Events.UnityEvent<DiscordRpc.JoinRequest> { }

public class DiscordManager : MonoBehaviour {

    public DiscordRpc.RichPresence presence;
    public string applicationId = "378384013624082462";
    public string optionalSteamId = "489380"; 
    public int callbackCalls;
    public UnityEngine.Events.UnityEvent onConnect;
    public UnityEngine.Events.UnityEvent onDisconnect;
    //public DiscordJoinEvent onJoin;
    //public DiscordJoinEvent onSpectate;
    //public DiscordJoinRequestEvent onJoinRequest;
    DiscordRpc.EventHandlers handlers;

    public static DiscordManager instance;

    void OnEnable()
    {
        instance = this;
        //Debug.Log("Discord: init");
        callbackCalls = 0;
        handlers = new DiscordRpc.EventHandlers();
        handlers.readyCallback = ReadyCallback;
        handlers.disconnectedCallback += DisconnectedCallback;
        handlers.errorCallback += ErrorCallback;
        //handlers.joinCallback += JoinCallback;
        //handlers.spectateCallback += SpectateCallback;
        //handlers.requestCallback += RequestCallback;
        DiscordRpc.Initialize(applicationId, ref handlers, true, optionalSteamId);
        InvokeRepeating("CheckState", 1f, 5.379f);
    }

    void OnDisable()
    {
        DiscordRpc.Shutdown();
    }

    void CheckState()
    {
        if (callbackCalls == 0)
            return;
        string state = (AppBase.v.isBeta ? "[Beta Branch] " : "") + "Playing Solo";
        string detail = "Home Base";
        int maxSize = 0;
        int partySize = 0;
        long startTime = 0;
        string largeimagekey = "in_lobby";
        //Setup State
        if(PhotonNetwork.inRoom)
        {
            maxSize = 4;
            object vis = "";
            PhotonNetwork.room.CustomProperties.TryGetValue("visibility", out vis);
            bool privateRoom = vis.ToString() == "PRIVATE";
            partySize = PhotonNetwork.room.PlayerCount;
            if (privateRoom)
                state = (AppBase.v.isBeta ? "[Beta Branch] " : "") + "Private Game";
            else
                state = (AppBase.v.isBeta ? "[Beta Branch] " : "") + "In Multiplayer";
        }
        //Setup Detail
        if(GameBase.instance.Difficulty >= 100)
        {
            largeimagekey = "in_gate";
            startTime = GameBase.instance.startTime;
            if (!GameBase.instance.inGame)
                detail = "Game Ending";
            else if (EventManager.InEvent)
            {
                detail = "In Event";
                largeimagekey = "in_event";
            }
            else
                detail = "Gate " + (int)Mathf.Floor(GameBase.instance.Difficulty / 100);
        }
        SetPresence(state, detail, partySize, maxSize, largeimagekey, startTime);
    }

    public static void SetPresence(string state, string detail, int partySize, int maxSize, string lgImgKey, long startTime)
    {
        if(instance != null)
        {
            DiscordRpc.RichPresence presence = instance.presence;
            presence.state = state;
            presence.details = detail;
            presence.partyMax = maxSize;
            presence.partySize = partySize;
            presence.largeImageKey = lgImgKey;
            presence.startTimestamp = startTime;
            DiscordRpc.UpdatePresence(ref presence);
        }
    }

    public void ReadyCallback()
    {
        ++callbackCalls;
        Debug.Log("Discord: ready");
        onConnect.Invoke();
    }

    public void DisconnectedCallback(int errorCode, string message)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: disconnect {0}: {1}", errorCode, message));
        onDisconnect.Invoke();
    }

    public void ErrorCallback(int errorCode, string message)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: error {0}: {1}", errorCode, message));
    }

    /*
     public void JoinCallback(string secret)
     {
         ++callbackCalls;
         Debug.Log(string.Format("Discord: join ({0})", secret));
         onJoin.Invoke(secret);
     }

     public void SpectateCallback(string secret)
     {
         ++callbackCalls;
         Debug.Log(string.Format("Discord: spectate ({0})", secret));
         onSpectate.Invoke(secret);
     }

     public void RequestCallback(DiscordRpc.JoinRequest request)
     {
         ++callbackCalls;
         Debug.Log(string.Format("Discord: join request {0}: {1}", request.username, request.userId));
         onJoinRequest.Invoke(request);
     }
    */

    void Update()
    {
        DiscordRpc.RunCallbacks();
    }

}
