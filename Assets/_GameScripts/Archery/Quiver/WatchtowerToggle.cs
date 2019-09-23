using UnityEngine;
using System.Collections;

public class WatchtowerToggle : MonoBehaviour {

	public void DisableQuiver()
    {
        if (Quiver.instance != null)
            Quiver.instance.Disabled = true; 
    }

    public void EnableQuiver()
    {
        if (Quiver.instance != null)
            Quiver.instance.Disabled = false;
    }
}
