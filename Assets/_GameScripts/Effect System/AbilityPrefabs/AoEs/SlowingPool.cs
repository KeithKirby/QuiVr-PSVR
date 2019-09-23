using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowingPool : AoEEffect {

    public float perc = 0.5f;

	public override void TickFirst(GameObject obj)
    {
        var nav = obj.GetComponent<GroundCreature>();
        if (nav != null)
        {
            if(!dummy)
                nav.SetTagged();
            nav.ChangeSpeed(perc);
        }
    }

    public override void RemoveEffect(GameObject obj)
    {
        if (obj == null)
            return;
        var nav = obj.GetComponent<GroundCreature>();
        if (nav != null)
        {
            nav.ChangeSpeed(1);
        }
    }
}
