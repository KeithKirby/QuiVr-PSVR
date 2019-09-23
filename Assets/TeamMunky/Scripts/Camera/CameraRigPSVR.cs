using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PS4
using UnityEngine.PS4.VR;
#endif

public class CameraRigPSVR : CameraRigBase
{
    override public void Init()
    {
        //Debug.Log(" ==Setup PSVR== ");
        GameGlobeData.deviceType = DeviceType.psvr;
        base.Init();
    }
}
