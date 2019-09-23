using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirstTimePlay : MonoBehaviour {

    public UnityEvent OnFirstTime;
    public UnityEvent Revert;
    bool hasPlayed;
    bool doneFirstTime;

    IEnumerator Start()
    {
        yield return true;
        CheckFirstTime();  
    }

    public void SetPlayed()
    {
        if(doneFirstTime)
        {
            Revert.Invoke();
            doneFirstTime = false;
        }
    }

    public void CheckFirstTime()
    {
        if (Settings.GetBool("FirstTime", true))
        {
            OnFirstTime.Invoke();
            doneFirstTime = true;
        }
    }
}
