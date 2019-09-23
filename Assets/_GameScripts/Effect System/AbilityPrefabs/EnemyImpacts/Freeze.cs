using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : EnemyHitBase {

    public override void DoAction(GameObject enemy, bool dummy)
    {
        if(NetworkEffects.instance != null && !dummy)
        {
            Immunities im = enemy.GetComponent<Immunities>();
            bool immune = false;
            if (im != null)
            {
                if (im.CheckImmune(EffectType))
                {
                    CombatText.ShowText("Immune", enemy.transform.position + (Vector3.up * 2), Color.white);
                    immune = true;
                }
            }
            Creature c = enemy.GetComponent<Creature>();
            if (c != null && pvpmanager.instance.PlayingPVP && c.Team == pvpmanager.instance.myTeam)
                immune = true;
            if (!immune && c != null)
                NetworkEffects.instance.Freeze(enemy, GetValue(), PhotonNetwork.inRoom, true);
        }
    }

}
