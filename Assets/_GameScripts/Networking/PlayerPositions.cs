using UnityEngine;
using System.Collections;

public class PlayerPositions : MonoBehaviour {
   
    public PlayerPos[] positions;
    PhotonView v;

    TeleportNode node;

    void Awake()
    {
        v = GetComponent<PhotonView>();
        node = GetComponent<TeleportNode>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

	public Transform UseNextAvailable(int userID)
    {
        for(int i=0; i<positions.Length; i++)
        {
            if(!positions[i].inUse)
            {
                positions[i].inUse = true;
                positions[i].userID = userID;
                UseNetwork(i, userID);
                return positions[i].pos;
            }
        }
        positions[0].inUse = true;
        positions[0].userID = userID;
        UseNetwork(0, userID);
        return positions[0].pos;
    }

    public int UseNextAvailableID(int userID)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (!positions[i].inUse)
            {
                positions[i].inUse = true;
                positions[i].userID = userID;
                UseNetwork(i, userID);
                return i;
            }
        }
        return -1;
    }

    void UseNetwork(int pid, int userID)
    {
        if(PhotonNetwork.inRoom)
            v.RPC("UseID", PhotonTargets.OthersBuffered, pid, userID);
    }

    [PunRPC]
    void UseID(int pid, int uid)
    {
        for(int i=0; i<positions.Length; i++)
        {
            if(positions[i].inUse && positions[i].userID == uid)
            {
                positions[i].inUse = false;
                positions[i].userID = -1;
            }
        }
        positions[pid].inUse = true;
        positions[pid].userID = uid;
        if(node != null)
            CheckFull(true);
    }

    public bool isFull()
    {
        foreach (var v in positions)
        {
            if (!v.inUse)
                return false;
        }
        return true;
    }

    void CheckFull(bool toHide)
    {
        if(toHide && isFull())
        {
            node.Hide();
        }
        else
        {
            foreach (var v in positions)
            {
                if (v.userID == PhotonNetwork.player.ID)
                    return;
            }
            if (!isFull() && node != null)
                node.Show();
        }
        
    }

    void ReleaseNetwork(int pid)
    {
        if (PhotonNetwork.inRoom && v!=null)
            v.RPC("ReleaseID", PhotonTargets.OthersBuffered, pid);
    }

    public void ResetAll()
    {
        foreach(var v in positions)
        {
            v.userID = -1;
            v.inUse = false;
        }
    }

    [PunRPC]
    void ReleaseID(int pid)
    {
        positions[pid].inUse = false;
        CheckFull(false);
    }

    public void ReleaseSpot(int playerID)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].userID == playerID)
            {
                positions[i].inUse = false;
                ReleaseNetwork(i);
            }
        }
    }


    void OnPhotonPlayerDisconnected(PhotonPlayer p)
    {
        if(PhotonNetwork.isMasterClient)
        {
            for(int i=0; i<positions.Length; i++)
            {
                if(positions[i].userID == p.ID)
                {
                    positions[i].inUse = false;
                    ReleaseNetwork(i);
                }
            }
        }
    }

}

[System.Serializable]
public class PlayerPos
{
    public bool inUse;
    public int userID;
    public Transform pos;
}
