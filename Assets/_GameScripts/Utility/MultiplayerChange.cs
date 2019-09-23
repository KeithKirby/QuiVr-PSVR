using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerChange : MonoBehaviour {

    public UnityEvent OnEnterMultiplayer;
    public UnityEvent OnLeaveMultiplayer;
    public bool CheckOnStart = true;

    void Start()
    {
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        if (CheckOnStart)
        {
            if (PhotonNetwork.inRoom)
                OnEnterMultiplayer.Invoke();
            else
                OnLeaveMultiplayer.Invoke();
        }
    }

    void OnJoinedRoom()
    {
        OnEnterMultiplayer.Invoke();
    }

    void OnLeftRoom()
    {
        OnLeaveMultiplayer.Invoke();
    }


}
