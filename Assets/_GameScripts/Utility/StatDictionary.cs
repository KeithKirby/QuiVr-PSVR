using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StatDictionary {

	public static string DictToString(Dictionary<string, object> dict)
    {
        string s = "";
        foreach(var v in dict.Keys)
        {
            s += v + ":" + dict[v].ToString() + "|";
        }
        return s.Substring(0, s.Length - 1);
    }

    public static string DictToString(Dictionary<string, Vector2> dict)
    {
        string s = "";
        foreach (var v in dict.Keys)
        {
            s += v + ":" + dict[v].x + "," + dict[v].y + "|";
        }
        return s.Substring(0, s.Length - 1);
    }

    public static Dictionary<string, float> FloatDict(string s)
    {
        Dictionary<string, float> dict = new Dictionary<string, float>();
        string[] pairs = s.Split('|');
        foreach(var v in pairs)
        {
            string[] split = v.Split(':');
            if (split.Length > 1)
            {
                float f = 0;
                float.TryParse(split[1], out f);
                dict.Add(split[0], f);
            }
        }
        return dict;
    }
    public static Dictionary<string, Vector2> V2Dict(string s)
    {
        Dictionary<string, Vector2> dict = new Dictionary<string, Vector2>();
        string[] pairs = s.Split('|');
        foreach (var v in pairs)
        {
            string[] split = v.Split(':');
            if (split.Length > 1)
            {
                float x = 0;
                float y = 0;
                var t = split[1].Split(',');
                if(t.Length > 1)
                {
                    float.TryParse(t[0], out x);
                    float.TryParse(t[1], out y);
                }
                dict.Add(split[0], new Vector2(x,y));
            }
        }
        return dict;
    }

}
