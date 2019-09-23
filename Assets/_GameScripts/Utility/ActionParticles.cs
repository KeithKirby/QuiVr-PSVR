using UnityEngine;
using System.Collections;

public class ActionParticles : MonoBehaviour {

    public GameObject SpawnParticles;
    public GameObject DestroyParticles;
    public Transform SpawnPoint;
    void Start()
    {
        if (SpawnPoint == null)
            SpawnPoint = transform;
        if(SpawnParticles != null)
        {
            GameObject SP = (GameObject)Instantiate(SpawnParticles, SpawnPoint.position, Quaternion.identity);
            SP.SetActive(true);
        }
    }

    bool isQuitting;
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (!isQuitting && DestroyParticles != null)
        {
#if !UNITY_EDITOR
            GameObject Dst = (GameObject)Instantiate(DestroyParticles, SpawnPoint.position, Quaternion.identity);
            Dst.SetActive(true);
#endif
        }
    }
}
