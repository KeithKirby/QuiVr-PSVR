using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DemoChange : MonoBehaviour {

    public UnityEvent OnDemo;
    public bool DestroyIfDemo;
    IEnumerator Start()
    {
        yield return true;
        if (AppBase.v != null && AppBase.v.isDemo)
        {
            if (DestroyIfDemo)
                Destroy(gameObject);
            else
                OnDemo.Invoke();
        }
            
    }
}
