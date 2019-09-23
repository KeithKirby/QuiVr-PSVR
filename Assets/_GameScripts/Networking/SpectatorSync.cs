using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class SpectatorSync : MonoBehaviour {

    float InterpRate = 9f;

    public bool active;

    public Text PlayerName;

    public GameObject dummyPlayer;
    public GameObject realPlayer;

    public Transform RealHead;
    public Transform RealHandLeft;
    public Transform RealHandRight;

    public Transform DummyHead;
    public Transform DummyHandLeft;
    public Transform DummyHandRight;

    Vector3 HeadPos;
    Vector3 LHandPos;
    Vector3 RHandPos;

    Quaternion HeadRot;
    Quaternion LHandRot;
    Quaternion RHandRot;

    public bool selfMute;
    public bool muted;
    AudioSource src;

    PhotonView photonView;

    public static SpectatorSync myInstance;
    public static List<SpectatorSync> Others;

    void Start()
    {
        if (Others == null)
            Others = new List<SpectatorSync>();
        if (photonView == null)
            photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.inRoom && photonView.isMine)
        {
            PlayerName.text = "";
            myInstance = this;
            GetComponent<PhotonVoiceSpeaker>().enabled = false;
            GetComponent<PhotonVoiceRecorder>().enabled = true;
        }
        else if (PhotonNetwork.inRoom && !photonView.isMine)
        {
            Others.Add(this);
            if(PlayerName != null && photonView.owner != null)
                PlayerName.text = photonView.owner.name;
        }
        realPlayer.transform.position = new Vector3(60, 0, 60);
        src = GetComponent<AudioSource>();
        HeadPos = DummyHead.transform.position;
        HeadRot = DummyHead.transform.rotation;

        LHandPos = DummyHandLeft.transform.position;
        LHandRot = DummyHandLeft.transform.rotation;

        RHandPos = DummyHandRight.transform.position;
        RHandRot = DummyHandRight.transform.rotation;
    }

    public void InitPlayer()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SetSelfMute(bool val)
    {
        if(photonView != null)
            photonView.RPC("NetworkSpecMute", PhotonTargets.OthersBuffered, val);
        selfMute = val;
    }

    [PunRPC]
    void NetworkSpecMute(bool val)
    {
        selfMute = val;
    }

    void Update ()
    {
        if(active)
        {
            if (PhotonNetwork.inRoom && !photonView.isMine)
            {
                if (!dummyPlayer.activeSelf)
                {
                    dummyPlayer.SetActive(true);
                }
                DummyHead.position = Vector3.Lerp(DummyHead.position, HeadPos, Time.deltaTime * InterpRate);
                DummyHead.rotation = Quaternion.Lerp(DummyHead.rotation, HeadRot, Time.deltaTime * InterpRate);

                DummyHandLeft.position = Vector3.Lerp(DummyHandLeft.position, LHandPos, Time.deltaTime * InterpRate);
                DummyHandLeft.rotation = Quaternion.Lerp(DummyHandLeft.rotation, LHandRot, Time.deltaTime * InterpRate);

                DummyHandRight.position = Vector3.Lerp(DummyHandRight.position, RHandPos, Time.deltaTime * InterpRate);
                DummyHandRight.rotation = Quaternion.Lerp(DummyHandRight.rotation, RHandRot, Time.deltaTime * InterpRate);

                if(src != null)
                {
                    if (selfMute || muted)
                        src.volume = 0;
                    else
                        src.volume = 0.8f;
                }              
            }
        }
        else if (dummyPlayer.activeSelf)
        {
            dummyPlayer.SetActive(false);
        }
        for (int i = Others.Count-1; i >= 0; i--)
        {
            if (Others[i] == null)
                Others.RemoveAt(i);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(RealHead.position);
            stream.SendNext(RealHead.rotation);

            //How did this get swapped??
            stream.SendNext(RealHandRight.position);
            stream.SendNext(RealHandRight.rotation);

            stream.SendNext(RealHandLeft.position);
            stream.SendNext(RealHandLeft.rotation);

            stream.SendNext(active);
        }
        else
        {
            HeadPos = (Vector3)stream.ReceiveNext();
            HeadRot = (Quaternion)stream.ReceiveNext();

            RHandPos = (Vector3)stream.ReceiveNext();
            RHandRot = (Quaternion)stream.ReceiveNext();

            LHandPos = (Vector3)stream.ReceiveNext();
            LHandRot = (Quaternion)stream.ReceiveNext();

            active = (bool)stream.ReceiveNext();
        }
    }

    public static int NumSpectators()
    {
        int i = 0;
        foreach(var v in Others)
        {
            if (v != null && v.active)
                i++;
        }
        if (myInstance != null && myInstance.active)
            i++;
        return i;
    }
}
