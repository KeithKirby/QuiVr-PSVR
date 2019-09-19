using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileOptions : MonoBehaviour
{
    [AdvancedInspector.Inspect]
    public void ResetProfile()
    {
        if (!PlayerProfile.Ready)
        {
            Debug.LogWarning("Player profile still not ready!");
            return;
        }
        PlayerProfile.Profile.ResetProfile();
    }


}
