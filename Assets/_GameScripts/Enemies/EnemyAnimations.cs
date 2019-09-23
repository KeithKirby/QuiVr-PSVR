using UnityEngine;
using System.Collections;

public class EnemyAnimations : MonoBehaviour {

    public Animation anim;
    public float IdleSpeed;
    public string Idle;

    public float AttackSpeed;
    public float AttackOffset;
    public string[] Attacks;

    public float WalkSpeed;
    public string Walk;
    public string[] Runs;
    int RunID = -1;
    int LimpID = -1;

    public float LimpSpeed;
    public string[] Limps;

    public float HitSpeed;
    public string GetHit;

    public string[] ExtraAnims;

    public EnemyState state;

    [HideInInspector]
    public float speedMult = 1;

    EnemyAudio sound;

	public bool overrideAnimations;
    float baseAttackOffset;
    void Awake()
    {
        hp = GetComponent<Health>();
        RunID = -1;
        LimpID = -1;
        if (Runs.Length > 0)
            RunID = Random.Range(0, Runs.Length);
        if(Limps.Length > 0)
            LimpID = Random.Range(0, Limps.Length);
        baseAttackOffset = AttackOffset;
        gc = GetComponent<GroundCreature>();
    }

    void Start()
    {
        if (Runs.Length > 0)
            anim[Runs[RunID]].speed = WalkSpeed;
        else
            anim[Walk].speed = WalkSpeed;
        anim[Idle].speed = IdleSpeed;
        if(anim.GetClip(GetHit) != null)
            anim[GetHit].speed = HitSpeed;
        sound = GetComponent<EnemyAudio>();
        foreach (var v in Attacks)
        {
            anim[v].speed = AttackSpeed;
        }
        foreach (var v in Limps)
        {
            anim[v].speed = LimpSpeed;
        }
        ApplyAnims();
    }

    public void SetSpeed(float s)
    {
        if (s == 0)
            anim.Stop();
        else if (!anim.isPlaying)
            anim.Play();
        if (RunID < 0)
            anim[Walk].speed = WalkSpeed*s;
        else
            anim[Runs[RunID]].speed = WalkSpeed*s;
        anim[Idle].speed = IdleSpeed*s;
    }

    public void RefreshSpeeds()
    {
        anim[Idle].speed = IdleSpeed*speedMult;
        if (RunID >= 0)
            anim[Runs[RunID]].speed = WalkSpeed*speedMult;
        else
            anim[Walk].speed = WalkSpeed;
        if (LimpID >= 0)
            anim[Limps[LimpID]].speed = LimpSpeed*speedMult;
        foreach(var v in Attacks)
        {
            anim[v].speed = AttackSpeed * speedMult;
        }
        if (speedMult > 0)
            AttackOffset = baseAttackOffset / speedMult;
        else
            AttackOffset = float.MaxValue;
        foreach(var v in ExtraAnims)
        {
            anim[v].speed = speedMult;
        }
    }

    public void SetWalkSpeed(float f)
    {
        if (RunID < 0)
            anim[Walk].speed = f;
        else
            anim[Runs[RunID]].speed = f;
    }

    public void SetState(EnemyState s)
    {
        state = s;
        ApplyAnims();
    }

    public void ArrowHit(ArrowCollision e)
    {
        if (e.hitObj.tag != "Armor" && e.hitObj.tag != "LowDamage")
            PlayHit();
    }

    public void ApplyAnims()
    {
		if (overrideAnimations)
			return;
        if (state == EnemyState.dead)
            Remove();
        else if (state == EnemyState.walking)
            PlayWalking();
        else if (state == EnemyState.idle)
            PlayIdle();
    }

    public float GetAnimSpeed()
    {
        if (state == EnemyState.dead)
            return 1;
        else if (state == EnemyState.walking)
            return WalkSpeed;
        else if (state == EnemyState.attacking)
            return AttackSpeed;
        else if (state == EnemyState.idle)
            return IdleSpeed;
        return 1;
    }

	public void Remove()
    {
        anim.enabled = false;
    }

    public void PlayAnimation(string animation, float speed=1, float crossfadeTime = 1f)
    {
        if(anim.GetClip(animation) != null)
        {
            anim[animation].speed = speed;
            anim.CrossFade(animation, crossfadeTime);
        }
    }

    public void Revive()
    {
        anim.Stop();
        anim.enabled = true;
        PlayWalking();
    }

    public bool isPlaying()
    {
        return anim.isPlaying;
    }

    [BitStrap.Button]
    public void PlayWalking()
    {
        anim.CrossFade(WalkAnim());
    }

    public string WalkAnim()
    {
        if (limping && LimpID >= 0 && Limps.Length > 0)
            return Limps[LimpID];
        if (RunID < 0)
            return Walk;
        else
            return Runs[RunID];
    }

    Health hp;
    bool limping;
    GroundCreature gc;
    void Update()
    {
        float perc = 0.5f;
        if (gc != null)
            perc = gc.LimpThreshold;
        if(!limping && state == EnemyState.walking && hp.currentHP < (perc*hp.maxHP) && LimpID >= 0 && !gc.MovementPaused)
        {
            if(GetComponent<GroundCreature>() != null)
                GetComponent<GroundCreature>().SetLimping();
            limping = true;
            anim.CrossFade(Limps[LimpID]);
        }
    }

    public void PlayHit()
    {
		if (overrideAnimations || anim.GetClip(GetHit) == null)
			return;
        anim.CrossFade(GetHit);
        Invoke("ApplyAnims", 0.25f);
    }

    public void PlayAttack(bool crossfade=false)
    {
        int i = Random.Range(0, Attacks.Length);
        if (crossfade)
            anim.CrossFade(Attacks[i]);
        else
        {
            anim.Stop();
            if(Attacks.Length > 0)
                anim.Play(Attacks[i]);
        }
        if(!playing)
        {
            Invoke("PlayAttackSound", AttackOffset);
            playing = true;
        }
    }

    bool playing;
    void PlayAttackSound()
    {
        if (state != EnemyState.dead && sound != null)
            sound.PlayAttack();
        playing = false;
    }

    public void PlayIdle()
    {
        anim.CrossFade(Idle, 1f);
    }

}

