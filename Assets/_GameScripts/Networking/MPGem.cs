using UnityEngine;
using System.Collections;

public class MPGem : MonoBehaviour {

	public ParticleSystem ps;
	public AudioClip useClip;

	public void Use()
	{
		var em = ps.emission;
		em.enabled = false;
		GetComponent<MeshRenderer> ().enabled = false;
		GetComponent<Collider> ().enabled = false;
		if (useClip != null) {
			VRAudio.PlayClipAtPoint (useClip, transform.position, 1);
		}
		Invoke ("DoMP", 2f);

	}
	
	void DoMP()
	{
		var em = ps.emission;
		em.enabled = true;
		GetComponent<MeshRenderer> ().enabled = true;
		GetComponent<Collider> ().enabled = true;
        JoinMultiplayer.Click();
	}
}
