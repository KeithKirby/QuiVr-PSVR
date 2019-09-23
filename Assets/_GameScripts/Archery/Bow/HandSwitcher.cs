using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.UI;
public class HandSwitcher : MonoBehaviour {

    BowAim ba;
    public Toggle LeftToggle;

    void Awake()
    {
        ba = GetComponent<BowAim>();
        GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += OnUse;
    }

    public void OnUse(object o, InteractableObjectEventArgs e)
    {
        if (!Settings.GetBool("QuickBowSwap"))
            return;
        if(!ba.HasArrow() && e.interactingObject == SteamVR_ControllerManager.freeHand && e.interactingObject.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() == null)
        {
            //Debug.Log("Good switch");
            LeftToggle.isOn = !LeftToggle.isOn;
        }
        else
        {
            //Debug.Log("Bad Switch - Ignoring");
        }
    }

}
