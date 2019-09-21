using UnityEngine;
using System.Collections;

public class DynamicMusic : MonoBehaviour {

    public static DynamicMusic instance;
#if !OCULUS_GO
	public infinite_fantasy_pro fantasy;

    void Awake()
    {
        instance = this;
    }

	IEnumerator Start () {
        yield return true;
        UseHeroic();
        Medium();
    }

    public void UseHeroic()
    {
        if (fantasy == null)
            return;
        fantasy.Heroic_onClick();
    }

    public void UseAdventure()
    {
        if (fantasy == null)
            return;
        fantasy.Adventure_onClick();
    }

    public void Toggle()
    {
        if (fantasy == null)
            return;
        if (fantasy.start)
        {
            fantasy.Stop_onClick();
        }
        else
        {
            Debug.Log("Resuming Music");
            fantasy.Fantasy_Play();
        }
    }

    public void UseBattle()
    {
        if (fantasy == null)
            return;
        StartCoroutine("BattlePlay");
    }

    IEnumerator BattlePlay()
    {
        fantasy.StopAll();
        fantasy.Stop_onClick();
        yield return new WaitForSeconds(3.5f);
        fantasy.Battle_onClick();
        StopCoroutine("BattlePlay");
    }

    public void Soft()
    {
        if (fantasy == null)
            return;
        fantasy.Soft_onClick();
        fantasy.SetVolumes();
    }

    public void Medium()
    {
        if (fantasy == null)
            return;
        fantasy.Med_onClick();
        fantasy.SetVolumes();
    }

    public void Forte()
    {
        if (fantasy == null)
            return;
        fantasy.Forte_onClick();
        fantasy.SetVolumes();
	}
    
#endif
    
}
