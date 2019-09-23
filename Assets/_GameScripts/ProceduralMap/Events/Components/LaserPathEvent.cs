using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPathEvent : EventTile {

    public GameObject[] EventTiles;
    public Transform StartLoc;
    public Transform EndLoc;
    public Transform LaserStart;
    public LineRenderer LaserLine;
    public AnimationCurve RotateCurve;

    bool PathComplete;

    public override void Start()
    {
        rotating = new List<int>();
        base.Start();
        LockTiles();
    }

    public override void StartIntro()
    {
        if (!startedIntro)
        {
            //LockTiles();
            StartCoroutine("Intro");
            if (TargetGate != null)
            {
                float gateHP = TargetGate.GetComponent<Health>().maxHP;
                int NumAllowed = 2;
                if (GameBase.instance.Difficulty > 1000)
                    NumAllowed = 3;
                else if (GameBase.instance.Difficulty >= 1500)
                    NumAllowed = 4;
                else if (GameBase.instance.Difficulty >= 2000)
                    NumAllowed = 5;
                else if (GameBase.instance.Difficulty >= 3000)
                    NumAllowed = 6;
                PlayerDeathDamage = (int)(gateHP / (float)NumAllowed) + 1;
            }
            base.StartIntro();
        }
    }

    [AdvancedInspector.Inspect]
    public void LockTiles()
    {
        //Set Start/End Laser Locations
        int startLoc = GetRandomNext(-2, 2);
        StartLoc.transform.localPosition = Vector3.zero;
        EndLoc.transform.localPosition = Vector3.down*43;
        StartLoc.transform.localPosition += Vector3.left * 7 * startLoc;
        int endLoc = GetRandomNext(-1, 2);
        if(endLoc == startLoc)
        {
            if (endLoc == 0)
                endLoc = 1;
            else
                endLoc = 0;
        }
        EndLoc.transform.localPosition += Vector3.left * 7 * endLoc;
        int diff = Mathf.Abs(startLoc - endLoc);
        int[,] matrix = new int[5,5];
        for(int i=0; i<5; i++)
        {
            //Rows
            int numOpen = 5;
            for(int j=0; j<5; j++)
            {
                //Columns
                GameObject t = EventTiles[j + (i*5)];
                if(numOpen >= 3 && ((i != 0 && i != 4) || (j != 0 && j != 4)))
                {
                    MeshRenderer r = t.GetComponent<MeshRenderer>();
                    int opt = GetRandomNext(0, 6);
                    t.transform.GetChild(0).gameObject.SetActive(true);
                    t.transform.localEulerAngles = new Vector3(0, 270, 90);
                    r.material.SetColor("_EmissionColor", Color.white*1.5f);
                    if (opt == 0 || opt == 4)
                    {
                        numOpen--;
                        r.material.SetColor("_EmissionColor", Color.black);
                        t.transform.GetChild(0).gameObject.SetActive(false);
                        matrix[j, i] = -1;
                    }
                    else if ((opt == 1 || opt == 3) && j != 0 && j != 4)
                    {
                        //Check Adjacent for Locked using matrix
                        bool adj = false;
                        int x = j - 1;
                        while (x >= 0)
                        {
                            if (matrix[x, i] == 1)
                            {
                                adj = true;
                                break;
                            }
                            else if (matrix[x, i] == 0)
                                break;
                            x--;
                        }
                        if (x == 0)
                            adj = true;
                        //Check Vertical
                        if(i > 0 && !adj)
                        {
                            x = i - 1;
                            while (x >= 0)
                            {
                                if (matrix[j, x] == 1)
                                {
                                    adj = true;
                                    break;
                                }
                                else if (matrix[j, x] == 0)
                                    break;
                                x--;
                            }
                            if (x == 0)
                                adj = true;
                        }              
                        if(!adj)
                        {
                            numOpen--;
                            r.material.SetColor("_EmissionColor", Color.yellow);
                            t.GetComponent<ArrowImpact>().enabled = false;
                            if (GetRandomNext(0, 11) > 5)
                                t.transform.localEulerAngles = new Vector3(90, 270, 90);
                            matrix[j, i] = 1;
                        }
                    }
                }
            }
        }
    }

    IEnumerator Intro()
    {
        yield return true;
        StartEvent();
    }

    public override void StartEvent()
    {
        Debug.Log("Starting WOF Event");
        StartCoroutine("EventLoop");
        base.StartEvent();
    }

    public override void Update()
    {
        //if(EventManager.InEvent)
        UpdateLaser();
        base.Update();
    }

    IEnumerator EventLoop()
    {
        while(!PathComplete)
        {
            yield return true;
        }  
    }

    void UpdateLaser()
    {
        Vector3 StartPt = LaserStart.position;
        List<Vector3> pts = new List<Vector3>();
        pts.Add(StartPt);
        bool hitEnd = false;
        int maxBounces = 50;
        int bounces = 0;
        Vector3 lastPt = StartPt;
        Vector3 curDir = Vector3.down;
        while(!hitEnd && bounces < maxBounces)
        {
            RaycastHit hit;
            if (Physics.Raycast(lastPt, curDir, out hit, 50f))
            {
                if(hit.collider.tag == "ForceDeflect")
                {
                    lastPt = hit.point;
                    pts.Add(lastPt);
                    curDir = Vector3.Reflect(curDir, hit.normal);
                    Debug.DrawRay(lastPt, hit.normal, Color.white);
                }
                else
                {
                    pts.Add(hit.point);
                    hitEnd = true;
                }
            }
            else
            {
                pts.Add(lastPt + (curDir*50f));
                hitEnd = true;
            }
            bounces++;
        }
        LaserLine.positionCount = pts.Count;
        LaserLine.SetPositions(pts.ToArray());
    }

    public void TileImpact(ArrowCollision imp)
    {
        int tile = GetTile(imp.impactPos);
        if(tile >= 0 && !rotating.Contains(tile))
            EventManager.instance.IntEvent1(this, tile);
    }

    int GetTile(Vector3 loc)
    {
        float minDist = float.MaxValue;
        int tileID = -1;
        for(int i=0; i<EventTiles.Length; i++)
        {
            float d = Vector3.Distance(EventTiles[i].transform.position, loc);
            if (d < minDist && d < 15)
            {
                tileID = i;
                minDist = d;
            }
        }
        return tileID;
    }

    public override void IntEvent1Response(int val)
    {
        if(val >= 0 && val < EventTiles.Length && !rotating.Contains(val))
        {
            RotateTile(val);
        }
    }

    public override void IntEvent2Response(int val)
    {
        LockTiles();
    }

    void RotateTile(int tileID)
    {
        StartCoroutine("DoRotateTile", tileID);
    }

    public void ResetBoard()
    {
        PlayerLife.Kill();
        EventManager.instance.IntEvent2(this, 0);
    }



    List<int> rotating;
    IEnumerator DoRotateTile(int tid)
    {
        rotating.Add(tid);
        GameObject tile = EventTiles[tid];
        Quaternion curAngle = tile.transform.localRotation;
        Quaternion newAngle = curAngle * Quaternion.Euler(0, 90, 0);
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime;
            tile.transform.localRotation = Quaternion.Lerp(curAngle, newAngle, RotateCurve.Evaluate(t));
            yield return true;
        }
        rotating.Remove(tid);
    }
}
