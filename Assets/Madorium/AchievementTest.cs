//using POpusCodec;
using UnityEngine;

public class AchievementTest : MonoBehaviour {

    AchievementTest _inst;

    // Use this for initialization
    void Start() {}

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Square"))
        {
            Achievement.EarnAchievement("ALL_HALLOWED");
        }

        if (Input.GetButtonDown("Fire3") || Input.GetButtonDown("X"))
        {
            Achievement.EarnAchievement("COMBO_100");
        }
    }
}