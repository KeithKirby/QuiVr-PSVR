using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using System.Linq;
//using Parse;

public class TileManager : MonoBehaviour {

    //Editor Values
    public static TileManager instance;
    public TileValues TileDB;
    public SkyValues SkyDB;
    TilePrefab[] Tiles;
    public PathManager[] ConnectingPaths;
    public Teleporter[] StartZoneTPs;
    public Gate BaseGate;
    public EnemyStream BaseStream;
    public Teleporter BaseGateTP;
    public AnimationCurve PathDiffByDist;
    public int EventTileFreq;
    public MapTile FrontCap;
    public MapTile BackCap;

    //Runtime Values
    public int Gates;
    public bool Infinite;
    public int AddedAreas;
    public int CurHeight;
    public List<GameObject> CurrentTiles;
    public List<EnemyStream> CurrentStreams;
    public List<Gate> CurrentGates;
    public List<PathManager> CombinedPaths;
    public List<TileVisibility> VisibilityZones;
    bool LastZoneChangedEnv;

    public string CurrentSeed;
    public int CurrentBudget;
    int CurAngle;
    List<Vector2> CurTileValues;
    public EnvironmentType CurEnv;

    //Hidden
    NetworkTileManager net;
    RandomValue Rand;
    int curLength;
    Hashtable UsedTiles;
    MapTile CurrentTile;
    EnemyStream LastStream;
    TilePrefab PrevTile;

    EnvironmentType[] Environments = { EnvironmentType.Snow };

    #region Life Cycle

    void Awake()
    {
        instance = this;
        Rand = new RandomValue(Random.Range(0, 99999));
        UsedTiles = new Hashtable();
        net = GetComponent<NetworkTileManager>();
        GatesAdded = new List<int>();
        EndGates = new List<int>();
        foreach(var t in StartZoneTPs)
        {
            t.OnTeleport.AddListener(delegate { ShowVisible(null); });
        }
        #if !UNITY_EDITOR
        UnityEngine.AI.NavMeshSurface nav = GetComponent<UnityEngine.AI.NavMeshSurface>();
        if (nav != null)
            nav.BuildNavMesh();
        #endif
    }

    [BitStrap.Button]
    public void CreateRandom()
    {
        CreateNew(null);
    }

    [BitStrap.Button]
    void BuildCurrentSeed()
    {
        CreateNew(CurrentSeed);
    }

    [BitStrap.Button]
    public void BuildWeeklySeed()
    {
        // dw - Weekly seed disabled
        //string weeklySeed = "a";
        //if(AppBase.v.isBeta)
        //ParseConfig.CurrentConfig.TryGetValue<string>("BetaWeeklySeed", out weeklySeed);
        //else
        //ParseConfig.CurrentConfig.TryGetValue<string>("WeeklySeed", out weeklySeed);
        //CreateNew(weeklySeed);        
        CreateNew(null);
    }
    
    public void CreateNew(string seed=null)
    {
        ClearTiles();

        if (StreamManager.instance == null)
        {
            FindObjectOfType<StreamManager>().SetInstance();
            FindObjectOfType<StreamManager>().AllStreams = new List<EnemyStream>();
        }  
        if (seed == null)
            seed = Random.Range(0, 99999) + "";
        DebugTimed.Log("-- Random Map Generated -- :: " + seed);
        SetSeed(seed);
        Generate();
        if (net != null)
            net.CreateNew(seed);

    }

    public bool WeeklyMap()
    {
        // dw - Weekly seed disabled
        //string weeklySeed;
        //ParseConfig.CurrentConfig.TryGetValue<string>("WeeklySeed", out weeklySeed);
        //return CurrentSeed == weeklySeed;
        return false;
    }

    [BitStrap.Button]
    public void ClearTiles()
    {
        for (int i = CurrentTiles.Count - 1; i >= 0; i--)
        {
            if(CurrentTiles[i] != null)
                DestroyImmediate(CurrentTiles[i]);
        }
        CurrentTiles = new List<GameObject>();
        UsedTiles = new Hashtable();
        CurAngle = 0;
        AddedAreas = 0;
        LastStream = null;
        CurrentTile = null;
        curLength = 0;
        LastZoneChangedEnv = false;
        GatesAdded = new List<int>();
        EndGates = new List<int>();
        CurTileValues = new List<Vector2>();
        CurTileValues.Add(new Vector2(0,1));
        if (net != null)
            net.ClearTiles();
        ClearPaths();
        ClearStreams();
        ClearTPList();
        if(Application.isPlaying)
        {
            BaseGate.OnDeath.RemoveListener(GameBase.instance.ForceEndGame);
            BaseGate.OnDeath.RemoveListener(StartArea.EnableStart);
        }
        BaseGate.GetComponentInChildren<GateGems>().ResetColor();
        CurrentGates = new List<Gate>();
        CurEnv = EnvironmentType.Snow;
    }

    #endregion

    #region Map Generation

    public void Generate()
    {
        if (EnvironmentManager.instance != null)
        {
            EnvironmentManager.instance.SetupSkys(Rand);
            //EnvironmentManager.ChangeEnvImmediate(EnvironmentType.Snow);
        }
        AddedAreas = 0;
        LastStream = null;
        CurrentTile = null;
        CurAngle = 0;
        curLength = 0;
        //Object Generation
        //Area 1
        CreateArea(1);
        //Gate 2
        CreateTile(NextTile(TileType.Gate, 2));

        int envChange = 0;
        bool changedLast = false;
        for(int i=0; i<Gates-2; i++)
        {
            envChange = 0;
            if (!changedLast)
                envChange = (int)TryChangeEnv().Value;
            if (envChange > 0)
                changedLast = true;
            else
                changedLast = false;
            //Area 2+
            CreateArea(1 - envChange); // Was 2 - envChange
            //Gate 3+
            CreateTile(NextTile(TileType.Gate, 1));
        }
        envChange = 0;
        if (!Infinite)
        {
            envChange = 0;
            if (!changedLast)
                envChange = (int)TryChangeEnv().Value;
            if (envChange > 0)
                changedLast = true;
            else
                changedLast = false;
            //Area 3
            CreateArea(1 - envChange);
            //End Cap
            CreateTile(NextTile(TileType.End, 1));
        }

        //Path and Zone Generation
        CollectGates();
        CreateTPZones();
        CreateStreams();
        SetGateStreams();
        CreatePaths();
        SetupVisibility();
        CurrentStreams.Reverse();
        if (GateManager.instance != null)
            GateManager.instance.SortFinal();
        Invoke("UpdateNav", 0.05f);// UpdateNav();
        if (TeleporterManager.instance != null)
            TeleporterManager.instance.ChangeArea();
        if (Infinite)
            Invoke("SetupInfinite", 0.25f);
    }

