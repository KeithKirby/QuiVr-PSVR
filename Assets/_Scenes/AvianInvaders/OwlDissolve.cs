using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlDissolve : MonoBehaviour
{

    public float Delay = 3f;

    public void StartDissolve()
    {
        StartCoroutine("Dissolve");
    }

    IEnumerator Dissolve()
    {
        yield return new WaitForSeconds(Delay);
        GetComponentInChildren<BeautifulDissolves.Dissolve>().TriggerDissolve();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
