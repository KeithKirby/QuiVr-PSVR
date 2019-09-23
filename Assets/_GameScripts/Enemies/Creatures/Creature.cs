using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Profiling;

[RequireComponent (typeof(Health))]
public abstract class Creature : MonoBehaviour {

    public List<GameObject> Dropables;
    public Transform WeaponHolder;
    public GameObject Weapon;
    public float Damage;
    public EnemyState state;
    public GameObject skinMesh;
    [HideInInspector]
    public Renderer skin;
    public Transform RootBone;
    public float modelScale;
    public Renderer[] armors;
    public float AttackDistance;
    public ParticleSystem DisplayParticles;
    [HideInInspector]
    public GameObject Target;
    Health targHP;
    EnemyAnimations anims;
    [HideInInspector]
    public EnemyAudio sound;
    [HideInInspector]
    public Health health;
    BloodEffects befct;
    bool dead;
    bool hasWep = true;
    bool tagged;
    float attackCD;
    [HideInInspector]
    public Enemy type;
    EnemyColliders cols;
    JointeDisabler[] Joints;
    [HideInInspector]
    public PunTeams.Team Team;
    ArrowCollision lastHit;
    [HideInInspector]
    public Transform Healthbar;
    [HideInInspector]
    public float TimeAlive;

    public virtual void Awake()
    {
        cols = GetComponent<EnemyColliders>();
        Dropables = new List<GameObject>();
        sound = GetComponent<EnemyAudio>();
        if (sound == null)
            sound = GetComponentInChildren<EnemyAudio>();
        anims = GetComponent<EnemyAnimations>();
        if (anims == null)
            anims = GetComponentInChildren<EnemyAnimations>();
        health = GetComponent<Health>();
        health.OnDeath.AddListener(delegate { Die(); });
        befct = GetComponent<BloodEffects>();
        if (GetComponent<ArrowImpact>() != null)
            GetComponent<ArrowImpact>().OnHit.AddListener(OnHit);
        if(anims != null)
            anims.PlayIdle();
        if (RootBone == null)
            RootBone = skinMesh.transform;
        if (health.HealthBar != null)
            Healthbar = health.HealthBar.transform;
        else
            Healthbar = RootBone;
        skin = skinMesh.GetComponent<Renderer>();
        //Joint[] AllJoints = GetComponentsInChildren<Joint>();
        Joints = GetComponentsInChildren<JointeDisabler>();
        foreach(var jd in Joints)
        {
            Joint j = jd.GetComponent<Joint>();
            jd.CopyValuesAndDestroyJoint(j);
        }
        if (modelScale == 0)
            modelScale = transform.localScale.x;
        try { GetView(); }
        catch { }
    }

