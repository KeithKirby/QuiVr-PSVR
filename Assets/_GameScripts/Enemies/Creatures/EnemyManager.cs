using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;

    public List<GameObject> Enemies;

    void Awake()
    {
        Enemies = new List<GameObject>();
        instance = this;
    }

    void Update()
    {
        for(int i=Enemies.Count-1; i>=0; i--)
        {
            if(Enemies[i] == null)
            {
                Enemies.RemoveAt(i);
            }
            else if(Enemies[i].GetComponent<Health>() != null && Enemies[i].GetComponent<Health>().isDead())
            {
                Enemies.RemoveAt(i);
            }
        }
    }

}
