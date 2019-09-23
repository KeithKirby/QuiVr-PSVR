using UnityEngine;
using System.Collections;
using VRTK;
public class CalibrateSpace : MonoBehaviour {

    public string Key;
    public Transform plrSpace;

    void Start()
    {
        Vector3 offset = Settings.GetV3(Key);
        offset = Clamp(offset);
        plrSpace.transform.localPosition = offset;
    }

    public void ResetCalibration()
    {
        plrSpace.transform.localPosition = Vector3.zero;
        Settings.Set(Key, Vector3.zero);
    }

    public void Calibrate()
    {
        GameObject cLeft = VRTK_DeviceFinder.GetControllerLeftHand();
        GameObject cRight = VRTK_DeviceFinder.GetControllerRightHand();
        if(cLeft == null || cRight == null)
        {
            Note n = new Note();
            n.title = "Error";
            n.body = "Can't calibrate without both controllers on";
            Notification.Notify(n);
            return;
        }
        Transform Bottom = cLeft.transform;
        if (cRight.transform.position.y < cLeft.transform.position.y)
            Bottom = cRight.transform;
        Vector3 center = Bottom.transform.position;
        Vector3 offset = center - plrSpace.transform.position;
        offset = Clamp(offset);
        Settings.Set(Key, offset);
        plrSpace.transform.localPosition = offset;
    }

    Vector3 Clamp(Vector3 inpt)
    {
        return new Vector3(Mathf.Clamp(inpt.x, -2f, 2f), Mathf.Clamp(inpt.y, -4f, 4f), Mathf.Clamp(inpt.z, -2f, 2f));
    }
}
