using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
	private LineRenderer line;
	private RaycastHit hit;

	void Start ()
	{
		line = GetComponent<LineRenderer>();
	}

	// Keep the pointer aimed at whatever it's hitting, or else just keep going for 100 units
	void Update ()
	{
		if (Physics.Raycast(transform.position, transform.forward, out hit))
			line.SetPosition(1, Vector3.forward * hit.distance);
		else
			line.SetPosition(1, Vector3.forward * 100);
	}
}
