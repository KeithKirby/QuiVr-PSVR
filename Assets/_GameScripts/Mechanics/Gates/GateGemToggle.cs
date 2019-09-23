using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateGemToggle : MonoBehaviour {

	public void Toggle(bool val)
    {
        GateGems.ToggleGems(val);
    }
}
