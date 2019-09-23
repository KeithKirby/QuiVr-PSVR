using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour {

    public static EnvironmentManager instance;
    public static EnvironmentType curEnv;
    public float ChangeDur;
    public SkyChanger Sky;
    public EnvParticles[] Particles;
    public EnvAudio[] AudioLoops;
    List<SkySetting> SkyOptions;

    public bool UseDynamicMusic;

    void Awake()
    {
        instance = this;
        Sky.ChangeDur = ChangeDur;
    }

    public static void ChangeEnvImmediate(EnvironmentType type)
    {
        if (instance != null)
        {
            curEnv = type;
            instance.SetSky(type);
            instance.ActivateParticles(type);
#if !UNITY_EDITOR
            instance.ActivateAudio(type);
#endif
        }
    }

    public static void ChangeEnv(EnvironmentType type)
    {
        if(instance != null && curEnv != type)
        {
            curEnv = type;
            instance.SetSky(type);
            instance.ActivateParticles(type);
            instance.ActivateAudio(type);
        }
    }

#region Sky
    public void SetSky(EnvironmentType env)
    {
        SkyColors sc = GetSky(env);
        if (sc != null)
            Sky.ChangeSky(sc);
    }

    SkyColors GetSky(EnvironmentType env)
    {
        if(SkyOptions != null && SkyOptions.Count > 0)
        {
            foreach (var v in SkyOptions)
            {
                if (v.env == env)
                    return v.SkyCol;
            }
        }
        else
        {
            if (env == EnvironmentType.Snow)
                return Sky.SkyDB.Skies[3];
            else if (env == EnvironmentType.Desert)
                return Sky.SkyDB.Skies[6];
        }
        return null;
    }

    public void SetupSkys(TileManager.RandomValue rand)
    {
        SkyValues db = Sky.SkyDB;
        SkyOptions = new List<SkySetting>();
        foreach(EnvironmentType v in System.Enum.GetValues(typeof(EnvironmentType)))
        {
            List<SkyColors> skies = db.GetColors(v);
            if(skies.Count > 0)
            {
                SkySetting s = new SkySetting();
                s.env = v;
                s.SkyCol = skies[rand.Next(0, skies.Count)];
                SkyOptions.Add(s);
            }
        }
    }

    [System.Serializable]
    public class SkySetting
    {
        public EnvironmentType env;
        public SkyColors SkyCol;
    }
#endregion

#region Particles
    public void ActivateParticles(EnvironmentType env)
    {
        foreach(var v in Particles)
        {
            if(v.env == env)
            {
                foreach(var g in v.Systems)
                {
                    if(!g.activeSelf)
                        g.SetActive(true);
                }
            }
            else
            {
                foreach (var g in v.Systems)
                {
                    if(g.activeSelf)
                        g.SetActive(false);
                }
            }
        }
    }

    [System.Serializable]
    public class EnvParticles
    {
        public GameObject[] Systems;
        public EnvironmentType env;

        public override string ToString()
        {
            return env.ToString();
        }
    }
#endregion

#region Audio

    public void ActivateAudio(EnvironmentType env)
    {
        foreach (var v in AudioLoops)
        {
            if (v.env == env && v.Loop.Length > 0)
            {
                if(UseDynamicMusic)
                {
                    if (v.env == EnvironmentType.Olympus)
                    {
                        MusicChanger.instance.paused = false;
                        MusicChanger.instance.SetDynamicMusic(false);
                        MusicChanger.PlayClip(v.Loop, v.MaxVol);
                    }
                    else
                    {
                        MusicChanger.instance.CurClip = "";
                        MusicChanger.instance.SetDynamicMusic(true);
                        MusicChanger.instance.FadeOutAudio(1f);
                    }
                }
                else
                {
                    MusicChanger.instance.SetDynamicMusic(false);
                    MusicChanger.PlayClip(v.Loop, v.MaxVol);
                }
            }
        }
    }

    [System.Serializable]
    public class EnvAudio
    {
        public string Loop;
        public float MaxVol;
        public EnvironmentType env;

        public override string ToString()
        {
            return env.ToString();
        }
    }
#endregion
}
