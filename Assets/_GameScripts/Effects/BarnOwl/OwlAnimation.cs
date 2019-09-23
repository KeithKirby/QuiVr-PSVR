using UnityEngine;
using System.Collections;

public class OwlAnimation : MonoBehaviour {

    public Animation anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
        anim["Land"].speed = 2;
        anim["Flap"].speed = 2;
        anim["Hover"].speed = 2;
    }
	
    public bool isPlaying()
    {
        return anim.isPlaying;
    }

	public void PlayAnim(owlAnim a)
    {
        if (a == owlAnim.Flying)
            anim.CrossFade("Flap");
        else if (a == owlAnim.Gliding)
            anim.CrossFade("Glide");
        else if (a == owlAnim.Landing)
            anim.CrossFade("Land");
        else if (a == owlAnim.Stand)
            anim.CrossFade("Idle");
        else if (a == owlAnim.Hover)
            anim.CrossFade("Hover");
    }
}

public enum owlAnim
{
    Flying,
    Gliding,
    Landing,
    Stand,
    Hover
}
