using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using VolumetricFogAndMist;
public class FogArea : MonoBehaviour {

    public float Radius;
    public Vector3 Size;
    //public VolumetricFogAndMist.FOG_PRESET Preset;

    public static List<FogArea> AllFog;
    /*
    public static void ClearFogObjects()
    {
        if (AllFog == null)
            return;
        for(int i=0; i<AllFog.Count; i++)
        {
            var v = AllFog[i];
            if(v != null)
            {
                FogAreaCullingManager cm = v.GetComponentInChildren<FogAreaCullingManager>();
                if (cm != null)
                    Destroy(cm.gameObject);
            }
        }
    }

    void Awake()
    {
        if (AllFog == null)
            AllFog = new List<FogArea>();
        AllFog.Add(this);
    }

    bool isQuitting;
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (!isQuitting)
        {
            AllFog.Remove(this);
        }
    }
    */
}
