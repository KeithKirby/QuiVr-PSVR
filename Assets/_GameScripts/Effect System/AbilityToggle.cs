using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityToggle : MonoBehaviour {

	public void ToggleAbilities(bool val)
    {
        ArrowEffects.EffectsDisabled = !val;
    }
}
