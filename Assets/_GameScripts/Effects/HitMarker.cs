using UnityEngine;
using System.Collections;

public class HitMarker : MonoBehaviour {

    public static HitMarker regular;
    public static HitMarker critical;

    public bool Crit;


    void Start()
    {
        if (Crit && critical == null)
            critical = this;
        else if (!Crit && regular == null)
            regular = this;
        else
            Destroy(gameObject);
    }

    public static void ShowHit(Vector3 position, bool crit)
    {
        if ((crit && critical == null) || (!crit && regular == null))
        {
            return;
        }
        ParticleSystem p = regular.GetComponent<ParticleSystem>();
        if (crit)
            p = critical.GetComponent<ParticleSystem>();
        ParticleSystem.EmitParams m = new ParticleSystem.EmitParams();
        m.position = position;
        p.Emit(m, 1);
    }
}
