using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PS4Invitation : MonoBehaviour
{
    public bool AcceptingInvite
    {
        get;
        set;
    }

    public Sony.NP.Matching.SessionInvitationEventResponse CurrentInvitation
    {
        get;
        private set;
    }

    public Sony.NP.Matching.PlayTogetherHostEventResponse PlayTogetherInvitation
    {
        get;
        private set;
    }

    public void ClearInvitation()
    {
        CurrentInvitation = null;
        PlayTogetherInvitation = null;
    }

    public PS4Invitation()
    {
        AcceptingInvite = false;
    }

    // Use this for initialization
    void Start () {
        Sony.NP.Main.OnAsyncEvent += OnNPAsyncEvent;
    }

    public void OnNPAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
    {
        // Notifications
        if (callbackEvent.Service == Sony.NP.ServiceTypes.Notification)
        {
            switch (callbackEvent.ApiCalled)
            {
                case Sony.NP.FunctionTypes.NotificationSessionInvitationEvent:  // Invitation accepted
                    ClearInvitation();
                    Debug.Log("PS4Invitation - Invitation");
                    CurrentInvitation = (Sony.NP.Matching.SessionInvitationEventResponse)callbackEvent.Response;
                    break;
                case Sony.NP.FunctionTypes.NotificationPlayTogetherHostEvent:
                    ClearInvitation();
                    Debug.Log("PlayTogether");                    
                    PlayTogetherInvitation = (Sony.NP.Matching.PlayTogetherHostEventResponse)callbackEvent.Response;
                    DumpPlayTogetherDetails(PlayTogetherInvitation);
                    break;
                case Sony.NP.FunctionTypes.NotificationRefreshRoom:
                    break;
                default:
                    break;
            }
        }
    }

    static public void DumpPlayTogetherDetails(Sony.NP.Matching.PlayTogetherHostEventResponse details)
    {
        OnScreenLog.Add("PlayTogetherHostEventResponse");
        OnScreenLog.Add("UserId:" + details.UserId);
        OnScreenLog.Add("Invitees");
        for(int i=0;i<details.Invitees.Length;++i)
            OnScreenLog.Add(string.Format("\t{0} - {1}", details.Invitees[i].AccountId, details.Invitees[i].OnlineId));
    }
}
