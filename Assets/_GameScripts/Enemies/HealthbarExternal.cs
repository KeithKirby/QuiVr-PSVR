using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarExternal : MonoBehaviour {

    Image Bar;
    SpriteRenderer Sprite;
    public Health HP;
    float dispVal;
    float lastHP;

    void Awake()
    {
        Bar = GetComponent<Image>();
        Sprite = GetComponent<SpriteRenderer>();
        if (Sprite != null && Sprite.material.HasProperty("_Fill"))
        {
            validSprite = true;
            SpriteMat = Sprite.material;
        }

    }

    bool validSprite;
    Material SpriteMat;
    void Update()
    {
        float fillAmt = 1;
        if(Bar != null)
            fillAmt = Bar.fillAmount;
        if (validSprite)
            fillAmt = SpriteMat.GetFloat("_Fill");
        if (HP != null)
        {
            if(HP.currentHP != lastHP)
            {
                lastHP = HP.currentHP;
            }
            dispVal = Mathf.Lerp(dispVal, lastHP, Time.deltaTime);
            float f = Mathf.Round(dispVal);
            float newFill = f / HP.maxHP;
            if (newFill != fillAmt)
            {
                if(Bar != null)
                    Bar.fillAmount = newFill;
                if (validSprite)
                    SpriteMat.SetFloat("_Fill", newFill);
            }

        }
    }
}
