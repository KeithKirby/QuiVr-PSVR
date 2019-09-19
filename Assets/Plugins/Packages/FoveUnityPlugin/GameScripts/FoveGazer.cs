using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoveGazer : MonoBehaviour {

    public static FoveGazer instance;
    public FoveInterface Interface;

    public bool DrawDebug;
    public Material DebugMaterial;

    void Awake()
    {
        instance = this;
    }

	public static FoveInterface GetInterface()
    {
        if(instance != null)
        {
            if(instance.Interface != null && instance.Interface.gameObject.activeInHierarchy)
            {
                return instance.Interface;
            }
        }
        return null;
    }

    GameObject lSp;
    GameObject rSp;
    void Update()
    {
        if(Interface != null)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
                FoveInterface.EnsureEyeTrackingCalibration();
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
                DrawDebug = !DrawDebug;
            if (DrawDebug)
            {
                if (lSp == null)
                    CreateSpheres();
                FoveInterface.EyeRays rays = Interface.GetGazeRays_Immediate();
                RaycastHit hit;
                if(Physics.Raycast(rays.left, out hit, 500))
                {
                    lSp.transform.position = hit.point;
                }
                if (Physics.Raycast(rays.right, out hit, 500))
                {
                    rSp.transform.position = hit.point;
                }
            }
        }

    }

    void CreateSpheres()
    {
        lSp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lSp.GetComponent<Collider>().enabled = false;
        lSp.transform.localScale = Vector3.one * 0.1f;
        lSp.GetComponent<MeshRenderer>().material = DebugMaterial;

        rSp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rSp.GetComponent<Collider>().enabled = false;
        rSp.transform.localScale = Vector3.one * 0.1f;
        rSp.GetComponent<MeshRenderer>().material = DebugMaterial;
    }
}
