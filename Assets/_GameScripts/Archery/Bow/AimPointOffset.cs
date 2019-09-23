using UnityEngine;
using System.Collections;

public class AimPointOffset : MonoBehaviour {

    public Vector3 LeftPos;
    public Vector3 RightPos;

    public void UpdatePos(float val)
    {
        transform.localPosition = Vector3.Lerp(LeftPos, RightPos, val);
    }
}
