using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvObjSwap : MonoBehaviour {

    public EnvObjects[] objects;

    void Start()
    {
        MapTile mt = GetComponentInParent<MapTile>();
        if (mt != null)
        {
            foreach (var v in objects)
            {
                if(v.env == mt.Environment)
                {
                    foreach(var o in v.objs)
                    {
                        o.SetActive(true);
                    }
                }
                else
                {
                    foreach (var o in v.objs)
                    {
                        o.SetActive(false);
                    }
                }
            }
        }
    }

    [System.Serializable]
	public class EnvObjects
    {
        public EnvironmentType env;
        public GameObject[] objs;
    }
}
