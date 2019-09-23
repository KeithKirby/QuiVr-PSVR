using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrigger : MonoBehaviour {

    Health hscr;
    List<Collider> cols;
    public GameObject ColliderHolder;

    void Awake()
    {
        hscr = GetComponentInParent<Health>();
        cols = new List<Collider>();
        Collider c = ColliderHolder.GetComponent<Collider>();
        if (c != null)
            cols.Add(c);
        foreach(var v in ColliderHolder.GetComponentsInChildren<Collider>())
        {
            cols.Add(v);
        }
        DisableColliders();
        hscr.OnDeath.AddListener(EnableColliders);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Arrow>() != null || c.GetComponent<PlayerHead>() != null)
        {
            EnableColliders();
            StopAllCoroutines();
            StartCoroutine("ResetColliders", 3f);
        }
    }

    IEnumerator ResetColliders(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(!hscr.isDead())
            DisableColliders();
    }

    public void EnableColliders(GameObject obj = null)
    {
        foreach(var v in cols)
        {
            if(v != null && !v.enabled)
                v.enabled = true;
        }
    }

    void DisableColliders()
    {
        foreach (var v in cols)
        {
            if (v != null && v.enabled && v.gameObject != ColliderHolder)
                v.enabled = false;
        }
    }
}
