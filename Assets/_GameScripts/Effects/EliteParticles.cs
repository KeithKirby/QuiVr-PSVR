using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EliteParticles : MonoBehaviour {

    public GameObject ElitePrefab;
    List<PooledEffect> Effects;

    public void Start()
    {
        Effects = new List<PooledEffect>();
    }

    public int UseEffect(GameObject o)
    {
        for(int i=0; i<Effects.Count; i++)
        {
            PooledEffect e = Effects[i];
            if(!e.inUse)
            {
                UseEffect(e, o);
                Effects[i].inUse = true;
                return i;
            }
        }
        PooledEffect nE = NewEffect();
        UseEffect(nE, o);
        Effects[Effects.Count-1].inUse = true;
        return Effects.Count - 1;
    }

    public void UseEffect(PooledEffect e, GameObject o)
    {
       // PlaygroundParticlesC p = e.obj.GetComponent<PlaygroundParticlesC>();
        //p.skinnedWorldObject = PlaygroundParticlesC.NewSkinnedWorldObject(o.transform, 10);
        //p.Emit(true);
    }

    /*
    IEnumerator EndMainThread(PlaygroundParticlesC p)
    {
        yield return new WaitForSeconds(1f);
        p.forceSkinnedMeshUpdateOnMainThread = false;
        StopCoroutine("EndMainThread");
    }
    */

    public void ReleaseObject(int i)
    {
        /*PlaygroundParticlesC p = Effects[i].obj.GetComponent<PlaygroundParticlesC>();
        p.skinnedWorldObject.gameObject = null;
        p.Emit(false);
        Effects[i].inUse = false;
        */
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

public class PooledEffect
{
    public GameObject obj;
    public bool inUse;
}
