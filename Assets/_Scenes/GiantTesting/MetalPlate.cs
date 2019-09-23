using UnityEngine;
using System.Collections;

public class MetalPlate : Weakspot {

	public override void UpdateTier()
	{
		GetComponent<MeshRenderer> ().material.color = tiers [currentTier].indiColor;
	}

	public void ReleasePlate()
	{
		GetComponent<Rigidbody> ().isKinematic = false;
		GetComponent<ArrowImpact> ().enabled = false;
	}
}
