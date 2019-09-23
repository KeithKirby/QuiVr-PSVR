using UnityEngine;
using System.Collections;
using SWS;
using System.Collections.Generic;

public class EnemyStream : MonoBehaviour
{
    public bool StreamOnStart;
    public int[] EnemyIDs;
    public GameObject TargetWall;
    public Gate RestoreGate;
    public float EasySpawnCD = 10;
    public float SpawnCD = 5;
    public bool difficultyOnStart;

    public List<GameObject> EnemiesOut;
    public bool streaming;
    float scd;
    public bool usingDifficulty;
    Health targHealth;
    public bool DistanceBased;
    [HideInInspector]
    public bool Ornamental;

    float typeCD;
    public bool inHeavy; 

    public List<CreaturePath> EPaths;

    [Range(1,4)]
    public int FakeMPNum;

    public float curCD;

    #region Base
    void Awake()
    {
        EnemiesOut = new List<GameObject>();
        usingDifficulty = difficultyOnStart;
        SetupDistancePaths();
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        if (StreamManager.instance != null)
            StreamManager.instance.AddStream(this);
    }

    void Start()
    {
        targHealth = TargetWall.GetComponent<Health>();
    }

    public static bool completed;
    float lastSpawn = 0;
    void Update()
    {
        if (Ornamental)
            return;
        if(streaming && !completed && GameBase.instance != null)
        {
            if (GameBase.instance.CurrentStream != this && streaming)
            {
                return;
            }
            if(!CreatureManager.bossOut)
                TickType();
            if (GameBase.instance.ShouldSpawnBoss() && !CreatureManager.bossOut)
            {
                /* TESTING NO BOSS
                SpawnBoss();
                if (PhotonNetwork.inRoom)
                    StreamManager.SpawnBoss(this);
                */
            }
            bool enemsout = EnemiesAlive();
            if (scd <= 0 || (!enemsout && lastSpawn >= 0.75f))
            {
                scd = spawncd();
                SpawnNew();
            }
            lastSpawn += Time.deltaTime;
            scd -= Time.deltaTime;
            for (int i = EnemiesOut.Count - 1; i >= 0; i--)
            {
                if (EnemiesOut[i] == null)
                    EnemiesOut.RemoveAt(i);
                else if (EnemiesOut[i].GetComponent<Health>().isDead())
                    EnemiesOut.RemoveAt(i);
            }
            if(usingDifficulty && GameBase.instance != null)
            {
                if(TileManager.instance == null || !TileManager.instance.Infinite)
                    GameBase.instance.TickDifficulty();
            }
        }
    }

    void TickType()
    {
        if(usingDifficulty)
        {
            typeCD -= Time.deltaTime;
            if(typeCD <= 0)
            {
                ChangeType(); 
            }
        }
    }

    public void ChangeType()
    {
        if (completed)
            return;
        inHeavy = !inHeavy;
        if (inHeavy && usingDifficulty)
        {
            SpawnStart();
            GameplayAudio.PlayHardWave();
            typeCD = StreamManager.instance.HeavyDuration.Evaluate(CreatureManager.EnemyDifficulty);
            //DynamicMusic.instance.Forte();
        }
        else
        {
            typeCD = StreamManager.instance.LightDuration.Evaluate(CreatureManager.EnemyDifficulty);
            //DynamicMusic.instance.Medium();
        }
    }

    void SpawnStart()
    {
        if(!Ornamental)
            SpawnNew();
    }

    public void ClearAll()
    {
        for (int i = EnemiesOut.Count - 1; i >= 0; i--)
        {
            if (EnemiesOut[i] != null)
                Destroy(EnemiesOut[i]);
        }
        EnemiesOut = new List<GameObject>();
    }

    public void ResetValues()
    {
        EndStream();
        completed = false;
        usingDifficulty = difficultyOnStart;
    }

    [AdvancedInspector.Inspect]
    public void BeginStream()
    {
        inHeavy = false;
        ChangeType();
        typeCD = StreamManager.instance.HeavyDuration.Evaluate(CreatureManager.EnemyDifficulty);
        streaming = true;
        if (GameBase.instance != null)
        {
            GameBase.instance.CurrentStream = this;
            GameBase.instance.CurrentTarget = TargetWall.GetComponent<Health>();
        }    
    }

    [AdvancedInspector.Inspect]
    public void EndStream()
    {
        streaming = false;
    }

