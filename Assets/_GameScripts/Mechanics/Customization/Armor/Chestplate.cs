using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Chestplate : MonoBehaviour {

    public MeshRenderer Mesh;
    public MeshRenderer[] ExtraMeshes;
    public ParticleSystem[] Particles;

    public void Setup(Color[] colors)
    {
        if(colors.Length > 0)
        {
            Material v = Mesh.material;
            if (v.HasProperty("_EmissionColor"))
            {
                v.SetColor("_EmissionColor", colors[0] * 2.5f);
                if (colors.Length > 1)
                {
                    Debug.Log("Had two colors");
                    v.color = colors[1];
                }
            }
            else
            {
                Debug.Log("No emmisssion color");
                v.color = colors[0];
            }
        }
        
    }

    public void ShadowsOnly()
    {
        GameObject editVis = (GameObject)Instantiate(Mesh.gameObject, Mesh.transform.parent, true);
        editVis.layer = 18;
        editVis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        Mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        for(int i=0; i<ExtraMeshes.Length; i++)
        {
            editVis = (GameObject)Instantiate(ExtraMeshes[i].gameObject, ExtraMeshes[i].transform.parent, true);
            editVis.layer = 18;
            editVis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            ExtraMeshes[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }
}
