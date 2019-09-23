using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvgEnemyPos : MonoBehaviour {

    public Transform Backup;
    public bool OverrideFollow;
    public bool ShouldFollow;

	void Update()
    {
        if(!OverrideFollow && EnvironmentManager.instance != null)
        {
            ShouldFollow = EnvironmentManager.curEnv != EnvironmentType.Olympus;
        }
        if (ShouldFollow)
        {
            Vector3 AvgPos = Vector3.zero;
            int num = 0;
            foreach (var v in CreatureManager.AllEnemies())
            {
                AvgPos += v.transform.position;
                num++;
            }
            AvgPos /= num;
            transform.position = AvgPos;
        }
        else
            transform.position = Backup.transform.position;
    }
}
