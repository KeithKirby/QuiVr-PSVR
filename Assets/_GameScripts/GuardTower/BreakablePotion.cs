using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BreakablePotion : MonoBehaviour {

    Vector3 StartPos;
    Quaternion StartRot;
    Collider col;
    Rigidbody rb;

    bool broken = false;

    public Transform PotionRespawn;

    [Header("Pour Info")]
    public ParticleSystem Pour;
    ParticleSystem.EmissionModule pourEm;
    public float PourDur;
    float udown;

    [Header("Break Info")]
    public float BreakImpactVel;
    public float BreakDist;
    public float BreakYVal;

    public GameObject[] Displays;    

    public UnityEvent OnBreak;
    public UnityEvent OnReset;
    public GameObject BreakEffect;

    SpriteRenderer sr;
    public ParticleSystem pr;
    public Sprite QMark;
    public Color QColor;
    public Sprite XMark;
    public Color XColor;

    VRTK.VRTK_InteractableObject io;

    void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        sr = GetComponentInChildren<SpriteRenderer>();
        io = GetComponent<VRTK.VRTK_InteractableObject>();
        pourEm = Pour.emission;
    }

    void Start()
    {
        StartPos = transform.position;
        StartRot = transform.rotation;
        foreach (var v in Displays)
        {
            v.SetActive(true);
        }
        if(TutorialManager.instance != null)
            TutorialManager.instance.OnReset.AddListener(delegate { CheckInTutorial(); });
    }


    public void Reset()
    {
        CheckInTutorial();
        VRTK.VRTK_InteractableObject io = GetComponent<VRTK.VRTK_InteractableObject>();
        io.enabled = true;
        io.ForceStopInteracting();
        broken = false;
        if (PotionRespawn != null)
        {
            transform.position = PotionRespawn.position;
            transform.rotation = PotionRespawn.rotation;
        }
        else
        {
            transform.position = StartPos;
            transform.rotation = StartRot;
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        col.enabled = true;
        pourEm.rateOverTime = 0;
        foreach (var v in Displays)
        {
            v.SetActive(true);
        }
        OnReset.Invoke();
    }

    public void Break()
    {
        GetComponent<VRTK.VRTK_InteractableObject>().ForceStopInteracting();
        broken = true;
        foreach (var v in Displays)
        {
            v.SetActive(false);
        }
        if (BreakEffect != null)
        {
            GameObject nbr = Instantiate(BreakEffect, BreakEffect.transform.position, BreakEffect.transform.rotation);
            nbr.SetActive(true);
        }
        pourEm.rateOverTime = 0;
        OnBreak.Invoke();
        Invoke("Reset", 3);
        TutorialManager.TryToggleTutorial();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, StartPos) > 25)
            Reset();

        if (!broken && io.IsGrabbed() && Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            udown += Time.deltaTime;
            pourEm.rateOverTime = 8;
            if (udown > PourDur)
            {
                Break();
            }
        }
        else
        {
            pourEm.rateOverTime = 0;
            udown = 0;
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (broken)
            return;
        if (col.relativeVelocity.magnitude > BreakImpactVel)
            Break();
        else if (Vector3.Distance(transform.position, StartPos) > BreakDist)
            Break();
        else if (Mathf.Abs(transform.position.y - StartPos.y) > BreakYVal)
            Break();
    }

    public void ToggleVisibility(bool val)
    {
        if (val)
            Reset();
        else
        {
            VRTK.VRTK_InteractableObject io = GetComponent<VRTK.VRTK_InteractableObject>();
            io.ForceStopInteracting();
            io.enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            col.enabled = false;
            foreach (var v in Displays)
            {
                v.SetActive(false);
            }
        }
    }

    [AdvancedInspector.Inspect]
    public void CheckInTutorial()
    {
        if (sr != null)
        {
            if(TutorialManager.inTutorial)
            {
                sr.sprite = XMark;
                sr.color = XColor;
                pr.gameObject.GetComponent<ParticleSystemRenderer>().material.color = XColor * 3;
            }
            else
            {
                sr.sprite = QMark;
                sr.color = QColor;
                pr.gameObject.GetComponent<ParticleSystemRenderer>().material.color = QColor * 3;
            }
        }
    }
}
