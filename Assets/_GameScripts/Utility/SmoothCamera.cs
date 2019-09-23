using UnityEngine;
using System.Collections;

public class SmoothCamera
    : MonoBehaviour
{
    public GameObject cameraTarget;
    public Camera cameraSelf;
    public bool enableSmooth = true;

    [Range(0.0f, 12.0f)]
    public float lerpPositionRate = 8.0f;
    [Range(1.0f, 12.0f)]
    public float lerpRotationRate = 4.0f;

    public void Start()
    {
        if (!cameraSelf)
            cameraSelf = GetComponent<Camera>();
    }

    void OnEnable()
    {
        cameraSelf.stereoTargetEye = StereoTargetEyeMask.None;
        cameraSelf.targetDisplay = 0;
    }

    public void FixedUpdate()
    {
        if (!cameraTarget)
            return;

        var posRate = lerpPositionRate;
        var rotRate = lerpRotationRate;

        if (enableSmooth)
        {
            transform.position = Vector3.Lerp(transform.position, cameraTarget.transform.position, Mathf.Clamp01(posRate * Time.fixedDeltaTime/Time.timeScale));
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraTarget.transform.rotation, Mathf.Clamp01(rotRate * Time.fixedDeltaTime/Time.timeScale));
        }
        else
        {
            transform.position = cameraTarget.transform.position;
            transform.rotation = cameraTarget.transform.rotation;
        }
    }
}