    public virtual void Start()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.Enemies.Add(gameObject);
        //InvokeRepeating("ToggleDisplayParticles", 1f, 1f);
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            Vector3 x = rb.centerOfMass;
            rb.centerOfMass = x;
            x = rb.inertiaTensor;
            rb.inertiaTensor = x;
            Quaternion r = rb.inertiaTensorRotation;
            rb.inertiaTensorRotation = r;
        }
        Invoke("CheckValid", 3f);
    }

    void CheckValid()
    {
        bool valid = false;
        if (GetComponent<SWS.navMove>() != null)
            valid = GetComponent<SWS.navMove>().pathContainer != null;
        else if(GetComponent<SWS.splineMove>() != null)
            valid = GetComponent<SWS.splineMove>().pathContainer != null;
        if (!valid)
            Kill();
    }

    public virtual void Update()
    {
        if (isDead())
            return;
        TimeAlive += Time.deltaTime;
        if (state == EnemyState.walking)
            Move();
        else if (state == EnemyState.attacking)
        {
            if(targHP != null && targHP.isDead())
            {
                if (GameBase.instance != null && GameBase.instance.CurrentTarget != targHP)
                    SetNewTarget(GameBase.instance.CurrentTarget.gameObject);
            }
            if(attackCD <= 0)
            {
                Attack();
                if(anims != null && anims.Attacks.Length > 0)
                    attackCD = Mathf.Max(0.75f, (anims.anim[anims.Attacks[0]].length/anims.AttackSpeed)*1.05f);
            }
            else
                attackCD -= Time.deltaTime;
        }
    }

    public bool hasWeapon()
    {
        return hasWep;
    }

    public virtual void TargetPlayer(int pid)
    {

    }

    void ToggleDisplayParticles()
    {
        if(dead || DisplayParticles == null || PlayerHead.instance == null)
            return;
        Vector3 partPos = DisplayParticles.transform.position;
        Vector3 dir = (PlayerHead.instance.transform.position- partPos).normalized;
        float dist = Vector3.Distance(partPos, PlayerHead.instance.transform.position);
        if (Vector3.Distance(PlayerHead.instance.transform.position, transform.position) > 15 && Physics.Raycast(partPos, dir, dist * 0.9f))
        {
            if (!DisplayParticles.isPlaying)
                DisplayParticles.Play();    
        }
        else
        {
            if (DisplayParticles.isPlaying)
                DisplayParticles.Stop();
        }
    }

    public abstract void Move();

    public abstract void Attack();

    public void SetState(EnemyState s)
    {
        state = s;
        if (anims != null)
            anims.SetState(s);
        if (cols != null)
            cols.SetColliderScale(s);
    }

    int enemyID;
    public virtual void SetCreature(Enemy e, int eid)
    {
        type = e;
        enemyID = eid;
        SetColors();
        SetWeapon();
        SetDamage();
        //Boss Setup
        if(type.status != CreatureStatus.Standard)
        {
            List<Creature> ignores = new List<Creature>();
            ignores.Add(this);
            if (EnemyStream.GetRealPlayerNum() < 3)
                GameBase.instance.KillPulse(transform.position + Vector3.up, ignores);
            GetComponent<Health>().OnDeath.AddListener(delegate
            {
                if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
                    GameBase.instance.IncreaseDifficulty(50);
                EnemyStream.completed = true;
                CreatureManager.bossOut = false;
                GameBase.instance.LastSuccess = (int)GameBase.instance.Difficulty;
                if (GameBase.instance.inGame)
                    ContinuePanel.Open();
                GameBase.instance.KillPulse(transform.position + Vector3.up);
                GameBase.instance.EnemiesSpawned = 0;
                //DynamicMusic.instance.UseHeroic();
            });
        }
        else if(type.status == CreatureStatus.Miniboss)
        {
            float bhealth = health.maxHP;
            bhealth *= 1+(EnemyStream.GetRealPlayerNum()*0.2f);
            bhealth *= 1 + (CreatureManager.EnemyDifficulty / 1000f);
            health.IncreaseHealth(bhealth - health.maxHP);
        }
    }

    public int GetEnemyID()
    {
        return enemyID;
    }

    void SetWeapon()
    {
        if(type.Weapons.Length > 0 && WeaponHolder != null)
        {
            if (Weapon != null)
                Destroy(Weapon);
            GameObject wepObj = type.Weapons[Random.Range(0, type.Weapons.Length)];
            Weapon = (GameObject)Instantiate(wepObj, WeaponHolder);
            Weapon.transform.localPosition = Vector3.zero;
            Weapon.transform.localEulerAngles = Vector3.zero;
            Weapon.transform.localScale = Vector3.one;
            Dropables.Add(Weapon);
        }
    }

    void SetDamage()
    {
        if (GameBase.instance != null && CreatureManager.EnemyDifficulty > 500)
        {
            float sMult = 1 + ((CreatureManager.EnemyDifficulty - 2500) / 2500);
            Damage *= sMult;
        }
    }

    public void SetColors()
    {
        if(skinMesh != null)
        {
            Renderer sm = skinMesh.GetComponent<Renderer>();
            if(type.BaseColors.Length > 0)
                sm.material.color = type.BaseColors[Random.Range(0, type.BaseColors.Length)];
            if (type.DetailColors.Length > 0 && sm.material.HasProperty("_EmissionColor"))
                sm.material.SetColor("_EmissionColor", type.DetailColors[Random.Range(0, type.DetailColors.Length)] * 3.5f);
        }
        if(armors.Length > 0 && type.ArmorColors.Length > 0)
        {
            foreach(var armorMesh in armors)
            {
                armorMesh.material.color = type.ArmorColors[Random.Range(0, type.ArmorColors.Length)];
            }
        }
    }

    void DifficultyAdjust(ArrowCollision e)
    {
        if(GameBase.instance != null && GameBase.instance.Difficulty > 0)
        {
            //Value Setup
            int combo = (int)Statistics.GetCurrentFloat("Combo");
            float cb = EnemyDB.v.ComboMultiplier.Evaluate(combo);
            float aimMult = e.aimed > 0 ? 2.5f: 1; //(new float[] { 1, 1.5f, 1.5f, 2f, 2f, 3f, 3f, 3f, 4f, 4f, 8f, 10.5f, 12f, 15f})[e.aimed];
            float shotDist = e.Distance();
            if (e.aimed == Arrow.MAX_TIER)
                aimMult = 5f;
            int energy = 0;
            //Aimed Shot Bonus
            if(e.aimed > 0)
            {
                if (GameOrb.instance != null)
                    energy += e.aimed;//GameOrb.GiveEnergy(e.impactPos, e.aimed);
            }
            //Crit Bonus
            if (e.Critical())
            {
                if (GameOrb.instance != null)
                    energy += (int)(1.5f * aimMult) + 4;//GameOrb.GiveEnergy(e.impactPos, (int)(1*aimMult));
                //GameBase.instance.IncreaseDifficulty((0.5f * cb) / DiffMult());
            }
            //Distance Bonus
            if (shotDist > 25)
            {
                if (GameOrb.instance != null)
                    energy += (int)((shotDist / 25f) * aimMult);//GameOrb.GiveEnergy(e.impactPos, (int)((shotDist/25f)*aimMult));
                //GameBase.instance.IncreaseDifficulty((0.3f * (e.Distance() / 10f)) / DiffMult());
            }
            //Combo Bonus
            if (combo > 4 && e.Distance() > 8)
            {
                int val = 0;
                if (combo > 10)
                    val = 1;
                if (GameOrb.instance != null)
                    energy += (int)((1 + val) * aimMult);//GameOrb.GiveEnergy(e.impactPos, (int)((1 + val) * aimMult));
                // GameBase.instance.IncreaseDifficulty((0.1f * cb) / DiffMult());
            }
            if(energy > 0)
            {
                if (!wouldDie(e.ArrowDamage) || (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient))
                {
                    if (type != null && type.EnergyMult >= 0)
                        energy = (int)Mathf.Ceil(energy * type.EnergyMult);
                    if (GameOrb.GiveEnergy(RootBone.position, energy))
                        CombatText.ShowText("+" + energy, Healthbar.position, Color.green, 1.3f);
                }
                else
                {
                    deathEnergy += energy;
                }
            }
        }
    }

    public void ModDeathEnergy(int energy)
    {
        deathEnergy += energy;
    }

    float DiffMult()
    {
        if (EnemyStream.GetRealPlayerNum() == 2)
            return 1.6f;
        else if (EnemyStream.GetRealPlayerNum() == 3)
            return 2.4f;
        else if (EnemyStream.GetRealPlayerNum() == 4)
            return 3.2f;
        return 1f;
    }

    public virtual void OnHit(ArrowCollision e)
    {
        if (e.isMine && !isDead())
        {
            DifficultyAdjust(e);
            tagged = true;
        } 
        if(e.isMine && e.Critical() && !isDead())
        {
            if (GameBase.instance != null && GameOrb.instance == null)
                GameBase.instance.TryGiveRevive(1, transform.position);
        }
        /*
        if(e.isMine)
        {
            CombatText.ShowText("" + e.ArrowDamage, e.impactPos + (Vector3.up*1.6f), Color.red);
        }
        */
        //Blood Splatters & Sound/Animation
        if (e.hitObj.tag != "Armor")
        {
            bool hard = (e.Critical() || e.hitObj.tag == "HighDamage");
            if(befct != null)
                befct.CreateEffect(e.impactPos, e.ImpactNormal, e.hitObj, hard);
            if (anims != null)
                anims.PlayHit();
            if(sound != null)
                sound.PlayHit();
        }
        if (state == EnemyState.dead)
            return;
        //Statistics
        if (!isDead() && e.isMine)
            HandleStats(e);
    }

    void HandleStats(ArrowCollision e)
    {
        if (!CreatureManager.InGame())
            return;
        if (e.hitObj.tag != "Armor")
        {
            Statistics.AddCurrent("Points", 10 * Mathf.Clamp((int)Mathf.Sqrt(e.Distance()), 1, 5), true);
            HitMarker.ShowHit(e.impactPos, e.Critical());
        }
        if (e.Distance() > Statistics.GetCurrentFloat("LongShot"))
        {
            Statistics.SetCurrent("LongShot", (int)e.Distance(), true);
            if (e.Distance() > Statistics.GetInt("LongestShot"))
                Statistics.SetValue("LongestShot", (int)e.Distance());
        }
        if (e.Critical())
        {
            Statistics.AddCurrent("Crit", 1);
            Statistics.AddValue("EnemyCrit", 1f);
        }
        Statistics.AddCurrent("Hit", 1);
        Statistics.AddCurrent("Combo", 1);
        int curCombo = Statistics.GetCurrentInt("Combo");
        int CurHighCombo = Statistics.GetCurrentInt("HighCombo");
        if (curCombo > CurHighCombo)
            Statistics.SetCurrent("HighCombo", curCombo, true);
        if (curCombo > Statistics.GetInt("BestCombo"))
            Statistics.SetValue("BestCombo", curCombo);
        int hit = (int)Statistics.GetCurrentFloat("Hit");
        int miss = (int)Statistics.GetCurrentFloat("ArrowsMissed");
        int accuracy = (int)((hit / (float)(hit + miss)) * 100f);
        int crits = (int)Statistics.GetCurrentFloat("Crit");
        int critPerc = (int)((crits/(float)hit)*100f);
        Statistics.SetCurrent("Accuracy", accuracy, true);
        Statistics.SetCurrent("CritPerc", critPerc, true);
        float Distance = e.Distance();
        Statistics.AddCurrent("ArrowsHit", 1);
        /*
        Statistics.AddToBitArray("Acc100", true, 100);
        Statistics.AddToBitArray("Acc500", true, 500);
        */
        Statistics.AddValue("EnemyHit", 1f);
        if (Distance <= 25)
            Statistics.AddValue("Hit0to25", 1f);
        else if (Distance <= 50)
            Statistics.AddValue("Hit25to50", 1f);
        else if (Distance <= 75)
            Statistics.AddValue("Hit50to75", 1f);
        else if (Distance <= 100)
            Statistics.AddValue("Hit75to100", 1f);
        else
            Statistics.AddValue("HitOver100", 1f);

        if (Statistics.GetFloat("LongestShot") < e.Distance())
            Statistics.SetValue("LongestShot", e.Distance());
        if(type.status == CreatureStatus.Standard && CreatureManager.EnemyDifficulty >= 100)
            HitAchievements(e, curCombo);
    }

    void HitAchievements(ArrowCollision e, int curCombo)
    {
        if (e.Distance() > 100)
        {
            Statistics.AddCurrent("LongShot_Combo", 1);
            if (Statistics.GetCurrentInt("LongShot_Combo") >= 3)
                Achievement.EarnAchievement("LONG_COMBO_1");
            if (Statistics.GetCurrentInt("LongShot_Combo") >= 5)
                Achievement.EarnAchievement("LONG_COMBO_2");
            if (Statistics.GetCurrentInt("LongShot_Combo") >= 10)
                Achievement.EarnAchievement("LONG_COMBO_3");
        }
        else
            Statistics.SetCurrent("LongShot_Combo", 0);
        if (e.Critical())
        {
            Statistics.AddCurrent("Crit_Combo", 1);
            if (Statistics.GetCurrentInt("Crit_Combo") >= 10)
                Achievement.EarnAchievement("CRIT_COMBO_10");
        }
        else
            Statistics.SetCurrent("Crit_Combo", 0);
        if (curCombo >= 100)
            Achievement.EarnAchievement("COMBO_100");

    }

    public void ArmorHit(GameObject obj)
    {
        if (anims != null)
            anims.PlayHit();
        for (int i = 0; i < Dropables.Count; i++)
        {
            if (obj == Dropables[i] || obj.transform.parent == Dropables[i])
            {
                DropObject(obj);
            }
        }
    }

    void DropObject(GameObject obj)
    {
        for (int i = Dropables.Count - 1; i >= 0; i--)
        {
            GameObject d = Dropables[i];
            if (d == obj)
            {
                Dropables.RemoveAt(i);
                d.transform.SetParent(null);
                Rigidbody r = d.GetComponent<Rigidbody>();
                r.isKinematic = false;
                r.AddExplosionForce(15, transform.position - (Vector3.up * 0.25f), 1);
                r.ResetCenterOfMass();
                r.ResetInertiaTensor();
                d.transform.SetParent(null);
                Destroy(d, 10f);
                if (d == Weapon)
                {
                    hasWep = false;
                    Damage /= 3f;
                }
            }
        }
    }

    float lastAttack;
    int aid;
    public void AttackAction()
    {
        float offset = 0;
        if (anims != null)
        {
            anims.PlayAttack();
            offset = anims.AttackOffset;
        }
        Invoke("AttackDamage", offset);
        lastAttack = Time.time;
    }

    public void AttackDamage()
    {
        if (Target == null)
            return;
        if ((skin == null || !skin.isVisible) && sound != null)
            sound.PlayAttack();
        transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if (Target != null && Target.GetComponent<Health>() != null)
            {
                Target.GetComponent<Health>().takeDamage(CalculateDamage());
                Target.GetComponent<Health>().SetAttacker(gameObject);
            }               
        }
    }

    public virtual void SetNewTarget(GameObject o)
    {
        Target = o;
        targHP = Target.GetComponent<Health>();
        state = EnemyState.walking;
        if (anims != null)
            anims.SetState(state);
    }

    public void UpdateTarget()
    {
        if(GameBase.instance != null && GameBase.instance.CurrentTarget != null)
        {
            Target = GameBase.instance.CurrentTarget.gameObject;
            targHP = GameBase.instance.CurrentTarget;
        } 
    }

    public float CalculateDamage()
    {
        if (!hasWeapon())
            return Damage / 3f;
        return Damage;
    }

    public EnemyAnimations Anims()
    {
        return anims;
    }

    public void Kill(bool nonPlayer=true)
    {
        NonPlayerDeath = nonPlayer;
        health.Kill();
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            GetComponent<NetworkCreature>().Kill();
    }

    public void SetTagged()
    {
        tagged = true;
    }

    [AdvancedInspector.Inspect]
    void IgnoreArrowsRed()
    {
        IgnoreArrows(PunTeams.Team.red, false);
    }

    public void IgnoreArrows(PunTeams.Team team, bool sync=true)
    {
        if (sync)
            GetComponent<NetworkCreature>().IgnoreArrow(team);
        if(team == PunTeams.Team.none || (pvpmanager.instance.PlayingPVP && pvpmanager.instance.myTeam == team))
        {
            /*
            foreach (var v in GetComponentsInChildren<Collider>())
            {
                v.gameObject.layer = 17;
            }
            */
        }
        Team = team;
        if(pvpmanager.instance != null && pvpmanager.instance.PlayingPVP)
            pvpmanager.instance.AddEnm(team);
        if(team == PunTeams.Team.red)
        {
            if (skinMesh != null)
            {
                Renderer sm = skinMesh.GetComponent<Renderer>();
                if (type.BaseColors.Length > 0)
                    sm.material.color = new Color(Random.Range(0.2f, 0.35f), Random.Range(0.8f, 1f), Random.Range(0.3f, 0.6f), 1);
                if (sm.material.HasProperty("_EmissionColor"))
                {
                    Debug.Log("Has Property EmissionColor");
                    Color c = new Color(.2f, 1, .5f, 1);
                    sm.material.SetColor("_EmissionColor", c * 3.5f);
                }
                    
            }
        }
    }

    public bool NonPlayerDeath;
    #region Death
    bool wouldDie(float dmg)
    {
        return health.currentHP - dmg <= 0;
    }

    public void SetLastHit(ArrowCollision e)
    {
        lastHit = e;
    }

    int deathEnergy = 5;
    bool noRagdoll = false;
    bool ragdolled;
    public virtual void Die()
    {
        Profiler.BeginSample("Health::Die -> Creature::Die begin");
        Destroy(gameObject, 6f);
        dead = true;
        bool wasAttacking = state == EnemyState.attacking;
        state = EnemyState.dead;
        clearRBVels();
        Profiler.EndSample();

        Profiler.BeginSample("Health::Die -> Creature::Die stats start");
        if (Settings.HasKey("Ragdoll"))
            noRagdoll = !Settings.GetBool("Ragdoll", true);
        if (tagged)
        {
            Statistics.AddValue("EnemiesKilled", 1f);
            Statistics.AddCurrent("EnmKilled", 1, true);
            if(!NonPlayerDeath)
                PowerupManager.instance.TryDrop(transform.position);
            if (pvpmanager.instance != null && Team != pvpmanager.instance.myTeam && pvpmanager.instance.PlayingPVP)
            {
                int rval = type.pvpResource;
                if (_isElite)
                    rval *= 3;
                pvpmanager.instance.myResource += rval;
            }
        }
        Profiler.EndSample();

        Profiler.BeginSample("Health::Die -> Creature::Die energy adding");
        if (GameOrb.instance != null && (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient) && !NonPlayerDeath)
        {
            if (_isElite)
                deathEnergy += 5;
            if (type != null && type.EnergyMult >= 0)
                deathEnergy = (int)Mathf.Ceil(deathEnergy * type.EnergyMult);
            if (GameOrb.GiveEnergy(RootBone.position, deathEnergy))
                CombatText.ShowText("+" + deathEnergy, Healthbar.position, Color.green, 1.3f);
        }
        Profiler.EndSample();

        Profiler.BeginSample("Health::Die -> Creature::Die Anims and sounds and effects");
        if (anims != null)
            anims.SetState(state);
        if(sound != null)
            sound.PlayDeath();
        ClearEliteEffect();
        Profiler.EndSample();

        if (Team != PunTeams.Team.none && pvpmanager.instance.PlayingPVP)
        {
            pvpmanager.instance.LoseEnemy(Team);
        }
        //No Ragdoll
        if (noRagdoll || NonPlayerDeath || CreatureManager.CurRagdolls >= CreatureManager.MAX_RAGDOLL)
        {
            Profiler.BeginSample("Health::Die -> Creature::Die No ragdoll");
            if(DeathExplosion != null)
            {
                DeathExplosion.transform.SetParent(null);
                DeathExplosion.SetActive(true);
            }
            destroy();
            Profiler.EndSample();
        }
        else //Ragdoll
        {
            Profiler.BeginSample("Health::Die -> Creature::Die ragdoll start");
            CreatureManager.CurRagdolls++;
            ragdolled = true;
            if (wasAttacking)
            {
                Dissolve();
                Invoke("destroy", 1.5f);
            }
            else
            {
                Invoke("Dissolve", 4f);
                Invoke("destroy", 5.5f);
            }
            if (DisplayParticles != null && DisplayParticles.isPlaying)
                DisplayParticles.Stop();
            Profiler.EndSample();

            Profiler.BeginSample("Health::Die -> Creature::Die joints rigidbody");
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].TryAddRB();
                Joints[i].CreateJointAndDestoryThis();
            }
            Profiler.EndSample();

            Profiler.BeginSample("Health::Die -> Creature::Die joints disabling");
            Joints = new JointeDisabler[0];
            foreach (var rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.ResetCenterOfMass();
                rb.ResetInertiaTensor();
                rb.isKinematic = false;
            }
            if (lastHit != null)
            {
                Rigidbody rb = lastHit.hitObj.GetComponent<Rigidbody>();
                float mult = lastHit.ArrowDamage / 20f;
                if (rb != null)
                    rb.AddForceAtPosition(lastHit.ImpactVelocity.normalized * 75 * lastHit.velocityMag * mult, lastHit.impactPos);
            }
            Profiler.EndSample();

            Profiler.BeginSample("Health::Die -> Creature::Die Ragdoll breaking and dropables collision ignore");
            if (GetComponent<RagdollBreak>() != null)
                GetComponent<RagdollBreak>().BreakHierarchy();
            if (Weapon != null)
            {
                BeautifulDissolves.Dissolve d = Weapon.GetComponentInChildren<BeautifulDissolves.Dissolve>();
                if (d != null)
                    d.Invoke("TriggerDissolve", 4f);
                DropObject(Weapon);
            }
            foreach (var b in Dropables)
            {
                if (b != Weapon && b != null)
                {
                    Collider c = b.GetComponent<Collider>();
                    if (c != null)
                    {
                        foreach (var v in GetComponentsInChildren<Collider>())
                        {
                            Physics.IgnoreCollision(c, v);
                        }
                    }
                }
            }
            Profiler.EndSample();
        }
    }

    public bool isDead()
    {
        return dead;
    }

    void clearRBVels()
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f));
        }
    }

    bool dissolved;
    public void Dissolve()
    {
        if(!dissolved)
        {
            dissolved = true;
            foreach (var v in GetComponentsInChildren<BeautifulDissolves.Dissolve>())
            {
                v.TriggerDissolve();
            }
            foreach (var v in GetComponentsInChildren<Rigidbody>())
            {
                v.gameObject.layer = 22;
                if (v.gameObject != Weapon)
                {
                    v.useGravity = false;
                    v.velocity += Vector3.up * 1f * Random.Range(0.5f, 1f);
                }
            }
        }
       
    }

    void destroy()
    {
        ClearFireEffect();
        foreach (var v in GetComponentsInChildren<Collider>())
        {
            v.enabled = false;
        }
        foreach (var v in GetComponentsInChildren<Rigidbody>())
        {
            v.isKinematic = true;
        }
        for (int i = 0; i < Dropables.Count; i++)
        {
            Destroy(Dropables[i]);
        }
        if (!PhotonNetwork.inRoom || view == null)
            Destroy(gameObject);
        else if (view != null && PhotonNetwork.inRoom)
        {
            if (ragdolled)
                DestroyFullNetworked();
            else
            {              
                UnityMainThreadDispatcher.Instance().Enqueue(DestroyFullNetworked, 5.5f);
                CancelInvoke();
                StopAllCoroutines();
                if (RootBone != null)
                    Destroy(RootBone.gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    void DestroyFullNetworked()
    {
        if (PhotonNetwork.isMasterClient || (view != null && view.isMine))
            PhotonNetwork.Destroy(gameObject);
    }

    PhotonView view;
    PhotonView GetView()
    {
        if (view == null && gameObject != null)
            view = gameObject.GetComponent<PhotonView>();
        return view;
    }

    void OnDestroy()
    {
        ClearEliteEffect();
        ClearFireEffect();
        if (ragdolled)
            CreatureManager.CurRagdolls--;
    }

    #endregion

    #region Elite
    public bool IsElite
    {
        get
        {
            return _isElite;
        }
    }

    bool _isElite;
    public GameObject EliteParticles;
    public void MakeElite()
    {
        if(!_isElite)
        {
            _isElite = true;
            if (EliteParticles != null)
                EliteParticles.SetActive(true);
            transform.localScale *= 1.5f;
            health.IncreaseHealth((health.maxHP * 3f)-health.maxHP);
        }
    }
    void ClearEliteEffect()
    {
        if(EliteParticles != null)
            EliteParticles.SetActive(false);
    }
    public bool iselite()
    {
        return _isElite;
    }
#endregion Elite

    public GameObject DeathExplosion;

#region Fire
    private int fireEffect;
    void ClearFireEffect()
    {
        if (fireEffect > -1 && OnFireParticles.instance != null)
        {
            OnFireParticles.instance.ReleaseObject(fireEffect);
            fireEffect = -1;
        }
    }

    bool OnFire;
    public void CatchFire(float dmg)
    {
        DoCatchFire(dmg);
        if (PhotonNetwork.inRoom)
            GetComponent<NetworkCreature>().CatchFire(dmg);
    }


    public void DoCatchFire(float dmg)
    {
        if (!gameObject.activeSelf)
            return;
        if (dmg == 0)
        {
            if(Healthbar != null)
                CombatText.ShowText("Immune", Healthbar.position + (Vector3.up * 0.3f), Color.white);
            else
                CombatText.ShowText("Immune", transform.position + (Vector3.up * 1.5f), Color.white);
            return;
        }
        if (OnFireParticles.instance != null && !OnFire)
        {
            OnFire = true;
            fireEffect = OnFireParticles.instance.UseEffect(skinMesh);
        }
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            health.takeDamage(dmg);
            StopCoroutine("Burn");
            StartCoroutine("Burn", dmg);
        }
        else if (PhotonNetwork.inRoom)
        {
            StopCoroutine("Burn");
            StartCoroutine("Burn", dmg);
        }
    }

    IEnumerator Burn(float dmg)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.5f);
            health.takeDamage(dmg);
        }
        ClearFireEffect();
        OnFire = false;
    }
#endregion

}


public enum EnemyState
{
    idle,
    walking,
    attacking,
    dead,
    custom
}
