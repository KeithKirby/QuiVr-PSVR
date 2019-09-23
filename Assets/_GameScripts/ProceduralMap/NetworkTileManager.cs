using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTileManager : MonoBehaviour {

    TileManager mgr;
    PhotonView view;

    public static NetworkTileManager instance;

	void Awake () {
        view = GetComponent<PhotonView>();
        mgr = GetComponent<TileManager>();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        instance = this;
    }

    public void CreateNew(string seed)
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            ExitGames.Client.Photon.Hashtable someCustomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "MapSeed", seed } };
            PhotonNetwork.room.SetCustomProperties(someCustomPropertiesToSet);
            view.RPC("NewRPC", PhotonTargets.Others, seed);
        }
    }

    [PunRPC]
    void NewRPC(string seed)
    {
        mgr.CreateNew(seed);
    }
	
	public void ClearTiles()
    {
        if(PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            view.RPC("ClearRPC", PhotonTargets.Others);
        }
    }

    [PunRPC]
    void ClearRPC()
    {
        mgr.ClearTiles();
    }


    void OnLeftRoom()
    {
        mgr.BuildWeeklySeed();
    }

    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if(PhotonNetwork.isMasterClient && mgr.Infinite)
        {
            Debug.Log("Sending Area number to new player: " + mgr.AddedAreas);
            view.RPC("GenerateAreas", plr, mgr.AddedAreas, mgr.GatesAdded.ToArray(), mgr.EndGates.ToArray());
        }
    }

    public int TestGenNum = 13;
    [AdvancedInspector.Inspect]
    public void TestGenerateAreas()
    {
        GenerateAreas(TestGenNum, mgr.GatesAdded.ToArray(), mgr.EndGates.ToArray());
    }

    [PunRPC]
    void GenerateAreas(int addedAreas, int[] GateIDs, int[] EndGateIDs)
    {
        if (mgr.Infinite && mgr.AddedAreas < addedAreas)
        {
            Debug.Log("Generating " + addedAreas + " from host - Last Random Number: " + mgr.GetLastRandom());
            foreach (var v in GateIDs)
            {
                mgr.AddGateID(v);
            }
            while (mgr.AddedAreas < addedAreas - 1)
            {
                mgr.AddSection(false);
            }
            mgr.AddSection();
            foreach(var v in EndGateIDs)
            {
                mgr.SetPreviousGateEnd(v);
            }
            Debug.Log("Finished Adding Section from Host: " + mgr.AddedAreas + " areas added.");
        }
    }

    public void AddSection()
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            view.RPC("NetworkAddSection", PhotonTargets.Others);
        }
    }

    [PunRPC]
    void NetworkAddSection()
    {
        mgr.AddSection();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(mgr.CurrentSeed); 
        }
        else
        {
            string curSeed = (string)stream.ReceiveNext();
            if (curSeed != mgr.CurrentSeed)
                mgr.CreateNew(curSeed);        
        }
    }
}
