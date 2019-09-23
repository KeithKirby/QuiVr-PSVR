using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour {

    static List<Creature> Creatures;
    public static int MAX_ENEMIES = 20;
    public static float EnemyDifficulty;
    public static bool bossOut;
    public const int  MAX_RAGDOLL = 5;
    public static int CurRagdolls;

    public static bool InGame()
    {
        return (GameBase.instance.inGame && GameBase.instance.Difficulty > 0) || (pvpmanager.instance.PlayingPVP);
    }

    public static int GetNum(string enemyName)
    {
        CheckList();
        int num = 0;
        foreach(var v in Creatures)
        {
            if (v.type.Name == enemyName)
                num++;
        }
        return num;
    }

    public static List<Creature> AllEnemies()
    {
        CheckList();
        return Creatures;
    }

    public static void AddCreature(Creature c)
    {
        CheckList();
        if (!Creatures.Contains(c) && !c.isDead())
            Creatures.Add(c);
    }

    static void CheckList()
    {
        if (Creatures == null)
            Creatures = new List<Creature>();
    }


    public static void ClearInvalid()
    {
        CheckList();
        for (int i = Creatures.Count - 1; i >= 0; i--)
        {
            var v = Creatures[i];
            if (v == null || v.GetComponent<Health>().isDead())
            {
                Creatures.RemoveAt(i);
            }
        }
    }

    public static int EnemyNum()
    {
        CheckList();
        return Creatures.Count;
    }

    public static float FindClosestEnemy(Vector3 pos)
    {
        float x = float.MaxValue;
        foreach (var v in AllEnemies())
        {
            if (v != null)
            {
                float dist = Vector3.Distance(pos, v.transform.position);
                if (dist < x)
                    x = dist;
            }
        }
        return x;
    }


    public static float CurrentForceValue()
    {
        float val = 0;
        foreach (var v in AllEnemies())
        {
            if (v != null)
                val += v.IsElite ? 3f : 1f;
        }
        return val;
    }

    public static int CloseToGate()
    {
        var currentGate = GateManager.CurrentGate();
        int numCloseToGate = 0;
        if (null != currentGate)
        {
            
            Vector3 pos = GateManager.CurrentGate().transform.position;
            foreach (var v in AllEnemies())
            {
                if (v != null)
                {
                    float dist = Vector3.Distance(pos, v.transform.position);
                    if (dist < 25f)
                        numCloseToGate++;
                }
            }
        }
        return numCloseToGate;
    }
}
