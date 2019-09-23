using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DummyArrow : MonoBehaviour {

    public Transform follow;
    public GameObject display;

    public GameObject TipEffects;
    public GameObject ShaftEffects;

    List<GameObject> curTipEffects;
    List<GameObject> curShaftEffects;

    Material[] BaseShaft;

    public ParticleSystem PowerupEffect;
    public ParticleSystem[] AimTiers;
    ParticleSystem.EmissionModule aimEmiss;
    AlphaFade fade;

    public bool followRotation;

    void Awake()
    {
        if(PowerupEffect != null)
        {
            fade = PowerupEffect.GetComponentInChildren<AlphaFade>();
            aimEmiss = PowerupEffect.emission;
        }
        curTipEffects = new List<GameObject>();
        curShaftEffects = new List<GameObject>();
    }

    void Update()
    {
        if(follow == null && display.GetComponentInChildren<BowPrefab>() != null)
        {
            follow = display.GetComponentInChildren<BowPrefab>().nockPoint;
        }
        else
        {
            transform.position = follow.position;
            if(followRotation)
                transform.rotation = follow.rotation;
        }
    }

    void OnDisable()
    {
        if (curTipEffects != null)
        {
            for (int i = curTipEffects.Count - 1; i >= 0; i--)
            {
                Destroy(curTipEffects[i]);
            }
            curTipEffects = new List<GameObject>();
        }
        if (curShaftEffects != null)
        {
            for (int i = curShaftEffects.Count - 1; i >= 0; i--)
            {
                Destroy(curShaftEffects[i]);
            }
            curShaftEffects = new List<GameObject>();
        }
        ArrowPrefab p = GetComponentInChildren<ArrowPrefab>();
        if (p != null && p.Shaft != null && BaseShaft != null)
        {
            p.Shaft.materials = BaseShaft;
        }
    }

    public void DisplayEffect(int eid)
    {
        if(curTipEffects == null)
        {
            curShaftEffects = new List<GameObject>();
            curTipEffects = new List<GameObject>();
        }
        if(ItemDatabase.v != null)
        {
            ItemEffect e = ItemDatabase.a.GetEffect(eid);
            ApplyShaftEffect(e.Display.ShaftEffect);
            ApplyTipEffect(e.Display.TipEffect);
        }
    }

    void ApplyShaftEffect(GameObject obj)
    {
        if (obj == null)
            return;
        GameObject ne = (GameObject)Instantiate(obj, ShaftEffects.transform);
        ne.transform.localPosition = Vector3.zero;
        ne.transform.localEulerAngles = Vector3.zero;
        ne.transform.localScale = Vector3.one;
        curShaftEffects.Add(ne);
        var MM = ne.GetComponentInChildren<WFX_MeshMaterialEffect>();
        if (MM != null)
        {          
            ArrowPrefab p = GetComponentInChildren<ArrowPrefab>();
            if (p != null && p.Shaft != null)
            {
                BaseShaft = p.Shaft.materials;
                if (MM.IsFirstMaterial)
                {
                    p.Shaft.material = MM.mat;
                }
                else
                {
                    List<Material> mats = new List<Material>();
                    foreach (var v in p.Shaft.materials)
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
        if (obj == null)
            return;
        GameObject ne = (GameObject)Instantiate(obj, TipEffects.transform);
        ne.transform.localPosition = Vector3.zero;
        ne.transform.localEulerAngles = Vector3.zero;
        ne.transform.localScale = Vector3.one;
        curTipEffects.Add(ne);
    }

    public void ClearEffects()
    {
        if(curTipEffects != null)
        {
            for (int i = 0; i < curTipEffects.Count; i++)
            {
                if (curTipEffects[i] != null)
                    Destroy(curTipEffects[i]);
            }
        }
        if(curShaftEffects != null)
        {
            for (int i = 0; i < curShaftEffects.Count; i++)
            {
                if (curShaftEffects[i] != null)
                    Destroy(curShaftEffects[i]);
            }
        }
        curTipEffects = new List<GameObject>();
        curShaftEffects = new List<GameObject>();
    }

    int aim;
    public void SetAim(int aimed)
    {
        aim = aimed;
        if (aimed > 0)
        {
            aimEmiss.rateOverTime = 1 + aimed;
            AudioClip c = null;
            if (!PowerupEffect.isPlaying)
                PowerupEffect.Play();
            if (aimed == 1)
                c = PowerupEffect.GetComponentInParent<PlayRandomClip>().Play();
            fade.ChangeBrightness(1 + 0.2f * aimed);
            if (aimed == 1)
                fade.FadeInImmediate();
        }
        else
        {
            StopAllCoroutines();
            PowerupEffect.Stop();
            fade.FadeOutImmediate();
        }
        for (int i = 0; i < AimTiers.Length; i++)
        {
            ParticleSystem p = AimTiers[i];
            if (p != null)
            {
                if (aimed > i)
                    p.Play();
                else if (p.isPlaying)
                    p.Stop();
            }
        }
    }
}
