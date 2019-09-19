using UnityEngine;
using System.Collections;

public class PlayerLife : MonoBehaviour {

    public bool isDead;
    public float respawnTime;
    public float invincibleTime;
    public bool invincible;
    public int BaseGateDamage;
    int GateDamage;

    public UnityStandardAssets.ImageEffects.Grayscale gscale;
    public AudioSource ambiance;
    public AudioClip SelfDeathSound;
    public AudioClip SelfReviveSound;
    public AudioClip DeathSound;
    public AudioClip ReviveSound;

    public static PlayerLife myInstance;

    void Awake()
    {
        myInstance = this;
        ResetGateDamage();
    }

    public void SetGateDamage(int val)
    {
        GateDamage = val;
    }

    public void ResetGateDamage()
    {
        GateDamage = BaseGateDamage;
    }

    void Start()
    {
        try
        {
            /*
            float deathTimer = 0;
            Parse.ParseConfig.CurrentConfig.TryGetValue<float>("RespawnTime", out deathTimer);
            if(deathTimer > 0)
                respawnTime = deathTimer;
            
            float invTimer = 0;
            Parse.ParseConfig.CurrentConfig.TryGetValue<float>("InvincibleTime", out invTimer);
            if (invTimer > 0)
                invincibleTime = invTimer;
                */
            respawnTime = PlayerProfile.Profile.RespawnTime;
            invincibleTime = PlayerProfile.Profile.InvincibleTime;
        }
        catch
        {
            Debug.Log("Parse Config could not be accessed");
        }
    }

    public static void Kill()
    {
        if (myInstance != null)
            myInstance.Die();
    }

    public static bool dead()
    {
        return myInstance != null && myInstance.isDead;
    }

    [AdvancedInspector.Inspect]
    void DieButton()
    {
        Die(-1, false, null);
    }

	public void Die(float killTime = -1, bool ignoreTeleport=false, Teleporter TPOverride=null)
    {
        if (isDead || invincible || TestOptions.SInvulnerablePlayer)
            return;
        if(GameBase.instance != null)
            GameBase.instance.AddDeath();
        /*
        if (TPOverride != null && !ignoreTeleport)
            TPOverride.ForceUse();        
        else if (!ignoreTeleport)
        {
            if (TeleporterManager.instance != null)
            {
                if (TeleporterManager.instance.DeathTP != null)
                    TeleporterManager.instance.DeathTP.ForceUse();
                else
                {
                    Teleporter n = TeleporterManager.instance.Areas[TeleporterManager.instance.currentArea].StartTeleporter;
                    if (n != null)
                        n.ForceUse();
                }               
            }
            else
                TelePlayer.instance.TeleportClosest(TelePlayer.instance.currentNode);
        }
        */
        if(killTime != 0)
        {
            //transform.localScale = Vector3.one * 15;
            MakeAllEtherial();
            ambiance.time = Random.Range(0f, 5f);
            ambiance.Play();
            if (SelfDeathSound != null)
                VRAudio.PlayClipAtPoint(SelfDeathSound, transform.position, 1f, 1f, 0);
            gscale.enabled = true;
            StopCoroutine("ReviveTimed");
            if(killTime > 0)
                StartCoroutine("ReviveTimed", killTime);
            else
                StartCoroutine("ReviveTimed", respawnTime);
        }     
    }

    public void MakeAllEtherial()
    {
        isDead = true;
        foreach (var v in GetComponentsInChildren<EtherialSwap>())
        {
            v.MakeEtherial();
        }
    }

    IEnumerator ReviveTimed(float reviveTime)
    {
        yield return true;
        yield return new WaitForSeconds(reviveTime);
        Revive();
        invincible = true;
        Invoke("SetVulnerable", invincibleTime);
        StopCoroutine("ReviveTimed");
    }

    void SetVulnerable()
    {
        invincible = false;
    }

    public void Revive()
    {
        if (!isDead || invincible)
            return;
        isDead = false;
        //Do damage to current gate
        if(NetworkEffects.instance != null)
            NetworkEffects.instance.HealCurTarget(-GateDamage);
        //CheckDeathTP(); //Invoke("CheckDeathTP", 1.5f);     
        
        //Revive Effects
        ambiance.Stop();
        transform.localScale = Vector3.one;
        if (ReviveSound != null)
            VRAudio.PlayClipAtPoint(SelfReviveSound, transform.position, 1f, 1f, 0);
        gscale.enabled = false;
        foreach (var v in GetComponentsInChildren<EtherialSwap>())
        {
            v.RevertEtherial();
        }
    }

    void CheckDeathTP()
    {
        if (TeleporterManager.instance == null)
            return;
        if (TelePlayer.instance.currentNode == TeleporterManager.instance.DeathTP)
        {
            Teleporter n = TeleporterManager.instance.RespawnTP;
            if(n == null || n.Disabled)
                n = TeleporterManager.instance.Areas[TeleporterManager.instance.currentArea].StartTeleporter;
            if (n != null)
                n.ForceUse();
        }
    }

    public void PlayDeathSound(Vector3 pos)
    {
        if (DeathSound != null)
            VRAudio.PlayClipAtPoint(DeathSound, pos, 1f, 1, 1f, 10f);
    }

    public void PlayReviveSound(Vector3 pos)
    {
        if (ReviveSound != null)
            VRAudio.PlayClipAtPoint(ReviveSound, pos, 1f, 1, 1f, 10f);
    }
}
