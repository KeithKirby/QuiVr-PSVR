using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSpectatorSync : MonoBehaviour {

    public GameObject Plr;

	public void InitPlayer()
    {
        Plr.SetActive(true);
    }
}
