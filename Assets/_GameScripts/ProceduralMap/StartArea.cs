using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartArea : MonoBehaviour {

    public GameObject[] MeshHolders;
    public GameObject[] ExtraFXHolders;
    static List<MeshRenderer> Renderers;
    static List<GameObject> ExtraFX;
    public static bool hidden;
    static List<GameObject> Holders;

    void Awake()
    {
        Renderers = new List<MeshRenderer>();
        ExtraFX = new List<GameObject>();
        Holders = new List<GameObject>();
        foreach(var v in MeshHolders)
        {
            foreach(var m in v.GetComponentsInChildren<MeshRenderer>())
            {
                Renderers.Add(m);
            }
            Holders.Add(v);
        }
        foreach(var v in ExtraFXHolders)
        {
            ExtraFX.Add(v);
            Holders.Add(v);
        }
    }

    public static void DisableStart()
    {
        foreach(var v in Holders)
        {
            if(v != null)
                v.SetActive(false);
        }
    }

    public static void EnableStart()
    {
        foreach (var v in Holders)
        {
            if(v != null)
                v.SetActive(true);
        }
        //TileManager.instance.ShowStartTiles();
    }

    public static void HideStart()
    {
        if (hidden)
            return;
        hidden = true;
        if(Renderers != null)
        {
            foreach(var v in Renderers)
            {
                if(v != null)
                    v.enabled = false;
            }
        }
        if(ExtraFX != null)
        {
            foreach(var v in ExtraFX)
            {
                if (v != null)
                    v.SetActive(false);
            }
        }
    }

    public static void ShowStart()
    {
        if (!hidden)
            return;
        hidden = false;
        if (Renderers != null)
        {
            foreach (var v in Renderers)
            {
                if (v != null)
                    v.enabled = true;
            }
        }
        if (ExtraFX != null)
        {
            foreach (var v in ExtraFX)
            {
                if (v != null)
                    v.SetActive(true);
            }
        }
    }
}
