using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Flammable : MonoBehaviour {

    public float fireDamage;
    [System.Serializable]
    public class floatEvent : UnityEvent<float> { }
    [SerializeField]
    public floatEvent OnCatchFire;

    void Start()
    {
        if (GetComponent<Creature>() != null)
        {
            OnCatchFire.AddListener(GetComponent<Creature>().CatchFire);
        }
        if(GetComponent<TargetDummy>() != null)
        {
            OnCatchFire.AddListener(GetComponent<TargetDummy>().CatchFire);
        }
    }

    public void CatchFire()
    {
        OnCatchFire.Invoke(fireDamage);
    }
}
