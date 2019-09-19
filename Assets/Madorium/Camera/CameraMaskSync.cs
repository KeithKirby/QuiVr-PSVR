using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaskSync : MonoBehaviour {

	Camera _camera;
    bool _ready = false;
    PSVRSetupManager _setupMan = null;
    RenderMode _renderMode = null;

    // Use this for initialization
    void Start() {
        _camera = GetComponent<Camera>();
        _setupMan = GameObject.FindObjectOfType<PSVRSetupManager>();
        if (_setupMan == null)
            throw new System.Exception("Missing PSVRSetupManager");
        _renderMode = GameObject.FindObjectOfType<RenderMode>();
        if (_renderMode == null)
            throw new System.Exception("Missing PSVRSetupManager");
        _ready = true;
        _renderMode.CameraMaskChanged += CameraMaskChanged;        
        UpdateScreenSetup();
    }

    void OnDestroy()
    {
        _renderMode.CameraMaskChanged -= CameraMaskChanged;
    }

    private void CameraMaskChanged(RenderMode.CameraMaskMode mode)
    {
        UpdateScreenSetup();
    }

    private void SetupPublisher_SetupScreenActive(bool show)
    {
        UpdateScreenSetup();
    }

    void UpdateScreenSetup()
    {
        if (_ready)
            _camera.cullingMask = _renderMode.CurrentMask;
    }
}