    public MapTile[] CreateArea(float Budget)
    {
        float AreaBudget = Budget;
        List<MapTile> areaTiles = new List<MapTile>();
        while (AreaBudget > 0.05f)
        {
            TilePrefab t = NextTile(TileType.Link, AreaBudget);
            if (t != null)
            {
                AreaBudget -= t.Value;
                MapTile m = CreateTile(t);
                if (m != null)
                    areaTiles.Add(m);
                else
                    Debug.Log("Could Not Create MapTile: " + t.Name);
            }
            else
                AreaBudget = 0;
        }
        return areaTiles.ToArray();
    }

    public void BakeNavNow()
    {
#if UNITY_EDITOR
        /*
        UnityEngine.AI.NavMeshSurface nav = GetComponent<UnityEngine.AI.NavMeshSurface>();
        nav.RemoveData();
        nav.navMeshData = null;
        nav.BuildNavMesh();
        */
#endif
    }

    TilePrefab NextTile(TileType TileType, float budget)
    {
        TilePrefab t = null;
        List<TilePrefab> tls = GetTileOfType(TileType);
        StripTileAngles(ref tls);
        StripDuplicate(ref tls);
        StripTileValues(ref tls, budget);
        t = SelectTile(tls);
        PrevTile = t;
        return t;
    }

    MapTile CreateEventTile(Gate lastGate)
    {
        EncounterTile tile = TileDB.GetValidEncounters(Rand);
        if(tile != null)
        {
            GameObject t = Instantiate(tile.TileRequirement, transform);
            t.transform.localPosition = Vector3.zero;
            t.transform.localEulerAngles = Vector3.zero;
            if (CurrentTile != null)
            {
                t.transform.position = CurrentTile.LinkPoint.position;
                t.transform.rotation = CurrentTile.LinkPoint.rotation;
            }
            CurrentTile = t.GetComponent<MapTile>();
            CurrentTile.CurHeight = CurHeight;
            CurrentTile.Environment = CurEnv;
            CurrentTiles.Add(t);
            CurAngle += CurrentTile.Direction;
            CurTileValues.Add(new Vector2(CurrentTile.Direction, CurrentTile.Value));

            //Add Encounter Component
            GameObject e = Instantiate(tile.EventObject, t.transform);
            e.transform.localPosition = Vector3.zero;
            e.transform.localEulerAngles = Vector3.zero;
            if (lastGate != null)
            {
                e.GetComponent<EventTile>().TargetGate = lastGate;
                HealthbarExternal[] bars = t.GetComponentsInChildren<HealthbarExternal>();
                for (int i = 0; i < bars.Length; i++)
                {
                    bars[i].HP = lastGate.GetHP();
                }
                lastGate.OnClose.AddListener(delegate { e.GetComponent<EventTile>().EnableEntrance(); });
            }

            CurrentTile.Awake();
            CurrentTile.SetRocks();

            UsedTiles[tile.Name] = 1;
            return CurrentTile;
        }
        return null;
    }

    MapTile CreateTile(TilePrefab p)
    {
        if (p == null)
        {
            Debug.Log("TilePrefab Null");
            return null;
        }           
        //Debug.Log("Creating Tile: " + p.Name + "_" + p.env);
        GameObject t = Instantiate(p.Prefab, transform);
        t.transform.localPosition = Vector3.zero;
        t.transform.localEulerAngles = Vector3.zero;
        if(CurrentTile != null)
        {
            t.transform.position = CurrentTile.LinkPoint.position;
            t.transform.rotation = CurrentTile.LinkPoint.rotation;
        }
        CurrentTile = t.GetComponent<MapTile>();
        CurrentTiles.Add(t);
        CurrentTile.CurHeight = CurHeight;
        CurHeight += (int)p.HeightChange;
        CurAngle += p.Direction;
        CurTileValues.Add(new Vector2(p.Direction, p.Value));
        UsedTiles[p.Name] = 1;
        return CurrentTile;
    }

    void NextEnvironment()
    {
        if(Environments.Length > 1)
        {
            List<EnvironmentType> opts = new List<EnvironmentType>();
            foreach(var v in Environments)
            {
                if (v != CurEnv)
                    opts.Add(v);
            }
            EnvironmentType LastEnv = CurEnv;
            CurEnv = opts[GetRandomNext(0, opts.Count)];
            CreateTile(TileDB.GetTransition(LastEnv, CurEnv));
        }
    }

    MapTile TryChangeEnv()
    {
        var v = System.Enum.GetValues(typeof(TileEnvironment));
        int nextID = GetRandomNext(0, v.Length);
        EnvironmentType next = (EnvironmentType) v.GetValue(nextID);
        if(next != CurEnv)
        {
            TilePrefab t = TileDB.GetTransition(next, CurEnv);
            if (t != null)
            {
                Debug.Log("Changing environment from " + CurEnv + " to " + next);
                MapTile m = CreateTile(t);
                CurEnv = next;
                return m;
            }
        }
        MapTile x = new MapTile();
        x.Value = 0;
        return x;
    }

    #endregion

    #region Map Setup

