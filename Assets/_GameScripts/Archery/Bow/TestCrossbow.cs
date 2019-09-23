using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCrossbow : MonoBehaviour {

    public Transform launchpoint;
    public GameObject ArrowPrefab;
    [Range(1,100)]
    public float LaunchSpeed;

    public bool LaunchAuto;
    [Range(0.05f, 5f)]
    public float AutoCD = 0.3f;
    public int aim;
    float cd;

    [Range(0,30)]
    public float ArrowDamage = 0f;

    public bool ClearEffectOnShoot = true;

    public List<Vector2> AppliedEffects;

    void Awake()
    {
        AppliedEffects = new List<Vector2>();
    }

    void Update()
    {
        cd += Time.deltaTime;
        if(cd > AutoCD && LaunchAuto)
        {
            cd = 0;
            Launch();
        }
    }

    [AdvancedInspector.Inspect]
    public void Launch()
    {
        GameObject arrowHold = (GameObject)Instantiate(ArrowPrefab);
        GameObject arrow = arrowHold.GetComponentInChildren<Arrow>().gameObject;
        Collider[] arrowCols = arrowHold.GetComponentsInChildren<Collider>();
        Collider[] BowCols = GetComponentsInChildren<Collider>();
        foreach (var c in arrowCols)
        {
            c.enabled = true;
            foreach (var C in BowCols)
            {
                Physics.IgnoreCollision(c, C);
            }
        }
        arrow.transform.SetParent(null);
        Destroy(arrowHold);
        arrow.transform.rotation = launchpoint.rotation;
        arrow.transform.position = launchpoint.position;
        arrow.GetComponent<ArrowPhysics>().enabled = true;
        arrow.GetComponent<Rigidbody>().isKinematic = false;
        arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward*LaunchSpeed;
        ApplyEffects(arrow);
        string disp = "";
        if (Armory.currentOutfit.Arrow != null)
            disp = Armory.currentOutfit.Arrow.ToString();
        if (GetComponentInParent<PlayerSync>() != null)
        {
            bool onFire = arrow.GetComponentInChildren<ArrowFire>().OnFire();
            EffectInstance[] effects = arrow.GetComponent<ArrowEffects>().effects.ToArray();
            GetComponentInParent<PlayerSync>().ShootArrow(arrow.transform.position, arrow.transform.rotation, arrow.GetComponent<Rigidbody>().velocity, onFire, disp, effects);
        }

        Arrow a = arrow.GetComponent<Arrow>();
        a.enabled = true;
        a.isMine = true;
        a.Init();
        a.Damage = ArrowDamage;
        a.SetAimed(aim);
        a.inFlight = true;
        a.Fired();
    }

    void ApplyEffects(GameObject obj)
    {
        ArrowEffects eft = obj.GetComponent<ArrowEffects>();
        if(eft != null && AppliedEffects.Count > 0)
        {
            List<EffectInstance> effects = new List<EffectInstance>();
            foreach(Vector2 v in AppliedEffects)
            {
                effects.Add(new EffectInstance((int)v.x, v.y));
            }
            eft.SetEffects(effects.ToArray(), false);
        }
        if (ClearEffectOnShoot)
            AppliedEffects = new List<Vector2>();
    }
}
