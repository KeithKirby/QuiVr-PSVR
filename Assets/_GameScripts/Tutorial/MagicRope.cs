using UnityEngine;
using System.Collections;

public class MagicRope : MonoBehaviour {

    public FixedJoint j;

    [BitStrap.Button]
    public void Break()
    {
        Destroy(j);
        GetComponentInParent<Shackles>().BreakRope();
    }
}
