using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using VRTK;

public class ButtonListen : MonoBehaviour {

    public UnityEvent OnButton;

	public void OnButtonUse(object o, ControllerInteractionEventArgs e)
    {
        OnButton.Invoke();
    }
}
