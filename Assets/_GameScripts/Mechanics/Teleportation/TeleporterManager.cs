using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class TeleporterManager : MonoBehaviour {

    public delegate void OnPlayerMoved();
    static public event OnPlayerMoved PlayerMovedEvt;

    public GameArea[] Areas;
    public int currentArea;
    public Teleporter HomeBase;
    public List<DynamicTeleporter> Dynamics;
    public static TeleporterManager instance;
    int curID = 0;

    public Teleporter DeathTP;
    [HideInInspector]
    public Teleporter RespawnTP;
    [HideInInspector]
    public Transform DeathTPTransform;

    public List<Teleporter> AllTeleporters;
    public List<Teleporter> AlwaysDisabled;
    [Header("Teleporter Powerup")]
    public int MAX_POWERED;
    public GameObject PlayerVortex;
    public GameObject[] Powerup;
    List<Teleporter> BoostedTPs;

    public delegate void DynamicTPCreated(Teleporter tp);
    public static DynamicTPCreated OnDynamicTPCreated;

    void Awake()
    {
        Dynamics = new List<DynamicTeleporter>();
        BoostedTPs = new List<Teleporter>();
        for(int i=1; i<Areas.Length; i++)
        {
            DisableArea(i, 0);
        }
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        if (DeathTP != null)
            DeathTPTransform = DeathTP.transform.parent;
    }

    public void UpdateDeathTP(Transform t, Vector3 offset)
    {
        if (DeathTPTransform != null)
            DeathTPTransform.GetComponent<DeathTP>().UpdateLocation(t, offset);
        RespawnTP = null;
    }

    public void UpdateDeathTP(Transform t, Teleporter respawnTP)
    {
        if (DeathTPTransform != null)
            DeathTPTransform.GetComponent<DeathTP>().UpdateLocation(t, Vector3.zero);
        RespawnTP = respawnTP;
    }

    public void SetRespawnTP(Teleporter tp)
    {
        RespawnTP = tp;
    }

    void OnEnable()
    {
        instance = this;
        if(TelePlayer.instance != null && HomeBase != null)
        {
            HomeBase.ForceUse();
        }
    }

    void Start()
    {
        foreach(var v in Areas)
        {
            v.PopulateTeleporters();
        }
        InvokeRepeating("CheckTPArea", 1f, 3.21f);
    }

    public void AddTP(Teleporter tp)
    {
        if (AllTeleporters == null)
            AllTeleporters = new List<Teleporter>();
        AllTeleporters.Add(tp);
        //Sort Teleporters deterministically based on distance from Origin
        AllTeleporters.Sort((x, y) => Vector3.Distance(transform.position, x.transform.position).CompareTo(Vector3.Distance(transform.position, y.transform.position)));
    }

    void CheckTPArea()
    {
        if(gameObject.activeInHierarchy && currentArea < Areas.Length && TelePlayer.instance != null)
        {
            if (!PlayerInArea(currentArea))
                EnsureArea();
        }
    }
    
    public void PlayerMoved()
    {
        UpdateBoostDisplays();
        if(null != PlayerMovedEvt)
            PlayerMovedEvt();
    }

    #region TP Areas

    public void DisableArea(int id, int ignoreOverlap=-1)
    {
        if (id < Areas.Length)
        {
            foreach (var v in Areas[id].GetTeleporters())
            {
                if (ignoreOverlap == -1 || !AreaContains(ignoreOverlap, v))
                    v.DisableTeleport();
            }
            try {
                foreach (var v in Dynamics)
                {
                    if (v.GameArea == id && v.teleporter != null)
                        v.teleporter.DisableTeleport();
                }
            }
            catch { }
        }
        foreach (var v in AlwaysDisabled)
        {
            v.DisableTeleport();
        }
    }

    public void EnableArea(int id)
    {
        if(id < Areas.Length)
        {
            foreach (var v in Areas[id].GetTeleporters())
                v.EnableTeleport();
        }
        try
        {
            foreach (var v in Dynamics)
            {
                if (v.GameArea == id && v.teleporter != null)
                    v.teleporter.EnableTeleport();
            }
            foreach (var v in AlwaysDisabled)
            {
                v.DisableTeleport();
            }
        }
        catch { }
        if (id > 3)
            StartArea.DisableStart();
        else
            StartArea.EnableStart();
        currentArea = id;
        SetPowerupRandom();
    }

    bool AreaContains(int areaID, Teleporter t)
    {
        return Areas[areaID].Contains(t);
    }

    public void Reset()
    {
        ClearDynamics();
        for (int i = 1; i < Areas.Length; i++)
        {
            DisableArea(i, 0);
        }
        EnableArea(0);
    }

    public void TeleportPlayerBack(int areaID)
    {
        Teleporter t = Areas[areaID].BackTeleporter;
        if (t != null)
            t.ForceUse();
    }

    public bool PlayerInArea(int areaID)
    {
        if(areaID < Areas.Length)
        {
            foreach (var v in Areas[areaID].GetTeleporters())
            {
                if (TelePlayer.instance.currentNode == v)
                    return true;
            }
        }
        return false;
    }

    public void AddArea(GameArea area, int index = -1)
    {
        List<GameArea> areas = new List<GameArea>();
        bool addedArea = false;
        for(int i=0; i<Areas.Length; i++)
        {
            if (index == i)
            {
                addedArea = true;
                areas.Add(area);
            }
            areas.Add(Areas[i]);
        }
        if (!addedArea)
            areas.Add(area);
        Areas = areas.ToArray();
    }

    public void EnsureArea()
    {
        if(currentArea >= 0 && currentArea < Areas.Length)
        {
            for(int i=0; i<Areas.Length; i++)
            {
                if(i != currentArea)
                {
                    Teleporter[] tps = Areas[i].GetTeleporters();
                    foreach (var v in tps)
                    {
                        if (!v.Disabled && !Areas[currentArea].Contains(v))
                            v.DisableTeleport();
                    }
                }
            }
            foreach (var v in Areas[currentArea].GetTeleporters())
            {
                if (v.Disabled)
                    v.EnableTeleport();
            }
        }
    }
    #endregion

    #region Teleporter Powerup

    public void ClearBoosted()
    {
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            ClearPowerups();
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("NetworkClearPowerups", PhotonTargets.Others);
        }
    }

    void ClearPowerups()
    {
        BoostedTPs = new List<Teleporter>();
        foreach(var v in Powerup)
        {
            v.SetActive(false);
        }
    }

    int[] boostedIDs = { };
    [AdvancedInspector.Inspect]
    public void SetPowerupRandom()
    {
        if (MAX_POWERED <= 0)
            return;
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if(GameBase.instance == null || GameBase.instance.Difficulty > 0)
            {
                boostedIDs = NewBoosted();
                if (PhotonNetwork.inRoom)
                    GetComponent<PhotonView>().RPC("NetworkSetPowerupTeleporters", PhotonTargets.Others, boostedIDs);
                SetBoosted(boostedIDs);
                CancelInvoke("SetPowerupRandom");
                Invoke("SetPowerupRandom", 30f);
            }
        }
    }

    public void UpdateBoostDisplays()
    {
        return;
        int x = 0;
        if(PlayerVortex != null)
            PlayerVortex.SetActive(false);
        for(int i=0; i<BoostedTPs.Count; i++)
        {
            if(BoostedTPs[i] != null && Powerup[i] != null)
            {
                Vector3 pos = BoostedTPs[i].transform.parent.position;
                ParticleSystem pt = Powerup[i].GetComponent<ParticleSystem>();
                bool playerPowered = TelePlayer.instance != null && BoostedTPs[i] == TelePlayer.instance.currentNode;
                Powerup[i].SetActive(!playerPowered);
                if (!playerPowered)
                {
                    if (Vector3.Distance(Powerup[i].transform.position, pos) > 1f)
                    {
                        pt.Clear();
                        pt.Play();
                        Powerup[i].transform.position = pos;
                    }
                    else if (!pt.isPlaying)
                        pt.Play();
                }
                else
                {
                    SetPlayerPowered(pos);
                }
                x = i + 1;
            }        
        }
        for(int i=x; i<Powerup.Length; i++)
        {
            if(Powerup[i] != null)
                Powerup[i].SetActive(false);
        }
    }

    void SetPlayerPowered(Vector3 pos)
    {
        PlayerVortex.SetActive(true);
        PlayerVortex.transform.position = pos;
    }

    void SetBoosted(int[] ids)
    {
        boostedIDs = ids;
        BoostedTPs = new List<Teleporter>();
        for(int i=0; i<ids.Length; i++)
        {
            if(AllTeleporters.Count > ids[i])
                BoostedTPs.Add(AllTeleporters[ids[i]]);
        }
        UpdateBoostDisplays();
    }

    int[] NewBoosted()
    {
        int numBoost = Mathf.Min(MAX_POWERED, Areas[currentArea].teleporters.Length);
        List<int> tpIDs = new List<int>();
        int x = 0;
        int i = 0;
        Teleporter[] tps = Areas[currentArea].GetTeleporters();
        while(x < numBoost && i < 50)
        {
            int id = Random.Range(0, tps.Length);
            Teleporter tp = tps[id];
            id = GetTPID(tp);
            if(!tpIDs.Contains(id) && !tp.Unboostable)
            {
                tpIDs.Add(id);
                x++;
            }
            i++;
        }
        return tpIDs.ToArray();
    }

    public bool CheckPowered(Teleporter tp)
    {
        if(BoostedTPs != null)
        {
            return BoostedTPs.Contains(tp);
        }
        return false;
    }

    #endregion

    #region Dynamic TPs

    public int CreateTP(Vector3 pos, bool localPoints, Vector3[] points, Quaternion[] rotations, bool useArea)
    {
        RemoveBadDynamics();
        GameObject newTeleporter;
        int id = GetNextID();
        int area = -1;
        if (useArea)
            area = currentArea;
        if (!PhotonNetwork.inRoom)
        {
            newTeleporter = (GameObject)Instantiate(Resources.Load("Base/DynamicTeleporter") as GameObject, pos, Quaternion.identity);
            AddDTP(newTeleporter, id);
            SetupDTP(id, localPoints, area, points, rotations);
        }
        else
        {
            newTeleporter = PhotonNetwork.Instantiate("Base/DynamicTeleporter", pos, Quaternion.identity, 0);
            int pid = newTeleporter.GetComponent<PhotonView>().viewID;
            int netID = (PhotonNetwork.player.ID+1)*10000;
            GetComponent<PhotonView>().RPC("SetupDTP", PhotonTargets.AllViaServer, id, localPoints, area, points, rotations, pid, netID);
            return id + netID;
        }
        return id;
    }

    [PunRPC]
    void SetupDTP(int id, bool localpts, int area, Vector3[] Positions, Quaternion[] Rotations, int photonid = 0, int netID = 0)
    {
        RemoveBadDynamics();
        if (id > curID)
            curID = id;
        id = id+netID;
        if (PhotonNetwork.inRoom)
        {
            PhotonView view = PhotonView.Find(photonid);
            if (view != null)
                AddDTP(view.gameObject, id);
        }
        int did = GetDTP(id);
        if (did >= 0)
        {   
            if(Dynamics[did].dtp != null)
            {
                Dynamics[did].GameArea = area;
                Dynamics[did].dtp.Setup(localpts, Positions, Rotations);
                if (OnDynamicTPCreated != null)
                    OnDynamicTPCreated.Invoke(Dynamics[did].teleporter);
            }
        }
    }

    void AddDTP(GameObject obj, int id)
    {
        DynamicTeleporter ndt = new DynamicTeleporter();
        ndt.Object = obj;
        ndt.ID = id;
        ndt.teleporter = obj.GetComponent<Teleporter>();
        ndt.dtp = obj.GetComponent<DynamicTP>();
        Dynamics.Add(ndt);
    }

    public void DestroyTP(int id)
    {
        int did = GetDTP(id);
        if(did >= 0 && Dynamics[did].Object != null)
        {
            if(PhotonNetwork.inRoom)
            {
                PhotonView v = Dynamics[did].Object.GetComponent<PhotonView>();
                GetComponent<PhotonView>().RPC("DestroyTPNetwork", v.owner, v.viewID);
            }   
            else
            {
                Destroy(Dynamics[did].Object);
                Dynamics.RemoveAt(did);
            }  
        }
    }

    [PunRPC]
    void DestroyTPNetwork(int vid)
    {
        PhotonView view = PhotonView.Find(vid);
        if(view != null)
        {
            for(int i=0; i<Dynamics.Count; i++)
            {
                if(view.gameObject == Dynamics[i].Object)
                {
                    PhotonNetwork.Destroy(Dynamics[i].Object);
                }
            }
        }
        RemoveBadDynamics();
    }

    void RemoveBadDynamics()
    {
        try {
            for (int i = Dynamics.Count - 1; i >= 0; i--)
            {
                if (Dynamics[i].Object == null)
                    Dynamics.RemoveAt(i);
            }
        }
        catch { }
        
    }

    public int GetDTP(int id)
    {
        for (int i = 0; i < Dynamics.Count; i++)
        {
            if (Dynamics[i].ID == id)
                return i;
        }
        return -1;
    }

    int GetNextID()
    {
        curID++;
        return curID;
    }

    public void ClearDynamics()
    {

        try {
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            {
                foreach (var v in Dynamics)
                {
                    if (v.Object != null)
                    {
                        v.Object.GetComponent<PhotonView>().RequestOwnership();
                    }
                }
            }
            for (int i = 0; i < Dynamics.Count; i++)
            {
                if (Dynamics[i].Object != null)
                {
                    if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
                    {
                        PhotonNetwork.Destroy(Dynamics[i].Object);
                    }
                    else if (!PhotonNetwork.inRoom)
                    {
                        Destroy(Dynamics[i].Object);
                    }
                }
            }
            RemoveBadDynamics();
        }
        catch { }
    }
    #endregion

    #region Networking

    public void NetworkTP(Teleporter tp, int pid)
    {
        for(int i=0; i<AllTeleporters.Count; i++)
        {
            if(AllTeleporters[i] == tp)
            {
                GetComponent<PhotonView>().RPC("NetworkTeleport", PhotonTargets.AllViaServer, i, pid);
                return;
            }
        }
        Debug.Log("Can't find teleporter to send teleport message to host.");
    }

    public void NetworkRelease(Teleporter tp, int pid)
    {
        for (int i = 0; i < AllTeleporters.Count; i++)
        {
            if (AllTeleporters[i] == tp)
            {
                GetComponent<PhotonView>().RPC("ReleaseNetwork", PhotonTargets.AllViaServer, i, pid);
                return;
            }
        }
    }

    public void NetSetValues(Teleporter tp, PhotonPlayer p, string vals)
    {
        for (int i = 0; i < AllTeleporters.Count; i++)
        {
            if (AllTeleporters[i] == tp)
            {
                GetComponent<PhotonView>().RPC("SetValues", p, i, vals);
            }
        }
    }

    [PunRPC]
    void SetValues(int tpid, string values)
    {
        if (tpid < AllTeleporters.Count)
        {
            Teleporter tp = AllTeleporters[tpid];
            string[] vals = values.Split(':');
            for (int i = 0; i < Mathf.Min(vals.Length, tp.Positions.Length); i++)
            {
                string[] mvals = vals[i].Split(',');
                int inUse = 0, id = 0;
                if (mvals.Length == 2)
                {
                    int.TryParse(mvals[0], out inUse);
                    int.TryParse(mvals[1], out id);
                }
                tp.Positions[i].inUse = (inUse == 1);
                tp.Positions[i].userID = id;
            }
        }
    }

    [PunRPC]
    void ReleaseNetwork(int tpid, int pid)
    {
        if(tpid < AllTeleporters.Count)
        {
            Teleporter tp = AllTeleporters[tpid];
            bool inside = false;
            foreach (var v in tp.Positions)
            {
                if (v.userID == pid)
                {
                    v.inUse = false;
                    v.userID = -1;
                }
                else if (v.userID == PhotonNetwork.player.ID)
                    inside = true;
            }
            if (!inside && !tp.isFull())
            {
                tp.Show();
            }
        } 
    }

    [PunRPC]
    void NetworkTeleport(int tpid, int playerID)
    {
        if (tpid < AllTeleporters.Count)
        {
            Teleporter tp = AllTeleporters[tpid];
            if (!tp.isFull())
            {
                Transform t = tp.ReserveNextAvailable(playerID);
                if (playerID == PhotonNetwork.player.ID)
                {
                    if (t != null)
                        TelePlayer.instance.Teleport(tp, t);
                    else
                        TelePlayer.instance.s_tryingTeleport = false; // This was called for both paths, but Teleport will clear now
                }
            }
            else
            {
                if (playerID == PhotonNetwork.player.ID)
                    TelePlayer.instance.s_tryingTeleport = false;
                tp.Hide();
            }
        }
        else if(playerID == PhotonNetwork.player.ID)
        {
            Debug.Log("Tried to teleport to invalid Teleporter ID, canceling teleport");
            TelePlayer.instance.s_tryingTeleport = false;
        }

    }
  
    public void ChangeArea()
    {
        EnableArea(currentArea);
        for (int i = 0; i < Areas.Length; i++)
        {
            if (i != currentArea)
                DisableArea(i, currentArea);
        }
    }

    [PunRPC]
    void SetAreaNetwork(int cid)
    {
        currentArea = cid;
        EnableArea(cid);
        for (int i = 0; i < Areas.Length; i++)
        {
            if (i != cid)
            {
                DisableArea(i, cid);
            }
        }
    }

    [PunRPC]
    void NetworkSetPowerupTeleporters(int[] ids)
    {
        SetBoosted(ids);
    }

    [PunRPC]
    void NetworkClearPowerups()
    {
        ClearPowerups();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting && !EventManager.InEvent)
        {
            stream.SendNext(currentArea);
            stream.SendNext(GetDisables());
        }
        else if (stream.isReading && !EventManager.InEvent)
        {
            currentArea = (int)stream.ReceiveNext();
            string disableDat = (string)stream.ReceiveNext();
            SetDisables(disableDat);
        }
    }

    string GetDisables()
    {
        string s = "";
        for(int i=0; i<AllTeleporters.Count; i++)
        {
            if (AllTeleporters[i].Disabled)
                s += 0 + "";
            else
                s += 1 + "";
        }
        return s;
    }

    void SetDisables(string dat)
    {
        for(int i=0; i<Mathf.Min(dat.Length, AllTeleporters.Count); i++)
        {
            bool disabled = dat[i] == '0';
            if(AllTeleporters[i].Disabled != disabled)
            {
                if (disabled)
                    AllTeleporters[i].DisableTeleport();
                else
                    AllTeleporters[i].EnableTeleport();
            }
        }
    }

    void OnPhotonPlayerJoinedRoom(PhotonPlayer other)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonView view = GetComponent<PhotonView>();
            view.RPC("SetAreaNetwork", other, currentArea);
            view.RPC("NetworkSetPowerupTeleporters", other, boostedIDs);
        }
    }

    void OnLeaveRoom()
    {
        foreach (var v in Areas)
        {
            foreach (var t in v.GetTeleporters())
            {
                t.ClearPositions();
                t.Show();
            }
        }
        ClearDynamics();
    }

    #endregion

    #region Utility

    public int GetTPID(Teleporter tp)
    {
        for(int i=0; i<AllTeleporters.Count; i++)
        {
            if (AllTeleporters[i] == tp)
                return i;
        }
        return -1;
    }

    #endregion
}

