using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

public class StreamManager : MonoBehaviour {

    public static StreamManager instance;
    public GameObject StreamTemplate;
    public List<EnemyStream> AllStreams;
    public AnimationCurve SpawnCDCurve;
    public AnimationCurve HeavyDuration;
    public AnimationCurve LightDuration;

    #region Life Cyle

    void Awake()
    {
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
#if !NOANALYTICS
        RemoteSettings.Updated += new RemoteSettings.UpdatedEventHandler(HandleRemoteSettings);
#endif
    }

    void OnEnable()
    {
        instance = this;
    }

    void HandleRemoteSettings()
    {
        //Disabled for Testing
        //CreatureManager.MAX_ENEMIES = RemoteSettings.GetInt("MAX_ENEMIES_ALLOWED", CreatureManager.MAX_ENEMIES);
    }

    [BitStrap.Button]
    public void SetInstance()
    {
        instance = this;
    }

    public void AddStream(EnemyStream st)
    {
        if (AllStreams == null)
            AllStreams = new List<EnemyStream>();
        AllStreams.Add(st);
        ClearBads();
        AllStreams.Sort((x, y) => Vector3.Distance(transform.position, x.transform.position).CompareTo(Vector3.Distance(transform.position, y.transform.position)));
    }

    public void RemoveStream(EnemyStream st)
    {
        AllStreams.Remove(st);
        ClearBads();
        if(AllStreams.Count > 0)
            AllStreams.Sort((x, y) => Vector3.Distance(transform.position, x.transform.position).CompareTo(Vector3.Distance(transform.position, y.transform.position)));
    }

    void ClearBads()
    {
        for(int i=AllStreams.Count-1; i>=0; i--)
        {
            if (AllStreams[i] == null)
                AllStreams.RemoveAt(i);
        }
    }

#endregion

#region Generation

    public static EnemyStream CreateStream(GameObject targetWall, Gate restoreGate)
    {
        if(instance != null && instance.StreamTemplate != null)
        {
            GameObject newStream = Instantiate(instance.StreamTemplate, instance.transform);
            newStream.transform.position = Vector3.forward * (1+instance.AllStreams.Count);
            newStream.SetActive(true);
            instance.ClearBadStreams();
            newStream.name = "DynamicStreamer" + (instance.AllStreams.Count-1);
            EnemyStream st = newStream.GetComponent<EnemyStream>();
            st.SetTarget(targetWall);
            st.RestoreGate = restoreGate;
            if(!instance.AllStreams.Contains(st))
                instance.AllStreams.Add(st);
            return st;
        }
        return null;
    }

    void ClearBadStreams()
    {
        if(AllStreams != null)
        {
            for (int i = AllStreams.Count - 1; i >= 0; i--)
            {
                if (AllStreams[i] == null)
                    AllStreams.RemoveAt(i);
            }
        }
    }

#endregion

#region Network Commands

    public static void SpawnBoss(EnemyStream st)
    {
        if (instance != null)
        {
            int id = instance.AllStreams.IndexOf(st);
            if (id >= 0)
            {
                instance.GetComponent<PhotonView>().RPC("SpawnBossNetwork", PhotonTargets.Others, id);
            }
        }
    }

    [PunRPC]
    void SpawnBossNetwork(int id)
    {
        if (id < AllStreams.Count && GameBase.instance.ShouldSpawnBoss())
            AllStreams[id].SpawnBoss();
    }

    public static void SetUsingDifficulty(EnemyStream st, bool val)
    {
        if(instance != null)
        {
            int id = instance.AllStreams.IndexOf(st);
            if (id >= 0)
            {
                instance.GetComponent<PhotonView>().RPC("SetDiffNetwork", PhotonTargets.MasterClient, id, val);
            }
        }
    }

    [PunRPC]
    void SetDiffNetwork(int id, bool val)
    {
        if (id < AllStreams.Count)
            AllStreams[id].SetUseDifficulty(val);
    }

    public static void SetEnemyVal(EnemyStream st, PhotonTargets targ, int viewID, int creatureID, int pathID, bool boss, bool spawning)
    {
        if(instance != null)
        {
            int id = instance.AllStreams.IndexOf(st);
            if (id >= 0)
            {
                instance.GetComponent<PhotonView>().RPC("SetNetwork", targ, id, viewID, creatureID, pathID, boss, spawning);
            }
        }
    }

