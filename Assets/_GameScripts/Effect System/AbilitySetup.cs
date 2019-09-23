using UnityEngine;
using System.Collections;

public class AbilitySetup : MonoBehaviour {

    public float OverrideVal;

	public void Setup(int effectID, float value, bool dummy)
    {
        if (OverrideVal > 0)
            value = OverrideVal;
        GetComponent<Effect>().Setup(dummy, effectID, value);
    }
}
