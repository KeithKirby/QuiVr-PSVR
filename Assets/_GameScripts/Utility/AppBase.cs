using UnityEngine;
using System.Collections;

public class AppBase : MonoBehaviour
{
    public AppValues values;
    public AchieveValues achieves;

    public static AppValues v;
    public static AchieveValues a;

    void Awake()
    {
        if(v == null)
            v = values;
        if(a == null)
            a = achieves;
    }

    public static Achieve GetAchievement(string id)
    {
        if(a != null)
        {
            foreach(var v in a.Achievements)
            {
                if (v.ID == id)
                    return v;
            }
        }
        return null;
    }
}
