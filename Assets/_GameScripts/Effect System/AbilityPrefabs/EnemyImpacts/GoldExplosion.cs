using UnityEngine;
using System.Collections;

public class GoldExplosion : EnemyHitBase
{
    public int ResourceGain;

    public override void DoAction(GameObject enemy, bool dummy)
    {
        if(!dummy && CreatureManager.InGame() && enemy.GetComponent<Creature>() != null)
        {
            Armory.instance.GiveResource(ResourceGain, false);
        }
    }
}
