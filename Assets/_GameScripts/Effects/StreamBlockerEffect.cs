using UnityEngine;
using System.Collections;
using SWS;

public class StreamBlockerEffect : MonoBehaviour {

    public ParticleSystem OutParticles;
    public ParticleSystem InParticles;

    public PathDisplay[] Paths;

    public static StreamBlockerEffect instance;
    void Awake()
    {
        instance = this;
    }

    public void TogglePathDisplay(PathManager path, bool on)
    {
        for(int i=0; i<Paths.Length; i++)
        {
            var v = Paths[i];
            if(v.path == path)
            {
                v.isOn = on;
                if(v.display != null)
                {
                    StreamDisplay disp = v.display.GetComponent<StreamDisplay>();
                    if (disp != null)
                    {
                        if (on)
                            disp.TurnOn();
                        else
                            disp.TurnOff();
                    }
                    else if (v.display.activeSelf != on)
                        v.display.SetActive(v.isOn);
                }
                return;
            }
        }
    }

    public void Disabled(Vector3 pos)
    {
        OutParticles.transform.position = pos;
        OutParticles.Play();
    }

    public void Enabled(Vector3 pos)
    {
        InParticles.transform.position = pos;
        InParticles.Play();
    }
}

[System.Serializable]
public struct PathDisplay
{
    public PathManager path;
    public bool isOn;
    public GameObject display;
}
