using UnityEngine;
using System.Collections;

public class CrackDisplay : MonoBehaviour {

    public Texture[] CrackTextures;
    public bool overrideGate;
    Health h;
    Gate g;
    Projector p;
    Material m;

    void Awake()
    {
        h = GetComponentInParent<Health>();
        g = GetComponentInParent<Gate>();
        p = GetComponent<Projector>();
        m = new Material(p.material);
        p.material = m;
    }

    void Update()
    {
        if (!h.isDead() && (overrideGate || (!overrideGate && g.isClosed())))
        {
            p.enabled = true;
            int i = 0;
            if (h.currentHP < h.maxHP)
            {
                i = (int)Mathf.Floor((1-(h.currentHP / h.maxHP)) * CrackTextures.Length);
                if (i == CrackTextures.Length)
                    i--;
            }
            m.SetTexture("_ShadowTex", CrackTextures[i]);
        }
        else
            p.enabled = false;
    }
}
