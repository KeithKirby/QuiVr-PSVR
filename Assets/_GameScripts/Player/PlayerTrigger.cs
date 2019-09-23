using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour {

    public UnityEvent OnPlayerEnter;
    public bool KillPlayer;

    void OnTriggerEnter(Collider col)
    {
        PlayerLife p = col.gameObject.GetComponentInParent<PlayerLife>();
        if (p != null && p == PlayerLife.myInstance)
        {
            OnPlayerEnter.Invoke();
            if (KillPlayer)
                PlayerLife.Kill();
        }

    }
}
