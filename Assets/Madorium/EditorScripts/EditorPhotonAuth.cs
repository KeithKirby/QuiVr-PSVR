using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonPs4Auth))]
public class EditorPhotonAuth : MonoBehaviour {

#if UNITY_EDITOR
    // Use this for initialization
    void Start () {
        PhotonPs4Auth.Ready = true;
        Destroy(FindObjectOfType<PS4Photon>().gameObject);
        Debug.Log("[EditorPhotonAuth] Authenticating photon for user editor!");
	}
#endif

}
