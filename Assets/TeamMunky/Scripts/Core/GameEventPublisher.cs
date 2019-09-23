using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Madorium.Events;

public class GameEventPublisher : MonoBehaviour
{
	static public event Events.EventHandler LevelInitialised;
    static public void PublishLevelInitialised()
    {
        if(null!=LevelInitialised)
            LevelInitialised();
    }

    static public event Events.EventHandler SetupComplete;
    static public void PublishSetupComplete()
    {
        if (null != SetupComplete)
            SetupComplete();
    }

    public delegate void OnHeightChanged();
    static public event OnHeightChanged HeightChanged;
    static public void PublishHeightChanged()
    {
        if (null != HeightChanged)
            HeightChanged();
    }

    public delegate void OnLoadStarted();
    static public event OnLoadStarted LoadStarted;
    static public void PublishLoadStarted()
    {
        if (null != LoadStarted)
            LoadStarted();
    }
}
