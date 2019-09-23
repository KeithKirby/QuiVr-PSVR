using UnityEngine;
using System.Collections;

public class ShotgonSpawn : MonoBehaviour {

    public GameObject flakPrefab;
    public float spawnCD;
    float cd;
    bool didSpawn;
    public bool SpawnOnStart;
    public Transform SpawnPoint;

    public bool requireAlive;

    public int count;
    public float posVariance;
    public float speedVariance;
    public bool spawnAutomatically;

    void Start()
    {
        if (SpawnPoint == null)
            SpawnPoint = transform;
        if (SpawnOnStart)
            SpawnImmediate();
    }

    void Update()
    {
        if (spawnAutomatically && cd <= 0)
        {
            SpawnItem();
        }
        else
            cd -= Time.deltaTime;
    }

    public void SpawnImmediate()
    {
        SpawnItem();
    }

    void SpawnItem()
    {
        didSpawn = true;
        cd = spawnCD;
        GameObject pObj = (GameObject)Instantiate(flakPrefab, SpawnPoint.position, SpawnPoint.rotation);
        FireballSetup.SetupFireball(pObj, Vector3.zero, 0, 0);
        for (int i=0; i<count; i++)
        {
            GameObject newObj = (GameObject)Instantiate(flakPrefab, SpawnPoint.position, SpawnPoint.rotation);
            FireballSetup.SetupFireball(newObj, Vector3.zero, posVariance, speedVariance);
        } 
    }

}
