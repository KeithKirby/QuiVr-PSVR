using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ReadyInterface : MonoBehaviour {

	public void OnUse()
    {
        if(ToggleMenu.instance != null)
        {
            if (ToggleMenu.instance.isOpen())
                ToggleMenu.instance.Toggle();
        }
        if (VRTK_UIPointer.Pointers == null)
            VRTK_UIPointer.Pointers = new System.Collections.Generic.List<VRTK_UIPointer>();
        foreach (var v in VRTK_UIPointer.Pointers)
        {
            v.On = false;
        }
    }
}
