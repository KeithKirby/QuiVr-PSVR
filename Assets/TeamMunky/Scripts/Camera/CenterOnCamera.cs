using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnCamera : MonoBehaviour
{	
    void LateUpdate()
    {
        var rig = CameraRigs.ActiveRig;
        if (null != rig)
        {
            var rigCam = rig.GetCamera();
            transform.position = rigCam.transform.position;
            transform.rotation = rig.transform.rotation;
        }
    }
}
