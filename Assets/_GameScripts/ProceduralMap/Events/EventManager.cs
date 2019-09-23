using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public static EventManager instance;
    public static List<EventTile> Tiles;
    public static bool InEvent;

    public RewardPillar Pillar;

    void Awake()
    {
        instance = this;
        if (Tiles == null)
            Tiles = new List<EventTile>();
    }

    public static void SetInEvent(bool val)
    {
        InEvent = val;
        if(PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            var iv = new ExitGames.Client.Photon.Hashtable();
            iv.Add("InEvent", val);
            PhotonNetwork.room.SetCustomProperties(iv);
        }
    }

    public static void Reset()
    {
        SetInEvent(false);
    }

    public void EnterTile(EventTile tile)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            EnterTileNetwork(id);
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("EnterTileNetwork", PhotonTargets.Others, id);
        }
    }

    [PunRPC]
    void EnterTileNetwork(int id)
    {
        if (id < Tiles.Count)
            Tiles[id].AllEnter();
        else
            Debug.Log("Event Tile [" + id + "] not found - Could not enter!");
    }

    public void StartIntro(EventTile tile)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("StartIntroNetwork", PhotonTargets.Others, id);
        }
    }
    [PunRPC]
    void StartIntroNetwork(int id)
    {
        if (id < Tiles.Count)
            Tiles[id].StartIntro();
        else
            Debug.Log("Event Tile [" + id + "] not found - Could not start Event Intro!");
    }

    public void SyncEventSeed(EventTile tile, int seed, int num)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                GetComponent<PhotonView>().RPC("NetworkSyncEventSeed", PhotonTargets.AllViaServer, id, seed, num);
            else if(!PhotonNetwork.inRoom)
                NetworkSyncEventSeed(id, seed, num);
        }
        else
            Debug.Log("Event Tile [" + id + "] not found - Could not sync Event Seed!");
    }
    [PunRPC]
    void NetworkSyncEventSeed(int id, int seed, int num)
    {
        if (id < Tiles.Count)
            Tiles[id].SyncSeed(seed, num);
        else
            Debug.Log("Error: Event ID out of range, could not sync seed");
    }

    public void SetEventSeed(EventTile tile, int seed)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if(PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("SetEventSeedNetwork", PhotonTargets.Others, id, seed);
            SetEventSeedNetwork(id, seed);
        }
        else
            Debug.Log("Event Tile [" + id + "] not found - Could not set Event Seed!");
    }
    [PunRPC]
    void SetEventSeedNetwork(int id, int seed)
    {
        if (id < Tiles.Count)
            Tiles[id].SetSeed(seed);
        else
            Debug.Log("Error: Event ID out of range, could not set seed");
    }

    #region Int Events

    public void IntEvent1(EventTile tile, int value)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetIntEvent1", PhotonTargets.AllViaServer, id, value);
            else
                NetIntEvent1(id, value);
        }
    }
    [PunRPC]
    void NetIntEvent1(int id, int value)
    {
        if (id < Tiles.Count)
            Tiles[id].IntEvent1Response(value);
    }

    public void IntEvent2(EventTile tile, int value)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetIntEvent2", PhotonTargets.Others, id, value);
            NetIntEvent2(id, value);
        }
    }
    [PunRPC]
    void NetIntEvent2(int id, int value)
    {
        if (id < Tiles.Count)
            Tiles[id].IntEvent2Response(value);
    }

    public void IVVEvent1(EventTile tile, int iv, Vector3 v1, Vector3 v2)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetIntEvent3", PhotonTargets.Others, id, iv, v1, v2);
            NetIVVEvent1(id, iv, v1, v2);
        }
    }
    [PunRPC]
    void NetIVVEvent1(int id, int iv, Vector3 v1, Vector3 v2)
    {
        if (id < Tiles.Count)
            Tiles[id].IVVEventResponse(iv, v1, v2);
    }

    #endregion

    #region Vector3 Events

    public void Vec3Event1(EventTile tile, Vector3 value)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetVec3Event1", PhotonTargets.Others, id, value);
            NetVec3Event1(id, value);
        }
    }
    [PunRPC]
    void NetVec3Event1(int id, Vector3 value)
    {
        if (id < Tiles.Count)
            Tiles[id].Vec3Event1Response(value);
    }

    public void Vec3Event2(EventTile tile, Vector3 value)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetVec3Event2", PhotonTargets.Others, id, value);
            NetVec3Event2(id, value);
        }
    }
    [PunRPC]
    void NetVec3Event2(int id, Vector3 value)
    {
        if (id < Tiles.Count)
            Tiles[id].Vec3Event2Response(value);
    }

    public void Vec3Event3(EventTile tile, Vector3 value)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetVec3Event3", PhotonTargets.Others, id, value);
            NetVec3Event3(id, value);
        }
    }
    [PunRPC]
    void NetVec3Event3(int id, Vector3 value)
    {
        if (id < Tiles.Count)
            Tiles[id].Vec3Event3Response(value);
    }

    #endregion

    #region Special Events
    public void QVEvent1(EventTile tile, Vector3 value, Quaternion rot)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetQVEvent1", PhotonTargets.Others, id, value, rot);
            NetQVEvent1(id, value, rot);
        }
    }
    [PunRPC]
    void NetQVEvent1(int id, Vector3 value, Quaternion rot)
    {
        if (id < Tiles.Count)
            Tiles[id].QVEvent1Response(value, rot);
    }

    public void SkipRequest(EventTile tile)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetSkipRequest", PhotonTargets.Others, id);
            NetSkipRequest(id);
        }
    }
    [PunRPC]
    void NetSkipRequest(int id)
    {
        if(id < Tiles.Count)
            Tiles[id].SkipRequest();
    }

    public void SkipEvent(EventTile tile)
    {
        int id = Tiles.IndexOf(tile);
        if (id >= 0)
        {
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetSkipEvent", PhotonTargets.Others, id);
            NetSkipEvent(id);
        }
    }
    [PunRPC]
    void NetSkipEvent(int id)
    {
        if (id < Tiles.Count)
            Tiles[id].SkipEvent();
    }
    #endregion
}
