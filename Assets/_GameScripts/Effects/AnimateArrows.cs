using UnityEngine;
using System.Collections;

public class AnimateArrows : MonoBehaviour {

    Renderer r;
    public Vector2 scrollSpeed;
    public bool enableAnimate = false;

    public void EnableAnimate()
    {
        enableAnimate = true;
    }
    public void DisableAnimate()
    {
        enableAnimate = false;
    }

    // Use this for initialization
    void Start () {
        r = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if(r != null)
        {
            Vector2 offset = Time.time * scrollSpeed;
            if(enableAnimate == true)
                r.material.mainTextureOffset = new Vector2(offset.x%1.0f, offset.y);
           
        }
            
	}
}
