using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrystals : MonoBehaviour {

    public List<GameObject> Crystals;

    void Awake()
    {
        Crystals = new List<GameObject>();
        foreach(var v in GetComponentsInChildren<ArrowImpact>())
        {
            Crystals.Add(v.gameObject);
            v.OnHit.AddListener(Impact);
        }
    }

    public void Impact(ArrowCollision o)
    {
        for(int i=0; i<Crystals.Count; i++)
        {
            if(Crystals[i] == o.hitObj)
            {
                Release(Crystals[i]);
                if (PhotonNetwork.inRoom)
                    GetComponentInParent<NetworkCreature>().InvokeIntEvent(i);
                Crystals.RemoveAt(i);
                return;
            }
        }
    }

    public void ReleaseNet(int id)
    {
        if (Crystals.Count <= id)
            id = Crystals.Count - 1;
        if(id >= 0)
        {
            Release(Crystals[id]);
            Crystals.RemoveAt(id);
        }       
    }

    public void ReleaseAll()
    {
        foreach(var v in Crystals)
        {
            if (v != null)
                Release(v);
        }
        Crystals = new List<GameObject>();
    }

    void Release(GameObject c)
    {
        c.transform.parent = null;
        Rigidbody rb = c.AddComponent<Rigidbody>();
        rb.AddExplosionForce(250, c.transform.position, 1f);
        BeautifulDissolves.Dissolve d = c.GetComponent<BeautifulDissolves.Dissolve>();
        if (d != null)
            d.Invoke("TriggerDissolve", 3f);
        Destroy(c, 5f);
    }
}