    //Gate Setup
    public void CollectGates()
    {
        CurrentGates = new List<Gate>();
        for(int i=0; i<CurrentTiles.Count; i++)
        {
            if (CurrentTiles[i].GetComponent<MapTile>().type == TileType.Gate)
                CurrentGates.Add(CurrentTiles[i].GetComponentInChildren<Gate>());
        }
        for(int i=0; i<CurrentGates.Count; i++)
        {
            CurrentGates[i].gameObject.name = "ForwardGate" + (i + 2);
            CurrentGates[i].GateIndex = i + 2;
        }
    }

    void SetGateStreams()
    {
        EnemyStream nxt = CurrentStreams[0];
        int sidx = 1;
        for(int i= CurrentGates.Count-1; i>=0; i--)
        {
            Gate g = CurrentGates[i];
            g.CloseArea = i+1;
            g.NextStreamer = nxt;
            if (sidx < CurrentStreams.Count)
            {
                g.Streamer = CurrentStreams[sidx];
                nxt = CurrentStreams[sidx];
                sidx++;
                SetGateFunctions(g);
            }
        }
        BaseGate.NextStreamer = CurrentStreams[CurrentStreams.Count - 1];
        //BaseGate.OnClose.AddListener(CurrentStreams[CurrentStreams.Count - 1].SpawnClose);
    }

    void SetGateFunctions(Gate g)
    {
        //g.OnClose.AddListener(g.NextStreamer.SpawnClose);
        if (TeleporterManager.instance != null)
        {
            g.OnClose.AddListener(TeleporterManager.instance.ClearDynamics);
            g.OnDeath.AddListener(TeleporterManager.instance.ClearDynamics);
            if(Infinite)
                g.OnDeath.AddListener(delegate 
                    {
                        SetPreviousGateEnd(GateManager.instance.GateID(g.Streamer.TargetWall.GetComponent<Gate>()));
                        if(EnemyStream.GetRealPlayerNum() > 1)
                            GameOrb.instance.HalfProgress();
                    });
        }
    }
    
    //Path Setup
    public void CreatePaths()
    {
        ClearPaths();
        List<PathManager> tmp = new List<PathManager>();
        int streamidx = 0;
        for(int i=CurrentTiles.Count-1; i>= 0; i--)
        {
            MapTile tile = CurrentTiles[i].GetComponent<MapTile>();
            //Link Old Paths through Link Tiles
            if(tile.type == TileType.Link)
            {
                foreach (var v in tmp)
                {
                    AddToPath(v, tile.LinkPaths);
                }
            } //Break if Gate
            else if(tile.type == TileType.Gate)
            {
                //Add In Roads
                foreach(var v in tmp)
                {
                    AddToPath(v, tile.InPaths);
                    CombinedPaths.Add(v);
                }
                foreach (var v in tile.SpawnPaths)
                {
                    tmp.Add(CreatePath(tile, v));
                }
                StreamAddPaths(streamidx, tmp.ToArray());
                streamidx++;
                tmp = new List<PathManager>();
                foreach (var v in tile.OutPaths)
                {
                    tmp.Add(CreatePath(tile, v));
                }
            }
            //Create new paths from tile
            foreach(var v in tile.SpawnPaths)
            {
                tmp.Add(CreatePath(tile, v));
            }
            //Hide Path Building Blocks
            tile.HidePaths();
        }
        foreach (var v in tmp)
        {
            AddToPath(v, ConnectingPaths);
        }
        foreach(var v in ConnectingPaths)
        {v.gameObject.SetActive(false);}
        StreamAddPaths(streamidx, tmp.ToArray());
    }

    void AddToPath(PathManager path, PathManager[] options)
    {
        float minDist = float.MaxValue;
        PathManager closest = null;
        foreach(var v in options)
        {
            float dist = Vector3.Distance(path.waypoints[path.waypoints.Length - 1].position, v.waypoints[0].position);
            if(dist < minDist && PathMatches(path, v))
            {
                minDist = dist;
                closest = v;
            }
        }
        if(closest != null)
        {
            for (int i = 0; i < closest.waypoints.Length; i++)
            {
                path.AddPosition(closest.waypoints[i].position);
            }
        }
    }

    bool PathMatches(PathManager p1, PathManager p2)
    {
        DynPathInfo d1 = p1.GetComponent<DynPathInfo>();
        DynPathInfo d2 = p2.GetComponent<DynPathInfo>();
        if(d1 != null && d2 != null)
        {
            if (d1.type == d2.type)
                return true;
            else if (d1.type == PathType.FlyingSwap && d2.type == PathType.Flying || d2.type == PathType.FlyingSwap && d1.type == PathType.Flying)
                return true;
            else if (d1.type == PathType.GroundSwap && d2.type == PathType.Ground || d2.type == PathType.GroundSwap && d1.type == PathType.Ground)
                return true;
            else
                return false;
        }
        return true;
    }

    PathManager CreatePath(MapTile tile, PathManager temp)
    {
        GameObject o = new GameObject();
        o.name = temp.gameObject.name;
        o.transform.SetParent(tile.transform);
        o.transform.localPosition = Vector3.zero;
        o.transform.localEulerAngles = Vector3.zero;
        DynPathInfo pi = temp.GetComponent<DynPathInfo>();
        if (pi != null)
            o.AddComponent(pi);
        PathManager p = o.AddComponent<PathManager>();
        for(int i=0; i<temp.waypoints.Length; i++)
        {
            p.AddPosition(temp.waypoints[i].position);
        }
        return p;
    }

    void ClearPaths()
    {
        foreach (var v in ConnectingPaths)
        { v.gameObject.SetActive(true); }
        if (CombinedPaths != null)
        {
            for (int i = 0; i < CombinedPaths.Count; i++)
            {
                if (CombinedPaths[i] != null)
                    Destroy(CombinedPaths[i]);
            }
        }
        CombinedPaths = new List<PathManager>();
    }

