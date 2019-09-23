using UnityEngine;
using System.Collections;

public class HealOnDeath : MonoBehaviour {

    public GameObject HealEffect;

	public void CreatureDied(GameObject cr, bool dummy)
    {
        GameObject eft = (GameObject)Instantiate(HealEffect, cr.transform.position + Vector3.up, Quaternion.identity);
        eft.GetComponent<Effect>().Setup(dummy, 1);
    }
}
