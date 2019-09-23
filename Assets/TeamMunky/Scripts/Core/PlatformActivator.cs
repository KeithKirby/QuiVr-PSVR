using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformActivator : MonoBehaviour {

    public DeviceTypeFlags Platforms;

    // Use this for initialization
    void Start () {
        GameEventPublisher.LevelInitialised += GameEventPublisher_LevelInitialised;
    }

    private void GameEventPublisher_LevelInitialised()
    {
        var dtFlag = (int)GameGlobeData.deviceType;
        if (((1<<dtFlag) & (int)Platforms) != 0)
        {
            for(int i=0;i<transform.childCount;++i)
            {
                var c = transform.GetChild(i);
                c.gameObject.SetActive(true);
            }
        }
    }
}