    //Stream Setup
    public void CreateStreams()
    {
        if (StreamManager.instance == null)
            StreamManager.instance = FindObjectOfType<StreamManager>();
        ClearStreams();
        Gate prev = null;
        GameObject CurGate = null;
        for(int i=CurrentTiles.Count-1; i>=0; i--)
        {
            MapTile t = CurrentTiles[i].GetComponent<MapTile>();
            if(t.type == TileType.Gate)
            {
                Gate g = t.gameObject.GetComponentInChildren<Gate>();
                CurGate = g.gameObject;
                EnemyStream stream = StreamManager.CreateStream(CurGate, prev);
                if(stream != null)
                {
                    CurrentStreams.Add(stream);
                }
                prev = g;
            }
        }
        EnemyStream stLast = StreamManager.CreateStream(BaseGate.gameObject, prev);
        if (stLast != null)
            CurrentStreams.Add(stLast);
        /*
        float x = 1f;
        for(int i=CurrentStreams.Count-1; i >= 0; i--)
        {
            CurrentStreams[i].SpawnCD = 8 - (x*Mathf.Sqrt(i));
        }
        */
        LastStream = CurrentStreams[0];
    }

    void StreamAddPaths(int index, PathManager[] paths)
    {
        if (index >= 0 && index < CurrentStreams.Count)
        {
            EnemyStream s = CurrentStreams[index];
            paths = paths.OrderBy(c => Vector3.Distance(c.waypoints[0].position, s.TargetWall.transform.position)).ToArray();
            int i = 0;
            for(int j=0; j<paths.Length; j++)
            {
                PathManager v = paths[j];
                DynPathInfo dp = v.GetComponent<DynPathInfo>();
                if (dp != null)
                {
                    float dist = Vector3.Distance(s.TargetWall.transform.position, v.waypoints[0].position);
                    float diff = PathDiffByDist.Evaluate(dist);
                    bool mpOnly = dp.MPOnly;
                    if (dist <= 35)
                        mpOnly = true;
                    bool spawnClose = i < 3 && dp.type == PathType.Ground && !dp.boss;
                    s.AddPath(v, dp.type, dp.size, dp.boss, mpOnly, dp.AlwaysOn, diff, spawnClose);
                    if (dp.type == PathType.Ground && !dp.boss)
                        i++;
                }
                else
                    s.AddPath(v);
            }
        }
    }

    void ClearStreams()
    {
        if (!Application.isPlaying && StreamManager.instance != null)
            StreamManager.instance.AllStreams = new List<EnemyStream>();
        if (CurrentStreams != null)
        {
            for (int i = 0; i < CurrentStreams.Count; i++)
            {
                if (CurrentStreams[i] != null)
                    DestroyImmediate(CurrentStreams[i].gameObject);
            }
        }
        CurrentStreams = new List<EnemyStream>();
    }

    //Teleport Setup
    public void CreateTPZones()
    {
        TeleporterManager mgr = TeleporterManager.instance;
        if (mgr == null)
            mgr = FindObjectOfType<TeleporterManager>();
        if (mgr == null)
            return;

        List<GameArea> newList = new List<GameArea>();
        newList.Add(mgr.Areas[0]);
        GameArea curArea = new GameArea();
        List<GameObject> tps = new List<GameObject>();
        tps.Add(BaseGateTP.gameObject);
        int areaID = 0;
        curArea.name = "DynamicArea 0";
        List<Teleporter> randomTPs = new List<Teleporter>(); //Used to roll for randoms
        for (int i=0; i<CurrentTiles.Count; i++)
        {
            MapTile tile = CurrentTiles[i].GetComponent<MapTile>();
            //Add to current list of tiles
            foreach (var v in tile.Inside)
            {
                /*
                randomTPs = new List<Teleporter>();
                foreach (var t in v.Teleporters)
                {
                    Teleporter tp = t.GetComponentInChildren<Teleporter>();
                    if (tp != null)
                        randomTPs.Add(tp);
                }
                for (int q = 0; q < v.Budget; q++)
                {
                    if (randomTPs.Count > 0)
                    {
                        int idx = GetRandomNext(0, randomTPs.Count);
                        tps.Add(randomTPs[idx].gameObject);
                        randomTPs.RemoveAt(idx);
                    }
                }
                for (int x = 0; x < randomTPs.Count; x++)
                { if(randomTPs[x] != null) DestroyImmediate(randomTPs[x].gameObject); }
                */
                foreach (var t in v.Teleporters)
                {
                    if(t != null)
                    {
                        Teleporter tp = t.GetComponentInChildren<Teleporter>();
                        if (tp != null)
                            tps.Add(tp.gameObject);
                    }
                }
            }
            //Change to next area if Gate
            if (tile.type == TileType.Gate)
            {
                curArea.TeleportNodes = tps.ToArray();
                if(curArea.TeleportNodes.Length > 0)
                {
                    curArea.StartTeleporter = curArea.TeleportNodes[0].GetComponent<Teleporter>();
                    curArea.BackTeleporter = curArea.TeleportNodes[curArea.TeleportNodes.Length - 1].GetComponent<Teleporter>();
                    if (Infinite)
                        curArea.BackTeleporter = StartZoneTPs[0];
                }
                newList.Add(curArea);

                areaID++;
                curArea = new GameArea();
                curArea.name = "Dynamic Area " + areaID;
                tps = new List<GameObject>();
                foreach (var v in tile.NextZone)
                {
                    /*
                    randomTPs = new List<Teleporter>();
                    foreach (var t in v.Teleporters)
                    {
                        randomTPs.Add(t.GetComponentInChildren<Teleporter>());
                    }
                    for (int q = 0; q < v.Budget; q++)
                    {
                        if (randomTPs.Count > 0)
                        {
                            int idx = GetRandomNext(0, randomTPs.Count);
                            tps.Add(randomTPs[idx].gameObject);
                            randomTPs.RemoveAt(idx);
                        }
                    }
                    for (int x = 0; x < randomTPs.Count; x++)
                    { if (randomTPs[x] != null) DestroyImmediate(randomTPs[x].gameObject); }
                    */
                    foreach (var t in v.Teleporters)
                    {
                        tps.Add(t.GetComponentInChildren<Teleporter>().gameObject);
                    }
                }
            }
        }

        //At end, add current area to TeleporterManager list
        curArea.TeleportNodes = tps.ToArray();
        if (curArea.TeleportNodes.Length > 0)
        {
            curArea.StartTeleporter = curArea.TeleportNodes[0].GetComponent<Teleporter>();
            curArea.BackTeleporter = curArea.TeleportNodes[(int)((curArea.TeleportNodes.Length - 1)/2f)].GetComponent<Teleporter>();
            if (Infinite)
                curArea.BackTeleporter = StartZoneTPs[0];
        }
        newList.Add(curArea);
        mgr.Areas = newList.ToArray(); // Set manager list
    }

