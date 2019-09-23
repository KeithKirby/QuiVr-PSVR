using UnityEngine;
using System.Collections;

public class RuneSpot : Weakspot {

	public override void UpdateTier()
	{
		GetComponent<SpriteRenderer> ().color = tiers [currentTier].indiColor;
	}
}
