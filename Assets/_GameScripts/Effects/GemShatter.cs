using UnityEngine;
using System.Collections;

public class GemShatter : MonoBehaviour {

    Rigidbody[] rbs;
    public float startForce;
    public float startRotForce;

    public float DestroyTime = 4;
    public float WaitShrinkTime = 2;

    // Update is called once per frame
    IEnumerator Start () {
        yield return new WaitForSeconds(0.5f);
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach(var v in rbs)
        {
            v.isKinematic = false;
            v.velocity = new Vector3(Random.Range(-1f, 1) * startForce, Random.Range(-1f, 1) * startForce, Random.Range(-1f, 1) * startForce);
            v.angularVelocity = new Vector3(Random.Range(-1f, 1) * startRotForce, Random.Range(-1f, 1) * startRotForce, Random.Range(-1f, 1) * startRotForce);
        }
        
        StartCoroutine("ScaleDown");
        Destroy(gameObject, DestroyTime);
	}

    IEnumerator ScaleDown()
    {
        yield return new WaitForSeconds(WaitShrinkTime);
        foreach (var v in GetComponentsInChildren<BeautifulDissolves.Dissolve>())
        {
            v.TriggerDissolve();
        }
        /*
        float t = 1;
        while (t >= 0)
        {
            yield return true;
            t -= Time.deltaTime / 2f;
            if (t < 0)
                t = 0;
            foreach(var v in rbs)
            {
                if(v != null)
                    v.transform.localScale = Vector3.one * Mathf.Abs(t+0.00001f);
            }
        }
        */
    }

    
}
