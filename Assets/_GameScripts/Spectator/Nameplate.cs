using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Nameplate : MonoBehaviour
{
    Text txt;

    void Awake()
    {
        txt = GetComponent<Text>();
        //InvokeRepeating("CheckVisibile", 0.5f, Random.Range(0.35f, 0.5f));
    }

    void CheckVisibile()
    {
        if(SpectatorSync.myInstance != null)
        {
            txt.enabled = !SpectatorSync.myInstance.active;
        }
    }

}
