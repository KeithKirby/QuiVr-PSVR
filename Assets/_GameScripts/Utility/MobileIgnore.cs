using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileIgnore : MonoBehaviour {

    public bool DestroyObject;

    void Awake()
    {
    #if UNITY_MOBILE
        if(DestroyObject)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    #endif
    }
}
