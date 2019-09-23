using UnityEngine;
using System.Collections;

public class GateHalf : MonoBehaviour {

    public Teleporter TeleCheck;
    public float closeSpeed;
    bool closing;
    bool closed;

    public Transform GateObject;
    Gate g;

    public GameObject GatePieces;
    public Transform PiecesHolder;

    Quaternion startRotation;

    public MeshRenderer GateDisplay;

    public AudioSource[] ClosingSounds;
    public AudioSource[] ClosedSounds;

    public Gate PrevGate;

    void Awake()
    {
        startRotation = GateObject.localRotation;
        g = GetComponentInParent<Gate>();
    }

    bool audioOn;

    void Update()
    {
        if(!closed && !g.isDisabled() && !EnemyStream.completed)
        {
            if(TeleCheck != null)
                closing = TeleCheck.hasPlayers();
            if (closing && GateObject.localEulerAngles.y > 0 && GateObject.localEulerAngles.y < 300 && !CreatureManager.bossOut)
            {
                GateObject.Rotate(-1 * Vector3.up * Time.deltaTime * calculatespeed());
                ToggleAudio(true);
            }
            else if((GateObject.localEulerAngles.y > 300 || GateObject.localEulerAngles.y <= 0) && (PrevGate == null || !PrevGate.isDestroyed()))
            {
                closed = true;
                GateObject.localEulerAngles = Vector3.zero;
                ToggleAudio(false);
                PlayEndSounds();
            }
            else
            {
                ToggleAudio(false);
            }
        }
        else
            ToggleAudio(false);
    }

    public void CloseNow()
    {
        Vector3 v = GateObject.localEulerAngles;
        v.y = 0;
        GateObject.localEulerAngles = v;
    }

    void PlayEndSounds()
    {
        foreach(var v in ClosedSounds)
        {
            v.Play();
        }
    }

    void ToggleAudio(bool on)
    {
        if(on != audioOn)
        {
            audioOn = on;
            foreach(var v in ClosingSounds)
            {
                if (on)
                    v.Play();
                else
                    v.Pause();
            }
        }
    }

    float calculatespeed()
    {
        if (!PhotonNetwork.inRoom)
            return closeSpeed;
        else
            return ((float)TeleCheck.playerCount() / (float)PhotonNetwork.playerList.Length)*closeSpeed;
    }

    public void Reset()
    {
        closed = false;
        closing = false;
        GateObject.localRotation = startRotation;
    }

    public bool isClosed()
    {
        return closed;
    }

    public void DoDestroy(bool immediate=false)
    {
        if(GatePieces != null && !immediate)
        {
            GameObject pieces;
            if(PiecesHolder == null)
                pieces = (GameObject)Instantiate(GatePieces, GateObject);
            else
                pieces = (GameObject)Instantiate(GatePieces, PiecesHolder);
            pieces.transform.localScale = Vector3.one;
            pieces.transform.localPosition = Vector3.zero;
            pieces.transform.localEulerAngles = Vector3.zero;
            pieces.transform.SetParent(null);
        }
        gameObject.SetActive(false);
    }
}
