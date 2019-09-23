using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using VRTK;
using System.Collections.Generic;
using UnityEngine.Profiling;

public class Arrow : MonoBehaviour
{
    TrailRenderer trail;
    public Transform tip;
    public float maxArrowLife = 10f;
    [HideInInspector]
    public bool inFlight = false;
    public float lodgeMovement;
    public float Damage;
    public float BonusDamage;
    public UnityEvent OnFire;
    [System.Serializable]
    public class ColliderHit: UnityEvent<Collider> { }
    public ColliderHit OnHit;
    public ColliderHit OnDeflect;

    public ParticleSystem dustParticles;

    public VRTK_InteractableObject interact;

    bool onFire;

    private bool collided = false;
    private Rigidbody rigidBody;
    private GameObject arrowHolder;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    public bool isMine;
    bool lodged;
    GameObject myPlayer;
    bool hasDeflected;

    ArrowEffects eft;
    Vector3 lastVel;
    ArrowAudio arrowAudio;

    const int MaxHitsCapacity = 1;
    RaycastHit[] hits = new RaycastHit[MaxHitsCapacity];
    Ray r, dc;
    ArrowCollision arrowCollision;

    int aimed;
    bool firstHit = false;

    public LayerMask CollisionMask;
    public float FirstHitDestroyDelay = 2f;

    public static List<Arrow> ArrowsOut;

    void Awake()
    {
        if (ArrowsOut == null)
            ArrowsOut = new List<Arrow>();
    }

    #region Base

    private IEnumerator Start()
    {
        arrowAudio = GetComponent<ArrowAudio>();
        r = new Ray();
        dc = new Ray();

        eft = GetComponent<ArrowEffects>();
        rigidBody = this.GetComponent<Rigidbody>();

        GameObject myInstance = null;
        if(!PhotonNetwork.inRoom)
            myInstance = LocalPlayer.instance.gameObject;
        else
            myInstance = PlayerSync.myInstance.Real;

        myPlayer = myInstance;
        if (isMine)
        {
            rigidBody.solverIterations = 8;
        }
        yield return true;
        if(!isMine)
        {
            CollisionMask.RemoveFromMask("teleporter");
        }
    }

    public void SetArrowHolder(GameObject holder)
    {
        arrowHolder = holder;
        arrowHolder.SetActive(false);
    }

    public void Init()
    {
        if (!isMine)
            gameObject.layer = 14; //mpArrows
        GetComponent<ArrowAudio>().Init();
        GetComponent<ArrowPhysics>().Init();
        Start();
    }

    public void OnNock()
    {
        collided = false;
        inFlight = false;
    }

    [HideInInspector]
    public const int MAX_TIER = 1;
    public void Powerup()
    {
        if(aimed < MAX_TIER)
        {
            aimed++;
            eft.SetAim(aimed);
            ChargeProgress(0);
            if (aimed == MAX_TIER)
                IncreaseDamage(50);
            else
                IncreaseDamage(5*aimed);
        }
    }

    public void Depower()
    {
        if(aimed > 0)
        {
            aimed = 0;
            eft.SetAim(aimed);
            ChargeProgress(0);
            Damage = 20;
        }
    }

    public void SetAimed(int val)
    {
        aimed = Mathf.Clamp(val, 0, MAX_TIER);
        for(int i=0; i<aimed; i++)
        {
            if (i == MAX_TIER)
                IncreaseDamage(50);
            else
                IncreaseDamage(5*i);
        }
    }

    public void ChargeProgress(float v)
    {
        v = Mathf.Clamp(v, 0, 1);
        eft.SetAimProgress(v);
    }

    public void IncreaseDamage(int dmg)
    {
        Damage += dmg;
    }

