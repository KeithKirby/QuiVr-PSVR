using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DLFilter
{
    General = 1<<0,
    Trophies = 1<<1,
    VR = 1 << 2
}

// Filtered logging
public class DLog : MonoBehaviour
{
    public DLFilter LogFilter;
    public static DLFilter s_logFilter;

    // Use this for initialization
	void Awake() {
        s_logFilter = LogFilter;
	}

    static public void Log(DLFilter f, string txt, params object[] args)
    {
        if((f & s_logFilter) != 0)
        {
            Debug.LogFormat(f.ToString() + ":" + txt, args);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
