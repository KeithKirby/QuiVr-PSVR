using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEPlayerShield : AoEEffect {

    public override void Setup(bool fake, int efID, float v)
    {
        base.Setup(fake, efID, v);
    }

    public override void TickFirst(GameObject obj)
    {
        PlayerHead ph = obj.GetComponent<PlayerHead>();
        if (ph != null)
        {
            if (ph == PlayerHead.instance)
                PlayerLife.myInstance.invincible = true;
        }
    }

    public override void Tick(GameObject obj)
    {
        PlayerHead ph = obj.GetComponent<PlayerHead>();
        if (ph != null)
        {
            if (ph == PlayerHead.instance)
                PlayerLife.myInstance.invincible = true;
        }
    }

    public override void RemoveEffect(GameObject obj)
    {
        PlayerHead ph = obj.GetComponent<PlayerHead>();
        if(ph != null)
        {
            if (ph == PlayerHead.instance)
                PlayerLife.myInstance.invincible = false;
        }
    }
}
