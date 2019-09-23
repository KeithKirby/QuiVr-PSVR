using UnityEngine;
using System.Collections;

public class BowAnimation : MonoBehaviour {
    public Animation animationTimeline;
    float currentFrame;

    public void SetFrame(float frame)
    {
        CheckAnimation();
        animationTimeline["Main"].speed = 0;
        currentFrame = frame;
        animationTimeline["Main"].time = frame;
        animationTimeline.Play("Main");
    }

    void CheckAnimation()
    {
        if (animationTimeline == null)
            animationTimeline = GetComponentInChildren<Animation>();
    }

    public float GetFrame()
    {
        return currentFrame;
    }
}