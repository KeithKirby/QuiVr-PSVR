using UnityEngine;
using System.Collections;
using VRTK;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;

public class BowAbility : MonoBehaviour {

    public ObscuredFloat curtime;
    public CooldownDisplay display;
    public static BowAbility instance;
    GameObject bowHand;
    public AudioClip ActivateClip;
    public AudioClip[] CooldownComplete;
    ItemEffect curEffect;
    public bool activated;

    void Awake () {
        instance = this;
        curtime = 10f;
	}

    void Start()
    {
        if (GameBase.instance != null)
            GameBase.instance.OnStartDifficulty.AddListener(ResetCooldown);
    }

    bool cooldownComplete;
    void Update()
    {
        if (ArrowEffects.EffectsDisabled)
            return;
        if (SteamVR_ControllerManager.bowHand != null && SteamVR_ControllerManager.bowHand != bowHand)
        {
            if (bowHand != null)
                bowHand.GetComponent<VRTK_ControllerEvents>().TriggerPressed -= TryUse;
            bowHand = SteamVR_ControllerManager.bowHand;
            bowHand.GetComponent<VRTK_ControllerEvents>().TriggerPressed += TryUse;
        }
        if (NVR_Player.isThirdPerson() && Input.GetKeyDown(KeyCode.LeftShift))
            UseNVR();
        curEffect = GetCurEffect();
        if (curEffect != null && BowAim.instance != null)
        {
            if(activated)
            {
                display.SetValue(0);
                if (BowAim.instance.GetArrow() == null)
                {
                    activated = false;
                }
            }
            else if(curtime >= 0)
            {
                curtime -= Time.deltaTime;
                float perc = 1f - (curtime / curEffect.StaticValue);
                if(curEffect.randomType == RandomType.Recharge)
                    perc = 1f - (curtime / (curEffect.VariableValue-cval));
                display.SetValue(perc);
                if (perc >= 1 && !cooldownComplete)
                {
                    CompleteCooldown();
                }
            }
        }
        else
        {
            curtime = 5;
            display.SetValue(0);
        }
    }

    void CompleteCooldown()
    {
        if (CooldownComplete.Length > 0)
        {
            VRAudio.PlayClipAtPoint(CooldownComplete[Random.Range(0, CooldownComplete.Length)], display.transform.position, 1);
            cooldownComplete = true;
        }
    }

    public void TryUse(object sender, ControllerInteractionEventArgs e)
    {
        if(!activated && curtime <= 0 && curEffect != null && BowAim.instance != null && BowAim.instance.GetArrow() != null && !ArrowEffects.EffectsDisabled)
        {
            float baseCD = curEffect.StaticValue;
            if (curEffect.randomType == RandomType.Recharge)
                baseCD = curEffect.VariableValue - cval;
            curtime = baseCD;
            activated = true;
            GameObject arrow = BowAim.instance.GetArrow();
            EffectInstance[] efts = Armory.ArmorEffects().ToArray();
            cooldownComplete = false;
            if (ActivateClip != null)
            {
                VRAudio.PlayClipAtPoint(ActivateClip, transform.position, 1);
            }
            foreach(var v in efts)
            {
                if(v.EffectID == curEffect.EffectID)
                {
                    arrow.GetComponent<ArrowEffects>().AddEffect(v.EffectID, v.EffectValue);
                    return;
                }
            }
        }
    }

    void UseNVR()
    {
        if (!activated && curtime <= 0 && curEffect != null && !ArrowEffects.EffectsDisabled)
        {
            float baseCD = curEffect.StaticValue;
            if (curEffect.randomType == RandomType.Recharge)
                baseCD = curEffect.VariableValue - cval;
            curtime = baseCD;
            activated = true;
            EffectInstance[] efts = Armory.ArmorEffects().ToArray();
            cooldownComplete = false;
            if (ActivateClip != null)
            {
                VRAudio.PlayClipAtPoint(ActivateClip, transform.position, 1);
            }
            foreach (var v in efts)
            {
                if (v.EffectID == curEffect.EffectID)
                {
                    NVR_Player.instance.SetArrowEffect(v.EffectID, v.EffectValue);
                    return;
                }
            }
        }
    }

    float cval;
    ItemEffect GetCurEffect()
    {
        if (Armory.instance != null)
        {
            List<EffectInstance> efts = Armory.ArmorEffects();
            foreach (var v in efts)
            {
                if (ItemDatabase.GetEffect(v.EffectID).type == EffectType.Activation)
                {
                    cval = v.EffectValue;
                    return ItemDatabase.GetEffect(v.EffectID);
                }
            }
        }
        return null;
    }

    public void ResetCooldown()
    {
        curtime = 0;
    }

    void OnDestroy()
    {
        if (GameBase.instance != null)
            GameBase.instance.OnStartDifficulty.RemoveListener(ResetCooldown);
    }
}