    Vector3 startPos;
    bool fired;
    public void Fired()
    {
        OnFire.Invoke();
        ArrowsOut.Add(this);
        fired = true;
        if (myPlayer != null && myPlayer.GetComponentInChildren<PlayerLife>() != null && myPlayer.GetComponentInChildren<PlayerLife>().isDead)
            DestroyFrame();
        Invoke("TryFireEffects", 0.33f);
        startPos = transform.position;
        trail = GetComponent<ArrowEffects>().CurrentArrow.Trail;
        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }
        DestroyArrow(maxArrowLife);
    }

    public bool WasFired()
    {
        return fired;
    }

    void DestroyFrame()
    {
        Destroy(dustParticles.gameObject, 1.2f);
        dustParticles.Play();
        dustParticles.transform.SetParent(null);
        Destroy(gameObject);
    }

    private void DelayedDestroy()
    {
        Destroy(arrowHolder);
        Destroy(this.gameObject);
    }

    private void DestroyArrow(float time)
    {
        Invoke("Dissolve", time - 2f);
        Invoke("DelayedDestroy", time);
    }

    void Dissolve()
    {
        if (Settings.GetInt("EffectQuality") < 1)
            return;
        foreach(var d in GetComponentsInChildren<BeautifulDissolves.Dissolve>())
        {
            d.TriggerDissolve();
        }
    }

    void Update()
    {
        if (lodged && !NetworkPhysicsObject.IsNAN(wantPos))
            transform.localPosition = wantPos;
    }
   
    void TryFireEffects()
    {
        if(!lodged)
        {
            eft.ApplyFiredEffects(transform.position);
        }
    }

    Vector3 DivideVectors(Vector3 num, Vector3 den)
    {
        return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);
    }
    #endregion

    public void catchFire()
    {
        onFire = true;
    }

    #region Collision

    GameObject hitObj;
    Vector3 hitPt = Vector3.zero;
    Vector3 hitNrm = Vector3.zero;
    void OnCollisionEnter(Collision col)
    {
        hitObj = col.gameObject;
        if (col.contacts.Length > 0)
        {
            hitPt = col.contacts[0].point;
            hitNrm = col.contacts[0].normal;
        }
        StartCoroutine("checkL");
        //CheckHit(hitObj, hitPt, hitNrm);
    }


    IEnumerator checkL()
    {
        yield return true;
        if (hitObj != null && hitObj.GetComponent<Collider>() != null && !lodged)
        {
            if (hitObj.layer == 16 || hitObj.tag == "Ground")
                CheckHit(hitObj, hitPt, hitNrm);
        }

    }
   
    void FixedUpdate()
    {
        RaycastCheck(Time.fixedDeltaTime);
        if(rigidBody)
            lastVel = rigidBody.velocity;
    }

    void LateUpdate()
    {
        //RaycastCheck(Time.deltaTime);
    }

    void RaycastCheck(float dt)
    {
        if (!lodged && !rigidBody.IsSleeping())
        {
            r.origin = tip.position;
            r.direction = rigidBody.velocity.normalized;

            dc.origin = transform.position;
            dc.direction = transform.forward;

            Debug.DrawRay(tip.position, rigidBody.velocity.normalized * rigidBody.velocity.magnitude * dt, Color.green);
            Debug.DrawRay(transform.position, transform.forward*0.8f, Color.blue);
             
            int firstCheck = Physics.SphereCastNonAlloc(r, 0.025f, hits, dt * rigidBody.velocity.magnitude*1.05f, CollisionMask);
            bool firstCheckGotHits = firstCheck > 0;
            if (firstCheckGotHits)
            {
                var hit = hits[0];
                CheckHit(hit.collider.gameObject, hit.point, hit.normal);
                return;
            }

            int secondCheck = Physics.SphereCastNonAlloc(dc, 0.0275f, hits, 0.8f, CollisionMask);
            bool secondCheckGotHits = secondCheck > 0;
            if(secondCheckGotHits)
            {
                var hit = hits[0];
                CheckHit(hit.collider.gameObject, hit.point, hit.normal);
            }
        }
    }

    void ProcessFirstHit()
    {
        firstHit = true;
        CancelInvoke("DelayedDestroy");
        Invoke("DelayedDestroy", FirstHitDestroyDelay);
    }

    void CheckHit(GameObject hit, Vector3 pt, Vector3 normal)
    {
        ArrowCollision c;
        if (rigidBody != null)
            c = new ArrowCollision(startPos, Damage * TestOptions.SDamageMultiplier, pt, rigidBody.velocity.magnitude, hit, onFire, -1 * rigidBody.velocity.normalized, isMine, hasDeflected, deflCount, gameObject, eft.effects.ToArray());
        else
            return;
        if (gameObject.layer == 15)
            return;

        if (!firstHit)
            ProcessFirstHit();
        c.ImpactVelocity = lastVel;
        c.aimed = aimed;
        c.BonusDamage = BonusDamage;
        var hitImpact = hit.GetComponent<ArrowImpact>();
        var hitCollider = hit.GetComponent<Collider>();

		if (hit.tag == "ForceDeflect") {
            ArrowImpact Impact = hitImpact;
            if (Impact != null)
                Impact.OnImpact(c);

            arrowAudio.Deflect(hitCollider);
            StopCoroutine("Deflect");
			StartCoroutine("Deflect", hit);
			return;
		}
        if (inFlight && !lodged)
        {
            ArrowImpact Impact = hitImpact;
			if (Impact == null && hit.tag != "Armor" && hit.tag != "IgnoreEnemy" && hit.tag != "ForceDeflect")
                Impact = hit.GetComponentInParent<ArrowImpact>();
            if(Impact == null)
            {
                if (Vector3.Angle(rigidBody.velocity, -normal) > 55f && arrowAudio.isHard(hitCollider) != 0)
                {
                    if(arrowAudio.isHard(hitCollider) == 1 || Vector3.Angle(rigidBody.velocity, -normal) > 72f)
                    {
                        arrowAudio.Deflect(hitCollider);
                        StopCoroutine("Deflect");
                        StartCoroutine("Deflect", hit);
                        return;
                    }      
                }
            }
            if (rigidBody.velocity.magnitude > 4 || Impact != null)
            {
                collided = true;
                inFlight = false;
                OnHit.Invoke(hitCollider);
                transform.position = transform.position + (rigidBody.velocity.normalized * lodgeMovement);
                /*
                if (hit.tag == "Armor" && hit.transform.parent != null)
                {
					if (hit.GetComponent<ArrowImpact> () != null) {
						Impact.OnImpact(c);
					}
                    if(hit.GetComponentInParent<Creature>() != null)
                        hit.GetComponentInParent<Creature>().ArmorHit(hit.gameObject);
                    StopCoroutine("Deflect");
                    StartCoroutine("Deflect", hit);
                    return;
                }
                */
                lodged = true;
                LodgeArrow(hit, pt, Impact != null, c);
                if (Impact != null)
                {
                    if (hit.tag == "DoubleDamage") Damage *= 2;
                    else if (hit.tag == "LowDamage") Damage *= 0.5f;
                    else if (hit.tag == "HighDamage") Damage *= 1.5f;
                    Impact.OnImpact(c);
                }
            }
        }
    }

    bool deflected;
    int deflCount = 0;
    IEnumerator Deflect(GameObject col)
    {
        deflected = true;
        deflCount++;
        hasDeflected = true;
        var collider = col.GetComponent<Collider>();
        OnDeflect.Invoke(collider);
        rigidBody.velocity *= 0.8f;
        yield return true;
        yield return true;
        deflected = false;
    }

    public Vector3 wantPos;
    Vector4 p;
    void LodgeArrow(GameObject obj, Vector3 hPoint, bool hasImpact, ArrowCollision c)
    {
        if (isMine && (!hasImpact || obj.tag == "Ground"))
        {
            bool usedMissEffect = eft.ApplyMissEffects(hPoint);
            float flightDistance = Vector3.Distance(transform.position, startPos);
            if(!usedMissEffect && wasMiss(hPoint, flightDistance) && obj.tag != "Armor")
            {
                float Distance = Vector3.Distance(transform.position, startPos);
                Statistics.AddCurrent("ArrowsMissed", 1);
                if (Statistics.GetCurrentInt("Combo") >= 5)
                    GameplayAudio.ComboBreak();
                Statistics.SetCurrent("Combo", 0, true);
                StatCheck.ArrowMiss();
                /*
                Statistics.AddToBitArray("Acc100", false, 100);
                Statistics.AddToBitArray("Acc500", false, 500);
                */
                int hit = (int)Statistics.GetCurrentFloat("Hit");
                int miss = (int)Statistics.GetCurrentFloat("ArrowsMissed");
                int accuracy = (int)((hit / (float)(hit + miss)) * 100f);
                Statistics.SetCurrent("Accuracy", accuracy, true);
                if (Distance <= 25)
                    Statistics.AddValue("Miss0to25", 1f);
                else if (Distance <= 50)
                    Statistics.AddValue("Miss25to50", 1f);
                else if (Distance <= 75)
                    Statistics.AddValue("Miss50to75", 1f);
                else if (Distance <= 100)
                    Statistics.AddValue("Miss75to100", 1f);
                else
                    Statistics.AddValue("MissOver100", 1f);
            }           
        }
        else if(isMine && obj.GetComponent<Teleporter>() == null)
        {
            eft.ApplyHitEffects(hPoint, c);
        }
        lodged = true;
        if(obj.GetComponent<Rigidbody>() != null)
        {
            float force = 80 * GetComponent<Rigidbody>().velocity.magnitude;
            p = new Vector4(hPoint.x, hPoint.y, hPoint.z, force);
           StartCoroutine("AddForce", obj);
        }
        //foreach (var v in GetComponentsInChildren<Collider>())
        //{
        //    //v.enabled = false;
        //}
        Vector3 offset = tip.position - transform.position;
        wantPos = hPoint - (offset*0.8f);
        transform.position = wantPos;
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(GetComponent<Rigidbody>());
        //GetComponent<Collider>().enabled = false;
        transform.SetParent(obj.transform);
        wantPos = transform.localPosition;
        if (interact != null)
            interact.enabled = true;
        if (ArrowsOut.Count > 50)
            Destroy(ArrowsOut[0].gameObject);
    }

    bool wasMiss(Vector3 hPoint, float flightDistance)
    {
        bool closeEnemy = CreatureManager.FindClosestEnemy(hPoint) < Mathf.Clamp(2 * Mathf.Sqrt(flightDistance), 5, 20);
        bool closeTarget = ArcheryGame.instance != null && ArcheryGame.instance.FindClosestTarget(hPoint) < Mathf.Clamp(2 * Mathf.Sqrt(flightDistance), 5, 20);
        bool closeDummy = TargetDummy.FindClosestDummy(hPoint) < Mathf.Clamp(2 * Mathf.Sqrt(flightDistance), 5, 20);
        bool HitObjEfft = false;
        ArrowEffects eft = GetComponent<ArrowEffects>();
        if(eft != null)
        {
            foreach(var e in eft.effects)
            {
                ItemEffect efct = ItemDatabase.GetEffect(e.EffectID);
                if(efct.type != EffectType.MissChance && efct.activation == ActivationType.HitObject)
                {
                    HitObjEfft = true;
                }
            }
        }
        if ((closeEnemy && !HitObjEfft) || closeTarget || closeDummy)
            return true;
        return false;
    }

    IEnumerator AddForce(GameObject obj)
    {
        yield return true;
        if(obj != null)
        {
            Rigidbody r = obj.GetComponent<Rigidbody>();
            if (r != null)
                r.AddExplosionForce(p.w, new Vector3(p.x, p.y, p.z), 3, 0.01f);
        }
    }

    void OnDestroy()
    {
        ArrowsOut.Remove(this);
    }

    public static void DestroyNearestArrow(Vector3 pos, float maxRadius=2f)
    {
        for(int i=ArrowsOut.Count-1; i>=0; i--)
        {
            if (ArrowsOut[i] == null)
                ArrowsOut.RemoveAt(i);
        }
        Arrow a = null;
        float currentMin = maxRadius;
        foreach(var v in ArrowsOut)
        {
            float d = Vector3.Distance(v.transform.position, pos);
            if (d < currentMin)
            {
                currentMin = d;
                a = v;
            }
        }
        if(a != null)
        {
            Destroy(a.gameObject);
        }
    }

    #endregion

}


