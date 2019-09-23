using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CurrentGateDisplay : MonoBehaviour {

    Image img;
    EMOpenCloseMotion motion;

    void Awake()
    {
        img = GetComponent<Image>();
        motion = GetComponentInParent<EMOpenCloseMotion>();
    }

    void FixedUpdate()
    {
        if (motion == null || motion.motionState != EMBaseMotion.MotionState.Closed)
        {
            img.fillAmount = 0;
            if (GameBase.instance != null && GameBase.instance.CurrentTarget != null)
            {
                img.fillAmount = GameBase.instance.CurrentTarget.currentHP / (float)GameBase.instance.CurrentTarget.maxHP;
            }
        }
    }
}
