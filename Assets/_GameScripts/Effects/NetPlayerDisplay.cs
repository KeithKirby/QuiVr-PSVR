using UnityEngine;
using System.Collections;

public class NetPlayerDisplay : MonoBehaviour {

    public float MinDist;
    ParticleSystem s;

    void Awake()
    {
        s = GetComponent<ParticleSystem>();
    }

	void Update()
    {
        if(PlayerHead.instance != null && s != null)
        {
            var e = s.emission;
            if (Vector3.Distance(PlayerHead.instance.transform.position, transform.position) > MinDist && (SpectatorSync.myInstance == null || !SpectatorSync.myInstance.active))
                e.enabled = true;
            else
                e.enabled = false;
        }
    }

}
