using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for instantiating global objects
public class PlatformInit : MonoBehaviour
{
    static bool _createdGlobals = false;

    public DeviceType Platform;
    public GameObject[] GlobalPf;

    // Use this for initialization
    void Awake()
    {
        if (!_createdGlobals)
        {
            _createdGlobals = true;
            if (Platform == GameGlobeData.deviceType)
            {
                foreach (var g in GlobalPf)
                {
                    Init(g);
                }
            }
        }
        Destroy(gameObject);
    }

    void Init(MonoBehaviour g)
    {
        var inst = Instantiate(g);
        DontDestroyOnLoad(inst);
    }

    void Init(GameObject g)
    {
        var inst = Instantiate(g);
        DontDestroyOnLoad(inst);
    }
}
