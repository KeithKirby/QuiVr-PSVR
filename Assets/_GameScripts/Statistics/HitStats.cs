using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Parse;

public class HitStats : MonoBehaviour {

    static List<HitStat> Stats;

    public static void PullStats()
    {
        Stats = new List<HitStat>();
        string st = "";
        //if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.ContainsKey("HitStats"))
        //st = ParseUser.CurrentUser.Get<string>("HitStats");
        st = PlayerProfile.Profile.HitStats;
        string[] sts = st.Split(':');
        foreach(var v in sts)
        {
            Stats.Add(new HitStat(v));
        }
    }

    public static void AddMiss(HitRange r)
    {
        GetStat(r).Misses++;
    }

    public static void AddHit(HitRange r)
    {
        GetStat(r).Hits++;
    }

    public static void AddCrit(HitRange r)
    {
        GetStat(r).Crits++;
    }

    public static HitStat GetStat(HitRange r)
    {
        if (Stats == null)
            Stats = new List<HitStat>();
        for(int i=0; i<Stats.Count; i++)
        {
            if (Stats[i].range == r)
                return Stats[i];
        }
        HitStat s = new HitStat(r);
        Stats.Add(s);
        return s;
    }

    public class HitStat
    {
        public HitRange range;
        public int Crits;
        public int Hits;
        public int Misses;

        public HitStat(HitRange r)
        {
            range = r;
        }

        public HitStat(string s)
        {
            string[] pts = s.Split(',');
            if(pts.Length > 0)
            {
                int r = 0;
                int.TryParse(pts[0], out r);
                range = (HitRange)r;
                if (pts.Length > 1)
                    int.TryParse(pts[1], out Hits);
                if (pts.Length > 2)
                    int.TryParse(pts[2], out Misses);
                if (pts.Length > 3)
                    int.TryParse(pts[3], out Crits);

            }
        }

        public override string ToString()
        {
            string s = "";
            s += (int)range + ",";
            s += Hits + "," + Misses + "," + Crits;
            return s;
        }
    }

	public enum HitRange
    {
        r0to25,
        r25to50,
        r50to75,
        r75to100,
        rOver100
    }
}
