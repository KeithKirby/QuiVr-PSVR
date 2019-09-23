using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class ObjEvent : UnityEvent<GameObject> { }

public class Spawner : MonoBehaviour {

    public GameObject[] prefabs;
    public bool spawnAutomatically;
    public float spawnCD;
    float cd;
    public bool onlyOneActive;
    bool didSpawn;
    public Transform SpawnPoint;
    public bool SpawnOnStart;
    public bool canSpawn = true;

    public bool ignoreSpawnRotation;

    public ObjEvent OnSpawn;

    public bool requireAlive;
    Health eb;

    List<GameObject> objectsOut;

    void Awake()
    {
        objectsOut = new List<GameObject>();
    }

    void Start()
    {
		cd = spawnCD;
        eb = GetComponentInParent<Health>();
        if (SpawnPoint == null)
            SpawnPoint = transform;
        if (SpawnOnStart)
            SpawnImmediate();       
    }

    void Update()
    {
        if(objectsOut != null)
        {
            for (int i = objectsOut.Count - 1; i >= 0; i--)
            {
                if (objectsOut[i] == null)
                    objectsOut.RemoveAt(i);
            }
        }       
        if (spawnAutomatically && cd <= 0 && (!didSpawn || !onlyOneActive))
        {
            SpawnItem();
        }
        else
            cd -= Time.deltaTime;
    }

	public GameObject SpawnImmediate()
    {
        return SpawnItem();
    }

    [BitStrap.Button]
    public void SpawnNoReturn()
    {
        SpawnItem();
    }

    public void UseItem()
    {
        didSpawn = false;
    }

    public void SetCanSpawn(bool val)
    {
        canSpawn = val;
    }

    GameObject SpawnItem()
    {
        if (!canSpawn || (requireAlive && eb != null && eb.isDead()))
            return null;
        didSpawn = true;
        cd = spawnCD;
        Quaternion r = Quaternion.identity;
        if (!ignoreSpawnRotation && SpawnPoint != null)
            r = SpawnPoint.rotation;
        GameObject newObj = (GameObject)Instantiate(prefabs[Random.Range(0, prefabs.Length)], SpawnPoint.position, r);
        OnSpawn.Invoke(newObj);
        objectsOut.Add(newObj);
        return newObj;
    }

    public void ClearAll()
    {
        for(int i=0; i<objectsOut.Count; i++)
        {
            if (objectsOut[i] != null)
                Destroy(objectsOut[i]);
        }
        objectsOut = new List<GameObject>();
    }
}
