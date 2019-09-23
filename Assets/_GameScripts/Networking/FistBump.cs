using UnityEngine;
using System.Collections;

public class FistBump : MonoBehaviour {

    public Transform Particles;
    public float cooldown = 1f;
    float cd;
    HandAnim myAnim;

    void Start()
    {
        myAnim = GetComponent<HandAnim>();
    }

    void Update()
    {
        if (cd >= 0)
            cd -= Time.deltaTime;
    }

	void OnTriggerEnter(Collider col)
    {
        if(cd <= 0 && col.tag == "OtherPlayer")
        {
            HandAnim ha = col.gameObject.GetComponent<HandAnim>();
            if (ha == null)
                ha = col.GetComponentInChildren<HandAnim>();
            if(ha != null && myAnim != null)
            {
                if(ha.GripValue > 0.8f && myAnim.GripValue > 0.8f)
                {
                    Vector3 midPt = (col.transform.position + transform.position) / 2f;
                    PlayParticles(midPt);
                    cd = cooldown;
                }
            }
        }
    }

    public void PlayParticles(Vector3 pos)
    {
        Particles.position = pos;
        Particles.GetComponent<ParticleSystem>().Play();
    }
}
