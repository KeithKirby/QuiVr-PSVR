using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetChildren : MonoBehaviour {

    public GameObject[] Prefabs;

    [AdvancedInspector.Inspect]
	public void Run()
    {
        foreach(var p in Prefabs)
        {
            MeshCollider col = p.GetComponent<MeshCollider>();
            if(col != null)
            {
                MeshFilter r3 = p.transform.GetChild(1).GetComponent<MeshFilter>();
                col.sharedMesh = r3.sharedMesh;
            }
        }
    }

}
