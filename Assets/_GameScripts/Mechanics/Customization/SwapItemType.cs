using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapItemType : MonoBehaviour {

    public GameObject Mirror;
    public GameObject Mannequin;

    public void Toggle()
    {
        if(Mirror.activeSelf)
        {
            Mirror.SetActive(false);
            Mannequin.SetActive(true);
        }
        else
        {
            Mirror.SetActive(true);
            Mannequin.SetActive(false);
        }
    }
}
