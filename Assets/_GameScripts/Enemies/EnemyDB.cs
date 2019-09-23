using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyDB : MonoBehaviour {

    public EnemyValues DataSet;

    public static EnemyValues v = null;

    void Awake()
    {
        if (v == null)
            v = DataSet;
    }

    public static int EnemyFromDifficulty(float difficulty, CreatureType t = CreatureType.Any)
    {
        if (difficulty > 1)
        {
            List<Enemy> Possible = new List<Enemy>();
            foreach(var e in v.Enemies)
            {
                if(e.DifficultyThreshold < difficulty)
                {
                    if(t == CreatureType.Any || t == e.type)
                    {
                        int r = e.Rarity;
                        if (difficulty > e.HalfThreshold && e.HalfThreshold < 100)
                            r /= 2;
                        if (difficulty > e.StopThreshold)
                            r = 0;
                        int num = CreatureManager.GetNum(e.Name);
                        if (!PhotonNetwork.inRoom || EnemyStream.GetRealPlayerNum() < 2)
                        {
                            if (e.SPLimit > 0 && num >= e.SPLimit)
                            {
                                r = 0;
                            }
                        }
                        else if (e.MaxNum > 0 && num >= e.MaxNum)
                            r = 0;
                        for (int i = 0; i < r; i++)
                        {
                            Possible.Add(e);
                        }
                    }
                }
            }
            if(Possible.Count > 0)
            {
                Enemy picked = Possible[Random.Range(0, Possible.Count)];
                for(int i=0; i<v.Enemies.Length; i++)
                {
                    if (picked == v.Enemies[i])
                        return i;
                }
            }
        }
        return 0;
    }

    public static void SetValues(GameObject Enemy, float difficulty)
    {

    }
}
