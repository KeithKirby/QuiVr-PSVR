using UnityEngine;
using System.Collections;

public class MoteParticles : MonoBehaviour {

    Vector3 StartPos;
    public ParticleSystem MoteEffect;
    public Transform Attractor;

    public static MoteParticles instance;

    int motesToSend;
    Vector3 posToSend;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartPos = MoteEffect.transform.position;
    }

    void Update()
    {
        if(Hips.instance != null)
        {
            Attractor.position = Hips.instance.transform.position;
        }
        if (motesToSend > 0)
        {
            MoteEffect.transform.position = posToSend;
            MoteEffect.Emit(5);
            motesToSend -= 5;
        }
        else
            motesToSend = 0;
    }

    [BitStrap.Button]
    void SpawnTest()
    {
        SpawnMotes(100);
    }

    public void SpawnMotes(int num=10)
    {
        SpawnMotes(StartPos, num);
    }

    public void SpawnMotes(Vector3 pos, int num = 10)
    {
        motesToSend += num;
        posToSend = pos;
    }
}
