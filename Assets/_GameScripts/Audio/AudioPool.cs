using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour {

    List<GameObject> Pool;
    public static AudioPool instance;

    void Awake()
    {
        instance = this;
    }


}
