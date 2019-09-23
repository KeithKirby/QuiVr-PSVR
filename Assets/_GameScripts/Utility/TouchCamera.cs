// Just add this script to your camera. It doesn't need any configuration.
using CnControls;
using UnityEngine;

public class TouchCamera : MonoBehaviour {

    public float MoveSpeed = 10f;
    public float TurnSpeed = 25f;

    public Vector3 StartPos;
    public Vector3 StartRot;

    void Start()
    {
        transform.position = StartPos;
        transform.eulerAngles = StartRot;
    }

	void Update()
    {
        float h = CnInputManager.GetAxis("HorizontalMove");
        float v = CnInputManager.GetAxis("VerticalMove");
        Vector3 moveDir = new Vector3(h, 0, v);
        transform.position += transform.TransformDirection(moveDir) * Time.deltaTime * MoveSpeed;

        float hT = CnInputManager.GetAxis("HorizontalTurn");
        float vT = CnInputManager.GetAxis("VerticalTurn");
        //transform.Rotate(new Vector3(1, 0, 0), -vT * Time.deltaTime * TurnSpeed);
        transform.RotateAround(Vector3.up, hT * Time.deltaTime * 1.5f);
    }
}
