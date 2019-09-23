using UnityEngine;
using System.Collections;
using SWS;
using VRTK.UnityEventHelper;
using UnityEngine.Events;
using VRTK;
using UnityEngine.UI;
using UnityEngine.VR; 

public class Tutorial : MonoBehaviour {

    public Teleporter teleporter;
    public Teleporter EndTeleporter;
    public bool inTutorial;
    public bool finishedTutorial;
    public bool TutStart;

    public GameObject shackledEnemy;

    public GameObject Owl;
    OwlAnimation oanim;
    OwlSounds voice;
    splineMove move;

    public Transform[] EasyTargets;
    public Transform[] MedTargets;

    public AudioClip[] OwlClips;

    public AudioClip SavedClip;
    public AudioClip TPRepeatClip;

    public GameObject Globes;
    public GameObject ReturnGlobe;
    public GameObject GloveItem;
    public GameObject Manequin;
    public GameObject HandDisplay;

    public GameObject InEffect;
    public GameObject OutEffect;
    public GameObject InEffectSmall;
    public GameObject OutEffectSmall;
    public GameObject PhysicsObjects;

    public GameObject OwlPortal;

    AudioClip curClip;
    int curID = 0;

    #region Base Methods

    void Awake()
    {
        try {
            oanim = Owl.GetComponent<OwlAnimation>();
            voice = Owl.GetComponent<OwlSounds>();
            move = Owl.GetComponent<splineMove>();
            Owl.SetActive(false);
        } catch { }
    }

    void Start()
    {
        Destroy(teleporter.GetComponentInChildren<ParticleSystem>());
        if (TutStart)
            Setup();
    }

    [BitStrap.Button]
	public void Setup()
    {
        inTutorial = true;
        SingleplayerSpawn.instance.startNode = teleporter;
        teleporter.OnTeleport.AddListener(delegate { StartTutorial(); });
    }

    public void StartTutorialImmediate()
    {
        teleporter.ForceUse();
        inTutorial = true;
        StartTutorial();
    }

    void StartTutorial()
    {
        EndTeleporter.Invoke("Hide", 0.2f);
        StartCoroutine("RunIntro");
    }

    public void Finished()
    {
        PlatformSetup.instance.IncrementVersionValue("TutorialCompleted", 1);
        SingleplayerSpawn.instance.startNode = EndTeleporter;
        inTutorial = false;
        finishedTutorial = true;
        Settings.Set("DoneTutorial", true);
    }

    float skipHold = 0;
    GameObject fh;
    VRTK_ControllerEvents freeHand;
    VRTK_ControllerEvents bowhand;
    public Text SkipText;
    public VRTK_InteractableObject_UnityEvents HomeOrb;
    void Update()
    {
        if(inTutorial)
        {
            if (SteamVR_ControllerManager.freeHand != null && SteamVR_ControllerManager.freeHand != fh)
            {
                fh = SteamVR_ControllerManager.freeHand;
                freeHand = fh.GetComponent<VRTK_ControllerEvents>();
                bowhand = SteamVR_ControllerManager.bowHand.GetComponent<VRTK_ControllerEvents>();
            }
            if (freeHand != null && bowhand != null)
            {
                if(UnityEngine.XR.XRDevice.model.Contains("Vive"))
                    SkipText.text = "Hold down [Both Touchpads] to skip tutorial";
                else if (UnityEngine.XR.XRDevice.model.Contains("Oculus"))
                    SkipText.text = "Hold down [Both Thumbsticks] to skip tutorial";
                else
                    SkipText.text = "Hold down [Spacebar] to skip tutorial";
            }
            else
                SkipText.text = "Hold down [Spacebar] to skip tutorial";
            if (Input.GetKey(KeyCode.Space) || (freeHand != null && freeHand.touchpadPressed && bowhand.touchpadPressed))
            {
                skipHold += Time.fixedDeltaTime;
                SkipText.text += ": " + (5 - (int)skipHold);
            }
            else
                skipHold = 0;
            if(skipHold >= 5)
            {
                StopAllCoroutines();
                inTutorial = false;
                Settings.Set("DoneTutorial", true);
                SkipText.text = "Leaving Tutorial - Loading Main Scene...";
                HomeOrb.Use();
            }
        }
    }

    #endregion

