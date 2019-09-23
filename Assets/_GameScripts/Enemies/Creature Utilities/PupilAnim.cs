using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilAnim : MonoBehaviour {

    public Animator anim;
    public float VariationSpeed = 1f;
    [Range(0, 3)]
    public float ChangeTime = 2f;
    [Range(0, 1.5f)]
    public float ChangeVar = 0.4f;
    public Transform LookTarget;

    //Runtime
    float curDil;
    float targetDil;
    bool inSpecial;
    float t;

    double lastT;
    void Update()
    {
        if(anim != null)
        {
            if(!inSpecial)
            {
                if(t <= 0)
                {
                    targetDil = Random.Range(0f, 1f);
                    t = ChangeTime + Random.Range(-ChangeVar, ChangeVar);
                    if(LookTarget != null)
                    {
                        float dist = Vector3.Distance(transform.position, LookTarget.position)/8f;
                        Vector3 newPos = LookTarget.transform.localPosition + new Vector3(Random.Range(-dist, dist), Random.Range(-dist, dist), Random.Range(-dist, dist));
                        newPos.x = Mathf.Clamp(newPos.x, -5, 5);
                        newPos.y = Mathf.Clamp(newPos.y, -1, 2);
                        newPos.z = Mathf.Clamp(newPos.z, -5, 5);
                        LookTarget.transform.localPosition = newPos;

                    }

                }
            }
            curDil = Mathf.Lerp(curDil, targetDil, Time.deltaTime*VariationSpeed);
            t -= Time.deltaTime;
            anim.Play("Pupil", -1, curDil);
        }
    }
}
