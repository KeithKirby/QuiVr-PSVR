using UnityEngine;
using System.Collections;

public class ProjectileLob : MonoBehaviour {

	public Vector3 target;

	public GameObject ProjectilePrefab;

	public float baseForce = 8;
	public float force;

	int iterations = 0;

    public void SetTarget(Vector3 t)
    {
        target = t;
    }

	public void Shoot()
	{
		force = baseForce;
        GameObject newProj = gameObject;
        if (ProjectilePrefab != null)
            newProj = (GameObject)Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
		float yRotation = GetYRotation();
		float barrelAngle = CalculateProjectileFiringSolution(force, (target.y-transform.position.y));
		Vector3 fVector = new Vector3(force, 0, 0); // fixed vector (image a 2d trajectory graph)          
		Vector3 rVector = Quaternion.Euler(0, yRotation, barrelAngle) * fVector; // transformed to 3d to point at target
		newProj.GetComponent<Rigidbody>().AddRelativeForce(rVector, ForceMode.Impulse); // add force on new axis
		iterations = 0;
		Destroy(newProj, 20f);
	}

	float CalculateProjectileFiringSolution(float vel, float alt)  
    {
	    float g = Mathf.Abs(Physics.gravity.y);
	               
	    Vector2 a = new Vector2(transform.position.x, transform.position.z);
	    Vector2 b = new Vector2(target.x, target.z);
	    float dis = Vector2.Distance(a, b);

	               
	    float dis2 = dis * dis;
	    float vel2 = vel * vel;
	    float vel4 = vel * vel * vel * vel;
		float calc = (vel4 - g * ((g * dis2) + (2 * alt * vel2)));
		if(calc <= 0 && iterations < 100)
		{
			force *= 1.1f;
			iterations++;
			return CalculateProjectileFiringSolution(force, alt);
		}
	    float num = vel2 + Mathf.Sqrt(calc);
	    float dom = g * dis;
	    float angle = Mathf.Atan(num / dom);
	 
	    return angle * Mathf.Rad2Deg;
    }

	float GetYRotation()
	{
       	Vector3 relativePos = transform.InverseTransformPoint(target);                              
	    float x = (relativePos.x);
	    float z = (relativePos.z);
	       
	    float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
	       
	    return angle-90;
	}
}
