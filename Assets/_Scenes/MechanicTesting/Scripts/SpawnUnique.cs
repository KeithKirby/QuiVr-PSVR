using UnityEngine;
using System.Collections;

public class SpawnUnique : MonoBehaviour {

    public GameObject[] SpawnObjs;

    public GameObject currentOut;

    Health ht;

    public float cd;

    bool spawning;

    bool stopped;

    int curIndex = 0;

	// Update is called once per frame
	void Update ()
    {
        if(ht != null && ht.isDead())
        {
            currentOut = null;
        }
	    if(currentOut == null && !spawning && !stopped)
        {
            spawning = true;
            Invoke("SpawnNew", cd);
        }
	}

    public void SpawnNew()
    {
		if (stopped) {
			spawning = false;
			return;
		}
			
        currentOut = (GameObject)Instantiate(SpawnObjs[curIndex], transform.position, transform.rotation);
        ht = currentOut.GetComponent<Health>();
        spawning = false;
        curIndex++;
        if (curIndex >= SpawnObjs.Length)
            curIndex = 0;
    }

    public void StopAndClear()
    {
        stopped = true;
        if(currentOut != null)
        {
            Destroy(currentOut);
        }
        Fireball[] fbs = FindObjectsOfType<Fireball>();
        foreach(var v in fbs)
        {
            v.Explode(false, false);
        }
    }

    public void Resume()
    {
        stopped = false;
    }
}
