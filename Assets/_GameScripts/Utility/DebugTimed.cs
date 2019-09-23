using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimed {

	public static void Log(string str)
    {
        UnityEngine.Debug.Log(System.DateTime.Now.ToLongTimeString() + " : " + str);
    }

}
