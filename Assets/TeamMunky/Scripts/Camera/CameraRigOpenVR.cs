using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraRigOpenVR : CameraRigBase
{
    public override void Init()
    {
        Debug.Log(" ==Setup OpenVR== ");
        GameGlobeData.deviceType = DeviceType.htcvive;
        if (null != SteamVR.instance)
        {
            //if (SteamVR.instance.hmd_TrackingSystemName == "oculus")
                //GameGlobeData.deviceType = DeviceType.oculus;
            Debug.Log("DeviceType is == " + SteamVR.instance.hmd_TrackingSystemName);
        }
        base.Init();
    }
}
