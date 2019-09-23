using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToward : MonoBehaviour {

    public float Speed = 1.5f;
    public Transform Target;
    public bool DestroyOnReach;
    public float CollisionRadius = 0.1f;
    [AdvancedInspector.Inspect]
    public AnimationCurve YChange;
    public UnityEvent OnCollide;

    float totaldist;
    float startY;
    public void Setup(Transform targ)
    {
        Target = targ;
        totaldist = Vector3.Distance(transform.position, targ.position);
        oldPos = transform.position;
    }

    void Start()
    {
        if (Target != null)
            Setup(Target);
    }

    Vector3 oldPos;
    void Update()
    {
        if(Target != null)
        {
            float curDist = Vector3.Distance(transform.position, Target.position);
            if (curDist <= CollisionRadius)
            {
                OnCollide.Invoke();
                if(DestroyOnReach)
                    Destroy(gameObject);
            }
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Speed*Time.deltaTime);
            float perc = Mathf.Clamp(curDist / totaldist, 0f, 1f);
            transform.position += Vector3.up * YChange.Evaluate(perc) * Time.deltaTime;
        }
    }

}
