using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenSystemMenu : MonoBehaviour {

    void Awake()
    {
        SetupPublisher.SetupScreenActive += SetupPublisher_SetupScreenActive;
        gameObject.SetActive(!SetupPublisher.IsSetupScreenActive);
    }

    void OnDestroy()
    {
        SetupPublisher.SetupScreenActive -= SetupPublisher_SetupScreenActive;
    }

    void SetupPublisher_SetupScreenActive(bool show)
    {
        gameObject.SetActive(!show);
    }
}
