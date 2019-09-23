using UnityEngine;
using System.Collections;

public class AnimateMaterial : MonoBehaviour {

    Renderer r;
    public Vector2 scrollSpeed;
    public bool BumpOnly;
    public bool IllumOnly;

	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if(r != null)
        {
            Vector2 offset = Time.time * scrollSpeed;
            if(!BumpOnly && !IllumOnly)
                r.material.mainTextureOffset = new Vector2(offset.x%1.0f, offset.y);
            if(BumpOnly && r.material.HasProperty("_BumpMap"))
                r.material.SetTextureOffset("_BumpMap", new Vector2(offset.x % 1.0f, offset.y));
            if(IllumOnly && r.material.HasProperty("_EmissionMap"))
                r.material.SetTextureOffset("_EmissionMap", new Vector2(offset.x % 1.0f, offset.y));
        }
            
	}
}
