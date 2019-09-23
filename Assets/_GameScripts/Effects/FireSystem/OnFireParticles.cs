using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnFireParticles : MonoBehaviour {

    public GameObject ElitePrefab;
    List<PooledEffect> Effects;
    public static OnFireParticles instance;

    void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        Effects = new List<PooledEffect>();
        for (int i = 0; i < 5; i++)
        {
            NewEffect();
        }
    }

    public int UseEffect(GameObject o)
    {
        for (int i = 0; i < Effects.Count; i++)
        {
            PooledEffect e = Effects[i];
            if (!e.inUse)
            {
                UseEffect(e, o);
                Effects[i].inUse = true;
                return i;
            }
        }
        PooledEffect nE = NewEffect();
        UseEffect(nE, o);
        Effects[Effects.Count - 1].inUse = true;
        return Effects.Count - 1;
    }

    public void UseEffect(PooledEffect e, GameObject o)
    {
        ParticleSystem p = e.obj.GetComponent<ParticleSystem>();
        var sh = p.shape;
        sh.enabled = true;
        sh.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
        sh.skinnedMeshRenderer = o.GetComponent<SkinnedMeshRenderer>();
        var emmission = p.emission;
        emmission.enabled = true;
        p.Play();
    }

    public void ReleaseObject(int i)
    {
        ParticleSystem p = Effects[i].obj.GetComponent<ParticleSystem>();
        var emmission = p.emission;
        emmission.enabled = false;
        Effects[i].inUse = false;
        p.Stop();
    }

    PooledEffect NewEffect()
    {
        PooledEffect n = new PooledEffect();
        GameObject eff = (GameObject)Instantiate(ElitePrefab);
        eff.transform.SetParent(transform);
        n.obj = eff;
        n.inUse = false;
        Effects.Add(n);
        return n;
    }
}
