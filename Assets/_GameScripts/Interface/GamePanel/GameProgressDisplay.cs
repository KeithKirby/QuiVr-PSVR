using UnityEngine;
using System.Collections;
using UnityEngine.UI;   

public class GameProgressDisplay : MonoBehaviour {

    Image img;
    EMOpenCloseMotion motion;

    void Awake()
    {
        img = GetComponent<Image>();
        motion = GetComponentInParent<EMOpenCloseMotion>();
    }

    void FixedUpdate()
    {
        if(motion == null || motion.motionState != EMBaseMotion.MotionState.Closed)
        {
            img.fillAmount = 0;
            if (GameBase.instance != null && GameBase.instance.Difficulty > 0)
            {
                int f = GameBase.instance.GetEnemyForces();
                float v = Mathf.Clamp(GameBase.instance.EnemiesSpawned, 0, f);
                img.fillAmount = v / (float)f;
            }
        }
    }
}