    IEnumerator RunIntro()
    {
        EndTeleporter.DisableTeleport();
        EndTeleporter.Hide();
        yield return new WaitForSeconds(1f);
        Debug.Log("Running Tutorial");
        Owl.SetActive(true);
        oanim.PlayAnim(owlAnim.Hover);
        WarpIn(Owl.transform.position);
        Quiver.instance.Disabled = true;
        Globes.SetActive(false);
        SetBools();
        SetPaths();
        curID = 0;
        yield return new WaitForSeconds(0.5f);
        move.StartMove();
        voice.PlayAudioclip(GetCurClip()); //Welcome Back
        yield return new WaitForSeconds(ClipWait() + 0.7f);
        voice.PlayAudioclip(GetCurClip()); //Bow and Quiver
        yield return new WaitForSeconds(ClipWait() + 1f);
        move.Resume();
        voice.PlayAudioclip(GetCurClip()); //CMD: Grab an Arrow
        Quiver.instance.Disabled = false;
        Quiver.instance.OnGrabbedArrow.AddListener(delegate { grabbedArrow = true; });
        float t = 0;
        while(!grabbedArrow)
        {
            yield return true;
            t += Time.deltaTime;
            if(t > 15)
            {
                t = 0;
                voice.PlayAudioclip(curClip);
            }
        }
        voice.PlayAudioclip(GetCurClip()); //CMD: Now hit targets
        ArcheryGame.instance.SpawnTargets(EasyTargets.Length, EasyTargets);
        ArcheryGame.instance.OnCompletedSet.AddListener(delegate { easyTargs = true; });
        while(!easyTargs)
        {
            yield return true;
        }
        CheckOwlPortal();
        ArcheryGame.instance.OnCompletedSet.RemoveAllListeners();
        curID++;
        /*
        voice.PlayAudioclip(GetCurClip()); //CMD: Good Now those
        yield return new WaitForSeconds(0.7f);
        ArcheryGame.instance.SpawnTargets(MedTargets.Length, MedTargets);
        ArcheryGame.instance.OnCompletedSet.AddListener(delegate { medTargs = true; });
        while (!medTargs)
        {
            yield return true;
        }
        ArcheryGame.instance.OnCompletedSet.RemoveAllListeners();
        */
        move.Resume();
        yield return new WaitForSeconds(0.5f);
        voice.PlayAudioclip(GetCurClip()); //CMD: Info Vital - Open Panel
        GameCanvas.instance.OnPanelOpen.AddListener(delegate { openedPanel = true; });
        yield return new WaitForSeconds(1f);
        HandDisplay.SetActive(true);
        WarpIn(HandDisplay.transform.position);
        while (!openedPanel)
        {
            yield return true;
        }
        GameCanvas.instance.OnPanelOpen.RemoveAllListeners();
        voice.PlayAudioclip(GetCurClip()); //Panel also shows...
        yield return new WaitForSeconds(ClipWait() + 1f);
        HandDisplay.SetActive(false);
        WarpOut(HandDisplay.transform.position);
        voice.PlayAudioclip(GetCurClip()); //Equipment might be useful
        yield return new WaitForSeconds(ClipWait() + 0.7f);
        move.Resume();
        voice.PlayAudioclip(GetCurClip()); //CMD: Here a glove for you
        PhysicsObjects.SetActive(true);
        WarpIn(GloveItem.transform.position);
        GloveItem.SetActive(true);
        GloveItem.GetComponent<VRTK_InteractableObject_UnityEvents>().OnUse.AddListener(delegate { grabbedGlove = true; });
        while(!grabbedGlove)
        {
            yield return true;
        }
        WarpOut(GloveItem.transform.position);
        GloveItem.SetActive(false);
        voice.PlayAudioclip(GetCurClip()); //CMD: Click Touchpad on Free Hand
        OrbManager.instance.ResetCooldown();
        OrbManager.instance.OnCreate.AddListener(OrbCreated);
        t = 0;
        while (!threwOrb)
        {
            yield return true;
            t += Time.deltaTime;
            if (t > 15)
            {
                t = 0;
                voice.PlayAudioclip(curClip);
            }
        }
        yield return new WaitForSeconds(1f);
        voice.PlayAudioclip(GetCurClip()); //Well Done, more explaining
        yield return new WaitForSeconds(ClipWait() + 0.7f);
        move.Resume();
        voice.PlayAudioclip(GetCurClip()); //Here, a representation...
        WarpIn(Manequin.transform.position);
        Manequin.SetActive(true);
        yield return new WaitForSeconds(ClipWait());
        voice.PlayAudioclip(GetCurClip()); //You will get more items
        yield return new WaitForSeconds(ClipWait());
        WarpOut(Manequin.transform.position);
        Manequin.SetActive(false);
        curID++;
        curID++;
        //voice.PlayAudioclip(GetCurClip()); //You will need that power to destroy foes
        //yield return new WaitForSeconds(ClipWait());
        /*
        voice.PlayAudioclip(GetCurClip()); //Here is the enemy
        yield return new WaitForSeconds(0.6f);
        WarpIn(shackledEnemy.transform.position, true);
        shackledEnemy.SetActive(true);
        shackledEnemy.GetComponentInChildren<Health>().OnDeath.AddListener(delegate { killed = true; });
        shackledEnemy.GetComponent<Shackles>().OnRelease.AddListener(delegate { released = true; });
        while(!killed && !released)
        {
            yield return true;
        }
        if(killed)
        {
            yield return new WaitForSeconds(1f);
            DissolveEnemy();
            yield return new WaitForSeconds(2f);
        }
        else if(released)
        {
            yield return new WaitForSeconds(1f);
            voice.PlayAudioclip(SavedClip);
            yield return new WaitForSeconds(SavedClip.length);
        }
        WarpOut(shackledEnemy.transform.position, true);
        shackledEnemy.SetActive(false);
        */
        yield return new WaitForSeconds(0.5f);
        voice.PlayAudioclip(GetCurClip()); //Power doesn't end, teleporter
        EndTeleporter.Show();
        EndTeleporter.EnableTeleport();
        EndTeleporter.OnTeleport.AddListener(delegate { teleported = true; });
        t = -10;
        while (!teleported)
        {
            yield return true;
            t += Time.deltaTime;
            if (t > 15)
            {
                t = 0;
                voice.PlayAudioclip(TPRepeatClip);
            }
        }
        move.Resume();
        EndTeleporter.OnTeleport.RemoveAllListeners();
        voice.PlayAudioclip(GetCurClip()); //Spirit Anchor has other powers, Globes
        yield return new WaitForSeconds(ClipWait());
        WarpIn(Globes.transform.position);
        ReturnGlobe.SetActive(true);
        //Turorial Complete
        Finished();
        voice.PlayAudioclip(GetCurClip()); //Amazing what ancestors have done
        yield return new WaitForSeconds(ClipWait() + 0.3f);
        voice.PlayAudioclip(GetCurClip()); //Meet me at keep when ready
        yield return new WaitForSeconds(ClipWait());
        WarpOut(Owl.transform.position);
        Owl.SetActive(false);
    }

