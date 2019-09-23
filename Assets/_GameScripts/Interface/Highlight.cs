using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {

    bool Setup;
    public GameObject HighlightObject;
    public Material HighlightMat;
    Renderer[] rends;

    void OnEnable()
    {
        if (HighlightObject == null)
        {
            CheckRenderer();
            AddMat();
        }
        else
            HighlightObject.SetActive(true);
    }

    void OnDisable()
    {
        if (HighlightObject == null)
        {
            CheckRenderer();
            RemoveMat();
        }
        else
            HighlightObject.SetActive(false);
    }

    public bool CheckRenderer()
    {
        if (HighlightObject != null)
            return false;
        else if (rends == null || rends.Length < 1 || rends[0] == null)
        {
            rends = GetComponentsInChildren<Renderer>();
            return true;
        }
        return false;
   
    }

    public void AddMat()
    {
        if (rends.Length > 0)
        {
            foreach (var r in rends)
            {
                if(r.GetType() == typeof(MeshRenderer) || r.GetType() == typeof(SkinnedMeshRenderer))
                {
                    List<Material> mats = GetMatList(r);
                    if (!mats.Contains(HighlightMat))
                        mats.Add(HighlightMat);
                    r.sharedMaterials = mats.ToArray();
                }

            }
        }
    }

    public void RemoveMat()
    {
        if(rends.Length > 0)
        {
            foreach (var r in rends)
            {
                if (r.GetType() == typeof(MeshRenderer) || r.GetType() == typeof(SkinnedMeshRenderer))
                {
                    List<Material> mats = GetMatList(r);
                    mats.Remove(HighlightMat);
                    r.sharedMaterials = mats.ToArray();
                }
            }
        }
    }

    List<Material> GetMatList(Renderer r)
    {
        List<Material> mats = new List<Material>();
        foreach(var v in r.sharedMaterials)
        {
            mats.Add(v);
        }
        return mats;
    }
}
