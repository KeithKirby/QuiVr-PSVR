using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHead : MonoBehaviour {

    public PlayerLife Life;
    public bool Dummy;
    public static PlayerHead instance;
    public static List<PlrHead> AllHeads;
    PlayerSync psync;
    PlrHead h;

    void Awake()
    {
        if (Life == null)
            Life = GetComponentInParent<PlayerLife>();
        h = new PlrHead(this, -1);
        if (PhotonNetwork.inRoom)
            h = new PlrHead(this, PhotonNetwork.player.ID);
        if (AllHeads == null)
            AllHeads = new List<PlrHead>();
        if (!Dummy)
            instance = this;
        else
        {
            psync = GetComponentInParent<PlayerSync>();
            h.plrID = psync.GetComponent<PhotonView>().ownerId;       
        }
        AllHeads.Add(h);   
    }

    void OnEnable()
    {
        if (!Dummy)
            instance = this;
        if (AllHeads == null)
            AllHeads = new List<PlrHead>();
        if (!AllHeads.Contains(h))
            AllHeads.Add(h);
    }

    // Heatmapper
    void Start()
    {
        //InvokeRepeating("SendPositionHeatmap", 5f, 1f);
    }

    void SendPositionHeatmap()
    {
        if(SceneManagerHelper.ActiveSceneBuildIndex == 1)
        {
            string seed = "NA";
            if (TileManager.instance != null)
                seed = TileManager.instance.CurrentSeed;
            if (pvpmanager.instance.PlayingPVP)
                seed = "PVP";
            seed += "_"+AppBase.v.NetworkVersion;
            UnityAnalyticsHeatmap.HeatmapEvent.Send("PlrPos_" + seed, transform.position);
        }
    }

    void OnDisable()
    {
        if(AllHeads != null)
            AllHeads.Remove(h);
    }

    void OnDestroy()
    {
        if (AllHeads != null)
            AllHeads.Remove(h);
    }

    public bool isDead()
    {
        if (Dummy)
            return psync.isDead();
        else
            return Life.isDead;
    }

    public static Transform GetRandomHead()
    {      
        if (AllHeads.Count > 0)
            return AllHeads[Random.Range(0, AllHeads.Count)].head.transform;
        return null;
    }

    public static int GetRandomHeadID()
    {
        if (AllHeads.Count > 0)
            return AllHeads[Random.Range(0, AllHeads.Count)].plrID;
        return -5;
    }

    public static int GetRandomHeadID(Vector3 pos, float MaxDistance)
    {
        List<PlrHead> CloseHeads = new List<PlrHead>();
        foreach (var v in AllHeads)
        {
            if (Vector3.Distance(pos, v.head.transform.position) <= MaxDistance)
                CloseHeads.Add(v);
        }
        if (CloseHeads.Count > 0)
            return AllHeads[Random.Range(0, CloseHeads.Count)].plrID;
        return -5;
    }

    public static Transform GetHead(int playerID)
    {
        if(PhotonNetwork.inRoom && AllHeads != null)
        {
            foreach(var v in AllHeads)
            {
                if (v.plrID == playerID)
                    return v.head.transform;
            }
        }
        else if(!PhotonNetwork.inRoom && instance != null)
        {
            return instance.transform;
        }
        return null;
    }

    public static float ClosestPlayerDistance(Vector3 pos)
    {
        float x = float.MaxValue;
        foreach(var v in AllHeads)
        {
            float d = Vector3.Distance(pos, v.head.transform.position);
            if (d < x)
                x = d;
        }
        return x;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!Dummy && Life != null)
        {
            if(col.gameObject.layer == 8) //Enemy
            {
                Creature c = col.GetComponentInParent<Creature>();
                if (c != null && !c.isDead())
                    Life.Die();
            }
        }
    }

    public class PlrHead
    {
        public PlayerHead head;
        public int plrID;

        public PlrHead(PlayerHead h, int id)
        {
            head = h;
            plrID = id;
        }
    }
}
