using UnityEngine;
using System.Collections;

public class GlovePrefab : MonoBehaviour {

    public SkinnedMeshRenderer Fingers;
    public SkinnedMeshRenderer Glove;
    public Transform WristObj;
    public GameObject[] playerObjects;
    public bool dummy;
    
    public void Setup(Color[] c, bool isDummy, LayerMask gloveMask)
    {
        dummy = isDummy;
        if (Fingers != null)
        {
            Fingers.gameObject.layer = LayerUtilities.ToLayer(gloveMask.value);
        }
        if (c.Length > 0 && Fingers != null)
        {
            Fingers.material.color = c[0];
        }

        if(Glove != null)
        {
            Glove.gameObject.layer = LayerUtilities.ToLayer(gloveMask.value);
        }

        if (c.Length > 1 && Glove != null)
        {
            if (Glove.material.HasProperty("_EmissionColor"))
            {
                Glove.material.SetColor("_EmissionColor", c[1] * 2.5f);
                if (c.Length > 2)
                    Glove.material.color = c[2];
            }
            else
                Glove.material.color = c[1];            
        }
        GloveHolder gh = GetComponentInParent<GloveHolder>();
        if(WristObj != null && gh != null)
        {
            var lh = WristObj.GetComponent<LookAt>();
            if (lh != null)
                lh.target = gh.WristTarget;
            WristObj.transform.localScale = Vector3.zero;
        }
        if(dummy)
        {
            foreach (var v in playerObjects)
                v.SetActive(false);
        }
    }


    void Update()
    {
        if(WristObj != null)
        {
            WristObj.transform.localScale = Vector3.zero;
            /*
            if (!dummy && !Settings.GetBool("UseWrist"))
            {
                WristObj.transform.localScale = Vector3.zero;
            }
            else
            {
                WristObj.transform.localScale = Vector3.one;
            }
            */
        }
    }

}