    public void KillAll()
    {
        foreach (var v in EnemiesOut)
        {
            if (v != null && !v.GetComponent<Health>().isDead())
            {
                v.GetComponent<Health>().Kill();
            }
        }
    }
    #endregion

    #region Setup

    public void SetTarget(GameObject t)
    {
        TargetWall = t;
        targHealth = TargetWall.GetComponent<Health>();
    }

    public void AddPath(PathManager path, PathType type = PathType.Ground, CreatureSize size = CreatureSize.Medium, bool boss = false, bool MPOnly = false, bool AlwaysOn = false, float diffThr = 1, bool SpawnClose = false)
    {
        CreaturePath p = new CreaturePath();
        p.name = path.gameObject.name;
        p.type = type;
        p.size = size;
        p.isBoss = boss;
        p.path = path;
        p.DifficultyThreshold = diffThr;
        p.MPOnly = MPOnly;
        p.SpawnOnClose = SpawnClose;
        p.AlwaysOn = AlwaysOn;
        EPaths.Add(p);
        SetupDistancePaths();
    }

    #endregion

    #region Spawning

    public void SpawnClose()
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return;
        if (!streaming || Ornamental)
            return;
        for(int i=0; i<EPaths.Count; i++)
        {
            var v = EPaths[i];
            if(v.SpawnOnClose)
            {
                if (v.type == PathType.Flying && CreatureManager.EnemyDifficulty >= 250)
                    SpawnNew(CreatureType.Flying, i);
                else
                {
                    SpawnNew(CreatureType.Walking, i);
                }

            }
        }
    }

    public int ClampVal { get { return clamp; } }
    int clamp = -1;
    public Creature SpawnNew(CreatureType cType = CreatureType.Any, int pid = -1, int eid = -1, int forceElite=-1)
    {
        if (Ornamental)
            return null;
        int enemyNum = CreatureManager.EnemyNum();
        int MAX = CreatureManager.MAX_ENEMIES;
        int plrs = GetRealPlayerNum();
        if (PhotonNetwork.inRoom)
            MAX += (GetRealPlayerNum() * 2);
        if (clamp > 0 && enemyNum > clamp)
            return null;
        if (enemyNum >= MAX)
        {
            clamp = MAX - (MAX-5);
            if (PhotonNetwork.inRoom)
                clamp -= plrs*2;
            return null;
        }
        clamp = -1;
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return null;
        int id = EnemyIDs[Random.Range(0, EnemyIDs.Length)];
        int enemyID = id;
        lastSpawn = 0;
        if (usingDifficulty && GameBase.instance != null)
            enemyID = EnemyDB.EnemyFromDifficulty(CreatureManager.EnemyDifficulty, cType);
        if (eid >= 0 && eid < EnemyDB.v.Enemies.Length)
            enemyID = eid;
        Enemy prefab = EnemyDB.v.Enemies[enemyID];
        int pathID = pid;
        if(pathID < 0)
            pathID = GetPath(prefab);
        PathManager path = EPaths[pathID].path;
        GameObject newEnemy = null;
        bool elite = GameBase.RollElite();
        if (forceElite >= 0)
            elite = forceElite > 0;
        //Single Player
        if (!PhotonNetwork.inRoom)
        {
            Vector3 spawnPos = path.GetPathPoints()[0];
            if (elite && prefab.elitePrefab != null)
                newEnemy = (GameObject)Instantiate(prefab.elitePrefab, spawnPos, Quaternion.identity);
            else
                newEnemy = (GameObject)Instantiate(prefab.prefab, spawnPos, Quaternion.identity);
            if (usingDifficulty)
            {
                GameBase.instance.EnemiesSpawned++;
                EnemyDB.SetValues(newEnemy, CreatureManager.EnemyDifficulty);
            }
            Creature c = newEnemy.GetComponent<Creature>();
            if(c != null)
            {
                c.SetState(EnemyState.walking);
                c.SetCreature(prefab, enemyID);
                CreatureManager.AddCreature(c);
                if (c is FlyingCreature)
                {
                    ((FlyingCreature)c).GetComponent<splineMove>().moveToPath = true;
                    ((FlyingCreature)c).SetNewPath(path);
                    ((FlyingCreature)c).SetNewTarget(TargetWall);
                }
                else if (c is GroundCreature)
                {
                    ((GroundCreature)c).GetComponent<navMove>().moveToPath = true;
                    ((GroundCreature)c).SetNewPath(path);
                    ((GroundCreature)c).SetNewTarget(TargetWall);
                    ((GroundCreature)c).agent.Warp(spawnPos);
                }
            }
            EnemiesOut.Add(newEnemy);
            return c;
        }
        // Multiplayer
        else if(PhotonNetwork.isMasterClient)
        {
            if(elite && prefab.elitePrefab != null)
                newEnemy = PhotonNetwork.InstantiateSceneObject("NewEnemies/Elites/" + prefab.elitePrefab.name, path.GetPathPoints()[0], Quaternion.identity, 0, null);
            else
                newEnemy = PhotonNetwork.InstantiateSceneObject("NewEnemies/" + prefab.prefab.name, path.GetPathPoints()[0], Quaternion.identity, 0, null);
            if (usingDifficulty)
            {
                EnemyDB.SetValues(newEnemy, CreatureManager.EnemyDifficulty);
                GameBase.instance.EnemiesSpawned++;
            }   
            //GetComponent<PhotonView>().RPC("SetNetwork", PhotonTargets.All, newEnemy.GetComponent<PhotonView>().viewID, enemyID, pathID, false, true);
            StreamManager.SetEnemyVal(this, PhotonTargets.All, newEnemy.GetComponent<PhotonView>().viewID, enemyID, pathID, false, true);
            return newEnemy.GetComponent<Creature>();
        }
        return null;
    }

    public void SpawnEnemy(int enemyID, int pid=-1, int forceElite=0, PunTeams.Team team = PunTeams.Team.none)
    {
        StreamManager.SpawnNewEnemy(this, enemyID, pid, forceElite, team);
    }

    [AdvancedInspector.Inspect]
    public void SpawnBoss()
    {
        if (CreatureManager.bossOut || Ornamental)
            return;
        CreatureManager.bossOut = true;
        inHeavy = false;
        GameBase.instance.numBosses++;
        Debug.Log("Boss " + GameBase.instance.numBosses + " Spawned");
        GameplayAudio.PlayHorn();
        //DynamicMusic.instance.UseBattle();
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            return;
        int bossID = Random.Range(0, EnemyDB.v.Bosses.Length);
        Enemy prefab = EnemyDB.v.Bosses[bossID];
        int pathID = GetPath(prefab);
        PathManager path = EPaths[pathID].path;
        GameObject newEnemy = null;
        //Single Player
        if (!PhotonNetwork.inRoom)
        {
            Vector3 spawnPos = path.GetPathPoints()[0];
            newEnemy = (GameObject)Instantiate(prefab.prefab, spawnPos, Quaternion.identity);
            if (usingDifficulty)
                EnemyDB.SetValues(newEnemy, CreatureManager.EnemyDifficulty);
            Creature c = newEnemy.GetComponent<Creature>();
            if (c != null)
            {
                c.SetState(EnemyState.walking);
                c.SetCreature(prefab, bossID);
                CreatureManager.AddCreature(c);
                if (c is FlyingCreature)
                {
                    ((FlyingCreature)c).SetNewTarget(TargetWall);
                    ((FlyingCreature)c).GetComponent<splineMove>().moveToPath = true;
                    ((FlyingCreature)c).SetNewPath(path);
                }
                else if (c is GroundCreature)
                {
                    ((GroundCreature)c).SetNewTarget(TargetWall);
                    ((GroundCreature)c).GetComponent<navMove>().moveToPath = true;
                    ((GroundCreature)c).SetNewPath(path);
                }
            }
            else
                Debug.Log("Enemy had no Creature component attached");
            EnemiesOut.Add(newEnemy);
        }
        //Multiplayer
        else if (PhotonNetwork.isMasterClient)
        {
            newEnemy = PhotonNetwork.InstantiateSceneObject("NewEnemies/" + prefab.prefab.name, path.GetPathPoints()[0], Quaternion.identity, 0, null);
            if (usingDifficulty)
                EnemyDB.SetValues(newEnemy, CreatureManager.EnemyDifficulty);
            //GetComponent<PhotonView>().RPC("SetNetwork", PhotonTargets.All, newEnemy.GetComponent<PhotonView>().viewID, bossID, pathID, true, true);
            StreamManager.SetEnemyVal(this, PhotonTargets.All, newEnemy.GetComponent<PhotonView>().viewID, bossID, pathID, true, true);
        }
        
    }

    float spawncd()
    {
        if (!usingDifficulty)
            return EasySpawnCD;
        float cd = StreamManager.instance.SpawnCDCurve.Evaluate(CreatureManager.EnemyDifficulty);//SpawnCD + Random.Range(-0.75f, 0.75f);
        int m = Mathf.Max(FakeMPNum, 1);
        float[] diffMults = { 1f, .65f, .4f, .25f };
        if (PhotonNetwork.inRoom)
            m = Mathf.Max(m, GetRealPlayerNum());
        float mult = diffMults[m-1];
        //float difDiv = 1 + SpawnCDCurve.Evaluate(CreatureManager.EnemyDifficulty);//Mathf.Clamp(1+(CreatureManager.EnemyDifficulty / 80f), 1f, 20f); 
        curCD = Mathf.Clamp(cd * mult, 1f, 10f);//Mathf.Clamp((cd / difDiv) * mult, 1f, 10f);
        if (!inHeavy)
        {
            if (PhotonNetwork.inRoom && GetRealPlayerNum() > 1)
                curCD *= 5f;
            else
                curCD *= 3.5f;
        }    
        return curCD/EnemyDB.v.EnemyDensity;
    }

    #endregion

    #region Difficulty
    public bool usesDifficulty()
    {
        return usingDifficulty;
    }

    public void SetUseDifficulty(bool val)
    {
        usingDifficulty = val;
    }
    #endregion

    #region Networking
    [PunRPC]
    public void ChangeUseDifficulty(bool val)
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            usingDifficulty = val;
        else if (PhotonNetwork.inRoom)
            StreamManager.SetUsingDifficulty(this, val);//GetComponent<PhotonView>().RPC("ChangeUseDifficulty", PhotonTargets.MasterClient, val);   
    }

    void OnJoinedRoom()
    {
        KillAll();
    }

    /*
    [PunRPC]
    void SetNetwork(int viewID, int creatureID, int pathID, bool boss, bool spawning)
    {
        var pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            Creature c = pv.gameObject.GetComponent<Creature>();
            if(c != null)
            {
                c.SetState(EnemyState.walking);
                EnemiesOut.Add(c.gameObject);
                if (spawning)
                {
                    GameBase.instance.TryAddCreature(c);
                    if (!boss)
                        c.SetCreature(EnemyDB.v.Enemies[creatureID], creatureID);
                    else
                        c.SetCreature(EnemyDB.v.Bosses[creatureID], creatureID);
                }
                if(c is FlyingCreature)
                {
                    ((FlyingCreature)c).SetNewTarget(TargetWall);
                    ((FlyingCreature)c).GetComponent<splineMove>().moveToPath = true;
                    ((FlyingCreature)c).SetNewPath(EPaths[pathID].path);
                }
                else if(c is GroundCreature)
                {
                    ((GroundCreature)c).SetNewTarget(TargetWall);
                    ((GroundCreature)c).GetComponent<navMove>().moveToPath = true;
                    ((GroundCreature)c).SetNewPath(EPaths[pathID].path);
                }
            }
        }
    }
    */

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(streaming);
            stream.SendNext(usingDifficulty);
            stream.SendNext(inHeavy);
        }
        else
        {
            bool sval = (bool)stream.ReceiveNext();
            if(streaming != sval)
            {
                if (sval)
                    BeginStream();
                else
                    EndStream();
            }
            usingDifficulty = (bool)stream.ReceiveNext();
            bool h = (bool)stream.ReceiveNext();
            if (h != inHeavy)
                ChangeType();
        }
    }
    */
    
    // !!!! -- Need to handle setup of map before sending enemy values (seed in RoomInfo?)
    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if(PhotonNetwork.isMasterClient)
        {
            //PhotonView p = GetComponent<PhotonView>();
            foreach (var v in EnemiesOut)
            {
                if(v != null && !v.GetComponent<Creature>().isDead())
                {
                    int viewID = v.GetComponent<PhotonView>().viewID;
                    int eid = 0;
                    Creature c = v.GetComponent<Creature>();
                    if (c != null)
                        eid = c.GetEnemyID();
                    int path = 0;
                    if (v.GetComponent<navMove>() != null && v.GetComponent<navMove>().pathContainer != null)
                        path = GetPathID(v.GetComponent<navMove>().pathContainer.gameObject);
                    else if (v.GetComponent<splineMove>() != null && v.GetComponent<splineMove>().pathContainer != null)
                        path = GetPathID(v.GetComponent<splineMove>().pathContainer.gameObject);
                    //p.RPC("SetNetwork", plr, viewID, eid, path, c.type.status != CreatureStatus.Standard, true);
                    StreamManager.SetEnemyVal(this, plr, viewID, eid, path, c.type.status != CreatureStatus.Standard, true);
                }
            }
        }
    }
    #endregion

    #region Utility

    bool EnemiesAlive()
    {
        if(GameBase.instance != null)
        {
            foreach(var v in CreatureManager.AllEnemies())
            {
                if (v != null && !v.isDead())
                    return true;
            }
            return false;
        }
        return true;
    }

    public static int GetRealPlayerNum()
    {
        if (PhotonNetwork.inRoom)
            return PhotonNetwork.playerList.Length - SpectatorSync.NumSpectators();
        return 1;
    }

    float DistMinusY(Vector3 p1, Vector3 p2)
    {
        Vector3 v1 = new Vector3(p1.x, 0, p1.z);
        Vector3 v2 = new Vector3(p2.x, 0, p2.z);
        return Vector3.Distance(v1, v2);
    }

    float minDist()
    {
        float minDist = float.MaxValue;
        foreach (var v in EnemiesOut)
        {
            if(v != null && TargetWall != null)
            {
                float cd = DistMinusY(v.transform.position, TargetWall.transform.position);
                if (cd < minDist)
                    minDist = cd;
            }
        }
        //float d = Mathf.Clamp(1 + (CreatureManager.EnemyDifficulty / 25f), 1f, 2f);
        return minDist;//d;
    }

    int GetPathID(GameObject path)
    {
        for(int i=0; i<EPaths.Count; i++)
        {
            if (EPaths[i].path.gameObject == path)
                return i;
        }
        return 0;
    }

    public void SwitchStream(EnemyStream newStream)
    {
        foreach(var v in EnemiesOut)
        {
            if(v != null)
                newStream.AddEnemy(v);
        }
        newStream.usingDifficulty = usingDifficulty;
        EnemiesOut = new List<GameObject>();
    }

    int GetDistancePath(PathType t, CreatureSize s, CreatureStatus stat)
    {
        bool boss = stat != CreatureStatus.Standard;
        List<int> ValidIDs = new List<int>();
        for (int i = 0; i < EPaths.Count; i++)
        {
            var v = EPaths[i];
            if (DistMinusY(v.path.transform.position, TargetWall.transform.position) > minDist() || v.AlwaysOn || boss)
            {
                if(v.DifficultyThreshold <= CreatureManager.EnemyDifficulty || v.AlwaysOn || boss)
                {
                    if(v.type == t && ((int)v.size >= (int)s || boss) && v.isBoss == boss)
                    {
                        if (!v.MPOnly || (PhotonNetwork.inRoom && GetRealPlayerNum() > 1))
                            ValidIDs.Add(i);
                    }
                }
            }
        }
        if (ValidIDs.Count < 1)
        {
            for (int i = EPaths.Count - 1; i >= 0; i--)
            {
                var v = EPaths[i];
                if (v.DifficultyThreshold <= CreatureManager.EnemyDifficulty || v.AlwaysOn)
                {
                    if (v.type == t && (int)v.size >= (int)s && v.isBoss == boss)
                    {
                        return i;
                    }
                }
            }
        }
        if(ValidIDs.Count > 0)
            return ValidIDs[Random.Range(0, ValidIDs.Count)];
        return 0;
    }

    void SetupDistancePaths()
    {
        EPaths.Sort((x, y) => DistMinusY(x.path.transform.position, TargetWall.transform.position).CompareTo(DistMinusY(y.path.transform.position, TargetWall.transform.position)));
    }

    public int GetPath(Enemy e)
    {
        PathType t = PathType.Ground;
        if (e.type == CreatureType.Flying)
            t = PathType.Flying;
        if(DistanceBased)
        {
            return GetDistancePath(t, e.size, e.status);
        }
        else
        {
            List<int> ValidIDs = new List<int>();
            for (int i = EPaths.Count - 1; i >= 0; i--)
            {
                var v = EPaths[i];
                if (GameBase.instance == null || v.DifficultyThreshold <= CreatureManager.EnemyDifficulty || v.AlwaysOn)
                {
                    if (v.type == t && (int)v.size >= (int)e.size && v.isBoss == (e.status != CreatureStatus.Standard))
                    {
                        if(!v.MPOnly || (PhotonNetwork.inRoom && GetRealPlayerNum() > 1))
                            ValidIDs.Add(i);
                    }
                }
            }
            if(ValidIDs.Count > 0)
                return ValidIDs[Random.Range(0, ValidIDs.Count)];
        }
        return 0;
    }

    public void AddEnemy(GameObject enemy)
    {
        Creature c = enemy.GetComponent<Creature>();
        int cPath = ClosestSwapPath(enemy.transform, c.type);
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            int viewID = enemy.GetComponent<PhotonView>().viewID;
            int eid = c.GetEnemyID();
            //GetComponent<PhotonView>().RPC("SetNetwork", PhotonTargets.All, viewID, eid, cPath, c.type.status != CreatureStatus.Standard, false);
            StreamManager.SetEnemyVal(this, PhotonTargets.All, viewID, eid, cPath, c.type.status != CreatureStatus.Standard, false);
        }
        else if(!PhotonNetwork.inRoom)
        {
            GroundCreature gc = enemy.GetComponent<GroundCreature>();
            FlyingCreature fc = enemy.GetComponent<FlyingCreature>();
            if(gc != null)
            {
                gc.GetComponent<navMove>().moveToPath = true;
                gc.SetNewPath(EPaths[cPath].path);
                gc.SetNewTarget(TargetWall);
                gc.SetState(EnemyState.walking);
            }
            else if(fc != null)
            {
                fc.GetComponent<splineMove>().moveToPath = true;
                fc.SetNewPath(EPaths[cPath].path);
                fc.SetNewTarget(TargetWall);
                fc.SetState(EnemyState.walking);
            }
            EnemiesOut.Add(enemy);
        }

    }

    public void AddEnemyClose(int enemyID)
    {
        Enemy enemy = EnemyDB.v.Enemies[enemyID];
        var pathIndex = GetPath(enemy);
        var path = EPaths[pathIndex].path;
        Vector3 spawnPos = path.GetPathPoints()[0];

        var newEnemy = (GameObject)Instantiate(enemy.prefab, spawnPos, Quaternion.identity);

        var anim = newEnemy.GetComponent<Animation>();
        if(null != anim)
            for(int i = 0; i < anim.GetClipCount(); ++i) {}

        var pathId = EPaths.PickRandomIndex();
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            int viewID = enemy.prefab.GetComponent<PhotonView>().viewID;
            int eid = enemyID;
            //GetComponent<PhotonView>().RPC("SetNetwork", PhotonTargets.All, viewID, eid, cPath, c.type.status != CreatureStatus.Standard, false);
            StreamManager.SetEnemyVal(this, PhotonTargets.All, viewID, eid, pathId, enemy.status != CreatureStatus.Standard, false);
        }
        else if (!PhotonNetwork.inRoom)
        {
            GroundCreature gc = newEnemy.GetComponent<GroundCreature>();
            FlyingCreature fc = newEnemy.GetComponent<FlyingCreature>();
            if (gc != null)
            {
                gc.GetComponent<navMove>().moveToPath = true;
                gc.SetNewPath(EPaths[pathId].path);
                gc.SetNewTarget(TargetWall);
                gc.SetState(EnemyState.walking);
            }
            else if (fc != null)
            {
                fc.GetComponent<splineMove>().moveToPath = true;
                fc.SetNewPath(EPaths[pathId].path);
                fc.SetNewTarget(TargetWall);
                fc.SetState(EnemyState.walking);
            }
            EnemiesOut.Add(newEnemy);
        }
    }

    int ClosestSwapPath(Transform o, Enemy type)
    {
        float dist = float.MaxValue;
        int id = 0;
        bool boss = type.status != CreatureStatus.Standard;
        for(int i=0; i<EPaths.Count; i++)
        {
            var v = EPaths[i];
            if(v.isBoss == boss)
            {
                if ((v.type == PathType.GroundSwap && type.type == CreatureType.Walking) ||
                    (v.type == PathType.FlyingSwap && type.type == CreatureType.Flying))
                {
                    float d = Vector3.Distance(o.position, v.path.transform.position);
                    if (d < dist)
                    {
                        id = i;
                        dist = d;
                    }
                }
            }
        }
        return id;
    }
    #endregion

    void OnDestroy()
    {
        if (StreamManager.instance != null)
            StreamManager.instance.RemoveStream(this);
    }
}

[System.Serializable]
public class CreaturePath
{
    public string name;
    public PathManager path;
    public bool isBoss;
    public CreatureSize size = CreatureSize.Medium;
    public PathType type = PathType.Ground;
    public bool AlwaysOn;
    public float DifficultyThreshold;
    public float AlwaysOnThreshold = 750f;
    public bool MPOnly;
    public bool SpawnOnClose;

    public override string ToString()
    {
        return name;
    }
}

public enum PathType
{
    Ground,
    GroundSwap,
    Flying,
    FlyingSwap
}
