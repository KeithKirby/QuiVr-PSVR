using UnityEngine;
using System.Collections;

public class ToggleExternal : MonoBehaviour {

    public GameObject[] Objects;
    public bool on; 

    public void Toggle()
    {
        on = !on;
        foreach(var v in Objects)
        {
            v.SetActive(on);
        }
    }
}
