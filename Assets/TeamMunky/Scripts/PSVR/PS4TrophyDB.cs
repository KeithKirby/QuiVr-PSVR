using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

namespace Sony.NP
{
    public class PS4TrophyDB : MonoBehaviour
    {
#if UNITY_PS4

        public List<DiscoveryID> DiscoveryToTrophyId;
        SonyNpMain _main;

        private void Awake()
        {
            _main = GetComponent<SonyNpMain>();
        }

        public int FindTrophyId(DiscoveryID id)
        {
            for (int i = 0; i < DiscoveryToTrophyId.Count; ++i)
            {
                if (DiscoveryToTrophyId[i] == id)
                    return i;
            }
            return -1;
        }
#endif
    }
}