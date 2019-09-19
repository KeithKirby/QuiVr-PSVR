using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazeInteraction : MonoBehaviour {

    public Collider RefCollider;
    public bool requireKey = true;
    public KeyCode InputKey;

    public UnityEvent OnInput;

    void Awake()
    {
        if (RefCollider != null)
            RefCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if(RefCollider != null && (!requireKey || Input.GetKeyDown(InputKey)))
        {
            FoveInterface fve = FoveGazer.GetInterface();
            if (fve != null)
                CheckFoveGaze(fve);
        }
    }

    void CheckFoveGaze(FoveInterface i)
    {
        RefCollider.isTrigger = false;
        if(i.Gazecast(RefCollider))
        {
            OnInput.Invoke();
        }
    }
}
