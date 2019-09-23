using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent (typeof(ArrowImpact))]
public class Weakspot : MonoBehaviour {

	public WeakspotIndicator[] tiers;

	public int currentTier;

	public UnityEvent OnComplete;

	public void Reset()
	{
		currentTier = 0;
		UpdateTier ();
	}

	void Start()
	{
		UpdateTier ();
	}

	public virtual void UpdateTier()
	{}

	public void NextTier()
	{
		currentTier++;
		if (currentTier >= tiers.Length) {
			CompleteSequence ();
		} else {
			UpdateTier();
		}
	}

	public void CompleteSequence()
	{
		OnComplete.Invoke ();
	}

}

[System.Serializable]
public class WeakspotIndicator
{
	public Color indiColor;
}
