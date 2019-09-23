using UnityEngine;
using System.Collections;

public class ExtraEffectRenderer : MonoBehaviour {

    Renderer r;
    Renderer[] children;
    Light light;
    public int FXClass = 1;
    public bool destroyObject;
    public bool Enable;
    public Component[] AdditionalComponents;

	void Awake()
    {
        int setting = Settings.GetInt("EffectQuality", 1);
        if (destroyObject && setting < FXClass)
            Destroy(gameObject);
        r = GetComponent<Renderer>();
        light = GetComponent<Light>();
        if(r == null)
        {
            children = GetComponentsInChildren<Renderer>();
        }
        if(Enable && r != null)
        {
            if (setting > FXClass)
            {
                if (destroyObject)
                    Destroy(r);
                r.enabled = false;
            }     
        }
        InvokeRepeating("CheckRend", 0.5f, Random.Range(0.137f, 0.21763f));
    }

    void CheckRend()
    {
        bool shouldRender = true;
        if (Enable)
            shouldRender = Settings.GetInt("EffectQuality", 1) <= FXClass;
        else
            shouldRender = Settings.GetInt("EffectQuality", 1) >= FXClass;
        if (r != null)
        {
            r.enabled = shouldRender;
        }
        if (light != null)
            light.enabled = shouldRender;
        else if (children != null && children.Length > 0)
        {
            foreach (var v in children)
            {
                v.enabled = shouldRender;
            }
        }
        if(AdditionalComponents != null && AdditionalComponents.Length > 0)
        {
            foreach(var v in AdditionalComponents)
            {
                if (v is Cloth)
                    ((Cloth)v).enabled = shouldRender;
                else if (v is Projector)
                    ((Projector)v).enabled = shouldRender;
            }
        }
    }
}
