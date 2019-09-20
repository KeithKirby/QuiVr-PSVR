using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RockUtility : MonoBehaviour {

    RockValues vals;
    RockValues.Rock MyRock;
    MeshFilter r;
    MeshCollider col;
    MeshRenderer rend;

    LODGroup lod;
    bool setup;

    void Awake()
    {
        return;
        if(!Application.isPlaying)
        {
            Setup();
        }

    }

    void Setup()
    {
#if UNITY_EDITOR
        string[] Paths = UnityEditor.AssetDatabase.FindAssets("RockMeshValues");
        string path = "";
        if (Paths.Length > 0)
            path = UnityEditor.AssetDatabase.GUIDToAssetPath(Paths[0]);
        if (path.Length > 0)
            vals = (RockValues)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(RockValues));
        RefreshRock();
        setup = true;
#endif
    }

    [AdvancedInspector.Inspect]
    void RefreshRock()
    {
        r = GetComponent<MeshFilter>();
        lod = GetComponent<LODGroup>();
        if (vals != null && r != null)
        {
            foreach (var rock in vals.Rocks)
            {
                foreach (var v in rock.Meshes)
                {
                    if (r.sharedMesh == v.BaseMesh)
                    {
                        MyRock = rock;
                        return;
                    }
                }
            }
            return;
        }
    }

    public RockValues.RockSide Side;

    [AdvancedInspector.Inspect]
    public void SetMesh()
    {
        if (!setup)
            Setup();
        col = GetComponent<MeshCollider>();
        rend = GetComponent<MeshRenderer>();
        if (MyRock != null)
        {
            //Set Base Mesh
            Mesh m = null;
            int i = (int)Side;
            if(MyRock.Meshes.Length > i)
            {
                RockValues.RockMesh meshes = MyRock.Meshes[i];

                //Set Base
                m = meshes.BaseMesh;
                r.sharedMesh = m;
                if(col != null)
                    col.sharedMesh = m;

                //LOD
                if (meshes.LODS.Length > 0)
                {
                    if (lod == null)
                        lod = gameObject.AddComponent<LODGroup>();
                    MeshRenderer[] childs = gameObject.GetComponentsInChildren<MeshRenderer>();
                    for(int q=0; q<childs.Length;q++)
                    {
                        MeshRenderer r = childs[q];
                        if (r.gameObject.name.Contains("LOD"))
                            DestroyImmediate(r.gameObject);
                    }
                    LOD[] lods = new LOD[meshes.LODS.Length + 2];
                    lods[0] = new LOD(1, new Renderer[] { rend });
                    lods[1] = new LOD(.30f, new Renderer[] { rend });
                    for(int j=0; j<meshes.LODS.Length; j++)
                    {
                        Mesh msh = meshes.LODS[j];
                        GameObject n = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        n.transform.SetParent(transform);
                        n.transform.localPosition = Vector3.zero;
                        n.transform.localEulerAngles = Vector3.zero;
                        n.transform.localScale = Vector3.one;
                        n.layer = gameObject.layer;
                        n.name = "LOD_" + (j + 1);
                        Renderer[] renderers = new Renderer[1];
                        renderers[0] = n.GetComponent<Renderer>();
                        renderers[0].sharedMaterial = rend.sharedMaterial;
                        DestroyImmediate(n.GetComponent<SphereCollider>());
                        n.GetComponent<MeshFilter>().sharedMesh = msh;
                        MeshCollider mc = n.AddComponent<MeshCollider>();
                        mc.sharedMesh = msh;
                        mc.sharedMaterial = vals.RockMat;
                        if(j < meshes.LODS.Length-1)
                            lods[j+2] = new LOD(0.12f, renderers);
                        else
                            lods[j + 2] = new LOD(0, renderers);
                    }
                    lod.SetLODs(lods);
                    lod.RecalculateBounds();
                }

            }

        }
    }

}
