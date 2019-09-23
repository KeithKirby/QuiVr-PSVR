using UnityEngine;
using System.Collections;

public class TargetObject : MonoBehaviour {

	public void DamageTaken(float val)
    {
        Statistics.AddCurrent("DamageTaken", (int)val);
    }

    public void DestroyTarget()
    {
        Debug.Log("Destroy Target called");
    }
}
