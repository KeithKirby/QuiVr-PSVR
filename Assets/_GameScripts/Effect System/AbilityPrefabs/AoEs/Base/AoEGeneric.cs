using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class AoEGeneric : AoEEffect {

    public AoEEvent OnFirstTick;
    public AoEEvent OnTick;
    public AoEEvent OnDieIn;

    public override void Setup(bool fake, int efID, float v)
    {
        base.Setup(fake, efID, v);
    }

    public override void TickFirst(GameObject obj)
    {
        OnFirstTick.Invoke(obj, dummy);
    }

    public override void Tick(GameObject obj)
    {
        OnTick.Invoke(obj, dummy);
    }

    public override void OnDieInside(GameObject obj)
    {
        OnDieIn.Invoke(obj, dummy);
    }
}

[System.Serializable]
public class AoEEvent : UnityEvent<GameObject, bool> { }
