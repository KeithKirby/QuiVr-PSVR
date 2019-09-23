using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Events;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;

public class OrbManager : MonoBehaviour {

    ObscuredFloat curtime;

    public CooldownDisplay display;
    GameObject freeHand;

    public GameObject OrbPrefab;

    public AudioClip SpawnOrb;
    public AudioClip[] CooldownComplete;

    GameObject curOrb;

    public static OrbManager instance;
    [System.Serializable]
    public class OrbEvent : UnityEvent<OrbAbility> { }
    public OrbEvent OnCreate;

    #region Base
    void Awake()
    {
        instance = this;
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
        if(SteamVR_ControllerManager.freeHand != null && SteamVR_ControllerManager.freeHand != freeHand)
        {
            if (freeHand != null)
                freeHand.GetComponent<VRTK_ControllerEvents>().TouchpadPressed -= TryUse;
            freeHand = SteamVR_ControllerManager.freeHand;
            freeHand.GetComponent<VRTK_ControllerEvents>().TouchpadPressed += TryUse;
        }
        curEffect = GetCurEffect();
        if (curEffect != null && (curOrb == null || curOrb.GetComponent<OrbAbility>().wasThrown()))
        {
            curtime -= Time.deltaTime;
            float perc = 1f - (curtime / curEffect.StaticValue);
            if (curEffect.randomType == RandomType.Recharge)
                perc = 1f - (curtime / (curEffect.VariableValue - cval));
            if (perc > 0.9f && NVR_Player.instance.NonVR && NVR_Player.instance.pos.forceFirstPerson)
            {
                curtime = curEffect.StaticValue - 5f;
                if (curEffect.randomType == RandomType.Recharge)
                    curtime = (curEffect.VariableValue - cval) - 5f;
            }
            display.SetValue(perc);
            if(perc >= 1 && !cooldownComplete)
            {
                CompleteCooldown();
            }
        }
        else if(curEffect == null)
        {
            curtime = 10;
            display.SetValue(0);
        }
    }

    #endregion

    #region Cooldown
    public void ResetCooldown()
    {
        curtime = 0;
    }

    void CompleteCooldown()
    {
        if(CooldownComplete.Length > 0)
        {
            VRAudio.PlayClipAtPoint(CooldownComplete[Random.Range(0, CooldownComplete.Length)], display.transform.position, 1);
            cooldownComplete = true;
        }
    }

    public void SetCooldown()
    {
        if(curEffect != null)
        {
            float baseCD = curEffect.StaticValue;
            if (curEffect.randomType == RandomType.Recharge)
                baseCD = curEffect.VariableValue - cval;
            curtime = baseCD;
        }
        else
            curtime = 10f;
        cooldownComplete = false;
    }

    #endregion

    #region Throwing
    public bool CanThrow()
    {
        return curtime <= 0 && curEffect != null;
    }

    public void TryUse(object sender, ControllerInteractionEventArgs e)
    {
        if(!ArrowEffects.EffectsDisabled && CanThrow() && freeHand.GetComponent<VRTK_InteractGrab>().GetGrabbedObject() == null)
        {
            if (SpawnOrb != null)
                VRAudio.PlayClipAtPoint(SpawnOrb, freeHand.transform.position, 1);
            SetCooldown();
            ReadyOrb();
            curOrb = (GameObject)Instantiate(OrbPrefab, freeHand.GetComponentInChildren<PullAnchor>().transform.transform.position, Quaternion.identity);
            freeHand.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
            freeHand.GetComponent<VRTK_InteractTouch>().ForceTouch(curOrb);
            freeHand.GetComponent<VRTK_InteractGrab>().AttemptGrab();
            display.SetValue(0);
            OrbAbility oa = curOrb.GetComponent<OrbAbility>();
            if(OnCreate != null)
                OnCreate.Invoke(oa);
            oa.Setup(curEffect.EffectID, cval, true);
        }
    }

    public void SetThrown()
    {
        if (PhotonNetwork.inRoom)
        {
            PlayerSync.myInstance.ToggleDummyOrb(false, curEffect.EffectID, false);
            PlayerSync.myInstance.ThrowOrb(curEffect.EffectID, curOrb.transform.position, curOrb.GetComponent<Rigidbody>().velocity, curOrb.GetComponent<Rigidbody>().angularVelocity, curOrb.transform.rotation);
        }
    }

    public void FakeOrb(int id, Vector3 pos, Vector3 vel, Vector3 avel, Quaternion rot)
    {
        GameObject fake = (GameObject)Instantiate(OrbPrefab, pos, rot);
        fake.GetComponent<VRTK_InteractableObject>().enabled = false;
        fake.GetComponent<Rigidbody>().velocity = vel;
        fake.GetComponent<Rigidbody>().angularVelocity = avel;
        fake.GetComponent<OrbAbility>().Setup(id, 0, false);
    }

    public void ThrowNVR(float power, Vector3 pos, Vector3 dir)
    {
        SetCooldown();
        //Object Setup
        curOrb = (GameObject)Instantiate(OrbPrefab, pos, Quaternion.identity);
        curOrb.GetComponent<VRTK_InteractableObject>().enabled = false;
        display.SetValue(0);
        OrbAbility oa = curOrb.GetComponent<OrbAbility>();
        if (OnCreate != null)
            OnCreate.Invoke(oa);
        oa.Setup(curEffect.EffectID, cval, true);
        Rigidbody rb = curOrb.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = dir * power;
        //Throw
        oa.thrown = true;
        SetThrown();
        Destroy(oa.gameObject, 20);
    }

    #endregion

    #region Utility

    public int GetEffectID()
    {
        if (curEffect != null)
            return curEffect.EffectID;
        return 0;
    }

    public void ReadyOrb()
    {
        if (PhotonNetwork.inRoom && curEffect != null)
            PlayerSync.myInstance.ToggleDummyOrb(!Settings.GetBool("LeftHanded"), curEffect.EffectID, true);
    }

    ItemEffect curEffect;
    float cval;
    ItemEffect GetCurEffect()
    {
        if (Armory.instance != null)
        {
            List<EffectInstance> efts = Armory.ArmorEffects();
            foreach (var v in efts)
            {
                ItemEffect e = ItemDatabase.GetEffect(v.EffectID);
                if (e.type == EffectType.Orb)
                {
                    cval = v.EffectValue;
                    return e;
                }
            }
        }
        return null;
    }

    void OnDestroy()
    {
        if (GameBase.instance != null)
            GameBase.instance.OnStartDifficulty.RemoveListener(ResetCooldown);
    }

    #endregion
}
