using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppModel : MonoBehaviour
{
    public enum LoadTypeName
    {
        Load,
        Restart
    }

    public bool CompletedSetup = false;
    public LoadTypeName LoadType = LoadTypeName.Load;

    public void Awake()
    {
        //Debug.Log("Disable FPS ticker");
        //UnityEngine.PS4.Utility.SetFPSTickerOutput(false); // Stop debug spew
    }
}