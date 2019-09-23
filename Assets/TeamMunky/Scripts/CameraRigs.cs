//using PhatRobit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class CameraRigs : MonoBehaviour
{
    public CameraRigBase ActiveRigRef;

    public CameraRigOculus CameraRigOculusRef;
    public CameraRigPSVR CameraRigPSVR;

    [HideInInspector]
    public static CameraRigs _instance;

    void Start()
    {
        //Debug.Log("Start CameraRig");
        if (_instance != null)
        {
            Debug.LogError("Multiple instances of Device Manager");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        //Debug.Log(" ==loadedDeviceName== " + UnityEngine.XR.XRSettings.loadedDeviceName);
        var deviceModel = XRDevice.model;

#if UNITY_PS4
        ActiveRigRef = CameraRigPSVR;
#else
        //if ("Oculus Rift CV1" == deviceModel)
        ActiveRigRef = CameraRigOculusRef;
        //else
            //Debug.Log("Unknown VR device:'" + deviceModel + "'");
#endif           
        
        ActiveRigRef.Init();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        ActiveRigRef.OnSceneChanged();
    }
    
    static public CameraRigBase ActiveRig
    {
        get
        {
            return _instance ? _instance.ActiveRigRef : null;
        }
    }
}