    public static void SetEnemyVal(EnemyStream st, PhotonPlayer targ, int viewID, int creatureID, int pathID, bool boss, bool spawning)
    {
        if (instance != null)
        {
            int id = instance.AllStreams.IndexOf(st);
            if (id >= 0)
            {
                instance.GetComponent<PhotonView>().RPC("SetNetwork", targ, id, viewID, creatureID, pathID, boss, spawning);
            }
        }
    }

    public static void SpawnNewEnemy(EnemyStream st, int enemyID, int pid, int forceElite, PunTeams.Team team=PunTeams.Team.none)
    {
        if (instance != null)
        {
            int id = instance.AllStreams.IndexOf(st);
            if (id >= 0)
            {
                if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                    instance.GetComponent<PhotonView>().RPC("SpawnEnemy", PhotonTargets.MasterClient, id, enemyID, pid, forceElite, (int)team);
            }
        }
    }

    [PunRPC]
    void SpawnEnemy(int streamID, int enemyID, int pid, int forceElite, int team)
    {
        if (streamID < AllStreams.Count)
        {
            Creature c = AllStreams[streamID].SpawnNew(CreatureType.Any, pid, enemyID, forceElite);
            PunTeams.Team t = (PunTeams.Team)team;
            if(c != null && t != PunTeams.Team.none)
            {
                c.IgnoreArrows(t);
            }
        }

    }

    [PunRPC]
    void SetNetwork(int streamID, int viewID, int creatureID, int pathID, bool boss, bool spawning)
    {
        if(streamID < AllStreams.Count)
        {
            var pv = PhotonView.Find(viewID);
            if (pv != null)
            {
                Creature c = pv.gameObject.GetComponent<Creature>();
                if (c != null)
                {
                    c.SetState(EnemyState.walking);
                    AllStreams[streamID].EnemiesOut.Add(c.gameObject);
                    if (spawning)
                    {
                        CreatureManager.AddCreature(c);
                        if (!boss)
                            c.SetCreature(EnemyDB.v.Enemies[creatureID], creatureID);
                        else
                            c.SetCreature(EnemyDB.v.Bosses[creatureID], creatureID);
                    }
                    if (pathID < AllStreams[streamID].EPaths.Count)
                    {
                        PathManager path = AllStreams[streamID].EPaths[pathID].path;
                        if (c is FlyingCreature)
                        {
                            ((FlyingCreature)c).SetNewTarget(AllStreams[streamID].TargetWall);
                            ((FlyingCreature)c).GetComponent<splineMove>().moveToPath = true;
                            ((FlyingCreature)c).SetNewPath(path);
                        }
                        else if (c is GroundCreature)
                        {
                            ((GroundCreature)c).SetNewTarget(AllStreams[streamID].TargetWall);
                            ((GroundCreature)c).GetComponent<navMove>().moveToPath = true;
                            ((GroundCreature)c).SetNewPath(path);
                            ((GroundCreature)c).agent.Warp(path.GetPathPoints()[0]);
                        }
                    }          
                }
            }
        }
    }

#endregion

#region Network Lifecycle

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(GetStreamerData());
        }
        else
        {
            string val = (string)stream.ReceiveNext();
            SetStreamerData(val);
        }
    }

    string GetStreamerData()
    {
        string s = "";
        for(int i=0; i<AllStreams.Count; i++)
        {
            EnemyStream st = AllStreams[i];
            s += st.streaming ? "1" : "0";
            s += st.usingDifficulty ? "1" : "0";
            s += st.inHeavy ? "1" : "0";
            s += (i < AllStreams.Count-1) ? "," : "";
        }
        return s;
    }

    void SetStreamerData(string s)
    {
        string[] dats = s.Split(',');
        for(int i=0; i<Mathf.Min(dats.Length, AllStreams.Count); i++)
        {
            if(dats[i].Length > 2)
            {
                EnemyStream st = AllStreams[i];
                bool streaming = dats[i][0] == '1';
                bool usingDiff = dats[i][1] == '1';
                bool inHeavy = dats[i][2] == '1';

                if (streaming != st.streaming)
                {
                    if (streaming)
                        st.BeginStream();
                    else
                        st.EndStream();
                }
                st.usingDifficulty = usingDiff;
                if (inHeavy != st.inHeavy)
                    st.ChangeType();
            }
        }
    }

#endregion

}
