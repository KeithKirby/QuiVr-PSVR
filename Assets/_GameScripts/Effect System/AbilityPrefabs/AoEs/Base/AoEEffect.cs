using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AoEEffect : Effect {

    public float tickTime;
    public List<GameObject> Inside;
    float icd;

    public EffectClassification EffectType;

    public float radius = 30;

    void Awake()
    {
        Inside = new List<GameObject>();
        AoEBase b = GetComponent<AoEBase>();
        SphereCollider col = GetComponentInChildren<SphereCollider>();
        if (b != null && b.col != null)
            col = b.col as SphereCollider;
        if (col != null)
            radius = col.radius;
    }

    public void SetRadius(float r)
    {
        radius = r;
    }

    void OnTriggerEnter(Collider col)
    {
        GameObject c = col.gameObject;
        if(c.GetComponentInParent<PlayerHead>() != null && !Inside.Contains(c))
        {
            TickFirst(c);
            Inside.Add(c);
        }

        if (dummy)
            return;

        Immunities im = col.GetComponent<Immunities>();
        if (im == null)
            im = col.GetComponentInParent<Immunities>();
        if (im != null)
        {
            if (im.CheckImmune(EffectType))
            {
                CombatText.ShowText("Immune", c.transform.position + (Vector3.up*2), Color.white);
                return;
            }

        }
        Creature cr = col.GetComponent<Creature>();
        if(cr != null && pvpmanager.instance.PlayingPVP)
        {
            if (cr.Team == pvpmanager.instance.myTeam)
                return;
        }
        bool t = false;
        if(col.GetComponent<Health>() == null && col.GetComponentInParent<Health>() != null)
        {
            t = true;
            c = col.GetComponentInParent<Health>().gameObject;
        }
        else if(col.GetComponent<Health>() != null)
            t = true;
        if (c != null && (c.GetComponent<Gate>() != null || c.GetComponent<HomeBase>() != null))
            return;
        if(!Inside.Contains(c) && t && !c.GetComponent<Health>().isDead() && Vector3.Distance(transform.position, c.transform.position) < radius * 1.2f)
        {
            Inside.Add(c);
            TickFirst(c);
        }
    }

    void Update()
    {
        if (icd <= 0)
        {
            icd = Mathf.Max(tickTime, 0.1f);
            foreach(var v in Inside)
            {
                Tick(v);
            }
        }
        else
        {
            icd -= Time.deltaTime;
        }
        for(int i= Inside.Count-1; i>=0; i--)
        {
            if(Inside[i] == null || (Inside[i].GetComponent<Health>() != null && Inside[i].GetComponent<Health>().isDead()))
            {
                if(Inside[i] != null)
                    OnDieInside(Inside[i]);
                Inside.RemoveAt(i);
            }
            else if(Vector3.Distance(transform.position, Inside[i].transform.position) > radius*1.2f)
            {
                RemoveEffect(Inside[i]);
                Inside.RemoveAt(i);
            }
        }
    }

    public virtual void TickFirst(GameObject obj) {}

    public virtual void Tick(GameObject obj) {}

    public virtual void RemoveEffect(GameObject obj) {}

    public virtual void OnDieInside(GameObject obj) {}

    public virtual void OnDestroy()
    {
        foreach(var v in Inside)
        {
            RemoveEffect(v);
        }
        Inside = new List<GameObject>();
    }
}