    void ClearTPList()
    {
        TeleporterManager mgr = TeleporterManager.instance;
        if (mgr == null)
            mgr = FindObjectOfType<TeleporterManager>();
        if (mgr == null)
            return;

        List<GameArea> newList = new List<GameArea>();
        newList.Add(mgr.Areas[0]);
        mgr.Areas = newList.ToArray();
    }

    //Visibility Zones
    void SetupVisibility()
    {
        VisibilityZones = new List<TileVisibility>();
        //Initial Vis Zone
        TileVisibility t1 = new TileVisibility();
        int val = 0;
        float ct = 0;
        for(int i=0; i<CurrentTiles.Count; i++)
        {
            MapTile mp = CurrentTiles[i].GetComponent<MapTile>();
            val += mp.Direction;
            ct += Mathf.Clamp(mp.Value, 1, 5f);
            t1.Visible.Add(new Visibility(CurrentTiles[i], ct, Mathf.Abs(val)));
            if ((Mathf.Abs(val) >= 90) || (ct >= 3 && Mathf.Abs(val) >= 45))
            {
                t1.hasFront = true;
                /*if (i + 1 < CurrentTiles.Count && ct < 3)
                    t1.Visible.Add(new Visibility(CurrentTiles[i + 1], i+1, Mathf.Abs(val)));*/
                break;
            }
        }
        t1.hasBack = false;
        VisibilityZones.Add(t1);

        //Subsequent Vis Zones
        for(int i=0; i<CurrentTiles.Count;i++)
        {
            TileVisibility zone = new TileVisibility();
            zone.Tile = CurrentTiles[i].GetComponent<MapTile>();
            int prevDir = 0;
            float prevCt = 0;
            float nextCt = 0;
            int nextDir = 0;
            bool hasBack = false;
            bool hasFront = false;
            //Prev Tiles
            for(int j=i-1; j >= 0; j--)
            {
                MapTile mp = CurrentTiles[j].GetComponent<MapTile>();
                prevDir += Mathf.Abs(mp.Direction);
                prevCt += Mathf.Clamp(mp.Value, 1, 4f);
                zone.Visible.Add(new Visibility(CurrentTiles[j], prevCt, prevDir));
                if ((Mathf.Abs(prevDir) >= 90)  || (prevCt >= 3 && Mathf.Abs(prevDir) >= 45))
                {
                    hasBack = true;
                    /*if (j - 1 >=0 && prevCt < 3)
                        zone.Visible.Add(new Visibility(CurrentTiles[j-1], prevCt+1, prevDir));
                    /*if (j - 2 >= 0)
                        zone.Visible.Add(CurrentTiles[j - 2]);*/
                    break;
                }
            }
            zone.Visible.Add(new Visibility(zone.Tile.gameObject, 0, 0));
            //Next Tiles
            for (int q=i+1; q<CurrentTiles.Count; q++)
            {
                MapTile mp = CurrentTiles[q].GetComponent<MapTile>();
                nextDir += Mathf.Abs(mp.Direction);
                nextCt += Mathf.Clamp(mp.Value, 1, 4f);
                zone.Visible.Add(new Visibility(CurrentTiles[q], nextCt, nextDir));
                if ((Mathf.Abs(nextDir) >= 90) 
                    || (nextCt >= 3 && Mathf.Abs(nextDir) >= 45))
                {
                    hasFront = true;
                    if (q + 1 < CurrentTiles.Count && nextCt < 3)
                        zone.Visible.Add(new Visibility(CurrentTiles[q+1], nextCt+1, nextDir));
                    /*if (q + 2 < CurrentTiles.Count)
                        zone.Visible.Add(CurrentTiles[q + 2]);*/
                    break;
                }

            }
            zone.hasBack = hasBack;
            zone.hasFront = hasFront;
            VisibilityZones.Add(zone);
        }
    }

    //Infinite Mode
    void SetupInfinite()
    {
        for(int i=0; i<CurrentGates.Count; i++)
        {
            Gate g = CurrentGates[i];
            g.destroyed = true;
            Health h = g.GetComponent<Health>();
            h.dead = true;
            h.currentHP = 0;
            g.DestroyDisplay(true);
            /*if(i > 0)
                g.OnDeath.AddListener(delegate { SetPreviousGateEnd(GateManager.instance.GateID(CurrentGates[i - 1])); });
            else*/
            if(i == 0)
                g.OnDeath.AddListener(delegate { SetPreviousGateEnd(GateManager.instance.GateID(BaseGate)); GameOrb.instance.ClearProgress(); });
            g.OnRestore.AddListener(delegate { GateAddSection(g); });
        }
    }
    #endregion

    #region System

