using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestActions : MonoBehaviour {

    public Teleporter watchtowerTP;
    public Teleporter startTP;
    public static TestActions instance;
    public List<RoomDebug> OpenRooms;

    void Awake()
    {
        instance = this;
    }

    [AdvancedInspector.Inspect]
    public void MPToggle()
    {
        if (MPSphere.instance != null)
            MPSphere.instance.Click();
    }

    [AdvancedInspector.Inspect]
    public void PrivateRoom()
    {
        if (MPSphere.instance != null)
            MPSphere.instance.PrivateGame();
    }

    [AdvancedInspector.Inspect]
    public void UseStartTP()
    {
        if(startTP != null)
            startTP.ForceUse();
    }

    [AdvancedInspector.Inspect]
    public void CloseNextGate()
    {
        if(GateManager.instance != null)
        {
            for(int i=0; i<GateManager.instance.AllGates.Count; i++)
            {
                Gate g = GateManager.instance.AllGates[i];
                if (g != null && !g.closed)
                {
                    g.ForceClose();
                    return;
                }
            }
        }
    }

    [AdvancedInspector.Inspect]
    public void NewTileset()
    {
        TileManager mgr = FindObjectOfType<TileManager>();
        if (mgr != null)
            mgr.CreateNew();
    }

    [AdvancedInspector.Inspect]
    public void ClearTiles()
    {
        TileManager mgr = FindObjectOfType<TileManager>();
        if (mgr != null)
            mgr.ClearTiles();
    }

    [AdvancedInspector.Inspect]
    public void UpdateRooms()
    {
        if (PhotonNetwork.connected && !PhotonNetwork.inRoom)
        {
            OpenRooms = new List<RoomDebug>();
            Debug.Log("Currently " + OpenRooms.Count + " Rooms");
            foreach(var v in PhotonNetwork.GetRoomList())
            {
                RoomDebug r = new RoomDebug();
                r.Name = v.Name;
                r.Players = v.PlayerCount;
            }
        }
        else
            Debug.Log("Can't Get room list - Not connected or already in room");
    }

    [System.Serializable]
    public class RoomDebug
    {
        public string Name;
        public int Players;

        public override string ToString()
        {
            if(Name != null)
                return Name;
            return "No Name";
        }

        [AdvancedInspector.Inspect]
        public void JoinRoom()
        {
            if(!PhotonNetwork.inRoom && PhotonNetwork.connected)
            {
                string type = "";
                if (MPSphere.instance != null)
                    type = MPSphere.instance.MPType;
                JoinMultiplayer.JoinSpecific(Name, type);
            }
        }
    }

}
