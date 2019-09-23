using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OwlGems : MonoBehaviour {

    public GameObject[] Gems;
    int broken;

    public ArmorOption SuccessItem;

    AudioSource src;

    IEnumerator Start()
    {
        src = GetComponent<AudioSource>();
        Clear();
        while(!SpecialItems.IsReady())
        {
            yield return true;
        }
        Reset();
    }

    public void Break()
    {
        broken++;
        src.pitch = 0.8f + (0.05f * broken);
        src.Play();
        CheckBroken();
    }

    void CheckBroken()
    {
        if(broken >= Gems.Length)
        {
            //SpecialItems.GiveItem(SuccessItem, true);
            Achievement.EarnAchievement("OWL_GEMS", true);
        }
    }

    [BitStrap.Button]
    public void TestAch()
    {
        Achievement.EarnAchievement("OWL_GEMS", true);
    }

    public void Reset()
    {
        broken = 0;
        foreach (var v in Gems)
        {
            v.SetActive(true);
        }       
    }

    void Clear()
    {
        foreach (var v in Gems)
        {
            v.SetActive(false);
        }
    }
}
