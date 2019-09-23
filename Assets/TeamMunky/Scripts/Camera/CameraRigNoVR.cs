using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigNoVR : CameraRigBase
{
    override public void Init()
    {
        Debug.Log(" ==Setup NoVR==");
        GameGlobeData.deviceType = DeviceType.novr;
        base.Init();
    }
}
