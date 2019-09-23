using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class ArrowFire : MonoBehaviour {

    public UnityEvent OnCatchFire;
    public GameObject FirePrefab;
    public Transform parentPosition;
    bool used;
    bool isOnFire;
	
	// Update is called once per frame
	void OnTriggerEnter(Collider col) 
    {
        if(col.tag == "Fire" && !used && !isOnFire)
        {
            used = true;
            CatchFire();
        }
	}

    public bool OnFire()
    {
        return isOnFire;
    }

    public void CatchFire()
    {
        isOnFire = true;
        GameObject newFire = (GameObject)Instantiate(FirePrefab);
        newFire.transform.SetParent(parentPosition);
        newFire.transform.localPosition = Vector3.zero;
        OnCatchFire.Invoke();
    }
}
