using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigBase : MonoBehaviour
{
    protected void Awake()
    {
        _rotationOffset = transform.rotation;
        _setPosition = transform.position;
    }

    void Start()
    {
        UpdateHeight();
    }

    void OnEnable()
    {
        PSVRManager.Recenter += PSVRManager_Recenter;
    }

    void OnDisable()
    {
        PSVRManager.Recenter -= PSVRManager_Recenter;
    }

    private void PSVRManager_Recenter()
    {
        UpdateHeight();
    }

    // Show avatar, if we have one
    public virtual void ShowAvatar(bool show)
    {
    }

    virtual public void SetAvatarTransform(Transform tfm)
    {
    }

    virtual public void SetAvatarPosition(Vector3 pos)
    {
    }

    public void SetTransform(Transform tfm)
    {
        SetPositionAndRotation(tfm.position, tfm.rotation);
    }

    public void SetLevelScale(float scale)
    {
        _levelScale = scale;
        UpdatePositionAndRotation();
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        _setPosition = pos;
        _setRotation = rot;
        UpdatePositionAndRotation();
    }

    void UpdatePositionAndRotation()
    {
        transform.position = _setPosition + new Vector3(0, 0, 0);
        transform.rotation = _setRotation * _rotationOffset;
    }

    public Camera GetCamera()
    {
        return RigCamera;
    }
    
    virtual public void Init()
    {
        gameObject.SetActive(true);
        GameGlobeData.spectate = false;
    }

    public Transform CameraOffset;
    public Camera RigCamera;
//    public float CameraStartScale = 1;

    public virtual void OnSceneChanged() { }
    
    public void UpdateHeight()
    {
        var height = PSVRManager.PlayerHeight; // * PSVRManager.PlayerScale;
        if(null!=CameraOffset)
            CameraOffset.localPosition = new Vector3(0, height, 0);
        //Debug.LogFormat("UpdateHeight height {0} scale {1} final {2}", PSVRManager.PlayerHeight, PSVRManager.PlayerScale, height);
        Debug.LogFormat("UpdateHeight height {0}", PSVRManager.PlayerHeight);
    }

    protected Quaternion _rotationOffset;

    Vector3 _setPosition;
    Quaternion _setRotation;

    public float RotationOffset = 0;
    protected float _levelScale = 1;
}
