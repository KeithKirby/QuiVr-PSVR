using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class NetworkEffect : MonoBehaviour {

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
        if (AppBase.v.OfflineMode)
            OnOfflineMode.Invoke();
    }

	void OnJoinedRoom()
    {
        JoinedRoom.Invoke();
    }

    void OnLeftRoom()
    {
        LeftRoom.Invoke();
    }
}
