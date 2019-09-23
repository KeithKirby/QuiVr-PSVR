using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {

    [Range(0.0f, 60.0f)]
    public float lerpPositionRate = 8.0f;
    [Range(1.0f, 12.0f)]
    public float lerpRotationRate = 4.0f;

    public Transform Target;

    public Vector3 PosOffset;

    public void Update()
    {
        if (Target == null)
            return;

        var posRate = lerpPositionRate;
        var rotRate = lerpRotationRate;
        transform.position = Vector3.Lerp(transform.position, Target.transform.position+PosOffset, Mathf.Clamp01(posRate * Time.fixedDeltaTime / Time.timeScale));
        transform.rotation = Quaternion.Slerp(transform.rotation, Target.transform.rotation, Mathf.Clamp01(rotRate * Time.fixedDeltaTime / Time.timeScale));
    }
}
