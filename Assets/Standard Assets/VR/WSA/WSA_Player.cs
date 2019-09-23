using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
#if UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif
public class WSA_Player : MonoBehaviour {

    public GameObject BaseCam;
    public GameObject WSAObj;
    public Transform WSACam;
    public Transform HeadObjects;
    public Transform LeftHand;
    public Transform RightHand;

    [Header("Bow Overrides")]
    public Transform LeftCheekNock;
    public Transform RightCheekNock;

    public static WSA_Player instance;

#if UNITY_WSA
    private void Awake()
    {
        instance = this;
        BaseCam.SetActive(false);
        WSAObj.SetActive(true);
        HeadObjects.SetParent(WSACam);
        HeadObjects.localPosition = Vector3.zero;
        HeadObjects.localRotation = Quaternion.identity;
        LeftHand.SetParent(WSAObj.transform);
        RightHand.SetParent(WSAObj.transform);
    }

    public static Vector3 NockPosition(uint id)
    {
        if (instance == null)
            return Vector3.zero;
        if (id == 0)
            return instance.RightCheekNock.position;
        return instance.LeftCheekNock.position;
    }

    public static GameObject DrawHand()
    {
        if(instance != null)
        {
            if (Settings.GetBool("LeftHanded"))
                return instance.LeftHand.GetComponentInChildren<HandAnim>().gameObject;
            return instance.RightHand.GetComponentInChildren<HandAnim>().gameObject;
        }
        return null;
    }
#endif
}
