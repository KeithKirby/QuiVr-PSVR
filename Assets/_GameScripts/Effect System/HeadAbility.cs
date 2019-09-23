using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Cam.Effects;
using Assets.Pixelation.Scripts;
public class HeadAbility : MonoBehaviour {

    public RetroSize RetroS;
    public Posterize Post;
    public Chunky Dither;

	void Start()
    {
        if(Armory.instance != null)
        {
            Armory.instance.OnEquip.AddListener(UpdateAbilities);
        }
        UpdateAbilities();
    }

    void UpdateAbilities(ArmorOption item=null)
    {
        List<int> ids = new List<int>();
        if (item == null)
        {
            foreach (var v in Armory.ArmorEffects())
            {
                ids.Add(v.EffectID);
            }
        }
        else
            ids.Add(item.GetEffect().EffectID);
        CheckAbilities(ids);
    }

    void CheckAbilities(List<int> IDs)
    {
        //Old Pixel
        if(IDs.Contains(18))
        {
            Post.enabled = true;
            RetroS.enabled = true;
        }
        else
        {
            Post.enabled = false;
            RetroS.enabled = false;
        }

        //Black and White Dithering
        if (IDs.Contains(19))
            Dither.enabled = true;
        else
            Dither.enabled = false;
    }

}
