using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
public class ArrowImpact : MonoBehaviour {

    public ArrowEvent OnHit;
    public ArrowEvent OnAimedHit;
    public bool DestroyArrow;
    public bool RequiresMine;
    public bool IgnoreParent;
    public bool Inactive;

	public void OnImpact(ArrowCollision e)
    {
        if (Inactive)
            return;
        Immunities immune = GetComponent<Immunities>();
        if(immune != null && immune.CheckImmune(EffectClassification.Arrow))
        {
            CombatText.ShowText("Immune", e.impactPos + (Vector3.up * 1.5f), Color.white);
            if (DestroyArrow)
                Destroy(e.arrowObj);
            return;
        }
        Health h = GetComponent<Health>();
        if (h != null && h.invincible)
            return;
        if (h != null && e.isMine)
        {
            if (h.GetComponent<Creature>() != null)
                h.gameObject.GetComponent<Creature>().SetTagged();

            float damage = e.ArrowDamage;
            string tag = e.hitObj.tag;
            if (tag == "LowDamage")
                damage *= 0.5f;
            else if (tag == "HighDamage")
                damage *= 1.5f;
            else if (e.Critical())
                damage *= 2f;
            damage += e.BonusDamage;

            h.takeArrowDamage(damage, e);
        }
        if(e.onFire && (GetComponent<Flammable>() != null || GetComponentInParent<Flammable>() != null) && e.isMine)
        {
            Flammable f = GetComponent<Flammable>();
            if (f == null)
                f = GetComponentInParent<Flammable>();
            f.CatchFire();
        }
        if (GetComponent<ComboImpact>() != null)
            GetComponent<ComboImpact>().DoHit(e);
        if (e.isMine || !RequiresMine)
        {
            OnHit.Invoke(e);
            if (e.aimed > 0)
                OnAimedHit.Invoke(e);
        }
        if (DestroyArrow)
            Destroy(e.arrowObj);
    }

    [AdvancedInspector.Inspect]
    public void DoImpact()
    {
        ArrowCollision c = new ArrowCollision(transform.position, 20, transform.position, 1, gameObject, false, Vector3.up, true, false, 0, null, new EffectInstance[] { });
        OnImpact(c);
    }

    [AdvancedInspector.Inspect]
    public void DoAimedImpact()
    {
        ArrowCollision c = new ArrowCollision(transform.position, 20, transform.position, 1, gameObject, false, Vector3.up, true, false, 0, null, new EffectInstance[] { });
        c.aimed = 1;
        OnImpact(c);
    }
}

[System.Serializable]
public class ArrowEvent : UnityEvent<ArrowCollision> { }

[System.Serializable]
public class ArrowCollision
{
    public Vector3 firePos;
    public Vector3 impactPos;
    public float velocityMag;
    public float ArrowDamage;
    public float BonusDamage = 0;
    public GameObject hitObj;
    public bool onFire;
    public Vector3 ImpactNormal;
    public bool isMine;
    public bool didDeflect;
    public int deflectCount;
    public GameObject arrowObj;
    public EffectInstance[] Effects;
    public Vector3 ImpactVelocity = Vector3.zero;
    public int aimed = 0;

    public ArrowCollision(Vector3 fpos, float dmg, Vector3 iPos, float vel, GameObject obj, bool fire, Vector3 inorm, bool mine, bool deflected, int deflCount, GameObject arrobj, EffectInstance[] effects)
    {
        firePos = fpos;
        impactPos = iPos;
        ArrowDamage = dmg;
        velocityMag = vel;
        hitObj = obj;
        onFire = fire;
        ImpactNormal = inorm;
        isMine = mine;
        didDeflect = deflected;
        deflectCount = deflCount;
        arrowObj = arrobj;
        Effects = effects;
    }

    public float Distance()
    {
        return Vector3.Distance(firePos, impactPos);
    }

    public float DistanceScale()
    {
        float dist = Distance();
        float logDist = Mathf.Clamp(Mathf.Log(dist, 2f), 0.7f, 15f);
        if (dist > 100)
            logDist += 2f;
        return logDist;
    }

    public bool Critical()
    {
        return hitObj.tag == "DoubleDamage" || HasEffect(10);
    }

    public bool HasEffect(int id)
    {
        foreach(var v in Effects)
        {
            if (id == v.EffectID)
                return true;
        }
        return false;
    }
}