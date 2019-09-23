using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Parse;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class PvPMatchmaking : MonoBehaviour {

    public static PvPMatchmaking instance;
    public static bool LookingForMatch;
    public static bool ConfirmingMatch;
    public static bool AcceptedMatch;
    public static string MapToJoin;
    public static bool MatchCreated;

    public static bool inRequest;

    #region Static Methods

    public static void Initialize()
    {
        if (instance != null)
            return;
        LookingForMatch = false;
        AcceptedMatch = false;
        ConfirmingMatch = false;
        GameObject PVPManager = new GameObject();
        PVPManager.name = "PVP_Manager";
        instance = PVPManager.AddComponent<PvPMatchmaking>();
    }

    public static void ConfirmMatch(bool owner)
    {
        ConfirmingMatch = true;
        UnityMainThreadDispatcher.Instance().Enqueue(() => PVPMatchUI.OpenUI("Opponent Found!", "Join Game?"));
        UnityMainThreadDispatcher.Instance().Enqueue(() => { instance.CancelInvoke("TryAgain"); instance.Invoke("TryAgain", 15f); });
    }

    public static bool ToggleWantMatch(bool val)
    {
        if (pvpmanager.instance.PlayingPVP)
            return false;
        LookingForMatch = val;
        if(!val)
        {
            Debug.Log("Tried to leave PvP Q but No Q entered");
            //instance.LeaveQueue();
        }
        else if(val)
        {
            if(PhotonNetwork.inRoom)
            {
                if (PhotonNetwork.room.CustomProperties.ContainsKey("visibility"))
                {
                    string visibility = PhotonNetwork.room.CustomProperties["visibility"].ToString();
                    if (visibility == "PUBLIC")
                        Notification.instance.AddNote(new Note("Sorry", "Networked Matchmaking still under construction", "", AppBase.v.ErrorIcon)); //instance.CheckMatchOrCreateNew(); LookingForMatch = true;
                    else if (visibility == "PRIVATE")
                    {
                        if (PhotonNetwork.isMasterClient)
                        {
                            if (PhotonNetwork.playerList.Length == 2 || PhotonNetwork.playerList.Length == 4)
                                GameTypeManager.instance.ChangeToPvP();
                            else
                                Notification.instance.AddNote(new Note("Error", "Incorrect number of players to start private PvP Game", "Need 2 or 4", AppBase.v.ErrorIcon));
                        }
                        else
                            Notification.instance.AddNote(new Note("Error", "Only Host can start private PvP Game", "", AppBase.v.ErrorIcon));
                        LookingForMatch = false;
                        return false;
                    }
                }
                else
                {
                    Debug.Log("Photon Room did not contain visibility tag");
                    LookingForMatch = false;
                    return false;
                }
            }
            else
                Notification.instance.AddNote(new Note("Sorry", "Networked Matchmaking still under construction", "", AppBase.v.ErrorIcon));//instance.CheckMatchOrCreateNew();
        }
        LookingForMatch = false; //true
        return false; //true
    }

    public static void CancelMatch()
    {
        ConfirmingMatch = false;
        LookingForMatch = false;
        AcceptedMatch = false;
        ToggleWantMatch(false);
    }

    public static void AcceptMatch()
    {
        Debug.Log("Match Accepted");
        //TODO
    }

    public static void JoinMatch(string Owner, string Matcher)
    {
        ConfirmingMatch = false;
        LookingForMatch = false;
        AcceptedMatch = false;
        MatchCreated = true;
        //Setup Match Join
    }

    #endregion

    #region Instance

    public PvPMatch Match;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InvokeRepeating("CheckExistingMatch", 2f, 2f);
    }

    public void LeaveQueue()
    {
        if(Match != null && !inRequest)
        {
            StartCoroutine("WebRequest", PvPMessageType.CANCEL);
        }
    }

    void CheckMatchOrCreateNew()
    {
        Debug.Log("Searching For Match");
        if(!inRequest)
        {
            if (Match == null)
            {
                Match = new PvPMatch(PlatformSetup.instance.UserID);
                StartCoroutine("WebRequest", PvPMessageType.JOIN);
            }
            else
                StartCoroutine("WebRequest", PvPMessageType.CHECK);
        }

    }

    void CheckExistingMatch()
    {
        if ((LookingForMatch || AcceptedMatch) && !inRequest)
        {
            StartCoroutine("WebRequest", PvPMessageType.CHECK);
        }
    }

    void WebResponse(bool success, string response)
    {

    }

    IEnumerator WebRequest(PvPMessageType msg)
    {
        inRequest = true;
        string bodyData = "{\"type\":\"" + msg.ToString() + "\", \"teamid\": \"" + Match.TeamID + "\", \"members\":\"" + GetString(Match.Players) + "\",\"grpMMR\":" + Match.GrpMMR + ",\"version\":\""+AppBase.v.NetworkVersion+"\",\"matchType\":\"2\" }";
        UnityWebRequest request = new UnityWebRequest("http://34.198.149.161:3000/PvPQueue", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            WebResponse(false, request.downloadHandler.text);
        }
        else
        {
            WebResponse(true, request.downloadHandler.text);
        }
        inRequest = false;
    }

    string GetString(List<string> array)
    {
        string s = "";
        foreach (var v in array)
        {
            s += v.Replace(',', '.') + ",";
        }
        return s.Substring(0, s.Length - 1);
    }

    #endregion

    public enum PvPMessageType
    {
        JOIN,
        CANCEL,
        CHECK,
        ACCEPT
    }

    public enum PvPMatchState
    {
        Searching,
        WaitingAcceptSelf,
        WaitingAcceptOthers,
        Declined,
        Completed
    }

    public class PvPMatch
    {
        public string TeamID;
        public PvPMatchState MatchState;
        public List<string> Players;
        public int GrpMMR;

        public PvPMatch(string teamID)
        {
            TeamID = teamID;
            MatchState = PvPMatchState.Searching;
            Players = new List<string>();
        }
    }

}

