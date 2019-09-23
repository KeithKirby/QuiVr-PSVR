using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour
{
	public bool local = true;
	public bool allowManual = false;
	public bool allowAutomatic = true;
	public bool lockZ = true;

	public float autoTimer = 2f;
	public Vector3 spinDirection = Vector3.up;
	public float speed = 50f;

	private float goAutomaticTime;
	private Vector3 autoSpinDirection;

	void Start()
	{
		autoSpinDirection = spinDirection;
	}

	void Update()
	{
		if(allowManual && (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f))
		{
			goAutomaticTime = Time.time + autoTimer;
		}

		if(Time.time > goAutomaticTime && allowAutomatic)
		{
			spinDirection = autoSpinDirection;
		}
		else
		{
			spinDirection = new Vector3(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"), 0);
		}
	}

	void FixedUpdate()
	{
		if(local)
			transform.Rotate(spinDirection, Time.deltaTime*speed);
		else
			transform.RotateAround(transform.position, spinDirection, Time.deltaTime*speed);

		if(!lockZ)
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
		else
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f));
	}
}
