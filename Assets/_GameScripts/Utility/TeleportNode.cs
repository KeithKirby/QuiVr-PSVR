using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TeleportNode : MonoBehaviour {
   
    TeleportPlayer tPlayer;
    public bool startNode;
    public PlayerPositions pPos;

	public UnityEvent onTeleport;

    public void Start()
    {
		GetComponent<ArrowImpact> ().DestroyArrow = true;
        gameObject.layer = 13; //myArrows Layer
        if(pPos != null)
            pPos = GetComponent<PlayerPositions>();
    }

    public void Init(TeleportPlayer t)
    {
        tPlayer = t;
    }

    public void Use(ArrowCollision e)
    {
        if(e.isMine && e.deflectCount < 2)
        {
			if (tPlayer == null)
				tPlayer = FindObjectOfType<TeleportPlayer>();
            if(tPlayer != null)
            {
                tPlayer.Teleport(this);
                onTeleport.Invoke();
            }
        }
    }

    public void ClearPositions()
    {
        foreach(var v in pPos.positions)
        {
            v.inUse = false;
            v.userID = -1;
        }
    }

    public void ForceUse()
    {
        tPlayer.Teleport(this);
    }

    public Transform GetLocation()
    {
        if (PhotonNetwork.inRoom && pPos != null)
        {
            return pPos.UseNextAvailable(PhotonNetwork.player.ID);
        }
        if(pPos != null)
            return pPos.positions[0].pos;
        return transform;
    }

    public Transform GetLocation(int id)
    {
        pPos.positions[id].userID = PhotonNetwork.player.ID;
        pPos.positions[id].inUse = true;
        return pPos.positions[id].pos;
    }

    public void Show()
    {
        GetComponent<Collider>().enabled = true;
        var v = GetComponentsInChildren<SpriteRenderer>();
        foreach(var s in v)
        {
            s.enabled = true;
        }
    }

    public void Hide()
    {
        GetComponent<Collider>().enabled = false;
        var v = GetComponentsInChildren<SpriteRenderer>();
        foreach (var s in v)
        {
            s.enabled = false;
        }
    }

    public bool isFull()
    {
        return pPos.isFull();
    }

    public PlayerPositions GetPositions()
    {
        return pPos;
    }

    public void ReleaseSpot(int playerID)
    {
        if(pPos != null)
        {
            pPos.ReleaseSpot(playerID);
        }
    }

    public void RequestSpotID()
    {
        Debug.Log("Requesting Spot ID");
        GetComponent<PhotonView>().RPC("NetworkRequestSpot", PhotonTargets.Others, PhotonNetwork.player.ID);
    }

    [PunRPC]
    void NetworkRequestSpot(int pid)
    {
        if(PhotonNetwork.isMasterClient)
        {
            int spotID = pPos.UseNextAvailableID(pid);
            GetComponent<PhotonView>().RPC("RespondSpotRequest", PhotonTargets.Others, pid, spotID);
        }
    }

    [PunRPC]
    void RespondSpotRequest(int playerID, int spotID)
    {
        if(PhotonNetwork.player.ID == playerID)
        {
            Debug.Log("Spot ID Request Respons: " + spotID);
            tPlayer.RecieveResponse(spotID);
        }
    }

}
