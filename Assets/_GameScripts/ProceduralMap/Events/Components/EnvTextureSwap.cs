using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvTextureSwap : MonoBehaviour {

    public Renderer[] Rends;
    public EnvTexture[] Textures;

    void Start()
    {
        MapTile mt = GetComponentInParent<MapTile>();
        if(mt != null)
        {
            foreach (var v in Textures)
            {
                if(v.env == mt.Environment)
                {
                    foreach(var r in Rends)
                    {
                        if(r != null && r.material != null && v != null)
                            r.material.mainTexture = v.tex;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class EnvTexture
    {
        public EnvironmentType env;
        public Texture2D tex;

        public override string ToString()
        {
            return env.ToString();
        }
    }
}
