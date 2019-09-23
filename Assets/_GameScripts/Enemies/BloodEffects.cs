using UnityEngine;
using System.Collections;

public class BloodEffects : MonoBehaviour {

    public GameObject[] Effects;

    public GameObject[] BigEffects;

    public void CreateEffect(ArrowCollision col)
    {
        CreateEffect(col.impactPos, col.firePos, gameObject, false);
    }

    public void CreateEffect(Vector3 pos, Vector3 lookAt, GameObject parent, bool big)
    {
        GameObject g;
        if (!big && Effects.Length > 0)
            g = (GameObject)Instantiate(Effects[Random.Range(0, Effects.Length)]);
        else if (BigEffects.Length > 0)
            g = (GameObject)Instantiate(BigEffects[Random.Range(0, BigEffects.Length)]);
        else
            return;
        g.transform.position = pos;
        g.transform.LookAt(pos - lookAt);
        g.transform.SetParent(parent.transform);
        Destroy(g, 8f);
    }
}
