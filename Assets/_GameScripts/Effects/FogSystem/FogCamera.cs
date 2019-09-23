using UnityEngine;
using System.Collections;
//using VolumetricFogAndMist;
using System.Reflection;
using System;

[RequireComponent (typeof(Camera))]
public class FogCamera : MonoBehaviour {
    /*
	void Start()
    {
        GenerateFog();
    }

    [BitStrap.Button]
    void RegenerateFog()
    {
        GenerateFog(true);
    }

    public void GenerateFog(bool clearOld=false)
    {
        VolumetricFog.RemoveAllFogAreas(gameObject);
        if (clearOld)
            FogArea.ClearFogObjects();
        if (FogArea.AllFog != null)
        {
            foreach (var v in FogArea.AllFog)
            {
                if (v != null)
                {
                    if(v.Radius > 0)
                        VolumetricFog.CreateFogArea(gameObject, v.transform.position, v.Radius, Vector3.zero, v.Preset);
                    else if(v.Size.magnitude > 0)
                        VolumetricFog.CreateFogArea(gameObject, v.transform.position, v.Size.x, v.Size, v.Preset);
                    if (VolumetricFog.LastArea != null)
                        VolumetricFog.LastArea.transform.SetParent(v.transform);
                }
            }
        }
        foreach(var v in FogSystem.GetSystems())
        {
            gameObject.AddComponent<VolumetricFog>(v);
        }
    */
}
