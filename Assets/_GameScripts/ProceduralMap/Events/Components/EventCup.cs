using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BeautifulDissolves;

public class EventCup : MonoBehaviour {

    [Header("Components")]
    public GameObject Holder;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject CoreDisplay;
    public GameObject BreakObj;
    public GameObject Explosion;
    public NavMeshAgent Agent;
    public GameObject ShieldDisplay;

    [HideInInspector]
    public CupsEvent Owner;
    [Header("Runtime")]
    public bool HasCore;
    public bool CanShoot;
    public bool Exploded;
    bool TopDissolved;

    RFX4_ShaderFloatCurve shaderCurve;

    void Awake()
    {
        if(ShieldDisplay != null)
            shaderCurve = ShieldDisplay.GetComponent<RFX4_ShaderFloatCurve>();
    }

    public void ShowTop()
    {
        Top.GetComponent<Dissolve>().TriggerReverseDissolve();
        TopDissolved = false;
    }

    public void HideTop()
    {
        Top.GetComponent<Dissolve>().TriggerDissolve();
        TopDissolved = true;
    }

    public void MoveTo(Vector3 pt, float dur)
    {
        if(Agent.isOnNavMesh)
        {
            Agent.SetDestination(pt);
            Agent.speed = Vector3.Distance(transform.position, pt) * dur;
        }
    }

    public void ToggleShield(bool val)
    {
        if(shaderCurve != null)
        {
            if (val)
                shaderCurve.Play();
            else
                shaderCurve.Reverse();
        }
    }

    [AdvancedInspector.Inspect]
    public void Explode()
    {
        if (Exploded)
            return;
        Exploded = true;
        Holder.SetActive(false);
        GameObject beft = Instantiate(BreakObj, CoreDisplay.transform.position, Quaternion.identity);
        beft.SetActive(true);
        if (HasCore)
        {
            GameObject expl = Instantiate(Explosion, CoreDisplay.transform.position, Quaternion.identity);
            RFX4_EffectSettingColor col = expl.AddComponent<RFX4_EffectSettingColor>();
            col.Color = CoreDisplay.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_TintColor");
            expl.SetActive(true);
        }
    }

    public void TryShoot(ArrowCollision e)
    {
        if(e.isMine && CanShoot && !Exploded)
        {
            Owner.BreakBall(this);
        }
    }

    public void Regenerate(bool withTop)
    {
        Holder.SetActive(true);
        CoreDisplay.SetActive(false);
        if(Exploded)
        {
            if (withTop)
                ShowTop();
            else
                Top.GetComponent<Dissolve>().SetDissolveAmount(1);
            Bottom.GetComponent<Dissolve>().TriggerReverseDissolve();
        }
        else
        {
            if (!withTop && !TopDissolved)
                HideTop();
        }
        Exploded = false;
    }
}
