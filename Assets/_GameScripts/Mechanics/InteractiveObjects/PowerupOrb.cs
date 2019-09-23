using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupOrb : MonoBehaviour {

    public GameObject Explosion;
    public int powerupID = -1;
    PowerupManager.PowerupInfo pup;

    static List<PowerupOrb> OrbsOut;

    void Start()
    {
        if (OrbsOut == null)
            OrbsOut = new List<PowerupOrb>();
        OrbsOut.Add(this);
        Invoke("Explode", 25f);
    }

    public void Setup(PowerupManager.PowerupInfo info)
    {
        powerupID = info.powerID;
        pup = info;
        RFX1_EffectSettingColor cols = GetComponent<RFX1_EffectSettingColor>();
        if (cols != null)
        {
            cols.Color = info.displayColor;
            cols.UpdateColor();
        }
    }

    public void Use()
    {
        PowerupManager.instance.UsePowerup(pup);
        Explode(true);
    }

    void Explode(bool used=false)
    {
        Explosion.transform.SetParent(null);
        Explosion.SetActive(true);
        if (used)
            Explosion.GetComponent<AudioSource>().enabled = true;
        Destroy(gameObject);
    }

	public void OnHit(ArrowCollision col)
    {
        Use();
    }

    void OnDestroy()
    {
        OrbsOut.Remove(this);
    }

    public static void RemoveAll()
    {
        if (OrbsOut == null)
            OrbsOut = new List<PowerupOrb>();
        for(int i=OrbsOut.Count-1; i>= 0; i--)
        {
            if (OrbsOut[i] != null)
                OrbsOut[i].Explode();
        }
        OrbsOut = new List<PowerupOrb>();
    }
}
