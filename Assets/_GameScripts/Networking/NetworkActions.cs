using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class NetworkActions : MonoBehaviour {

    public UnityEvent OnJoined;
    public UnityEvent OnLeft;

    void Awake()
    {
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

	public void OnJoinedRoom()
    {
        OnJoined.Invoke();
    }

    public void OnLeftRoom()
    {
        OnLeft.Invoke();
    }

    public void OnPhotonPlayerConnected(PhotonPlayer plr)
    {

    }
}
