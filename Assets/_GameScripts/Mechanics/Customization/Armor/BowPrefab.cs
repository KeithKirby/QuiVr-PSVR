using UnityEngine;
using System.Collections;

public class BowPrefab : MonoBehaviour {

    public Animation anim;
    public float drawOffset;
    public Transform nockPoint;
    bool didSetup;

    public SkinnedMeshRenderer Detail;

    void Start()
    {
        if (!didSetup)
            Setup(new Color[] { });
    }


	public void Setup(Color[] c)
    {
        didSetup = true;
        if(GetComponentInParent<BowAnimation>() != null)
            GetComponentInParent<BowAnimation>().animationTimeline = anim;
        if (GetComponentInParent<BowAim>() != null)
            GetComponentInParent<BowAim>().pullOffset = drawOffset;
        if (GetComponentInParent<BowHandle>() != null)
        {
            GetComponentInParent<BowHandle>().arrowNockingPoint = nockPoint;
            GetComponentInParent<BowHandle>().SetupNockAudio();
        }
            
        if(c.Length > 0 && Detail != null)
        {
            Detail.material.SetColor("_EmissionColor", c[0]*4);
        }
    }
}
