using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowEffects : MonoBehaviour {

    public static bool EffectsDisabled;

    public GameObject TipHolder;
    public GameObject ShaftHolder;

    public ArrowPrefab CurrentArrow;
    public ParticleSystem PowerupEffect;
    public ParticleSystem ChargeEffect;
    ParticleSystem.EmissionModule chargeEmiss;
    public ParticleSystem[] AimTiers;
    ParticleSystem.EmissionModule aimEmiss;
    public AnimationCurve AimHapticCurve;
    AlphaFade fade;

    public List<EffectInstance> effects;

    void Awake()
    {
        effects = new List<EffectInstance>();
        fade = PowerupEffect.GetComponentInChildren<AlphaFade>();
        aimEmiss = PowerupEffect.emission;
        chargeEmiss = ChargeEffect.emission;
        ShaftAdded = new List<GameObject>();
        TipAdded = new List<GameObject>();
    }

    #region Powerup

    int aim;
    public void SetAim(int aimed)
    {
        aim = aimed;
        if (aimed > 0)
        {
            //aimEmiss.rateOverTime = 1 + aimed;
            AudioClip c = null;
            if(!PowerupEffect.isPlaying)
                PowerupEffect.Play();
            if(aimed == 1)
                c = PowerupEffect.GetComponentInParent<PlayRandomClip>().Play();
            if (BowAim.instance.stringActions != null)
            {
                StopCoroutine("CurveHapticsRoutine");
                StartCoroutine("CurveHapticsRoutine");
            }               
            fade.ChangeBrightness(1 + 0.2f*aimed);
            if(aimed == 1)
                fade.FadeIn();
        }
        else
        {
            StopAllCoroutines();
            PowerupEffect.Stop();
            fade.FadeOut();
        }
        /*
        for (int i = 0; i < AimTiers.Length; i++)
        {
            ParticleSystem p = AimTiers[i];
            if (p != null)
            {
                if (aimed > i)
                    p.Play();
                else if(p.isPlaying)
                    p.Stop();
            }
        }
        */
    }

    public void SetAimProgress(float v)
    {
        chargeEmiss.rateOverTime = v * 100f;
    }

    private IEnumerator CurveHapticsRoutine()
    {
        float t = 0;
        while (t < 1)
        {
            ushort hapticPulseStrength = (ushort)(ushort.MaxValue * AimHapticCurve.Evaluate(t) * (0.008f + (0.001f*(aim-1))));
            if(hapticPulseStrength > 10)
                BowAim.instance.stringActions.TriggerHapticPulse(hapticPulseStrength);
            t += Time.deltaTime;
            yield return true;
        }
    }

    #endregion

    public void SetEffects(EffectInstance[] efts, bool sync = true)
    {
        effects = new List<EffectInstance>();
        foreach (var v in efts)
        {
            bool has = false;
            foreach(var e in effects)
            {
                if (e.EffectID == v.EffectID)
                    has = true;
            }
            if (!has)
            {
                effects.Add(v);
                DisplayEffect(v.EffectID, sync);
            }    
        }
    }

    public void SetEffects(string efts)
    {
        EffectInstance[] e = EffectInstance.GetList(efts);
        SetEffects(e);
    }

    public void AddEffect(int id, float val, bool sync=true, bool fullSync = false)
    {
        foreach(var v in effects)
        {
            if (v.EffectID == id)
                return;
        }
        effects.Add(new EffectInstance(id, val));
        ItemEffect e = ItemDatabase.GetEffect(id);
        if (id == -5) //Replace with Aimed Shot ability
        {
            GetComponent<Arrow>().Powerup();
            return;
        }
        DisplayEffect(id, sync && !fullSync);
        if(fullSync) // For arrows in hand
            DoFullSync(id, val);
    }

    void AddEffect(EffectInstance eft, bool sync=true, bool fullSync = false)
    {
        AddEffect(eft.EffectID, eft.EffectValue, sync, fullSync);
    }

    public bool HasEffect(int id)
    {
        foreach(var v in effects)
        {
            if (v.EffectID == id)
                return true;
        }
        return false;
    }

    public void ApplyDrawEffects()
    {
        foreach (var v in effects)
        {
            ItemEffect e = ItemDatabase.GetEffect(v.EffectID);
            if(e.activation == ActivationType.Draw)
            {
                if (e.EffectID == 12) //Firebrand
                    GetComponentInChildren<ArrowFire>().CatchFire();
            }
        }
    }

    public void ApplyArrowMods()
    {
        foreach (var v in effects)
        {
            ItemEffect e = ItemDatabase.GetEffect(v.EffectID);
            if (e.activation == ActivationType.ArrowMod)
            {
                if(e.EffectID == 25) //Pack Mentality
                {
                    float add = 0;
                    if(PhotonNetwork.inRoom)
                    {
                        foreach (var p in PlayerSync.Others)
                        {
                            EffectInstance ei = p.HasNetworkEffect(e.EffectID);
                            if (ei != null)
                                add += ei.EffectValue;
                        }
                    }
                    GetComponent<Arrow>().BonusDamage += (int)add;
                }                    
            }
        }
    }

    public void ApplyHitEffects(Vector3 pos, ArrowCollision imp)
    {
        foreach (var v in effects)
        {
            ItemEffect e = ItemDatabase.GetEffect(v.EffectID);
            if (e.activation == ActivationType.HitAnything)
            {
                if (e.EffectActivation != null)
                {
                    AbilityManager.instance.UseAbility(v.EffectID, pos, transform.forward, false, v.EffectValue);
                }
            }
            if(e.activation == ActivationType.HitEnemy || (e.activation == ActivationType.EnemyCritical && imp.Critical()))
            {
                bool canHit = false;
                Creature c = imp.hitObj.GetComponent<Creature>();
                if (c == null)
                    c = imp.hitObj.GetComponentInParent<Creature>();
                if (c != null && (!c.isDead() || e.activation == ActivationType.EnemyCritical))
                    canHit = true;
                if(!canHit)
                    canHit = imp.hitObj.GetComponent<TargetDummy>() != null || imp.hitObj.GetComponentInParent<TargetDummy>() != null;
                if (e.EffectActivation != null && canHit)
                {
                    AbilityManager.instance.UseAbility(v.EffectID, pos, transform.forward, false, v.EffectValue);
                }
            }
        }
    }

    public void ApplyFiredEffects(Vector3 pos)
    {
        foreach (var v in effects)
        {
            ItemEffect e = ItemDatabase.GetEffect(v.EffectID);
            if (e.activation == ActivationType.Fire)
            {
                if (e.EffectActivation != null)
                {
                    AbilityManager.instance.UseAbility(v.EffectID, pos, transform.forward, false, v.EffectValue);
                }
            }
        }
    }

    public bool ApplyMissEffects(Vector3 pos)
    {
        bool usedMiss = false;
        foreach(var v in effects)
        {
            ItemEffect e = ItemDatabase.GetEffect(v.EffectID);
            if(e.activation == ActivationType.HitAnything || e.activation == ActivationType.HitObject)
            {
                bool shoulduse = true;
                if(e.activation == ActivationType.HitObject && e.type == EffectType.MissChance)
                {
                    float chance = Random.Range(0, 100f);
                    float req = (e.VariableValue + v.EffectValue);
                    if (e.randomType != RandomType.RandomChance)
                        req = e.StaticValue;
                    if (chance > req)
                        shoulduse = false;
                }
                if (e.EffectActivation != null && shoulduse)
                {
                    AbilityManager.instance.UseAbility(v.EffectID, pos, transform.forward, false, v.EffectValue);
                    usedMiss = true;
                }
            }
        }
        return usedMiss;
    }

    public string GetEffects()
    {
        return EffectInstance.StringList(effects.ToArray());
    }

    public void DisplayEffect(int eid, bool sync=true)
    {
        if(ItemDatabase.v != null)
        {
            if(sync && PhotonNetwork.inRoom && BowAim.instance.GetArrow() == gameObject)
            {
                //Add effect on knocked arrow
                PlayerSync.myInstance.ArrowEffect(new int[] { eid });
            }
            ItemEffect e = ItemDatabase.GetEffect(eid);
            ApplyShaftEffect(e.Display.ShaftEffect);
            ApplyTipEffect(e.Display.TipEffect);
        }
    }

    List<GameObject> ShaftAdded;
    List<GameObject> TipAdded;
    void ApplyShaftEffect(GameObject obj)
    {
        if (obj == null || ShaftAdded.Contains(obj))
            return;
        ShaftAdded.Add(obj);
        GameObject ne = (GameObject)Instantiate(obj, ShaftHolder.transform);
        ne.transform.localPosition = Vector3.zero;
        ne.transform.localEulerAngles = Vector3.zero;
        ne.transform.localScale = Vector3.one;
        var MM = ne.GetComponentInChildren<WFX_MeshMaterialEffect>();
        if(MM != null)
        {
            ArrowPrefab p = CurrentArrow;
            if (p != null && p.Shaft != null)
            {
                if (MM.IsFirstMaterial)
                {
                    p.Shaft.material = MM.mat;
                } 
                else
                {
                    List<Material> mats = new List<Material>();
                    foreach(var v in p.Shaft.materials)
                    {
                        mats.Add(v);
                    }
                    mats.Add(MM.mat);
                    p.Shaft.materials = mats.ToArray();
                }
            }
        }
    }

    void ApplyTipEffect(GameObject obj)
    {
        if (obj == null || TipAdded.Contains(obj))
            return;
        TipAdded.Add(obj);
        GameObject ne = (GameObject)Instantiate(obj, TipHolder.transform);
        ne.transform.localPosition = Vector3.zero;
        ne.transform.localEulerAngles = Vector3.zero;
        ne.transform.localScale = Vector3.one;
        var MM = ne.GetComponentInChildren<WFX_MeshMaterialEffect>();
        if (MM != null)
        {
            ArrowPrefab p = CurrentArrow;
            if (p != null && p.Tip != null)
            {
                if (MM.IsFirstMaterial)
                    p.Tip.materials[0] = MM.mat;
                else
                {
                    List<Material> mats = new List<Material>();
                    foreach (var v in p.Tip.materials)
                    {
                        mats.Add(v);
                    }
                    mats.Add(MM.mat);
                    p.Tip.materials = mats.ToArray();
                }
            }
        }
    }

    public void SetNocked()
    {
        if(PhotonNetwork.inRoom)
        {
            List<int> ids = new List<int>();
            foreach(var v in effects)
            {
                ids.Add(v.EffectID);
            }
            if(ids.Count > 0)
            {
                PlayerSync.myInstance.ArrowEffect(ids.ToArray());
            }
        }
    }

    void DoFullSync(int id, float val)
    {
        if(PhotonNetwork.inRoom && GetComponentInParent<NetworkArrowHolder>() != null)
        {
            GetComponentInParent<NetworkArrowHolder>().AddEffect(id, val);
        }
    }
}
