using UnityEngine;
using System.Collections;

public class QuiverPrefab : MonoBehaviour {

    public MeshRenderer Detail;
	
	// Update is called once per frame
	public void Setup(Color[] c) {
	    if(c.Length > 0 && Detail != null)
        {
            if(Detail.material.HasProperty("_EmissionColor"))
                Detail.material.SetColor("_EmissionColor", c[0]*3f);
        }
	}
}
