using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImpactWithReset : MonoBehaviour {

    public float ResetTime;
    public UnityEvent OnReset;

    void Awake()
    {
        ArrowImpact imp = GetComponent<ArrowImpact>();
        if (imp != null)
            imp.OnHit.AddListener(delegate { OnHit(); });
    }

    void OnHit()
    {
        Reset();
    }

    void Reset()
    {
        StopAllCoroutines();
        StartCoroutine("ResetDelay");
    }

    IEnumerator ResetDelay()
    {
        yield return new WaitForSeconds(ResetTime);
        OnReset.Invoke();
    }
}
