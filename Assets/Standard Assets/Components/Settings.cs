using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;

public class Settings
{
    #region Variables

    private static string path = System.IO.Path.Combine(Application.persistentDataPath, "config.ini");
    private static Dictionary<string, string> IniDictionary = new Dictionary<string, string>();
    private static bool Initialized = false;
    private static bool invalidPath = false;

    #endregion

    #region Set Methods

    /// <summary>
    /// Write data to INI file.
    /// </summary>
    /// <param name="_Key"></param>
    /// <param name="_Value"></param>
    public static void Set(string _Key, string _Value)
    {
        if (!Initialized)
            Initialized = FirstRead();
        PopulateIni(_Key, _Value);
        //write ini
        WriteIni();
    }

    public static void Set(string _Key, object _Value)
    {
        Set(_Key, _Value.ToString());
    }

    #endregion

    #region Get Methods

    /// <summary>
    /// Read data from INI file.
    /// </summary>
    /// <param name="_Key"></param>
    /// <returns></returns>
    public static string Get(string _Key)
    {
        if (!Initialized)
        {
            Initialized = FirstRead();
        }
        if (IniDictionary.ContainsKey(_Key))
        {
            return IniDictionary[_Key];
        }
        return null;
    }

    public static int GetInt(string _Key, int defaultVal = 0)
    {
        string v = Get(_Key);
        int x = defaultVal;
        if (v != null)
        {
            string[] pts = v.Split('.');
            int.TryParse(pts[0], out x);
        }
        return x;
    }

    public static float GetFloat(string _Key, float defaultVal = 0)
    {
        string v = Get(_Key);
        float x = defaultVal;
        if (v != null)
            float.TryParse(v, out x);
        return x;
    }

    public static bool GetBool(string _Key, bool defaultVal = false)
    {
        string v = Get(_Key);
        if (v != null)
            return v == "true" || v == "True" || v == "TRUE";
        return defaultVal;
    }

    public static Quaternion GetQuat(string _Key)
    {
        string v = Get(_Key);
        if (v.Length < 4)
            return Quaternion.identity;
        string[] temp = v.Substring(1, v.Length - 2).Split(',');
        float x, y, z, w = 0;
        float.TryParse(temp[0], out x);
        float.TryParse(temp[1], out y);
        float.TryParse(temp[2], out z);
        float.TryParse(temp[3], out w);
        Quaternion rValue = new Quaternion(x, y, z, w);
        return rValue;
    }

    public static Vector3 GetV3(string _Key)
    {
        string v = Get(_Key);
        if (v == null || v.Length < 4)
            return Vector3.zero;
        string[] temp = v.Substring(1, v.Length - 2).Split(',');
        float x, y, z = 0;
        float.TryParse(temp[0], out x);
        float.TryParse(temp[1], out y);
        float.TryParse(temp[2], out z);
        Vector3 rValue = new Vector3(x, y, z);
        return rValue;
    }

    public static Vector2 GetV2(string _Key)
    {
        string v = Get(_Key);
        if (v == null || v.Length < 4)
            return Vector2.zero;
        string[] temp = v.Substring(1, v.Length - 2).Split(',');
        float x, y = 0;
        float.TryParse(temp[0], out x);
        float.TryParse(temp[1], out y);
        Vector2 rValue = new Vector2(x, y);
        return rValue;
    }

    public static string[] GetStringArray(string _Key)
    {
        string v = Get(_Key);
        if (v == null)
            return new string[] { };
        return v.Split(',');
    }

    public static bool HasKey(string _Key)
    {
        var v = Get(_Key);
        return v != null;
    }

    #endregion

    #region System Methods

    private static bool FirstRead()
    {
#if UNITY_PS4
        if (PlayerPrefs.HasKey("SettingsData"))
        {
            var sd = PlayerPrefs.GetString("SettingsData");
            using (StringReader sr = new StringReader(sd))
            {
                string line;
                string theKey = "";
                string theValue = "";
                while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                {
                    line.Trim();
                    string[] ln = line.Split('=');
                    if (ln.Length >= 2)
                    {
                        theKey = ln[0].Trim();
                        theValue = ln[1].Trim();
                    }
                    if (theKey == "" || theValue == "")
                        continue;
                    PopulateIni(theKey, theValue);
                }
            }
        }
#else
        if (File.Exists(path)) {
            using (StreamReader sr = new StreamReader(path)) {
                string line;
                string theKey = "";
                string theValue = "";
                while (!string.IsNullOrEmpty(line = sr.ReadLine())) {
                    line.Trim();
                    string[] ln = line.Split('=');
                    if(ln.Length >= 2)
                    {
                        theKey = ln[0].Trim();
                        theValue = ln[1].Trim();
                    }
                    if (theKey == "" || theValue == "")
                        continue;
                    PopulateIni(theKey, theValue);
                }
            }
        }
#endif
        return true;
    }

    private static void PopulateIni(string _Key, string _Value)
    {
        if (IniDictionary.ContainsKey(_Key))
            IniDictionary[_Key] = _Value;
        else
            IniDictionary.Add(_Key, _Value);
    }

    private static void WriteIni()
    {
#if UNITY_PS4
        StringBuilder sb = new StringBuilder();
        using (StringWriter sw = new StringWriter(sb))
        {
            foreach (KeyValuePair<string, string> val in IniDictionary)
            {
                string value = val.Value.ToString();
                value = value.Replace(Environment.NewLine, " ");
                value = value.Replace("\r\n", " ");
                sw.WriteLine(val.Key.ToString() + " = " + value);
            }
            sw.Flush();
            var str = sb.ToString();
            PlayerPrefs.SetString("SettingsData", str);
        }        

#else
        using (StreamWriter sw = new StreamWriter(path)) {
            foreach (KeyValuePair<string, string> val in IniDictionary) {
				string value = val.Value.ToString();
				value = value.Replace(Environment.NewLine, " ");
				value = value.Replace("\r\n", " ");
				sw.WriteLine(val.Key.ToString() + " = " + value);
            }
        }
#endif
    }

    #endregion

    public static void ReplaceSettings(string rep)
    {
        if (!Initialized)
            Initialized = FirstRead();
        string[] pts = rep.Split(',');
        foreach (var val in pts)
        {
            string[] parts = val.Split('=');
            if (parts.Length == 2)
            {
                Set(parts[0], parts[1]);
            }
        }
    }
}