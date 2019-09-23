using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDisabler : MonoBehaviour
{
    Camera _camera;
    bool _updating = false;

	// Use this for initialization
	void Start () {
        _camera = GetComponent<Camera>();        
        StartCoroutine(UpdateScreenSetup());
    }

    void OnEnable()
    {
        SetupPublisher.SetupScreenActive += SetupPublisher_SetupScreenActive;    
    }

    void OnDisable()
    {
        SetupPublisher.SetupScreenActive -= SetupPublisher_SetupScreenActive;
    }

    private void SetupPublisher_SetupScreenActive(bool show)
    {
        DoSetup();
    }

    void DoSetup()
    {
        if(!_updating)
        {
            _updating = true;
            DoSetup();
        }
    }

    IEnumerator UpdateScreenSetup()
    {
        PSVRSetupManager setupMan = null;
        while (null == setupMan)
        {
            setupMan = GameObject.FindObjectOfType<PSVRSetupManager>();
            yield return new WaitForSecondsRealtime(0.5f);
        }
        //if (SetupPublisher.IsSetupScreenActive)
            //_camera.cullingMask = setupMan.RenderSystemUI;
        //else
            //_camera.cullingMask = setupMan.RenderGame;
        _updating = false;
    }
}