[System.Serializable]
public class GameArea
{
    public string name;
    public Teleporter StartTeleporter;
    public Teleporter BackTeleporter;
    public GameObject[] TeleportNodes;
    public Teleporter[] teleporters;

    public void AddTeleporter(GameObject tp)
    {
        List<GameObject> tps = new List<GameObject>();
        if(TeleportNodes != null)
        {
            foreach (var v in TeleportNodes)
            {
                tps.Add(v);
            }
        }
        tps.Add(tp);
        TeleportNodes = tps.ToArray();
        PopulateTeleporters();
    }

    public Teleporter[] GetTeleporters()
    {
        if (teleporters == null)
            PopulateTeleporters();
        return teleporters;
    } 

    public void PopulateTeleporters()
    {
        teleporters = new Teleporter[TeleportNodes.Length];
        for(var i=0; i<TeleportNodes.Length; i++)
        {
            teleporters[i] = TeleportNodes[i].GetComponentInChildren<Teleporter>();
        }
    }

    public bool Contains(Teleporter tp)
    {
        if(teleporters != null)
        {
            foreach (var v in teleporters)
            {
                if (v == tp)
                    return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        return name;
    }
}

[System.Serializable]
public class DynamicTeleporter
{
    public int GameArea = -1;
    public int ID;
    public GameObject Object;
    public DynamicTP dtp;
    public Teleporter teleporter;
}
