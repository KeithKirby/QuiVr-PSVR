using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomSwapper : MonoBehaviour {

    public SwapItem[] Swaps;

    void Start()
    {
        foreach(var v in Swaps)
        {
            v.TryReplace();
        }
    }

    [System.Serializable]
    public class SwapItem
    {
        public GameObject DefaultObj;
        public GameObject TransferObj;
        [Range(0, 100)]
        public float PercentChance;
        public Vector2 MinDate;
        public Vector2 MaxDate;

        public void TryReplace()
        {
            if(validDate())
            {
                if(UnityEngine.Random.Range(0, 100) < PercentChance && TransferObj != null)
                {
                    if (DefaultObj != null)
                        DefaultObj.SetActive(false);
                    TransferObj.SetActive(true);
                }
            }
        }

        bool validDate()
        {
            DateTime Min = new DateTime(DateTime.Now.Year, (int)MinDate.x, (int)MinDate.y);
            DateTime Max = new DateTime(DateTime.Now.Year, (int)MaxDate.x, (int)MaxDate.y);
            DateTime now = DateTime.Now;
            return now >= Min && now <= Max;
        }
    }
}
