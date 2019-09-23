using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    public GameObject _camera;

    private SteamVR_Controller.Device device = null;

    // Use this for initialization
    void Start () {
		
	}

    public void SetDeviceIndex(int index)
    {
        Debug.Log("fdjskhjh === " + index);
        device = SteamVR_Controller.Input(index);
    }

    // Update is called once per frame
    void Update () {
        if (device == null) return;
        Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad));
        if (touchpad.magnitude > 0.3f && device.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            Vector3 point = transform.rotation * Vector3.forward;
            point.y = 0;
            point.Normalize();
            Vector3 dir = new Vector3(touchpad.x, 0, touchpad.y);

            float ang = Vector3.Angle(new Vector3(0, 0, 1), dir);
            Vector3 cross = Vector3.Cross(new Vector3(0, 0, 1), dir);
            if (cross.y < 0) ang = -ang;

            Vector3 move = Quaternion.AngleAxis(ang, Vector3.up) * point;

            Vector3 pos = _camera.transform.position + (2 * Time.deltaTime * move);
            {

                _camera.transform.position = pos;
            }
        }
    }
}
