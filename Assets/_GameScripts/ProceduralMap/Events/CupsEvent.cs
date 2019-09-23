using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CupsEvent : EventTile {

    public List<EventCup> Cups;
    public Transform SphereHolder;
    public List<Transform> SkySpheres;
    public Transform CupOrigin;
    public GameObject StartIndicator;

    [Header("Gate Damage")]
    public GameObject LaserPrefab;
    public GameObject LaserImpact;
    public int LaserDamage;
    List<Vector3> StartPoints;

    public override void Awake()
    {
        base.Awake();
        StartPoints = new List<Vector3>();
        foreach(var c in Cups)
        {
            StartPoints.Add(c.transform.localPosition);
            c.Owner = this;
        }
    }

    public override void Start()
    {
        base.Start();
        for(int i=0; i<Cups.Count; i++)
        {
            Cups[i].Agent.Warp(transform.TransformPoint(StartPoints[i]));
        }
    }

    public override void StartIntro()
    {
        if(!startedIntro)
        {
            StartCoroutine("Intro");
            int modReq = 3;
            if(PhotonNetwork.inRoom)
                modReq = new int[] { 3, 2, 1, 1 }[PhotonNetwork.playerList.Length-1];
            if(modReq > 1000)
            {
                modReq = 2;
                if (PhotonNetwork.inRoom)
                    modReq = new int[] { 2, 2, 1, 1 }[PhotonNetwork.playerList.Length - 1];
            }
            for(int i=SkySpheres.Count-1; i>=0; i--)
            {
                if(i%modReq != 0)
                {
                    Destroy(SkySpheres[i].gameObject);
                    SkySpheres.RemoveAt(i);
                }
            }
            float speed = 8f;
            int NumAllowed = 4;
            if (GameBase.instance.Difficulty > 2500)
            {
                NumAllowed = 2;
                speed = 15f;
            }
            else if (GameBase.instance.Difficulty > 1200)
            {
                NumAllowed = 3;
                speed = 10f;
            }
            if (PhotonNetwork.inRoom)
                NumAllowed += new int[] { 0, 1, 2, 2 }[PhotonNetwork.playerList.Length - 1];
            foreach (var v in Cups)
            {
                v.Agent.speed = speed;
            }
            float gateHP = TargetGate.GetComponent<Health>().maxHP;
            LaserDamage = (int)(gateHP / NumAllowed);
            base.StartIntro();
        }

    }

    public override void EndEvent()
    {
        base.EndEvent();
    }

    public override void Update()
    {
        if (startedIntro)
            SphereHolder.Rotate(Vector3.up * 35 * Time.deltaTime);
        base.Update();
    }

    public bool eventStarted;
    IEnumerator Intro()
    {
        float t = 0;
        while(!eventStarted)
        {
            t += Time.deltaTime;
            yield return true;
            if (t > 30 && !StartIndicator.activeSelf)
                StartIndicator.SetActive(true);
        }
        TryStartMusic();
        StartIndicator.SetActive(false);
        yield return new WaitForSeconds(1f);
        OnIntroEnd();
    }

    public override void StartEvent()
    {
        Debug.Log("Starting Event Loop");
        StartCoroutine("EventLoop");
        base.StartEvent();
    }

    IEnumerator EventLoop()
    {
        int itr = 0;
        while(SkySpheres.Count > 0)
        {
            itr++;

            int numReq = 1;
            if (PhotonNetwork.inRoom)
                numReq = (new int[] { 1, 2, 3, 3 })[PhotonNetwork.playerList.Length-1];
            if (GameBase.instance.Difficulty > 1000)
            {
                numReq = 2;
                if (PhotonNetwork.inRoom)
                    numReq = (new int[] { 2, 3, 4, 4 })[PhotonNetwork.playerList.Length-1];
            }

            if (SkySpheres.Count - numReq < numReq)
                numReq = SkySpheres.Count;
            else if (SkySpheres.Count == 2 && numReq == 1)
                numReq = 2;

            yield return SetPhase(numReq);
            yield return MixPhase(itr+3);
            yield return PlayerPhase(numReq);

            bool missed = false;
            foreach(var v in Cups)
            {
                if(v.HasCore && !v.Exploded)
                {
                    missed = true;
                    DamageGate(v.CoreDisplay.transform.position);
                    v.CoreDisplay.SetActive(false);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (missed)
                yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        if(!TargetGate.destroyed)
        {
            EndEvent();
        }
    }

    IEnumerator SetPhase(int req)
    {
        foreach(var v in Cups)
        {
            v.Regenerate(false);
            v.CanShoot = false;
            v.HasCore = false;
            v.CoreDisplay.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
      
        List<EventCup> cups = CupListCopy();
        for (int i=0; i<req; i++)
        {
            if(SkySpheres.Count > 0)
            {
                Transform nextOrb = SkySpheres[0];
                SkySpheres.RemoveAt(0);                
                EventCup c = cups[GetRandomNext(0, cups.Count)];
                cups.Remove(c);
                nextOrb.SetParent(null);
                while(Vector3.Distance(nextOrb.position, c.CoreDisplay.transform.position) > 0.1f)
                {
                    nextOrb.position = Vector3.MoveTowards(nextOrb.position, c.CoreDisplay.transform.position, Time.deltaTime * 12f);
                    yield return true;
                }
                c.CoreDisplay.GetComponent<MeshRenderer>().sharedMaterial = nextOrb.GetComponent<MeshRenderer>().sharedMaterial;
                Destroy(nextOrb.gameObject);
                c.CoreDisplay.SetActive(true);
                c.HasCore = true;
            }
        }
        yield return new WaitForSeconds(3.5f);
        foreach(var v in Cups)
        {
            v.ShowTop();
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(3.25f);
        foreach(var v in Cups)
        {
            v.CoreDisplay.SetActive(false);
        }
        foreach (var v in Cups)
        {
            v.ToggleShield(true);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator MixPhase(int mixTimes)
    {
        for(int i=0; i<mixTimes; i++)
        {
            for (int q=0; q<Cups.Count; q++)
            {
                Vector3 newTarg = new Vector3(GetRandomNext(-15, 15), 0, GetRandomNext(-15, 15)) + CupOrigin.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(newTarg, out hit, 3.0f, NavMesh.AllAreas))
                {
                    newTarg = hit.position;
                }
                Cups[q].MoveTo(newTarg, 1f);
            }
            yield return new WaitForSeconds(1.5f);
        }
        //Debug.Log("Mix Ended");
        ShuffleSpots();
        for (int i = 0; i < Cups.Count; i++)
        {
            Vector3 newTarg = transform.TransformPoint(StartPoints[i]);
            Cups[i].MoveTo(newTarg, 1f);
        }
        yield return new WaitForSeconds(3f);
    }

    IEnumerator PlayerPhase(int req)
    {
        foreach (var v in Cups)
        {
            v.CanShoot = true;
            v.ToggleShield(false);
            yield return new WaitForSeconds(0.05f);
        }
        float t = 0;
        int numBroken = 0;
        bool hasCharged = true;
        int plus = 1;
        if (GameBase.instance.Difficulty >= 1000)
            plus = 0;
        while (t < 30 && numBroken < req+plus && hasCharged)
        {
            numBroken = 0;
            hasCharged = false;
            foreach(var v in Cups)
            {
                if (v.Exploded)
                    numBroken++;
                if (!v.Exploded && v.HasCore)
                    hasCharged = true;
            }
            t += Time.deltaTime;
            yield return true;
        }
        foreach(var v in Cups)
        {
            v.CanShoot = false;
        }
        foreach (var v in Cups)
        {
            if (v.HasCore)
                v.CoreDisplay.SetActive(true);
            v.HideTop();
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2.5f);
    }

    List<EventCup> CupListCopy()
    {
        List<EventCup> cups = new List<EventCup>();
        foreach (var v in Cups)
            cups.Add(v);
        return cups;
    }

    void ShuffleSpots()
    {
        List<Vector3> newPts = new List<Vector3>();
        int q = StartPoints.Count;
        for(int i=0; i<q; i++)
        {
            int randID = StartPoints.Count>1?GetRandomNext(0, StartPoints.Count):0;
            newPts.Add(StartPoints[randID]);
            StartPoints.RemoveAt(randID);
        }
        StartPoints = newPts;
    }

    public void BreakBall(EventCup cup)
    {
        int cupID = Cups.IndexOf(cup);
        mgr.IntEvent1(this, cupID);
    }

    public void DamageGate(Vector3 startPt)
    {
        if (LaserPrefab != null)
        {
            GameObject newLaser = Instantiate(LaserPrefab, startPt, Quaternion.identity);
            newLaser.transform.SetParent(transform);
            newLaser.transform.LookAt(TargetGate.transform.position);
            newLaser.SetActive(true);
            AudioSource src = newLaser.GetComponent<AudioSource>();
            src.pitch = Random.Range(0.9f, 1.1f);
            Destroy(newLaser, 5f);
            if (LaserImpact != null)
            {
                GameObject NewImpact = Instantiate(LaserImpact, TargetGate.transform.position, Quaternion.identity);
                NewImpact.SetActive(true);
                Destroy(NewImpact, 5f);
            }
        }
        if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            TargetGate.GetComponent<Health>().takeDamage(LaserDamage);
    }

    public override void OnActionTaken(string action)
    {
        if (action == "HIT_START")
            EventManager.instance.IntEvent2(this, 1);
        base.OnActionTaken(action);
    }

    public override void IntEvent1Response(int val)
    {
        Cups[val].Explode();
    }

    public override void IntEvent2Response(int val)
    {
        eventStarted = true;
    }
}
