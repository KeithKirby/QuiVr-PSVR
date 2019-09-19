using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class VRGuideController : MonoBehaviour {

    public Transform PlayArea;
    public Transform MainCamera;

    public float ActivateRightAngle = 90;
    public float ActivateLeftAngle = 90;

    public float CenterGuide_Left = 45.0f;
    public float CenterGuide_Right = 45.0f;
    public float CenterGuide_Up = 45.0f;

    public bool ForceAlwaysVisible = false;
    bool _wantRecenter = true;

    public VRGuide GuidesLeft;
    public VRGuide GuidesRight;
    public VRGuide GuidesCenter;

    private void OnEnable()
    {
        PSVRManager.Recenter += PSVRManager_Recenter;
    }

    private void OnDestroy()
    {
        PSVRManager.Recenter -= PSVRManager_Recenter;
    }

    private void PSVRManager_Recenter()
    {
        _wantRecenter = true;
    }

    void UpdateCenterGuide(float yAngle, float zAngle)
    {
        bool show = yAngle < -CenterGuide_Left || yAngle > CenterGuide_Right || zAngle > CenterGuide_Up;
        GuidesCenter.Show = DisableVRGuide.Enabled && show ;
    }


    // Update is called once per frame
    void Update()
    {
        if (_wantRecenter && null != PSVRManager.instance && null != PlayArea)
        {
            _wantRecenter = false;
            var camPos = MainCamera.position;
            var floorPos = PlayArea.position; // Put the guide on the floor
            transform.position = new Vector3(camPos.x, floorPos.y, camPos.z);
            Debug.Log("VRGuideController Recenter:" + transform.position);
        }

        float fwdDot = Vector3.Dot(transform.forward, MainCamera.forward);
        float rightDot = Vector3.Dot(transform.right, MainCamera.forward);
        float upDot = Vector3.Dot(transform.up, MainCamera.forward);

        // Right 0 to 180
        // Left 0 to -180
        float yAngle = -360 * Mathf.Atan2(-rightDot, fwdDot) / (2f * Mathf.PI);
        float zAngle = -360 * Mathf.Atan2(-upDot, fwdDot) / (2f * Mathf.PI);
        UpdateCenterGuide(yAngle, zAngle);

        if (null != GuidesLeft)
        {
            GuidesLeft.Show = DisableVRGuide.Enabled && (ForceAlwaysVisible || yAngle <= -ActivateLeftAngle); // Facing left
        }
        if (null != GuidesRight)
        {
            GuidesRight.Show = DisableVRGuide.Enabled && (ForceAlwaysVisible || yAngle >= ActivateRightAngle); // Facing left
        }

#if !UNITY_EDITOR
        /*
        var move0 = PS4Input.MoveGetButtons(0, 0);
        if((move0 & (int)MoveGetButtons.Circle) != 0)
        {
            rotationAngle += 30.0f;
            if (rotationAngle > 360)
                rotationAngle -= 360;
            Rotator.transform.eulerAngles = new Vector3(0, rotationAngle, 0);
        }
        if ((move0 & (int)MoveGetButtons.Cross) != 0)
        {
            rotationAngle -= 30.0f;
            if (rotationAngle < 0)
                rotationAngle += 360;
            Rotator.transform.eulerAngles = new Vector3(0, rotationAngle, 0);
        }
        */
#endif
}
}