using UnityEngine;
using System.Collections;
using VRTK;

public class ReturnBowandQuiver : MonoBehaviour {

    public GameObject quiver;
    public GameObject bow;

    public BowSetup bs;

    public static ReturnBowandQuiver instance;

    void Awake()
    {
        instance = this;
    }

	public void Init(GameObject b, GameObject q)
    {
        bow = b;
        quiver = q;
    }
	
	public void ReturnObjects()
    {
        bow.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
        bow.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bow.transform.SetParent(null);
        //bow.transform.position = transform.position;
        bow.transform.localScale = Vector3.one;
        quiver.GetComponent<Quiver>().ReleaseSnap();
        //quiver.transform.position = transform.position;
        quiver.GetComponent<Rigidbody>().velocity = Vector3.zero;
        quiver.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
        quiver.transform.SetParent(null);
        quiver.transform.localScale = Vector3.one;
        Invoke("Reset", (0.05f*Time.timeScale));
        //bs.Reset();
    }

    void Reset()
    {
        bs.Reset();
    }
}
