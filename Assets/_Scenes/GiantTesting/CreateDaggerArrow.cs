using UnityEngine;
using System.Collections;

public class CreateDaggerArrow : MonoBehaviour {

	public GameObject daggerPrefab;
	GameObject currentObj;

	public void Create()
	{
		if (currentObj != null)
			Remove ();

		currentObj = (GameObject)Instantiate (daggerPrefab);
		currentObj.GetComponent<DaggerArrow> ().Grab ();
	}

	public void Remove()
	{
		currentObj.GetComponent<DaggerArrow> ().Release ();
		Destroy (currentObj);
	}
}
