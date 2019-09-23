using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NVR_Bow : MonoBehaviour {

    NVR_Player nvplayer;

    public Transform BowAim;
    public Transform OrbAim;
    public AnimationCurve ReticleScale;

    public GameObject DummyArrow;
    public GameObject Bow;
    public GameObject DummyOrb;

    public AnimationCurve BowForceCurve;
    public AnimationCurve BowAnimCurve;
    public float OrbThrowForce = 10f;

    public LineRenderer lineRenderer;
    public LineRenderer orbLine;

    public LayerMask ImpactMask;

    TestCrossbow cb;
    BowAim baim;
    BowAnimation ba;
    BowAudio baud;
    float pull;
    float orbpull;
    float icd;

    void Awake()
    {
        nvplayer = GetComponent<NVR_Player>();
        cb = Bow.GetComponent<TestCrossbow>();
        ba = Bow.GetComponent<BowAnimation>();
        baud = Bow.GetComponent<BowAudio>();
        if (cb != null)
            baim = cb.gameObject.GetComponent<BowAim>();
    }

    public void ActivateNVR()
    {
        baim = Bow.GetComponent<BowAim>();
        baim.Display.transform.localPosition = baim.DisplayOffsetLeft;
    }

    float ScrollAmt;
    bool Scrolled;
    float LastScroll;
    bool canShoot;
    bool arrowDrawn;
    [HideInInspector]
    public bool orbReadied;
    float aim = 0;
    int aimPower;
    bool startAimed;
    void Update()
    {
        if(nvplayer.NonVR && canShoot)
        {
            icd -= Time.deltaTime;
            HandleBowDraw();
            HandleOrbDraw();
        }
    }

    void HandleBowDraw()
    {
        if (Input.GetMouseButton(0) && icd <= 0 && !orbReadied && !nvplayer.pos.forceFirstPerson)
        {
            if (!DummyArrow.activeInHierarchy)
            {
                DummyArrow.SetActive(true);
                DrawChance();
            }
            arrowDrawn = true;
            baim.OverrideHasArrow = true;
            ScrollAmt = Input.GetAxis("Mouse ScrollWheel");
            float lc = ScrollAmt;
            ScrollAmt *= (1 + LastScroll);
            LastScroll = Mathf.Abs(lc);
            if (Mathf.Abs(ScrollAmt) > 0f)
            {
                Scrolled = true;
                pull += ScrollAmt;
                if (pull <= 0)
                    pull = 0.05f;
                else if (pull > 1)
                    pull = 1;
            }
            if (pull < 1)
            {
                if (!Scrolled)
                    pull += Time.deltaTime;
                if (pull > 1)
                    pull = 1;
                SetValues(pull);
            }
            aim += Time.deltaTime;
            if(startAimed || aim > 2.5f)
            {
                startAimed = true;
                if (aim >= 1)
                {
                    aimPower++;
                    DummyArrow.GetComponent<DummyArrow>().SetAim(aimPower);
                    aim = 0;
                }
            }
        }
        else if (pull > 0)
        {
            Scrolled = false;        
            DummyArrow.SetActive(false);
            DummyArrow.GetComponent<DummyArrow>().SetAim(0);         
            Shoot();
            aim = 0;
            startAimed = false;
            aimPower = 0;
            pull = 0;
            SetValues(pull);
        }
    }

    public bool isAiming()
    {
        return arrowDrawn;
    }

    public bool cantOrb;

    void HandleOrbDraw()
    {
        if (!arrowDrawn && Input.GetKey(KeyCode.Space) && OrbManager.instance.CanThrow())
        {
            orbReadied = true;
            if(!DummyOrb.activeInHierarchy)
            {
                DummyOrb.SetActive(true);
                DummyOrb.GetComponent<DummyOrb>().Setup(OrbManager.instance.GetEffectID());
                OrbManager.instance.ReadyOrb();
            }
            ScrollAmt = Input.GetAxis("Mouse ScrollWheel");
            float lc = ScrollAmt;
            ScrollAmt *= (1 + LastScroll);
            LastScroll = Mathf.Abs(lc);
            if (Mathf.Abs(ScrollAmt) > 0f)
            {
                Scrolled = true;
                orbpull += ScrollAmt;
                if (orbpull <= 0)
                    orbpull = 0.05f;
                else if (orbpull > 1)
                    orbpull = 1;
            }
            if (orbpull < 1)
            {
                if (!Scrolled)
                    orbpull += Time.deltaTime;
                if (orbpull > 1)
                    orbpull = 1;
            }
        }
        else if (orbpull > 0)
        {
            Scrolled = false;
            OrbManager.instance.ThrowNVR(orbpull*OrbThrowForce, DummyOrb.transform.position, DummyOrb.transform.forward);
            DummyOrb.SetActive(false);
            orbReadied = false;
            orbpull = 0;
        }
    }

    public void Aim()
    {
        DrawBowLine();
        DrawOrbLine();
    }

    public void ToggleActive(bool val)
    {
        canShoot = val;
        lineRenderer.enabled = val;
        BowAim.gameObject.SetActive(val);
        Bow.SetActive(val);
    }

    public void DrawChance()
    {
        if (ArrowEffects.EffectsDisabled)
            return;
        EffectInstance[] on = Armory.ArmorEffects().ToArray();
        var db = ItemDatabase.a.Effects;
        DummyArrow a = DummyArrow.GetComponent<DummyArrow>();
        foreach (var v in on)
        {
            ItemEffect eff = ItemDatabase.GetEffect(v.EffectID);
            float chance = eff.StaticValue;
            if (eff.randomType == RandomType.RandomChance)
                chance = eff.VariableValue + v.EffectValue;
            if (ItemDatabase.GetEffect(v.EffectID).type == EffectType.DrawChance && UnityEngine.Random.Range(0, 100) < chance)
            {
                a.DisplayEffect(v.EffectID);
                cb.AppliedEffects.Add(new Vector2(v.EffectID, v.EffectValue));

            }
            else if (ItemDatabase.GetEffect(v.EffectID).type == EffectType.Passive || ItemDatabase.GetEffect(v.EffectID).type == EffectType.MissChance)
            {
                a.DisplayEffect(v.EffectID);
                cb.AppliedEffects.Add(new Vector2(v.EffectID, v.EffectValue));
            }
        }
    }

    public void SetArrowEffect(int eid, float val)
    {
        DummyArrow.GetComponent<DummyArrow>().DisplayEffect(eid);
        cb.AppliedEffects.Add(new Vector2(eid, val));
    }

    void DrawBowLine()
    {
        if (pull > 0)
        {
            UpdateTrajectory(cb.launchpoint.position, cb.launchpoint.forward, cb.LaunchSpeed, 0.01f, 100f, lineRenderer, BowAim);
        }
        else
        {
            UpdateTrajectory(cb.launchpoint.position, cb.launchpoint.forward, BowForceCurve.Evaluate(1), 0.01f, 100f, lineRenderer, BowAim);
        }
    }

    void DrawOrbLine()
    {
        if (orbpull > 0)
        {
            UpdateTrajectory(DummyOrb.transform.position, DummyOrb.transform.forward, orbpull*OrbThrowForce, 0.01f, 100f, orbLine, OrbAim);
            BowAim.position = Vector3.zero;
        }
        else
        {
            orbLine.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            OrbAim.position = Vector3.zero;
        }
    }

    float bowPull = 0f;
    void SetValues(float t)
    {
        ba.SetFrame(BowAnimCurve.Evaluate(t) * 3);
        bowPull = t;
        cb.LaunchSpeed = BowForceCurve.Evaluate(t);
    }

    void Shoot()
    {
        icd = 0.25f;
        baud.PlayTwang(bowPull * 0.8f);
        cb.aim = aimPower;
        cb.Launch();
        arrowDrawn = false;
        baim.OverrideHasArrow = false;
    }

    #region Trajectory Display
    Vector3 lineNormal;
    Vector3 hitPos;
    void UpdateTrajectory(Vector3 startPos, Vector3 direction, float speed, float timePerSegmentInSeconds, float maxTravelDistance, LineRenderer r, Transform aim)
    {
        var positions = new List<Vector3>();
        Vector3 lastPos = startPos;
        Vector3 currentPos = startPos;
        positions.Add(startPos);

        float traveledDistance = 0.0f;
        bool hasHitSomething = false;
        while (traveledDistance < maxTravelDistance)
        {
            traveledDistance += speed * timePerSegmentInSeconds;
            hasHitSomething = TravelTrajectorySegment(currentPos, direction, speed, timePerSegmentInSeconds, positions);
            if (hasHitSomething)
            {
                break;
            }
            timePerSegmentInSeconds *= 1.075f;
            lastPos = currentPos;
            currentPos = positions[positions.Count - 1];
            direction = currentPos - lastPos;
            direction.Normalize();
        }
        r.widthMultiplier = Mathf.Clamp(traveledDistance / 125f, 0.025f, 1.5f);
        BuildTrajectoryLine(positions);
        if (hasHitSomething)
        {
            if (!aim.gameObject.activeSelf && canShoot)
                aim.gameObject.SetActive(true);
            aim.position = hitPos;
            aim.up = lineNormal;
            aim.localScale = Vector3.one * ReticleScale.Evaluate(traveledDistance);
        }
        else
            aim.gameObject.SetActive(false);
    }

    bool TravelTrajectorySegment(Vector3 startPos, Vector3 direction, float speed, float timePerSegmentInSeconds, List<Vector3> positions)
    {
        var newPos = startPos + direction * (speed * 10f) * timePerSegmentInSeconds + Physics.gravity * timePerSegmentInSeconds;
        RaycastHit hitInfo;
        var hasHitSomething = Physics.Linecast(startPos, newPos, out hitInfo, ImpactMask);
        if (hasHitSomething)
        {
            newPos = hitInfo.point;
            hitPos = newPos;
            lineNormal = hitInfo.normal;
        }
        positions.Add(newPos);
        return hasHitSomething;
    }

    void BuildTrajectoryLine(List<Vector3> positions)
    {
        lineRenderer.positionCount = (positions.Count);
        for (var i = 0; i < positions.Count; ++i)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }

    # endregion
}
