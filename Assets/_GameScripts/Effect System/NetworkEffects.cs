using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEffects : MonoBehaviour {

    PhotonView v;
    public static NetworkEffects instance;
    [Header("Freeze")]
    public Material IceMat;
    [Header("Infest")]
    public GameObject CreatureInfest;
    public GameObject SpreadInfest;
    public float InfestDamage = 5f;
    public float SpreadRange = 3f;

    void Awake()
    {
        v = GetComponent<PhotonView>();
        Infested = new List<Creature>();
        instance = this;
        InvokeRepeating("TickDamage", 1f, 1f);
        InvokeRepeating("TrySpread", 2f, 2f);
    }

    #region Infest
    [AdvancedInspector.Inspect]
    void InfestRandom()
    {
        List<Creature> all = CreatureManager.AllEnemies();
        if (all.Count > 0)
            Infest(all[Random.Range(0, all.Count)]);
    }

    List<Creature> Infested;
    public void Infest(Creature c, bool sync=true)
    {
        if (c == null)
            return;
        Health h = c.GetComponent<Health>();
        if(!h.isDead() && !Infested.Contains(c))
        {
            if (sync && PhotonNetwork.inRoom)
                v.RPC("NetworkInfest", PhotonTargets.Others, c.GetComponent<PhotonView>().viewID);
            Infested.Add(c);
            GameObject newInfest = Instantiate(CreatureInfest, c.skinMesh.transform);
            newInfest.SetActive(true);
            newInfest.transform.localPosition = Vector3.zero;
            newInfest.transform.localScale = Vector3.one;
            newInfest.transform.localEulerAngles = Vector3.zero;
            h.OnDeath.AddListener(delegate {
                Infested.Remove(c);
                newInfest.GetComponent<ParticleSystem>().Stop();
            });
        }
    }

    [PunRPC]
    void NetworkInfest(int pvID)
    {
        PhotonView f = PhotonView.Find(pvID);
        if (f != null)
            Infest(f.GetComponent<Creature>(), false);
    }

    public void TrySpread()
    {
        CheckInfested();
        List<Creature> all = CreatureManager.AllEnemies();
        for(int i=all.Count-1; i>= 0; i--)
        {
            if (Infested.Contains(all[i]))
                all.RemoveAt(i);
        }
        for(int i=0; i<Infested.Count; i++)
        {
            Creature c = Infested[i];
            for (int j = all.Count - 1; j >= 0; j--)
            {
                Creature t = all[j];
                if (!t.isDead() && Vector3.Distance(c.transform.position, t.transform.position) <= SpreadRange)
                {
                    InfestOrb(c, t);
                    all.RemoveAt(j);
                }
            }
        }
    }

    void InfestOrb(Creature c, Creature targ)
    {
        GameObject orb = Instantiate(SpreadInfest, c.RootBone.position, Quaternion.identity);
        orb.SetActive(true);
        MoveToward mt = orb.GetComponent<MoveToward>();
        mt.OnCollide.AddListener(delegate {
            Infest(targ);
        });
        mt.Setup(targ.RootBone);
    }

    void CheckInfested()
    {
        if (Infested == null)
            Infested = new List<Creature>();
        for(int i=Infested.Count-1; i>=0; i--)
        {
            Creature c = Infested[i];
            if (c == null || c.isDead())
                Infested.RemoveAt(i);
        }
    }

    void TickDamage()
    {
        CheckInfested();
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            for(int i=Infested.Count-1; i>=0; i--)
            {
                Health h = Infested[i].GetComponent<Health>();
                if(h != null && !h.isDead())
                {
                    h.takeDamage(InfestDamage);
                }
                if (h.isDead())
                    Infested.RemoveAt(i);
            }
        }
    }
    #endregion

    #region Life Leech
    public void HealCurTarget(float val)
    {
        Health targ = null;
        if (GameBase.instance != null)
            targ = GameBase.instance.CurrentTarget;
        else if (pvpmanager.instance != null && pvpmanager.instance.PlayingPVP)
            targ = pvpmanager.instance.GetMyGate();
        if(PhotonNetwork.inRoom)
        {
            if (targ.GetComponent<PhotonView>() != null)
                targ.takeDamageImmediate(-1 * val);
            else if (targ.GetComponent<Gate>() != null && GateManager.instance != null)
                GateManager.instance.HealGate(targ.GetComponent<Gate>(), val);
        }
        else
            targ.takeDamageImmediate(-1 * val);

    }
    #endregion

    #region Freeze
    public void Freeze(GameObject Enemy, float duration, bool sync, bool dummy)
    {
        if (Enemy == null)
            return;
        if(sync && PhotonNetwork.inRoom)
        {
            PhotonView pv = Enemy.GetComponent<PhotonView>();
            if (pv != null)
                v.RPC("NetworkFreeze", PhotonTargets.Others, pv.viewID, duration);
        }
        Creature c = Enemy.GetComponent<Creature>();
        if (c == null)
            c = Enemy.GetComponentInParent<Creature>();
        if(c != null)
        {
            Renderer r = c.skinMesh.GetComponent<Renderer>();
            r.materials = AddMaterial(r.materials, IceMat);
            var nav = Enemy.GetComponent<GroundCreature>();
            if (nav != null)
                nav.ChangeSpeed(0.001f, !dummy);
            var fnav = Enemy.GetComponent<FlyingCreature>();
            if (fnav != null)
                fnav.ChangeSpeed(0.001f, !dummy);
            StartCoroutine(Unfreeze(Enemy, duration, dummy));
        }
    }

    IEnumerator Unfreeze(GameObject enemy, float duration, bool dummy)
    {
        float t = 0;
        Creature c = enemy.GetComponent<Creature>();
        if(c != null)
        {
            while (t < duration && !c.isDead())
            {
                t += Time.deltaTime;
                yield return true;
            }
            if(c.skinMesh != null)
            {
                Renderer r = c.skinMesh.GetComponent<Renderer>();
                if (r != null)
                    r.materials = RemoveMaterial(r.materials, IceMat);
                var nav = enemy.GetComponent<GroundCreature>();
                if (nav != null)
                    nav.ChangeSpeed(1, !dummy);
                var fnav = enemy.GetComponent<FlyingCreature>();
                if (fnav != null)
                    fnav.ChangeSpeed(1, !dummy);
            }
        }
    }

    [PunRPC]
    void NetworkFreeze(int enemyID, float duration)
    {
        PhotonView f = PhotonView.Find(enemyID);
        if (f != null)
            Freeze(f.gameObject, duration, false, true);
    }

    #endregion

    #region Utility

    public Material[] AddMaterial(Material[] origMats, Material mat)
    {
        List<Material> mats = new List<Material>();
        foreach(var v in origMats)
        {
            mats.Add(v);
        }
        if (!mats.Contains(mat))
            mats.Add(mat);
        return mats.ToArray();
    }

    public Material[] RemoveMaterial(Material[] origMats, Material mat)
    {
        List<Material> mats = new List<Material>();
        foreach (var v in origMats)
        {
            mats.Add(v);
        }
        for(int i=mats.Count-1; i>=0; i--)
        {
            if (mats[i].name.ToLower().Contains(mat.name.ToLower()))
            {
                mats.RemoveAt(i);
            }
        }
        return mats.ToArray();
    }

    #endregion
}
