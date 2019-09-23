using UnityEngine;
using System.Collections;

public class ChestFollow : MonoBehaviour {

    //Attach this script to a model of a chest that pivots (center point) is at the neck. Make this model a child of the player VR camera.
    public float rotationSpeed = 2;
    public float weight = 4;
    public float torsoHeight = 0.3f;
    [Range(0, 1)]
    public float leanWithHead = 0.5f;
    [Tooltip("For models that are facing the wrong diretion, like blender models. Set to 0,0,0 if not sure.")]
    public Vector3 modelRotationFix = new Vector3(90, 0, 0);//For models that are facing the wrong diretion, like blender models. Set to 0,0,0 if not sure.

    Quaternion lastRot;
    Vector3 weightPoint;
    public Transform mainCamera;

    public Vector3 down;

    void Start()
    {
        down = Vector3.down;
        lastRot = transform.rotation;
        weightPoint = mainCamera.position + down * torsoHeight;
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(lastRot,
            Quaternion.LookRotation(mainCamera.position - weightPoint, -mainCamera.forward) * Quaternion.Euler(modelRotationFix),
            rotationSpeed * Time.unscaledDeltaTime);

        lastRot = transform.rotation;

        weightPoint = Vector3.Lerp(weightPoint,
            mainCamera.position + Vector3.Lerp(mainCamera.up * -torsoHeight, down * torsoHeight, leanWithHead),
            weight * Time.unscaledDeltaTime);
    }

    public void ResetChestWeightTarget()
    {
        weightPoint = mainCamera.position + down * torsoHeight;
    }
}
