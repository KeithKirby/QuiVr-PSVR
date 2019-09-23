using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using UnityEngine.AI;
public class MapTile : MonoBehaviour {

    public TileValues TileData;
    public TileType type;
    public Transform LinkPoint;
    public EnvironmentType Environment;
    public int Direction;
    public float Value;
    public int CurHeight;
    NavMeshLink[] navlinks;

    [Header("Paths")]
    [AdvancedInspector.Inspect("isLink")]
    public PathManager[] LinkPaths;
    public PathManager[] SpawnPaths;

    [Space]
    [AdvancedInspector.Inspect("isGate")]
    public PathManager[] InPaths;
    [AdvancedInspector.Inspect("isGate")]
    public PathManager[] OutPaths;

    [Header("Teleporters")]
    public TPZone[] Inside;
    [AdvancedInspector.Inspect("isGate")]
    public TPZone[] NextZone;

    [Space]
    [Header("Misc")]
    public Transform OrbAnchor;

    MeshRenderer[] rends;
    Collider[] cols;
    ExtraEffectRenderer[] ExtraFX;

    public void Awake()
    {
        rends = GetComponentsInChildren<MeshRenderer>();
        cols = GetComponentsInChildren<Collider>();
        AllTPs = GetComponentsInChildren<Teleporter>();
        ExtraFX = GetComponentsInChildren<ExtraEffectRenderer>();
        navlinks = GetComponentsInChildren<NavMeshLink>();
    }

    Teleporter[] AllTPs;
    IEnumerator Start()
    {
        collidersOn = true;
        foreach (var tp in AllTPs)
        {
            tp.OnTeleport.AddListener(delegate 
            {
                EnvironmentManager.ChangeEnv(Environment);
                TileManager.instance.ShowVisible(this);
                UseTile();
            });
        }
        yield return true;
        foreach(var v in navlinks)
        {
            v.UpdateLink();
        }
    }

    public void UseTile()
    {
        if(GameOrb.instance != null && OrbAnchor != null)
        {
            GameOrb.Target = OrbAnchor.position;
        }
    }

    [AdvancedInspector.Inspect]
    public void SetRocks()
    {
        if (TileData != null)
        {
            foreach (var v in GetComponentsInChildren<MeshRenderer>())
            {
                TileData.TryRockReplace(v, Environment);
            }
        }
        else
            Debug.Log("Set TileData before running material replacement.");
    }

    public bool visible;
    public void HideTile()
    {
        visible = false;
        foreach(var v in rends)
        {
            if (v != null)
                v.enabled = false;
        }
        foreach(var v in AllTPs)
        {
            LookAt la = v.GetComponent<LookAt>();
            if(la != null)
                la.visible = false;
            RotateTimed rt = v.GetComponentInChildren<RotateTimed>();
            if(rt != null)
                rt.paused = true;
        }
        foreach (var v in ExtraFX)
        {
            if (v != null)
                v.enabled = false;
        }
    }

    public void ShowTile()
    {
        visible = true;
        foreach (var v in rends)
        {
            if (v != null)
                v.enabled = true;
        }
        foreach (var v in AllTPs)
        {
            LookAt la = v.GetComponent<LookAt>();
            if (la != null)
                la.visible = false;
            RotateTimed rt = v.GetComponentInChildren<RotateTimed>();
            if (rt != null)
                rt.paused = true;
        }
        foreach (var v in ExtraFX)
        {
            if (v != null)
                v.enabled = true;
        }
        GateGems gems = GetComponentInChildren<GateGems>();
        if (gems != null)
            gems.ForceUpdate();
    }

    public void DisableTile()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void EnableTile()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public bool collidersOn = true;
    public void ToggleColliders(bool on)
    {
        collidersOn = on;
        if(cols != null)
        {
            foreach (var v in cols)
            {
                v.enabled = on;
            }
        }
    }

    public void HidePaths()
    {
        foreach(var v in LinkPaths)
        { if (v != null) v.gameObject.SetActive(false); }
        foreach (var v in SpawnPaths)
        { if (v != null) v.gameObject.SetActive(false); }
        foreach (var v in InPaths)
        { if(v != null) v.gameObject.SetActive(false); }
        foreach (var v in OutPaths)
        { if (v != null) v.gameObject.SetActive(false); }
    }

    [System.Serializable]
    public class TPZone
    {
        public string Name;
        public int Budget;
        public GameObject[] Teleporters;

        public override string ToString()
        {
            return Name;
        }
    }

    bool isGate()
    {
        return type == TileType.Gate;
    }

    bool isLink()
    {
        return type == TileType.Link;
    }

}
