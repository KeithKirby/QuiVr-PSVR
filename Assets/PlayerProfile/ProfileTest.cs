using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PlayerProfile.Init();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("ProfileTest.Save");
            PlayerProfile.Profile.Save(PlayerProfile.SaveCategory.Default);
        }

        if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log("ProfileTest.Load");
            PlayerProfile.Profile.Load();
        }
    }
}