using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class CoreEvents : MonoBehaviour {

    public UnityEvent Enabled;
    public UnityEvent Disabled;
    public UnityEvent Started;
    public UnityEvent Destroyed;

    void Start () {
        Started.Invoke();
	}

    void OnEnable()
    {
        Enabled.Invoke();
    }


    void OnDisable()
    {
        Disabled.Invoke();
    }

    void OnDestroy()
    {
        Destroyed.Invoke();
    }


}
