using UnityEngine;
using System.Collections;
using VRTK;

public class ButtonBroadcast : MonoBehaviour {

	void Start()
    {
        ButtonListen bl = FindObjectOfType<ButtonListen>();
        if(bl != null)
        {
            var ev = GetComponent<VRTK_ControllerEvents>();
            ev.GripPressed += bl.OnButtonUse;
        }
    }
}
