using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    static int PlayTogetherRetries = 3;
    static float PlayTogetherRetryInterval = 10;

    static GameState _inst;

    private void Awake()
    {
        if (_inst == null)
        {            
            _inst = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            Debug.Log("GameState created twice, destroying");
        }
    }

    static public void LeaveMultiplayer(Action onComplete = null)
    {
        if(null!=_inst)
            _inst.StartCoroutine(DoLeaveMultiplayer(onComplete));
    }

    static IEnumerator DoLeaveMultiplayer(Action onComplete)
    {
        if (_inMultiplayer)
        {
            Debug.Log("LeaveMultiplayer Start");
            _inMultiplayer = false;
#if UNITY_PS4
            yield return LeaveMultiplayerPS4();
#else
            LeaveMultiplayerPC();
#endif
        }
        if (null != onComplete)
            onComplete();
    }

    static void LeaveMultiplayerPC()
    {
        if (PhotonNetwork.inRoom)
            Debug.Log("Leaving Multiplayer Game...");
        if (FindObjectOfType<TeleportPlayer>())
            FindObjectOfType<TeleportPlayer>().SetAllActive();
        PlayerPositions[] nodes = FindObjectsOfType<PlayerPositions>();
        foreach (var n in nodes)
        {
            n.ResetAll();
        }
        if (SingleplayerSpawn.instance != null)
            SingleplayerSpawn.instance.DevCam.SetActive(true);
        Statistics.SaveStatistics();
        if (PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();
        if (LocalPlayer.instance != null)
            Destroy(LocalPlayer.instance.gameObject);
        if (MobilePlayerSync.myInstance != null)
            Destroy(MobilePlayerSync.myInstance.gameObject);
        if (SingleplayerSpawn.instance != null)
            SingleplayerSpawn.instance.SpawnSinglePlayer();
        ToggleMenu.instance.Close();
    }

    static IEnumerator LeaveMultiplayerPS4()
    {
        PS4Plus.Inst.IsMultiplePlayersActive = false;
        if (PhotonNetwork.inRoom)
            Debug.Log("Leaving Multiplayer Game...");
        if (FindObjectOfType<TeleportPlayer>())
            FindObjectOfType<TeleportPlayer>().SetAllActive();
        PlayerPositions[] nodes = FindObjectsOfType<PlayerPositions>();
        foreach (var n in nodes)
        {
            n.ResetAll();
        }
        if (SingleplayerSpawn.instance != null)
            SingleplayerSpawn.instance.DevCam.SetActive(true);
        Statistics.SaveStatistics();
        if (PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();
        PS4Matchmaking.Inst.LeaveRoom();
        if (LocalPlayer.instance != null)
            Destroy(LocalPlayer.instance.gameObject);
        if (MobilePlayerSync.myInstance != null)
            Destroy(MobilePlayerSync.myInstance.gameObject);
        ToggleMenu.instance.Close();
        PS4Photon.instance.DoDisconnect();        
        yield return new WaitForSecondsRealtime(1);
        if (SingleplayerSpawn.instance != null)
            SingleplayerSpawn.instance.SpawnSinglePlayer();
        yield return new WaitForSecondsRealtime(1);
    }

    static public bool JoinOrCreateRandomRoom()
    {
        //#if !UNITY_EDITOR && UNITY_PS4
#if UNITY_PS4
        if (PS4MatchmakingHelper.Inst.InvitePending)
        {
            Debug.Log("JoinOrCreateRandomRoom failed, invite is pending");
            return false;
        }
        else if (PS4MatchmakingHelper.Inst.JoiningRandomRoom)
        {
            Debug.Log("JoinOrCreateRandomRoom failed, already joining room");
            return false;
        }
        else
        {
            PS4MatchmakingHelper.Inst.JoiningRandomRoom = true;
            GameState.LeaveMultiplayer(() =>
            {
                PS4ConnectionCheck.CheckConnection(
                    (connectionState) =>
                    {
                        PS4MatchmakingHelper.Inst.JoiningRandomRoom = false;
                        switch (connectionState)
                        {
                            case PS4ConnectionState.Ok:
                                JoinOrCreateRandomRoomPS4();
                                break;
                            case PS4ConnectionState.ErrorCheckPlusEntitlement:
                                PS4Plus.Inst.DisplayJoinPlusDialog();
                                break;
                            case PS4ConnectionState.ErrorAlreadyChecking:
                            case PS4ConnectionState.ErrorCheckAvailability:
                            case PS4ConnectionState.ErrorPhotonCouldNotConnect:
                                Debug.LogFormat("CheckConnection failed {0}", connectionState);
                                break;
                        }
                    }
                );
            });
            return false;
        }
#else
        JoinOrCreateRandomRoomPC();
#endif    
        return true;
    }

    // Resets the game to multiplayer mode
    static public void InitMultiplayerMode()
    {
        if (!_inMultiplayer)
        {
            Debug.Log("Joining Multiplayer Game...");
            _inMultiplayer = true;
            TeleportPlayer plr = FindObjectOfType<TeleportPlayer>();
            if (plr != null)
                plr.SetAllActive();
            PlayerPositions[] nodes = FindObjectsOfType<PlayerPositions>();
            foreach (var n in nodes)
            {
                n.ResetAll();
            }
            Statistics.SaveStatistics();
            try
            {
                FindObjectOfType<Scoreboard>().ResetScores();
            }
            catch
            {
                //Debug.Log("Error finding game components");
            }
            if (SingleplayerSpawn.instance != null)
                SingleplayerSpawn.instance.DevCam.SetActive(true);
            DestroyPlayer();

            if (!PS4Plus.Inst.VoiceChatOk)
                PS4Plus.Inst.DisplayChatRestriction();
        }
        else
        {
            Debug.LogError("Error joining Multiplayer Game but already in a game!");
        }
    }

    static void JoinOrCreateRandomRoomPS4(string prefix = "", int maxPlrs = 4, bool isPrivate = false, bool forceHost = false)
    {
        var match = GameObject.FindObjectOfType<PS4MatchmakingHelper>();
        if (match.CanJoinGame)
        {
            InitMultiplayerMode();
            match.JoinRandomOrCreate(OnJoinComplete);
        }
    }

    static public void CreateRoomPS4(bool isPrivate, Action<bool> onComplete)
    {
        var match = GameObject.FindObjectOfType<PS4MatchmakingHelper>();
        if (match.CanJoinGame)
        {
            InitMultiplayerMode();
            match.CreateRoom((ok) => { OnJoinComplete(ok); if (null != onComplete) onComplete(ok); }, isPrivate);
        }
    }

    static void DestroyPlayer()
    {
        if (LocalPlayer.instance != null)
            Destroy(LocalPlayer.instance.gameObject);
        if (MobilePlayerSync.myInstance != null)
            Destroy(MobilePlayerSync.myInstance.gameObject);
    }

    static public bool AcceptInvite(Sony.NP.Matching.SessionInvitationEventResponse invite)
    {
        var match = GameObject.FindObjectOfType<PS4MatchmakingHelper>();
        if (match.CanJoinGame)
        {
            InitMultiplayerMode();
            match.JoinPSNRoom(invite.SessionId, OnJoinComplete);
            return true;
        }
        else
        {
            return false;
        }
    }

    static void OnJoinComplete(bool success)
    {
        if (success)
        {
        }
        else
        {
            GameState.LeaveMultiplayer();
        }
    }

    static void JoinOrCreateRandomRoomPC(string prefix = "", int maxPlrs = 4, bool isPrivate = false, bool forceHost = false)
    {
        if (PhotonNetwork.connected && PhotonNetwork.insideLobby)
        {
            Debug.Log("Joining Multiplayer Game...");
            TeleportPlayer plr = FindObjectOfType<TeleportPlayer>();
            if (plr != null)
                plr.SetAllActive();
            PlayerPositions[] nodes = FindObjectsOfType<PlayerPositions>();
            foreach (var n in nodes)
            {
                n.ResetAll();
            }
            Statistics.SaveStatistics();
            try
            {
                FindObjectOfType<Scoreboard>().ResetScores();
            }
            catch
            {
                //Debug.Log("Error finding game components");
            }
            if (SingleplayerSpawn.instance != null)
                SingleplayerSpawn.instance.DevCam.SetActive(true);

            ConnectAndJoinRandom.instance.Connect(prefix, maxPlrs, forceHost);

            if (LocalPlayer.instance != null)
                Destroy(LocalPlayer.instance.gameObject);
            if (MobilePlayerSync.myInstance != null)
                Destroy(MobilePlayerSync.myInstance.gameObject);
        }
    }

    static public bool InMultiplayer
    {
        get
        {
            return _inMultiplayer;
        }
    }

    static bool _inMultiplayer = false;
}