    [AdvancedInspector.Inspect]
    public void AddSection(bool updateNav = true)
    {
        if((CurrentGates.Count+1) % EventTileFreq == 0)
        {
            bool ev = AddEvent();
            if(ev)
                return;
        }
        DebugTimed.Log("Adding New Section to Map");
        MapTile LastTile = CurrentTiles[CurrentTiles.Count - 1].GetComponent<MapTile>();
        List<MapTile> newTiles = new List<MapTile>();
        float evChange = 0;
        if (!LastZoneChangedEnv)
        {
            MapTile envChange = TryChangeEnv();
            if (envChange.Value > 0)
                newTiles.Add(envChange);
            evChange = envChange.Value;
            if (evChange > 0)           
                LastZoneChangedEnv = true;

        }
        else
            LastZoneChangedEnv = false;
        newTiles.AddRange(CreateArea(1 - evChange)); // Was 2-envChange
        newTiles.Add(CreateTile(NextTile(TileType.Gate, 1)));
        SetupVisibility();
        AddTPZone(newTiles.ToArray());
        AddGate(newTiles.ToArray());
        AddStream(newTiles.ToArray(), LastTile);
        AddPaths(newTiles.ToArray(), LastTile);
        if (updateNav)
            UpdateNav();
        AddedAreas++;
        if (AddedAreas > 1)
        {
            DisableNextCol(AddedAreas - 2);
        }
        GateManager.instance.SortFinal();
    }

    public bool AddEvent()
    {
        MapTile LastTile = CurrentTiles[CurrentTiles.Count - 1].GetComponent<MapTile>();
        List<MapTile> newTiles = new List<MapTile>();
        Gate lastGate = null;
        if (LastTile != null)
            lastGate = LastTile.gameObject.GetComponentInChildren<Gate>();
        MapTile etile = CreateEventTile(lastGate);
        if (etile == null)
            return false;
        if (LastTile.type == TileType.Gate)
        {
            lastGate = LastTile.GetComponentInChildren<Gate>();
            lastGate.NextStreamer.Ornamental = true;
            lastGate.OnDeath.AddListener(GameBase.instance.ForceEndGame);
            lastGate.SetEmission(Color.red);
            lastGate.GetComponentInChildren<GateGems>().ChangeColor(Color.red);
        }
        newTiles.Add(etile);

        MapTile newGate = CreateTile(NextTile(TileType.Gate, 1));
        Gate gt = newGate.GetComponentInChildren<Gate>();
        etile.GetComponentInChildren<EventTile>().SetNextGate(gt);
        gt.OnDeath.AddListener(StartArea.EnableStart);
        gt.OnDeath.AddListener(GameBase.instance.ForceEndGame);
        gt.SetEmission(Color.red);
        gt.GetComponentInChildren<GateGems>().ChangeColor(Color.red);
        newTiles.Add(newGate);
        SetupVisibility();
        AddTPZone(newTiles.ToArray());
        AddGate(newTiles.ToArray());
        AddStream(newTiles.ToArray(), LastTile);
        AddPaths(newTiles.ToArray(), LastTile);
        AddedAreas++;
        if (AddedAreas > 1)
        {
            DisableNextCol(AddedAreas - 2);
        }
        GateManager.instance.SortFinal();
        return true;
    }

