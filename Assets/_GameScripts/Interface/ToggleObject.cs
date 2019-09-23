using UnityEngine;
using System.Collections;

public class ToggleObject : MonoBehaviour {

    public GameObject obj;

	public void Toggle()
    {
        obj.SetActive(!obj.activeSelf);
    }
}
