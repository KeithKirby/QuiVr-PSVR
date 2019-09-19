using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRelativeScale : MonoBehaviour {

    public Transform Target;
    public float ScaleMultiplier = 1;
    public Camera TargetCamera;

    Vector3 _initScale;
    Camera _camera;

    private void Start()
    {
        if(null == Target)
        {
            Debug.LogWarning("[CameraRelativeScale::Start] Target == null");
            return;
        }

        _camera = TargetCamera;
        _initScale = Target.localScale;
    }

    public void SetTargetCamera(Camera c)
    {
        _camera = c;
    }

    void LateUpdate () {
        if (null == _camera)
            return;

        float dist = Vector3.Distance(_camera.transform.position, this.transform.position);
        transform.localScale = _initScale * dist * ScaleMultiplier;
	}
}
