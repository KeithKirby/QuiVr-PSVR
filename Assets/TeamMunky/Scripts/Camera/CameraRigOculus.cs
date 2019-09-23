using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigOculus : CameraRigBase
{
    public GameObject OculusAvatar;

    public override void ShowAvatar(bool show)
    {
        OculusAvatar.SetActive(show);
    }

    override public void SetAvatarPosition(Vector3 pos)
    {
        OculusAvatar.transform.position = pos;
    }

    override public void SetAvatarTransform(Transform tfm)
    {
        OculusAvatar.transform.position = tfm.position;
        OculusAvatar.transform.rotation = Quaternion.Inverse(_rotationOffset) * tfm.rotation;
    }

    public override void Init()
    {
        Debug.Log(" ==Setup Oculus== ");
        GameGlobeData.deviceType = DeviceType.oculus;
        base.Init();
        OculusAvatar.SetActive(true);
    }
}
