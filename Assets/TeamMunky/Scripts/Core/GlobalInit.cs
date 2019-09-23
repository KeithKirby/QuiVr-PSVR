using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for instantiating global objects
public class GlobalInit : MonoBehaviour
{
    static bool _createdGlobals = false;

    public GameObject[] GlobalPf;

    // Use this for initialization
    void Awake()
    {
        if (!_createdGlobals)
        {
#if UNITY_PS4
            // Handy spot to make tty output tolerable
            //Application.SetStackTraceLogType( LogType.Log, StackTraceLogType.None );
            //Application.SetStackTraceLogType( LogType.Warning, StackTraceLogType.None );
#endif
            _createdGlobals = true;
            foreach(var g in GlobalPf)
            {
                Init(g);
            }
        }
        Destroy(gameObject);
    }

    void Init(MonoBehaviour g)
    {
        var inst = Instantiate(g);
        DontDestroyOnLoad(inst);
        //Debug.Log( " ==GlobalInit Instantiated MonoBehaviour " + inst + " == " );
    }

    void Init(GameObject g)
    {
        var inst = Instantiate(g);
        DontDestroyOnLoad(inst);
        //Debug.Log( " ==GlobalInit Instantiated GameObject " + inst + " == " );
    }
}
