using UnityEngine;
using System.Collections;

public class Blocker : MonoBehaviour {

	void OnEnable()
    {
        if (StreamBlockerEffect.instance != null)
            StreamBlockerEffect.instance.Enabled(transform.position);
    }

    void OnDisable()
    {
        if (StreamBlockerEffect.instance != null)
            StreamBlockerEffect.instance.Disabled(transform.position);
    }
}
