using UnityEngine;
using System.Collections;
//using Parse;
using System.Collections.Generic;
using System;

abstract public class ScoreDatabase : MonoBehaviour
{
    abstract public void RequestScore(ScoreManager scr, string ScoreClass, int MaxAge, string UserID = "");
    abstract public void TopList(ScoreManager scr, string ScoreClass, int limit, int MaxAge);
    abstract public void SaveNewScore(ScoreManager scr, string ScoreClass, int score, int MaxAge, string UserID = "", string ScoreName = "");
    abstract public string[] GetMPIdAndComment();

    static public ScoreDatabase Inst;
}

[System.Serializable]
public class GenericScore
{
    public string name;
    public int score;
    public int age;
    public string ID;

    public GenericScore(string n, int s, int a, string id)
    {
        name = n;
        score = s;
        age = a;
        ID = id;
    }
}
