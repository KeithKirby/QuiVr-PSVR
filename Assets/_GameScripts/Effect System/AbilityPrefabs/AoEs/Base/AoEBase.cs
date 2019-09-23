using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class AoEBase : Effect {

    public Collider col;
    public ParticleSystem[] particles;
    public Renderer[] renderers;
    public UnityEvent OnEnd;

    public bool IgnoreGround;
    public bool InvertDir;
    public bool OverrideDir;
    public Vector3 OverrideVect;
    public float OverrideDuration;

    AoEEffect eft;

    void Awake()
    {
        eft = GetComponent<AoEEffect>();
    }

	void Start()
    {
        RaycastHit hit;
        int layer = 13;
        int mask = ~(1 << layer);
        if(!IgnoreGround && Physics.Raycast(new Ray(transform.position + (transform.TransformDirection(Vector3.up)*0.1f), transform.TransformDirection(Vector3.down)), out hit, 2f, mask))
        {
            Vector3 norm = hit.normal;
            transform.position += norm * 0.05f;
            int i = 1;
            if (InvertDir)
                i = -1;
            transform.rotation = Quaternion.LookRotation(norm*i);
            transform.Rotate(Vector3.left, 90);
        }
        if (OverrideDir)
            transform.eulerAngles = OverrideVect;
        if (col.gameObject.GetComponent<SphereCollider>() != null && eft != null)
            eft.SetRadius(col.GetComponent<SphereCollider>().radius*1.15f);
    }

    public override void Setup(bool dummy, int effectID, float value)
    {
        base.Setup(dummy, effectID, value);
        if(OverrideDuration > 0)
            Invoke("Clear", OverrideDuration);
        else if (baseEffect.randomType == RandomType.Duration)
            Invoke("Clear", baseEffect.VariableValue + val);
        else
            Invoke("Clear", baseEffect.StaticValue);
        if(eft != null)
            eft.Setup(dummy, effectID, value);
    }

    void Clear()
    {
        col.enabled = false;
        OnEnd.Invoke();
        foreach(var v in particles)
        {
            if(v != null)
            {
                var em = v.emission;
                em.enabled = false;
            }
        }
        foreach(var v in renderers)
        {
            if(v != null)
            {
                v.enabled = false;
            }
        }
        if(eft != null)
            eft.enabled = false;
        Destroy(gameObject, 5);
    }
}
