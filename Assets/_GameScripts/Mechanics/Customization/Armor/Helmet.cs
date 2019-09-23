using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Helmet : MonoBehaviour {

    public Renderer[] Meshes;
    public ParticleSystem[] Particles;
    public Material HoodSwap;

    static Material HoodBase;

	public void Setup(Color[] colors, bool dummy, SkinnedMeshRenderer hood)
    {
        List<Material> mats = new List<Material>();
        foreach(var v in Meshes)
        {
            if(v != null)
            {
                foreach (var m in v.materials)
                    mats.Add(m);
            }   
        }
        for(int i=0; i<colors.Length; i++)
        {
            if(i < mats.Count)
            {
                if (mats[i].HasProperty("_EmissionColor"))
                {
                    mats[i].SetColor("_EmissionColor", colors[i] * 2.3f);
                }
                else
                    mats[i].color = colors[i];
            }
        }
        if(colors.Length > 1 && Particles.Length > 0)
        {
            Particles[0].startColor = colors[1];
        }
        if (hood != null)
        {
            if (HoodBase == null)
                HoodBase = hood.sharedMaterial;

            if (HoodSwap != null)
                hood.material = HoodSwap;
            else
                hood.material = HoodBase;
        }
        if (dummy)
            gameObject.tag = "OtherPlayer";
        else
        {
            gameObject.tag = "Player";
            gameObject.layer = 17;
        }
            
    }

    public void ShadowsOnly()
    {
        foreach (var v in Meshes)
        {
            GameObject editVis = (GameObject)Instantiate(v.gameObject, v.transform.parent, true);
            editVis.layer = 18;
            editVis.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            v.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        foreach(var v in Particles)
        {
            v.gameObject.layer = 18;
        }
    }
}