    [HideInInspector]
    public List<int> GatesAdded;
    public void GateAddSection(Gate g=null, bool updateNav=true)
    {
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if (g != null)
            {
                if (GatesAdded.Contains(GateManager.instance.GateID(g)))
                {
                    DebugTimed.Log("Gate already generated new tile, ignoring");
                    return;
                }
                else
                    GatesAdded.Add(GateManager.instance.GateID(g));
            }
            AddSection(updateNav);
            net.AddSection();
        }
    }

    public void AddGateID(int id)
    {
        if(!GatesAdded.Contains(id))
            GatesAdded.Add(id);
    }

    void DisableNextCol(int id)
    {
        List<MapTile> dTiles = new List<MapTile>();
        int c = 0;
        for(int i=0; i<CurrentTiles.Count; i++)
        {
            MapTile t = CurrentTiles[i].GetComponent<MapTile>();
            if (t.type == TileType.Gate)
            {
                c++;
                if (c > id)
                    return;
            }
            if (c == id)
                t.ToggleColliders(false);
        }
    }

    public void ShowVisible(MapTile tile=null)
    {
        if(VisibilityZones != null && VisibilityZones.Count > 0)
        {
            TileVisibility vz = GetVis(tile);
            if(vz != null)
            {
                MapTile front = vz.Front();
                MapTile back = vz.Back();
                if (front != null)
                {
                    FrontCap.transform.position = front.LinkPoint.position;
                    FrontCap.transform.rotation = front.LinkPoint.rotation;
                    FrontCap.Environment = front.Environment;
                    FrontCap.SetRocks();
                }
                else
                    FrontCap.transform.position = Vector3.up * -200;
                if (back != null)
                {
                    BackCap.transform.position = back.transform.position;
                    BackCap.transform.rotation = back.transform.rotation;
                    BackCap.Environment = back.Environment;
                    BackCap.SetRocks();
                }
                else
                    BackCap.transform.position = Vector3.up * -200;
                foreach (var v in CurrentTiles)
                {
                    if(v != null)
                    {
                        MapTile m = v.GetComponent<MapTile>();
                        bool shouldShow = vz.Contains(v);
                        if (shouldShow)
                        {
                            if (!m.collidersOn)
                                m.EnableTile();
                            if(!m.visible)
                                m.ShowTile();
                        }
                        else
                        {
                            if (!m.collidersOn)
                                m.DisableTile();
                            if(m.visible)
                                m.HideTile();
                        }

                    }
                }
            }
        }
    }

    #endregion

    #region Map Augmentation

    void AddTPZone(MapTile[] tiles)
    {
        TeleporterManager tm = TeleporterManager.instance;
        if (tm == null)
            tm = FindObjectOfType<TeleporterManager>();
        GameArea curLast = tm.Areas[tm.Areas.Length - 1];
        for(int i=0; i<tiles.Length; i++)
        {
            MapTile t = tiles[i];
            if (t.type == TileType.Link)
            {
                foreach(var zone in t.Inside)
                {
                    foreach(var tp in zone.Teleporters)
                    {
                        if (tp != null)
                            curLast.AddTeleporter(tp);
                    }
                }
            }
            else if(t.type == TileType.Gate)
            {
                foreach (var zone in t.Inside)
                {
                    foreach (var tp in zone.Teleporters)
                    {
                        if(tp != null)
                            curLast.AddTeleporter(tp);
                    }
                }
                curLast = new GameArea();
                foreach (var zone in t.NextZone)
                {
                    foreach (var tp in zone.Teleporters)
                    {
                        if (tp != null)
                            curLast.AddTeleporter(tp);
                    }
                }
                curLast.name = "Dynamic TP Zone " + tm.Areas.Length;
                curLast.StartTeleporter = curLast.TeleportNodes[0].GetComponentInChildren<Teleporter>();
                curLast.BackTeleporter = curLast.TeleportNodes[(int)((curLast.TeleportNodes.Length - 1) / 2f)].GetComponentInChildren<Teleporter>();
                if (Infinite)
                    curLast.BackTeleporter = StartZoneTPs[0];
                tm.AddArea(curLast);
            }
        }
    }

    void AddPaths(MapTile[] tiles, MapTile LastGate)
    {
        List<PathManager> tmp = new List<PathManager>();
        int streamidx = CurrentStreams.Count - 1;
        for (int i = tiles.Length - 1; i >= 0; i--)
        {
            MapTile tile = tiles[i];
            //Link Old Paths through Link Tiles
            if (tile.type == TileType.Link)
            {
                foreach (var v in tmp)
                {
                    AddToPath(v, tile.LinkPaths);
                }
            } //Break if Gate
            else if (tile.type == TileType.Gate)
            {
                //Add In Roads
                foreach (var v in tmp)
                {
                    AddToPath(v, tile.InPaths);
                    CombinedPaths.Add(v);
                }
                foreach (var v in tile.SpawnPaths)
                {
                    tmp.Add(CreatePath(tile, v));
                }
                StreamAddPaths(streamidx, tmp.ToArray());
                streamidx--;
                tmp = new List<PathManager>();
                foreach (var v in tile.OutPaths)
                {
                    tmp.Add(CreatePath(tile, v));
                }
            }
            //Create new paths from tile
            foreach (var v in tile.SpawnPaths)
            {
                tmp.Add(CreatePath(tile, v));
            }
            //Hide Path Building Blocks
            tile.HidePaths();
        }
        foreach (var v in tmp)
        {
            AddToPath(v, LastGate.InPaths);
        }
        StreamAddPaths(streamidx, tmp.ToArray());
    }

    void AddGate(MapTile[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].type == TileType.Gate)
            {
                Gate g = tiles[i].GetComponentInChildren<Gate>();
                g.gameObject.name = "Forward Gate" + (CurrentGates.Count + 2);
                g.GateIndex = CurrentGates.Count + 2;
                g.ReviveRequirement = 150 + (CurrentGates.Count * 45);
                if(Infinite)
                {
                    g.destroyed = true;
                    g.DestroyDisplay(true);
                    Health h = g.GetComponent<Health>();
                    h.dead = true;
                    h.currentHP = 0;
                    //g.OnDeath.AddListener(delegate { SetPreviousGateEnd(g.Streamer.TargetWall.GetComponent<Gate>()); });
                    g.OnRestore.AddListener(delegate { GateAddSection(g); });
                }
                CurrentGates.Add(g);
            }
                
        }
    }

    [HideInInspector]
    public List<int> EndGates;
    public void SetPreviousGateEnd(int gateID)
    {
        if (!EndGates.Contains(gateID) && GateManager.instance.AllGates.Count > gateID)
        {
            Gate g = GateManager.instance.GetGate(gateID);
            if (g.CurHP() < 100)
                g.GetHP().currentHP = 100f;
            DebugTimed.Log(g.gameObject.name + " (id:"+gateID+") set to end game on destroy - Cur HP: " + g.CurHP());
            EndGates.Add(gateID);
            g.OnDeath.AddListener(StartArea.EnableStart);
            g.OnDeath.AddListener(GameBase.instance.ForceEndGame);
            g.GetComponentInChildren<GateGems>().ChangeColor(Color.red);
        }
        else
        {
            Gate g = GateManager.instance.GetGate(gateID);
            if (g.CurHP() < 100)
                g.GetHP().currentHP = 100f;
            DebugTimed.Log(g.gameObject.name + " (id:" + gateID + ") already set to end game, skipping... - Cur HP: " + g.CurHP());
        }
            
    }

    void AddStream(MapTile[] tiles, MapTile LastGate)
    {
        if (StreamManager.instance == null)
            StreamManager.instance = FindObjectOfType<StreamManager>();
        Gate prev = LastGate.GetComponentInChildren<Gate>();
        GameObject CurGate = null;
        for (int i=tiles.Length-1; i >= 0; i--)
        {
            MapTile t = tiles[i];
            if (t.type == TileType.Gate)
            {
                Gate g = t.gameObject.GetComponentInChildren<Gate>();
                CurGate = g.gameObject;
                if(prev.NextStreamer != null)
                    prev.NextStreamer.RestoreGate = g;
                EnemyStream stream = StreamManager.CreateStream(CurGate, null);
                if (stream != null)
                {
                    CurrentStreams.Add(stream);
                    g.Streamer = prev.NextStreamer;
                    g.NextStreamer = stream;
                    LastStream = stream;
                    g.CloseArea = CurrentGates.Count;
                    SetGateFunctions(prev);
                }
            }
        }
    }

    #endregion

    #region Utility

    public void SetSeed(string input)
    {
        CurrentSeed = input;
        Rand = new RandomValue(input.GetHashCode());
    }

    public void ResetSeed()
    {
        SetSeed(CurrentSeed);
    }

    private int GetRandomNext(int min, int max)
    {
        return Rand.Next(min, max);
    }

    public int GetLastRandom()
    {
        if (Rand != null)
            return Rand.LastValue;
        return -1;
    }

    List<TilePrefab> GetTileOfType(TileType type)
    {
        List<TilePrefab> tls = new List<TilePrefab>();
        foreach(var v in TileDB.GetTiles(CurEnv, type))
        {
            if (v.type == type && (!v.Unique || !UsedTiles.ContainsKey(v.Name)))
            {
                if (type == TileType.Gate && v.HeightChange != 0 && CurrentGates.Count % EventTileFreq == 0)
                { }
                else
                    tls.Add(v);
            }

        }
        return tls;
    }

    void StripTileAngles(ref List<TilePrefab> tls)
    {
        for(int i=tls.Count-1; i>=0; i--)
        {
            TilePrefab p = tls[i];
            bool invalidDir = CurAngle + p.Direction > 90 || CurAngle + p.Direction < -90;
            bool invalidSharp = Mathf.Abs(CurAngle) + Mathf.Abs(p.Direction) > 180;
            bool allowedAngle = CheckAngle(p.Direction, p.Value);
            if ((invalidDir || invalidSharp || !allowedAngle))
            {
                tls.RemoveAt(i);
            }

        }
    }

    bool CheckAngle(float dir, float length)
    {
        if (CurrentTiles.Count < 1)
            return true;
        float x = 0;
        for(int i=CurTileValues.Count-1; i>=0; i--)
        {
            if (CurTileValues[i].x == 0)
                return true;
            x += CurTileValues[i].x;
            if (Mathf.Abs(x + dir) >= 180)
                return false;
        }
        return true;
    }

    void StripDuplicate(ref List<TilePrefab> tls)
    {
        if (CurTileValues.Count < 2)
            return;
        for (int i = tls.Count - 1; i >= 0; i--)
        {
            TilePrefab p = tls[i];
            if (((CurTileValues[CurTileValues.Count-1].x == p.Direction && CurTileValues[CurTileValues.Count - 1].y == p.Value &&
                CurTileValues[CurTileValues.Count - 2].x == p.Direction && CurTileValues[CurTileValues.Count - 2].y == p.Value) || 
                (PrevTile != null && p.Name == PrevTile.Name))
                && tls.Count > 1)
                tls.RemoveAt(i);
        }
    }

    void StripTileValues(ref List<TilePrefab> tls, float budget)
    {
        for (int i = tls.Count - 1; i >= 0; i--)
        {
            TilePrefab p = tls[i];
            if (curLength + p.Value > budget && tls.Count > 1)
                tls.RemoveAt(i);
        }
    }

    List<TilePrefab> GetOfRarity(List<TilePrefab> tiles, TileRarity wanted)
    {
        List<TilePrefab> tls = new List<TilePrefab>();
        foreach (var v in tiles)
        {
            if (v.Rarity == wanted)
                tls.Add(v);
        }
        return tls;
    }

    TilePrefab SelectTile(List<TilePrefab> tls)
    {
        int RarityCheck = GetRandomNext(0, 1000);
        if(RarityCheck < 2)
        {
            List<TilePrefab> URare = GetOfRarity(tls, TileRarity.UltraRare);
            if(URare.Count > 0)
                return URare[GetRandomNext(0, URare.Count)];
        }
        if (RarityCheck < 15)
        {
            List<TilePrefab> Rare = GetOfRarity(tls, TileRarity.Rare);
            if (Rare.Count > 0)
                return Rare[GetRandomNext(0, Rare.Count)];
        }
        if (RarityCheck < 50)
        {
            List<TilePrefab> Uncommon = GetOfRarity(tls, TileRarity.Uncommon);
            if (Uncommon.Count > 0)
                return Uncommon[GetRandomNext(0, Uncommon.Count)];
        }
        List<TilePrefab> Common = GetOfRarity(tls, TileRarity.Common);
        if (Common.Count > 0)
            return Common[GetRandomNext(0, Common.Count)];
        if (tls.Count > 0)
            return tls[0];
        return null;
    }

    TileVisibility GetVis(MapTile t)
    {
        if (t == null)
            return VisibilityZones[0];
        else
        {
            foreach(var v in VisibilityZones)
            {
                if (v.Tile == t)
                    return v;
            }
        }
        return null;
    }

    public void ShowStartTiles()
    {
        if(CurrentTiles.Count > 0)
            ShowVisible(CurrentTiles[0].GetComponent<MapTile>());
    }

    public bool noNavBuilding;
    void UpdateNav()
    {
        if (noNavBuilding)
            return;
        UnityEngine.AI.NavMeshSurface nav = GetComponent<UnityEngine.AI.NavMeshSurface>();
        if (Application.isPlaying)
            nav.UpdateNavMesh(nav.navMeshData);
    }

