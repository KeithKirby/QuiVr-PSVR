using UnityEngine;
using System.Collections;

public class KillTrigger : MonoBehaviour {

    Collider trigger;
    public ParticleSystem[] systems;
    public bool ActivateOnGameStart = false;

    void Awake()
    {
        trigger = GetComponent<Collider>();
    }

	void Start()
    {
        if(ActivateOnGameStart)
            GameBase.instance.OnStartGame.AddListener(EnableTrigger);
    }

    public void EnableTrigger()
    {
        trigger.enabled = true;
        foreach (var v in systems)
        {
            var e = v.emission;
            e.enabled = true;
        }
    }

    public void DisableTrigger()
    {
        trigger.enabled = false;
        foreach(var v in systems)
        {
            var e = v.emission;
            e.enabled = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Creature c = col.GetComponent<Creature>();
        if (c == null)
            c = col.GetComponentInParent<Creature>();
        if(c != null)
        {
            c.Kill();
        }

        PlayerHead ph = col.GetComponent<PlayerHead>();
        if (ph == null)
            ph = GetComponentInParent<PlayerHead>();
        if (ph != null && ph == PlayerHead.instance && !PlayerLife.myInstance.invincible && !PlayerLife.dead())
        {
            Debug.Log("Trigger [" + gameObject.name + "] killing player");
            PlayerLife.Kill();
        }

    }
}
