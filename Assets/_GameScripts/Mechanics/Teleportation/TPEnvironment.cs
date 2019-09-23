using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Teleporter))]
public class TPEnvironment : MonoBehaviour {

    public EnvironmentType Environment;

	void Start()
    {
        GetComponent<Teleporter>().OnTeleport.AddListener(delegate { EnvironmentManager.ChangeEnv(Environment); });
    }
}
