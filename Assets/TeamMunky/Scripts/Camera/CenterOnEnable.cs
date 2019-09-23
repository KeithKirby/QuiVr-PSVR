using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CenterOnEnable : MonoBehaviour {

    Camera _lastActiveCamera;

    private void OnEnable()
    {
        _lastActiveCamera = null;
        TeleporterManager.PlayerMovedEvt += TeleporterManager_PlayerMovedEvt;

    }

    private void OnDisable()
    {
        TeleporterManager.PlayerMovedEvt -= TeleporterManager_PlayerMovedEvt;
    }

    private void TeleporterManager_PlayerMovedEvt()
    {
        _lastActiveCamera = null;
    }

    // Update is called once per frame
    void Update () {
		if(_lastActiveCamera != Camera.main)
        {
            _lastActiveCamera = Camera.main;
            if(_lastActiveCamera!=null)
            {
                transform.position = _lastActiveCamera.transform.position;
                transform.rotation = _lastActiveCamera.transform.rotation;
            }
        }
	}
}
