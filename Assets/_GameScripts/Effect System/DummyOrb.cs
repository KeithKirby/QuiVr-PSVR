using UnityEngine;
using System.Collections;

public class DummyOrb : MonoBehaviour {

    int EffectID;
    GameObject disp;

    public void Setup(int id)
    {
        EffectID = id;
        if (disp != null)
            Destroy(disp);
        disp = (GameObject)GameObject.Instantiate(ItemDatabase.a.Effects[EffectID].Display.OrbEffect, transform);
        disp.transform.localPosition = Vector3.zero;
        disp.transform.localScale = Vector3.one;
        disp.transform.localEulerAngles = Vector3.zero;
    }
}
