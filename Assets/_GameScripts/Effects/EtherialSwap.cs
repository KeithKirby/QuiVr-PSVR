using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EtherialSwap : MonoBehaviour {

    public Material etherMat;
    bool etherial;

    public List<MeshnMat> meshes;

    void Awake()
    {
        SetupMeshes();
    }

    void SetupMeshes()
    {
        if(meshes == null)
            meshes = new List<MeshnMat>();
        for(int i=meshes.Count-1; i >= 0; i--)
        {
            if (meshes[i].mr == null)
                meshes.RemoveAt(i);
        }
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            bool contains = false;
            foreach(var v in meshes)
            {
                if (v.SameRenderer(r))
                    contains = true;
            }
            if(r.GetType() != typeof(ParticleSystemRenderer) && r.GetType() != typeof(TrailRenderer) && !contains)
                meshes.Add(new MeshnMat(GetComponent<Renderer>()));
        }
        foreach (var v in GetComponentsInChildren<Renderer>())
        {
            bool contains = false;
            foreach (var x in meshes)
            {
                if (x.SameRenderer(v))
                    contains = true;
            }
            if (v.GetType() != typeof(ParticleSystemRenderer) && v.GetType() != typeof(TrailRenderer) && !contains)
                meshes.Add(new MeshnMat(v));
        }
    }

	public bool isEtherial()
    {
        return etherial;
    }
	
	public void MakeEtherial()
    {
        CheckMeshes();
        etherial = true;
        foreach (var v in meshes)
        {
            v.Replace(etherMat);
        }
        SetLayerRecursively(gameObject, 15, 0);
    }

    void CheckMeshes()
    {
        foreach(var v in meshes)
        {
            if(v.mr == null)
            {
                SetupMeshes();
                return;
            }
        }
    }

    public void RevertEtherial()
    {
        etherial = false;
        foreach (var v in meshes)
        {
            if(v.mr != null)
                v.Reset();
        }
        SetLayerRecursively(gameObject, 0, 15);
    }

    void SetLayerRecursively(GameObject obj, int newLayer, int old)
    {
        if (null == obj)
        {
            return;
        }
        if(obj.layer == old)
            obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer, old);
        }
    }
}

[System.Serializable]
public class MeshnMat
{
    public Renderer mr;
    public Material[] origMats;

    public MeshnMat(Renderer r)
    {
        mr = r;
        origMats = r.materials;
    }

    public bool SameRenderer(Renderer r)
    {
        return r == mr;
    }

    public void Reset()
    {
        mr.materials = origMats;
    }

    public void Replace(Material m)
    {
        Material[] arr = new Material[origMats.Length];
        for(var i=0; i<arr.Length; i++)
        {
            arr[i] = m;
        }
        if(mr != null)
            mr.materials = arr;
    }
}
