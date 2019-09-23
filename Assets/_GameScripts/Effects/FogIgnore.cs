using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogIgnore : MonoBehaviour {

    public static List<GameObject> Ignores;

    void Awake()
    {
        if (Ignores == null)
            Ignores = new List<GameObject>();
        Ignores.Add(gameObject);
    }

    void OnDestroy()
    {
        if (Ignores != null)
            Ignores.Remove(gameObject);
    }

    public static void HideIgnores()
    {
        if (Ignores == null)
            return;
        foreach (var v in Ignores)
        {
            if (v != null && v.activeSelf)
            {
                v.SetActive(false);
            }
        }
    }

    public static void ShowIgnores()
    {
        if (Ignores == null)
            return;
        foreach (var v in Ignores)
        {
            if (v != null && !v.activeSelf)
            {
                v.SetActive(true);
            }
        }
    }
}
