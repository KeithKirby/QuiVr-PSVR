using UnityEngine;
using System.Collections;

public class TestWaypoints : MonoBehaviour {

    public Vector3 Center;
    public Vector3 Variation;

    Vector3 target = Vector3.zero;
    public float optSpeed;
    Health hscr;

	void Start ()
    {
        hscr = GetComponent<Health>();
        //bse.Initialize(new Vector3(Random.Range(-15f, 15f), 0, Random.Range(5f, 25f)));
        NewDest();
	}


    public void NewDest()
    {
        //bse.SetDestination(new Vector3(Random.Range(-15f, 15f), 0, Random.Range(5f, 25f)));
        target = GetPoint();
    }

    void Update()
    {
        /*
        if(bse == null && (hscr == null || !hscr.isDead()))
        {
            transform.LookAt(target);
            transform.position += optSpeed * Time.deltaTime * (target-transform.position).normalized;
            if(Vector3.Distance(transform.position, target) < 2f)
            {
                NewDest();
            }
        }
        */
    }

    public void SetupRange(Vector3 center, Vector3 variation)
    {
        Center = center;
        Variation = variation;
    }

    public Vector3 GetPoint()
    {
        return Center + new Vector3(Random.Range(-1 * Variation.x, Variation.x), Random.Range(-1 * Variation.y, Variation.y), Random.Range(-1 * Variation.z, Variation.z));
    }
}
