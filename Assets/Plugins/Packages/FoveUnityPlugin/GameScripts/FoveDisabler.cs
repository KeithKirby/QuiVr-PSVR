using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class FoveDisabler : MonoBehaviour {

    public GameObject FoveObject;
    public GameObject WSA;
    public GameObject Alternate;

#if UNITY_WSA

    void Start()
    {
        if(WSA != null)
        {
            Alternate.SetActive(false);
            WSA.SetActive(true);
        }
    }

#endif
#if UNITY_STANDALONE_WIN
    IEnumerator Start()
    {
        string headset = UnityEngine.XR.XRDevice.model;
        yield return true;
        if (!headset.Contains("Oculus") && !headset.Contains("Vive") && FoveObject != null)
        {
            FoveObject.SetActive(true);
            Alternate.SetActive(false);
            yield return true;
            if (!FoveInterface.IsHardwareConnected())
            {
                if (FoveObject != null)
                    FoveObject.SetActive(false);
                Alternate.SetActive(true);
            }
        }
    }
#endif
}
