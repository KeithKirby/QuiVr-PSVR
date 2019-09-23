using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeautifulDissolves;

public class GateGems : MonoBehaviour {

    public GameObject[] GemDisplays;
    public AudioClip[] Falls;
    Health hp;

    public GameObject[] GemHolders;
    List<GateGem> Gems;

    List<GateGem> OnGems;
    List<GateGem> OffGems;

    Color startCol;

    static List<GateGems> AllGems;

    void Awake()
    {
        if (AllGems == null)
            AllGems = new List<GateGems>();
        AllGems.Add(this);
        OnGems = new List<GateGem>();
        OffGems = new List<GateGem>();
        hp = GetComponent<Health>();
        if (hp == null)
            hp = GetComponentInParent<Health>();
        Gems = new List<GateGem>();
        foreach (var v in GemDisplays)
        {
            GateGem g = new GateGem(v, this);
            Gems.Add(g);
        }
        foreach (var g in Gems)
        {
            OnGems.Add(g);
        }
        if (Gems.Count > 0)
        {
            startCol = Gems[0].GemRend.material.GetColor("_EmissionColor");
            float biggestValue = startCol.r;
            if (startCol.g > biggestValue) biggestValue = startCol.g;
            if (startCol.b > biggestValue) biggestValue = startCol.b;
            if (biggestValue > 1f)
            {
                startCol =
                    new Color(
                        startCol.r / biggestValue,
                        startCol.g / biggestValue,
                        startCol.b / biggestValue
                        );
            }
        }
        ToggleHolders(Settings.GetBool("GateGems"));       
    }

    void Update()
    {
        if (hp.isDead())
            return;
        float perc = hp.currentHP / hp.maxHP;
        int alive = (int)Mathf.Floor(perc * Gems.Count);
        int diff = OnGems.Count - alive;
        if (diff > 0 && OnGems.Count > 0) //Remove Gems
        {
            for (int i = 0; i < diff; i++)
            {
                int rid = Random.Range(0, OnGems.Count);
                GateGem g = OnGems[rid];
                OnGems.RemoveAt(rid);
                g.Pop();
                OffGems.Add(g);
                PlayRandomGem(g.BaseGem.transform.position);
            }
        }
        else if (diff < 0 && OffGems.Count > 0) //Replace Gems
        {
            for (int i = 0; i < Mathf.Abs(diff); i++)
            {
                int rid = Random.Range(0, OffGems.Count);
                GateGem g = OffGems[rid];
                OffGems.RemoveAt(rid);
                g.Reset();
                OnGems.Add(g);
            }
        }
    }

    public static void ToggleGems(bool val)
    {
        foreach(var v in AllGems)
        {
            v.ToggleHolders(val);
        }
    }

    public void ToggleHolders(bool val)
    {
        foreach(var v in GemHolders)
        {
            v.SetActive(val);
        }
    }

    public void ForceUpdate()
    {
        foreach(var v in OnGems)
        {
            v.Reset();
        }
        foreach(var v in OffGems)
        {
            v.SetUsed();
        }
    }

    public void PlayRandomGem(Vector3 pos, float vol = 1)
    {
        if (Falls.Length > 0)
        {
            VRAudio.PlayClipAtPoint(Falls[Random.Range(0, Falls.Length)], pos, vol, 1, 1, 8f);
        }
    }

    public void ChangeColor(Color c, float brightness=3.5f)
    {
        foreach (var v in Gems)
        {
            v.ChangeColor(c, brightness);
        }
    }

    public void ResetColor()
    {
        if (Gems == null)
            return;
        foreach (var v in Gems)
        {
            v.ChangeColor(startCol, 3f);
        }
    }

    public class GateGem
    {
        public GameObject BaseGem;
        public Renderer GemRend;
        public Dissolve GemDissolve;
        public GameObject GemPhysics;
        Dissolve physDissolve;
        GateGems gg;

        public GateGem(GameObject obj, GateGems gate)
        {
            BaseGem = obj;
            gg = gate;
            GemRend = obj.GetComponent<Renderer>();
            GemDissolve = obj.GetComponent<Dissolve>();
            GemPhysics = obj.GetComponentInChildren<Rigidbody>().gameObject;
            GemPhysics.SetActive(false);          
            RigidbodyEvents rev = GemPhysics.AddComponent<RigidbodyEvents>();
            rev.OnCollision = new RigidbodyEvents.FloatEvent();
            rev.OnCollision.AddListener(PlayGem);
            rev.CollisionVelThreshold = 2f;
            physDissolve = GemPhysics.GetComponent<Dissolve>();
            if(physDissolve != null)
                physDissolve.OnDissolveFinish.AddListener(delegate { GemPhysics.GetComponent<Rigidbody>().isKinematic = true; });
        }

        void PlayGem(float vel)
        {
            if (vel > 1f)
                gg.PlayRandomGem(GemPhysics.transform.position, vel / 20f);
        }

        public void Pop()
        {
            GemRend.enabled = false;
            GemPhysics.SetActive(true);
            Rigidbody rb = GemPhysics.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.AddExplosionForce(500, rb.transform.position - rb.transform.forward * 0.25f, 5);
            rb.angularVelocity = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            if(physDissolve != null)
            {
                physDissolve.SetDissolveAmount(0);
                physDissolve.Invoke("TriggerDissolve", 5f);
            }
            else
            {
                Run.After(5, () => {
                    if(GemPhysics != null)
                        GemPhysics.SetActive(false);
                });
            }
        }

        public void SetUsed()
        {
            GemRend.enabled = false;
            GemPhysics.SetActive(false);
        }

        public void Reset()
        {
            GemPhysics.GetComponent<Rigidbody>().isKinematic = true;
            GemPhysics.SetActive(false);
            GemPhysics.transform.localPosition = Vector3.zero;
            GemPhysics.transform.localRotation = Quaternion.identity;
            if(physDissolve != null)
                physDissolve.CancelInvoke("TriggerDissolve");
            GemRend.enabled = true;
            if (GemDissolve != null)
                GemDissolve.TriggerReverseDissolve();
            else
                GemRend.enabled = true;
        }

        public void ChangeColor(Color c, float brightness)
        {
            GemRend.material.SetColor("_EmissionColor", c * brightness);
            GemPhysics.GetComponent<Renderer>().material.SetColor("_EmissionColor", c * brightness);
        }
    }

    void OnDestroy()
    {
        if (AllGems != null)
            AllGems.Remove(this);
    }

}