#endregion

    [System.Serializable]
    public class TileVisibility
    {
        public MapTile Tile;
        public List<Visibility> Visible;
        public bool hasBack;
        public bool hasFront;

        public TileVisibility()
        {
            Visible = new List<Visibility>();
        }
        
        public bool Contains(GameObject t)
        {
            for(int i=0; i<Visible.Count; i++)
            {
                if (Visible[i].Tile == t)
                    return true;
            }
            return false;
        }

        public MapTile Front()
        {
            if (!hasFront)
                return null;
            return Visible[Visible.Count - 1].Tile.GetComponent<MapTile>();
        }

        public MapTile Back()
        {
            if (!hasBack)
                return null;
            for (int i=0; i<Visible.Count; i++)
            {
                if(Visible[i].Tile == Tile.gameObject && i > 0)
                {
                    return Visible[i - 1].Tile.GetComponent<MapTile>();
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class Visibility
    {
        public GameObject Tile;
        public float Distance;
        public float Angle;


        public Visibility(GameObject t, float d, float a)
        {
            Tile = t;
            Distance = d;
            Angle = a;
        }
    }

    public class RandomValue
    {
        System.Random R;
        public int LastValue = -1;

        public RandomValue(string seed)
        {
            R = new System.Random(seed.GetHashCode());
        }

        public RandomValue(int seed)
        {
            R = new System.Random(seed);
        }

        public int Next(int min, int max)
        {
            int x = R.Next();//R.Next(min, max);
            LastValue = x;
            x = (int)Mathf.Floor(min + (((float)x / int.MaxValue)*(max-min)));
            if (x >= max)
                x = Mathf.Clamp(x, min, max - 1);
            return x;
        }
    }
}
