using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : MonoBehaviour {

    public ParticleSystem LightHit;
    public ParticleSystem HardHit;

    public static List<TargetDummy> Dummies;
    public GameObject skinMesh;
    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        ArrowImpact i = GetComponent<ArrowImpact>();
        if (i != null)
            i.OnHit.AddListener(OnImpact);
        if (Dummies == null)
            Dummies = new List<TargetDummy>();
        Dummies.Add(this);
    }

    public void OnImpact(ArrowCollision e)
    {
        ParticleSystem p = LightHit;
        if (e.aimed > 0)
            p = HardHit;
        if(p != null)
        {
            p.transform.position = e.impactPos;
            p.transform.LookAt(e.impactPos - e.ImpactVelocity);
            p.Play();
        }
        Hit(e);
        HitMarker.ShowHit(e.impactPos, e.Critical());
    }

    OnFireParticles FirePool;
    private int fireEffect;
    bool OnFire;
    public void CatchFire(float dmg)
    {
        if (FirePool == null)
            FirePool = FindObjectOfType<OnFireParticles>();
        if (FirePool != null && !OnFire)
        {
            OnFire = true;
            fireEffect = FirePool.UseEffect(skinMesh);
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

    void ClearFireEffect()
    {
        if (fireEffect > -1 && FirePool != null)
        {
            FirePool.ReleaseObject(fireEffect);
            fireEffect = -1;
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

    public void Hit(ArrowCollision col)
    {
        if (col.isMine)
        {
            float dist = Vector3.Distance(transform.position, col.impactPos);
            float tdist = col.Distance();
            Statistics.AddCurrent("Hit", 1);
            Statistics.AddCurrent("Combo", 1, true);
            /*
            Statistics.AddToBitArray("Acc100", true, 100);
            Statistics.AddToBitArray("Acc500", true, 500);
            */
            if (col.Distance() > Statistics.GetCurrentFloat("LongShot"))
            {
                Statistics.SetCurrent("LongShot", (int)col.Distance(), true);
            }
            if (Statistics.GetCurrentFloat("Combo") > Statistics.GetInt("BestCombo"))
                Statistics.SetValue("BestCombo", (int)Statistics.GetCurrentFloat("Combo"));
            int hit = (int)Statistics.GetCurrentFloat("Hit");
            int miss = (int)Statistics.GetCurrentFloat("ArrowsMissed");
            int accuracy = (int)((hit / (float)(hit + miss)) * 100f);
            Statistics.SetCurrent("Accuracy", accuracy, true);

            float Distance = col.Distance();
            Statistics.AddCurrent("ArrowsHit", 1);
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

            if (Statistics.GetFloat("LongestShot") < col.Distance())
                Statistics.SetValue("LongestShot", col.Distance());
        }
    }

    void OnDestroy()
    {
        Dummies.Remove(this);
    }

    public static float FindClosestDummy(Vector3 pos)
    {
        if (Dummies == null)
            Dummies = new List<TargetDummy>();
        float x = float.MaxValue;
        foreach (var v in Dummies )
        {
            if (v != null)
            {
                float dist = Vector3.Distance(pos, v.transform.position);
                if (dist < x)
                    x = dist;
            }
        }
        return x;
    }
}
