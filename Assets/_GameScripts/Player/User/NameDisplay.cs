using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameDisplay : MonoBehaviour {

    public Text NameText;
    float cd = 0;

    IEnumerator Start()
    {
        yield return true;
        if(PlatformSetup.instance != null)
            NameText.text = Cosmetics.GetFullName(true);
    }

    public void CheckName()
    {
        if (PlatformSetup.instance != null && NameText.text != Cosmetics.GetFullName(true))
            NameText.text = Cosmetics.GetFullName(true);
    }

    void Update()
    {
        if(SteamManager.Initialized)
        {
            cd += Time.unscaledDeltaTime;
            if(cd > 0.511f)
            {
                CheckName();
                cd = 0;
            }
        }
    }
}
