using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NVR_Player : MonoBehaviour {

    public GameObject Bow;

    public GameObject ThirdPersonCam;
    public GameObject RegCam;
    public static bool usingFove;
    public bool NonVR;

    bool initialized;
   
    public static NVR_Player instance;

    [HideInInspector]
    public NVR_Positions pos;
    [HideInInspector]
    public NVR_Bow nvbow;
    public CameraPosition TPCamMgr;

    void Awake()
    {
        instance = this;
        pos = GetComponent<NVR_Positions>();
        nvbow = GetComponent<NVR_Bow>();
    }

    public static bool isThirdPerson()
    {
        return instance != null && instance.NonVR;
    }

	IEnumerator Start ()
    {
        bool Fove = false;
#if UNITY_STANDALONE_WIN
        try
        {
            Fove = FoveInterface.IsHardwareConnected();
        }
        catch { }
#endif
        usingFove = Fove;
        yield return true;
        Cursor.lockState = CursorLockMode.None;
        yield return true;
        if (Settings.GetBool("ThirdPerson"))
            NonVR = true;
        while(!NonVR && !Fove)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.V))
                NonVR = true;
            yield return true;
        }
        Debug.Log("Starting 3rd Person Mode");
        if(!Fove)
            Settings.Set("ThirdPerson", true);
        ThirdPersonCam.SetActive(true);
        foreach (var comp in RegCam.GetComponents(typeof(Component)))
        {
            if (comp.GetType() != typeof(Camera) && comp.GetType() != typeof(Transform))
                Destroy(comp);
        }
        foreach(var mr in RegCam.GetComponentsInChildren<MeshRenderer>())
        {
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        foreach (var mr in RegCam.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }      
        pos.ActivateNVR();  
        pos.ActivateNVR();
        Cursor.lockState = CursorLockMode.Locked;
        if (TestActions.instance != null && TelePlayer.instance.currentNode == TestActions.instance.watchtowerTP)
            TestActions.instance.watchtowerTP.OnTeleport.Invoke(TestActions.instance.watchtowerTP);
    }

	void Update () {
        if(NonVR)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.V))
            {
                Settings.Set("ThirdPerson", false);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
                pos.ChangeView();
            Cursor.lockState = CursorLockMode.Locked;
            pos.MoveAim();
            nvbow.Aim();
            pos.SetPositions(); 
        }     
	}

    public void ToggleHands(bool val)
    {
        pos.ToggleHands(val);
        nvbow.ToggleActive(val);
    }

    public void SetArrowEffect(int eid, float val)
    {
        nvbow.SetArrowEffect(eid, val);
    }

    public bool FirstPersonMode()
    {
        return pos.isFirstPerson();
    }

    public void ForceFirstPerson(bool val)
    {
        pos.ForceFP(val);
        if (val)
            TPCamMgr.Invoke("ResetPosition", 0.25f);
    }

}
