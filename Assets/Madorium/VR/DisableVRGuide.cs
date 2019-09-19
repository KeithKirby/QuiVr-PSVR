using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVRGuide : MonoBehaviour
{
    static int _disableCount = 0;

    static public bool Enabled
    {
        get
        {
            return _disableCount == 0;
        }
    }

    void OnEnable()
    {
        ++_disableCount;
    }

    void OnDisable()
    {
        --_disableCount;
        if(_disableCount < 0)
        {
            throw new System.Exception("VR Guide underflow");
        }
    }
}