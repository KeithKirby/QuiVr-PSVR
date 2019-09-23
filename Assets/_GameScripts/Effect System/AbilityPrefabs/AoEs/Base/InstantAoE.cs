using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantAoE : Effect {

    public float Delay;
    public float Radius;
    public float explosionForce;
    public AudioClip[] Clips;
    public EffectClassification EffectType;
    public bool RotateToGround;
    [Header("Achievement Check")]
    public int DmgForAchievement = -1;
    public bool requireKill;
    public string AchievementID;

    public override void Setup(bool fake, int efID, float v)
    {
        base.Setup(fake, efID, v);
        AudioSource src = GetComponent<AudioSource>();
        if(src != null && Clips.Length > 0)
        {
            src.clip = Clips[Random.Range(0, Clips.Length)];
            src.Play();
        }
    }

    IEnumerator Start()
    {
        yield return true;
        if (Delay > 0)
            yield return new WaitForSeconds(Delay);
        //Damage
        if (RotateToGround)
            TrySetRotation();
        float dmg = baseEffect.StaticValue;
        if (baseEffect.randomType == RandomType.Damage)
            dmg = val;
        AbilitySetup s = GetComponent<AbilitySetup>();
        if (s.OverrideVal > 0)
            dmg = s.OverrideVal;
        if(!dummy)
        {
            int numDamaged = 0;
            int numKilled = 0;
            List<GameObject> enms = new List<GameObject>();
            if (GameBase.instance != null)
            {
                foreach (var v in CreatureManager.AllEnemies())
                {
                    if (v != null && Vector3.Distance(v.transform.position, transform.position) < Radius)
                    {
                        Immunities im = v.GetComponent<Immunities>();
                        bool immune = false;
                        if (im != null)
                        {
                            if (im.CheckImmune(EffectType))
                            {
                                CombatText.ShowText("Immune", v.transform.position + (Vector3.up * 2), Color.white);
                                immune = true;
                            }
                        }
                        if(pvpmanager.instance != null && pvpmanager.instance.PlayingPVP && v.Team == pvpmanager.instance.myTeam)
                                immune = true;
                        if (!immune)
                        {
                            v.SetTagged();
                            v.GetComponent<Health>().takeDamage(dmg);
                            numDamaged++;
                            if (v.GetComponent<Health>().isDead())
                                numKilled++;
                        }
                    }
                }
            }
            if(TargetDummy.Dummies != null)
            {
                foreach (var v in TargetDummy.Dummies)
                {
                    if (v != null && Vector3.Distance(v.transform.position, transform.position) < Radius)
                    {
                        Health h = v.GetComponent<Health>();
                        if(h != null)
                            h.takeDamage(dmg);
                    }
                }
            }
            if(AchievementID.Length > 1 && (numKilled >= DmgForAchievement || (!requireKill && numDamaged >= DmgForAchievement)))
            {
                Achievement.EarnAchievement(AchievementID);
            }
        }
        yield return true;
        yield return true;

        //Physics
        Collider[] objects = UnityEngine.Physics.OverlapSphere(transform.position, Radius*1.5f);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null && !r.isKinematic && r.GetComponent<BreakablePotion>() == null)
            {
                r.AddExplosionForce(explosionForce, transform.position, Radius*2);
            }
        }
    }

    void TrySetRotation()
    {
        RaycastHit hit;
        int layer = 13;
        int mask = ~(1 << layer);
        if (Physics.Raycast(new Ray(transform.position + (transform.TransformDirection(Vector3.up) * 0.1f), transform.TransformDirection(Vector3.down)), out hit, 2f, mask))
        {
            Vector3 norm = hit.normal;
            transform.position += norm * 0.05f;
            int i = -1;
            transform.rotation = Quaternion.LookRotation(norm * i);
            transform.Rotate(Vector3.left, 90);
        }
    }
}
