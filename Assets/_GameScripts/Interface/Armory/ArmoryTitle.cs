using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmoryTitle : MonoBehaviour {

    Text txt;

    void Awake()
    {
        txt = GetComponent<Text>();
    }

    void OnEnable()
    {
        if(txt != null)
        {
            txt.text = Cosmetics.GetFullName(true) + "'s  Armory";
        }
    }
}
