using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCooldowns : MonoBehaviour {

    public float UseCD = 5f;
    public RotateTimed ObjRotate;
    public AnimationCurve RotateSpeed;
    float cd;

    public void Use()
    {
        if (cd <= 0 && CreatureManager.EnemyDifficulty <= 0)
        {
            cd = UseCD;
            ObjRotate.Rotation = new Vector3(0, -1000, 0);
            OrbManager.instance.ResetCooldown();
            BowAbility.instance.ResetCooldown();
        }
    }

    void Update()
    {       
        if (cd >= 0)
            cd -= Time.deltaTime;
        ObjRotate.Rotation.y = RotateSpeed.Evaluate(cd);
    }

}