    void CheckOwlPortal()
    {
        if(Armory.HasEffectEquipped(13))
        {
            OwlPortal.SetActive(true);
        }
    }

    AudioClip GetCurClip()
    {
        AudioClip c = OwlClips[0];
        if (curID < OwlClips.Length && curID >= 0)
            c = OwlClips[curID];
        curClip = c;
        curID++;
        if (curID >= OwlClips.Length)
            curID = OwlClips.Length - 1;
        return c;
    }

    float ClipWait()
    {
        if(curClip != null)
            return curClip.length;
        return 0.25f;
    }

    public void WarpIn(Vector3 pos, bool big=false)
    {
        AudioSource src = InEffect.GetComponent<AudioSource>();
        ParticleSystem pt = InEffect.GetComponent<ParticleSystem>();
        if (!big)
        {
            src = InEffectSmall.GetComponent<AudioSource>();
            pt = InEffectSmall.GetComponent<ParticleSystem>();
        }
        InEffect.transform.position = pos;
        InEffectSmall.transform.position = pos;
        src.Play();
        pt.Play();
    }

    public void WarpOut(Vector3 pos, bool big = false)
    {
        AudioSource src = OutEffect.GetComponent<AudioSource>();
        ParticleSystem pt = OutEffect.GetComponent<ParticleSystem>();
        if(!big)
        {
            src = OutEffectSmall.GetComponent<AudioSource>();
            pt = OutEffectSmall.GetComponent<ParticleSystem>();
        }
        OutEffect.transform.position = pos;
        OutEffectSmall.transform.position = pos;
        src.Play();
        pt.Play();
    }

    #region TutorialSpecifics

    void DissolveEnemy()
    {
        foreach(var v in shackledEnemy.GetComponentsInChildren<Rigidbody>())
        {
            v.useGravity = false;
        }
        foreach(var v in shackledEnemy.GetComponentsInChildren<BeautifulDissolves.Dissolve>())
        {
            v.TriggerDissolve();
        }
    }

    void SetPaths()
    {
        foreach(var v in move.events)
        {
            v.AddListener(delegate {
                move.Pause();
                Owl.transform.LookAt(PlayerHead.instance.transform.position);
                Vector3 r = Owl.transform.eulerAngles;
                r.x = 0; r.z = 0;
                Owl.transform.localEulerAngles = r;
            });
        }
    }

    void SetBools()
    {
        grabbedArrow = false;
        easyTargs = false;
        medTargs = false;
        openedPanel = false;
        killed = false;
        released = false;
        grabbedGlove = false;
        threwOrb = false;
        teleported = false;
    }

    bool grabbedArrow;
    bool easyTargs;
    bool medTargs;
    bool openedPanel;
    bool killed;
    bool released;
    bool grabbedGlove;
    bool threwOrb;
    bool teleported;

    void OrbCreated(OrbAbility orb)
    {
            orb.OnThrow.AddListener(delegate { threwOrb = true; });
    }

    #endregion
}
