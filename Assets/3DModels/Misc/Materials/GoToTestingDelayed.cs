using UnityEngine;
using System.Collections;

public class GoToTestingDelayed : MonoBehaviour {

	public ParticleSystem ps;
	public AudioClip useClip;

	public void Go()
	{
		if (useClip != null) {
			VRAudio.PlayClipAtPoint (useClip, transform.position, 1);
		}
		GetComponent<MeshRenderer> ().enabled = false;
		GetComponent<Collider> ().enabled = false;
		var emit = ps.emission;
		emit.enabled = false;
		Invoke ("ChangeScene", 2f);
	}

	public void ChangeScene()
	{
		GetComponent<ChangeScene> ().Click ();
	}
}
