using UnityEngine;
using System.Collections;

public class BomberCreature : GroundCreature {

    public bool enraged;
    bool exploded;
    public string ChargeAnim;
    public GameObject ImpactParticles;
    public float enrageDist;
    public AudioClip[] EnrageClips;
    public AudioClip[] ExplodeClips;
    public Transform RootBody;

    public override void Update()
    {
        if(Target != null)
        {
            if ((health.currentHP < health.maxHP || Vector3.Distance(transform.position, Target.transform.position) < enrageDist) && !enraged)
                TryEnrage();
        }
        base.Update();
    }

    public void TryEnrage()
    {
        if(!enraged)
        {
            enraged = true;
            ChangeBaseSpeed(1.5f);
            Anims().PlayAnimation(ChargeAnim);
            sound.PlayRandomClip(EnrageClips, 0.99f, 30);
        }
    }

    public override void Attack()
    {
        exploded = true;
        AttackDamage();
        Explode();
    }

    public void Explode()
    {
        var nc = GetComponent<NetworkCreature>();
        ExplodeDisplay();
        if (nc != null && PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            nc.Explode();
        Die();
        Dissolve();
    }

    public void ExplodeDisplay()
    {
        if (ImpactParticles != null)
        {
            Instantiate(ImpactParticles, RootBody.position, Quaternion.identity);
        }
        if(ExplodeClips.Length > 0)
        {
            VRAudio.PlayClipAtPoint(ExplodeClips[Random.Range(0, ExplodeClips.Length)], transform.position, 1* VolumeSettings.GetVolume(AudioType.Effects), 1, 1f, 40f);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Creature c = col.gameObject.GetComponentInParent<Creature>();
        if (c != null && !c.isDead() && c.type.size < type.size)
        {
            c.Kill();
        }
    }

}
