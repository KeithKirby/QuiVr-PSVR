using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class PS4SaveSystem : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
#if UNITY_PS4
        PS4PlayerPrefs.SetTitleStrings("QuiVR", "The defining VR archery experience.", "Archer equipment and progress");
#endif

    }

    // Update is called once per frame
    void Update()
    {
    }
}