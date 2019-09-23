using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraEffectToggle : MonoBehaviour {

    public GameObject[] ToggleObjects;
    public int FXClass = 1;
    public bool destroyObject;
    public bool Enable;

    void Awake()
    {
        int setting = Settings.GetInt("EffectQuality", 1);
        if (destroyObject && setting < FXClass)
        {
            for (int i = 0; i < ToggleObjects.Length; i++)
            {
                if (ToggleObjects[i] != null)
                    Destroy(ToggleObjects[i]);
            }
            Destroy(this);
        }
        else if (Enable && setting > FXClass)
        {
            if (destroyObject)
            {
                for(int i=0; i<ToggleObjects.Length; i++)
                {
                    if(ToggleObjects[i] != null)
                        Destroy(ToggleObjects[i]);
                }
                Destroy(this);
            }
            else
            {
                for (int i = 0; i < ToggleObjects.Length; i++)
                {
                    if (ToggleObjects[i] != null)
                        ToggleObjects[i].SetActive(true);
                }
            }
        }
        else if(!Enable)
        InvokeRepeating("CheckRend", 0.5f, Random.Range(0.137f, 0.21763f));
    }

    void CheckRend()
    {
        bool shouldRender = true;
        if (Enable)
            shouldRender = Settings.GetInt("EffectQuality", 1) <= FXClass;
        else
            shouldRender = Settings.GetInt("EffectQuality", 1) >= FXClass;
        foreach(var v in ToggleObjects)
        {
            if(v != null && v.activeSelf != shouldRender)
            {
                v.SetActive(shouldRender);
            }
        }
    }

}
