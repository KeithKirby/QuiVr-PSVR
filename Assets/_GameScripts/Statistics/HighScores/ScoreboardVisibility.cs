using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ScoreboardVisibility : MonoBehaviour
{
    public UnityEvent OnOfflineMode;
    public UnityEvent JoinedRoom;
    public UnityEvent LeftRoom;

    void Awake()
    {
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    IEnumerator Start()
    {
        yield return true;
        yield return true;
        if (AppBase.v.OfflineMode || !PS4Plus.Inst.UserPermissionsOk)
        {
            OnOfflineMode.Invoke();
        }
    }

    void OnJoinedRoom()
    {
        if (PS4Plus.Inst.UserPermissionsOk)
        {
            JoinedRoom.Invoke();
        }
    }

    void OnLeftRoom()
    {
        if (PS4Plus.Inst.UserPermissionsOk)
        {
            LeftRoom.Invoke();
        }
    }
}