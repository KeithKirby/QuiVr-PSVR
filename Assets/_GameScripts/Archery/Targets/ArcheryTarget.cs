using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class ArcheryTarget : MonoBehaviour {

    public Transform BrokenHolder;
    ArcheryTargetGame tGame;
    bool broken;
    [System.Serializable]
    public class ObjectEvent : UnityEvent<GameObject> { };
    public ObjectEvent OnBreak;

    public AudioClip[] Shatters;

    public bool Reusable;
    public bool IgnoreArcheryGame;

    public void Init(ArcheryTargetGame t)
    {
        tGame = t;
    }

	public void Hit(ArrowCollision col)
    {
        if(!broken && col.isMine)
        {

            float dist = Vector3.Distance(transform.position, col.impactPos);
            float tdist = col.Distance();
            if (!IgnoreArcheryGame)
                AddPointsDist(dist, tdist);
            Break(true);

            if(!IgnoreArcheryGame)
            {
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
    }

    public void Break(bool sound=false)
    {
        if (broken)
            return;
        if(sound)
        {
            VRAudio.PlayClipAtPoint(Shatters[Random.Range(0, Shatters.Length)], transform.position, 0.75f, 1f, 0.98f);
        }
        broken = true;
        GetComponentInChildren<Collider>().enabled = false;
        GetComponentInChildren<MeshRenderer>().enabled = false;
        if (!Reusable)
        {           
            BrokenHolder.gameObject.SetActive(true);
            Rigidbody[] rbs = BrokenHolder.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
            {
                Rigidbody r = rbs[i];
                r.isKinematic = false;
            }
            Destroy(gameObject, 5f);
        }
        else
        {
            GameObject pieces = Instantiate(BrokenHolder.gameObject, BrokenHolder.position, BrokenHolder.rotation);
            pieces.transform.SetParent(transform);
            pieces.transform.localScale = BrokenHolder.localScale;
            pieces.SetActive(true);
            Rigidbody[] rbs = pieces.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
            {
                Rigidbody r = rbs[i];
                r.isKinematic = false;
            }
            Destroy(pieces, 5f);
        }
        OnBreak.Invoke(gameObject);
    }

    public void Reset()
    {
        broken = false;
        GetComponentInChildren<Collider>().enabled = true;
        GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    public bool isBroken()
    {
        return broken;
    }

    void AddPointsDist(float dist, float tdist)
    {
        float f = 10;
        if (dist <= 0.16f)
            f = 50;
        else if (dist <= 0.32f)
            f = 40;
        else if (dist <= 0.48f)
            f = 30;
        else if (dist <= 0.64f)
            f = 20;
        float x = f*(tdist / 40f);
        
        float y = Mathf.Max(Mathf.Round(x/ 10f), 1);
        y *= 10;

       Statistics.AddCurrent("Points", (int)y, true);
       AddPoints((int)y);
    }

    void AddPoints(int pts)
    {
        if(tGame != null)
        {
            tGame.AddScore(pts);
        }
        if(ArcheryScore.instance != null)
        {
            ArcheryScore.instance.GivePoints(pts, transform.position);
        }
    }

    [BitStrap.Button]
    public void PlaceSpawnPoint()
    {
        GameObject SP = new GameObject("TargetSpawn");
        SP.transform.position = transform.position;
        SP.transform.rotation = transform.rotation;
        if (ArcheryGame.instance != null)
        {
            Debug.Log("Addint Spawn Point");
            ArcheryGame.instance.SpawnPoints.Add(SP.transform);
        }
            
    }
}
