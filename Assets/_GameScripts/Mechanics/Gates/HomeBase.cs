using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : MonoBehaviour {

    public int DamageOnHit=0;
    public GameObject LaserPrefab;
    public GameObject LaserImpact;

    Health hscr;

    void Awake()
    {
        hscr = GetComponent<Health>();
        if(hscr != null)
        {
            hscr.OnAttacker.AddListener(Attacked);
        }
    }

    public void Attacked(GameObject enemy)
    {
        if (DamageOnHit > 0 && null!=enemy)
        {
            Creature c = enemy.GetComponent<Creature>();
            if (c != null && c.type.status == CreatureStatus.Standard)
            {
                if (LaserPrefab != null)
                {
                    GameObject newLaser = Instantiate(LaserPrefab, LaserPrefab.transform.position, Quaternion.identity);
                    newLaser.transform.SetParent(transform);
                    newLaser.transform.LookAt(enemy.transform.position + Vector3.up);
                    newLaser.SetActive(true);
                    AudioSource src = newLaser.GetComponent<AudioSource>();
                    src.pitch = Random.Range(0.9f, 1.1f);
                    Destroy(newLaser, 5f);
                    if (LaserImpact != null)
                    {
                        GameObject NewImpact = Instantiate(LaserImpact, enemy.transform.position + Vector3.up, Quaternion.identity);
                        NewImpact.SetActive(true);
                        Destroy(NewImpact, 5f);
                    }
                }
                c.health.takeDamage(DamageOnHit);
            }
        }
    }
}
