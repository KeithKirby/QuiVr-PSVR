using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour {

    public static PowerupManager instance;
    public static int powerID;
    public GameObject OrbPrefab;
    public int droprate;
    public float timer;
    public int numArrows;
    int prevNum;
    public PowerupInfo[] Powers;

    public int nextDrop;
    public int cur = 0;

    void Awake()
    {
        instance = this;
        nextDrop = droprate + (int)Random.Range(-droprate*0.35f, droprate * 0.35f);
    }

    public void TryDrop(Vector3 pos)
    {
        if (cur > nextDrop)
        {
            cur = 0;
            nextDrop = droprate + (int)Random.Range(-droprate * 0.35f, droprate * 0.35f); ;
            GameObject o = Instantiate(OrbPrefab, pos + Vector3.up, Quaternion.identity);
            o.GetComponent<PowerupOrb>().Setup(Powers[Random.Range(0, Powers.Length)]);
        }
        else
            cur++;
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                powerID = -1;
        }
        else if(numArrows != prevNum)
        {
            prevNum = numArrows;
            if (numArrows == 0)
                powerID = -1;
        }
    }

    public void ClearPowerup()
    {
        numArrows = 0;
        timer = 0;
        powerID = -1;
    }

    public void UsePowerup(PowerupInfo info)
    {
        numArrows = info.ArrowCount;
        prevNum = 0;
        timer = info.Duration;
        powerID = info.powerID;
        //Setup
    }

    [System.Serializable]
    public class PowerupInfo
    {
        public string pname;
        public Color displayColor;
        public int powerID;
        public int ArrowCount;
        public float Duration;

        public override string ToString()
        {
            if (pname != null)
                return pname;
            return base.ToString();
        }
    }
}
