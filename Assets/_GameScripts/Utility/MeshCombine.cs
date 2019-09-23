using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;

public class MeshCombine : MonoBehaviour {

    public int MaxCombineSize = 15;
    public PhysicMaterial PhysMat;

    public List<GameObject> Combined;
    public List<GameObject> Disabled;

    [AdvancedInspector.Inspect]
    public void Combine()
    {
        Revert();
        Combined = new List<GameObject>();
        Disabled = new List<GameObject>();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        Hashtable Meshes = new Hashtable();
        foreach(var v in meshFilters)
        {
            MeshRenderer r = v.GetComponent<MeshRenderer>();
            if(r != null)
            {
                if (Meshes.ContainsKey(r.sharedMaterial))
                {
                    ((List<MeshFilter>)Meshes[r.sharedMaterial]).Add(v);
                }
                else
                {
                    List<MeshFilter> lst = new List<MeshFilter>();
                    lst.Add(v);
                    Meshes.Add(r.sharedMaterial, lst);
                }
            }
        }
        List<Material> mats = new List<Material>();
        List<List<MeshFilter>> CombineLists = new List<List<MeshFilter>>();
        foreach(Material k in Meshes.Keys)
        {
            List<MeshFilter> mlist = (List<MeshFilter>)Meshes[k];
            int n = 0;
            for (int i = 0; i < mlist.Count; i += Mathf.Max(1, MaxCombineSize))
            {
                n++;
                CombineLists.Add(mlist.GetRange(i, Mathf.Min(MaxCombineSize, mlist.Count - i)));
                mats.Add(k);
            }
            Debug.Log(mlist.Count + " meshes using " + k.ToString() + ", split into " + n + " parts");
        }
        for(int j=0; j<CombineLists.Count; j++)
        {
            var l = CombineLists[j];
            Material m = mats[j];
            CombineInstance[] combine = new CombineInstance[l.Count];
            int i = 0;
            while (i < l.Count)
            {
                combine[i].mesh = l[i].sharedMesh;
                combine[i].transform = l[i].transform.localToWorldMatrix;
                l[i].gameObject.SetActive(false);
                Disabled.Add(l[i].gameObject);
                i++;
            }
            GameObject g = new GameObject();
            g.transform.SetParent(transform);
            g.transform.position = Vector3.zero;
            g.transform.localEulerAngles = Vector3.zero;
            g.transform.localScale = Vector3.one;
            g.name = m.ToString().Split('(')[0] + "- " + l.Count;
            Combined.Add(g);
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.sharedMaterial = m;
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/"+g.name.Replace(" ", "").Replace(".", "") + ".asset");
#endif
            mf.sharedMesh = mesh;
            MeshCollider mc = g.AddComponent<MeshCollider>();
            mc.material = PhysMat;
        }
    }

    [AdvancedInspector.Inspect]
    public void Revert()
    {
        if(Combined != null)
        {
            for (int i = 0; i < Combined.Count; i++)
            {
                DestroyImmediate(Combined[i]);
            }
        }
        if(Disabled != null)
        {
            foreach(var v in Disabled)
            {
                if(v != null)
                    v.SetActive(true);
            }
        }
        Disabled = new List<GameObject>();
        Combined = new List<GameObject>();
    }
}
