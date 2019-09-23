using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using UnityEngine.Events;
using I2.Loc;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager instance;
    public TutorialSequence[] Tutorials;
    public ScrollDisplay[] ScrollImages;
    public TutorialScroll Scroll;
    public GameObject[] TutorialObjects;
    public Teleporter[] BaseTeleporters;
    Teleporter LastStart;
    public GameObject Owl;
    splineMove Movement;
    public static bool inTutorial;
    //Base Owl Location
    Vector3 OwlStartPos;
    Quaternion OwlStartRot;

    public UnityEvent OnReset;

    string TutEventCompleted;

    #region Life Cycle

    void Awake()
    {
        instance = this;
        Movement = Owl.GetComponent<splineMove>();
        OwlStartPos = Owl.transform.position;
        OwlStartRot = Owl.transform.rotation;
        InitListeners();
    }

    void Start()
    {
        Owl.GetComponent<OwlEvents>().ToggleInvincible(false);
    }

    public static void Reset()
    {
        if (instance != null)
            instance.ResetSystem();
    }

    public static void TryToggleTutorial()
    {
        if (inTutorial)
            Reset();
        else
            TryStartTutorial();
    }

    public static void TryStartTutorial()
    {
        if(instance != null)
        {
            instance.TryTutorial();
        }
    }

    public void ResetSystem()
    {
        inTutorial = false;
        StopAllCoroutines();
        TutEventCompleted = "";
        TryHideScroll();
        ClearListeners();
        OnReset.Invoke();
        Owl.GetComponent<OwlEvents>().ToggleInvincible(false);
        if(Quiver.instance != null)
            Quiver.instance.Disabled = false;
        foreach (var v in TutorialObjects)
        {
            v.SetActive(false);
        }
        ToggleBaseTeleporters(true);
        if(LastStart != null)
            LastStart.OnLeave.RemoveListener(LeftStartTP);
        if (Vector3.Distance(Owl.transform.position, OwlStartPos) > 1f)
            Owl.GetComponent<OwlEvents>().Teleport(OwlStartPos, OwlStartRot);
        Owl.GetComponent<OwlAnimation>().PlayAnim(owlAnim.Stand);
        Owl.GetComponent<OwlSounds>().StopAudio();
    }
    #endregion

    public void TryTutorial()
    {
        if(TelePlayer.instance != null && !PhotonNetwork.inRoom)
        {
            PlatformSetup.CheckVRUnit();
            Teleporter pTp = TelePlayer.instance.currentNode;
            for(int i=0; i<Tutorials.Length; i++)
            {
                if(Tutorials[i].tpStart == pTp)
                {
                    Debug.Log("Starting Tutorial: " + Tutorials[i].Title);
                    StopAllCoroutines();
                    StartCoroutine("RunTutorial", i);
                }
            }
        }
    }

    public void ToggleBaseTeleporters(bool enable)
    {
        if(enable)
        {
            foreach (var v in BaseTeleporters)
            {
                if (TelePlayer.instance == null || TelePlayer.instance.currentNode != v)
                    v.EnableTeleport();
            }
        }
        else
        {
            foreach (var v in BaseTeleporters)
            {
                v.DisableTeleport();
            }
        }
    }

    IEnumerator RunTutorial(int tid)
    {
        inTutorial = true;
        SetupListeners();
        Owl.GetComponent<OwlEvents>().ToggleInvincible(true);
        Owl.GetComponent<OwlRagdoll>().Reset();
        TutEventCompleted = "";
        TutorialSequence T = Tutorials[tid];
        if(T.LeaveResets)
        {
            LastStart = T.tpStart;
            LastStart.OnLeave.AddListener(LeftStartTP);
        }
        yield return new WaitForSeconds(0.5f);
        splineMove spline = Owl.GetComponent<splineMove>();
        spline.pathContainer = null;
        OwlSounds audio = Owl.GetComponent<OwlSounds>();
        PathManager path = null;
        bool startedMove = false; 
        if (T.path != null)
        {
            path = T.path.GetComponent<PathManager>();
            Vector3 stPt = T.path.transform.position;
            Quaternion stRot = T.path.transform.rotation;
            if(path != null)
            {
                stPt = path.waypoints[0].position;
                stRot = path.waypoints[0].rotation;
                spline.SetPath(path);
                SetPaths();
            }
            Owl.GetComponent<OwlEvents>().Teleport(stPt, stRot);
            Owl.GetComponent<OwlAnimation>().PlayAnim(owlAnim.Hover);
        }
        for(int i=0; i<T.steps.Length; i++)
        {
            TutorialStep step = T.steps[i];
            TutEventCompleted = ""; //Clear existing Event to start step with clean slate
            //Check Valid Step for Current State
            if (step.SpecialCaseStart.Length == 0 || CheckSpecialCase(step.SpecialCaseStart))
            {
                if(spline.pathContainer != null)
                    Owl.transform.SetPositionAndRotation(spline.pathContainer.waypoints[spline.currentPoint].position, spline.pathContainer.waypoints[spline.currentPoint].rotation);
                //Variable Setup
                yield return new WaitForSeconds(step.StartDelay);
                float perc = 0;
                float t = 0;
                List<TutorialEvent> Events = new List<TutorialEvent>();
                foreach (var v in step.events) { Events.Add(v); }

                //Move next location
                if (step.PathMoveNext && path != null)
                    spline.Resume();
                //Play Clip
                audio.PlayAudioclip(step.clip);
                string loc = "";
                if(step.SubtitleID.Length > 0)
                    loc = ScriptLocalization.Get("Tutorial/" +step.SubtitleID);
                if (loc == null || loc.Length < 1)
                {
                    loc = step.SubtitleText;
                    Debug.Log("Could not find Localized Text: Tutorial/" + step.SubtitleID);
                }
                Subtitles.Show(loc, step.clip.length);
                //Show Tut Scroll if needed, else try close
                if (step.ScrollID.Length > 0)
                    ShowScroll(step.ScrollID);
                else
                    TryHideScroll();
                //Wait for Clip Finished or Requirement met
                while (audio.isPlaying())
                {
                    //End Early if Requirement met
                    if (step.CompleteStepCheck.Length > 0 && step.CompleteStepCheck == TutEventCompleted)
                        break;

                    //Check for Events during clip
                    t += Time.deltaTime;
                    perc = t / step.clip.length;
                    for (int j = 0; j < Events.Count; j++)
                    {
                        if (Events[j].TriggerTime < perc && Events[j].TriggerTime < 1)
                        {
                            Events[j].TEvent.Invoke();
                            Events.RemoveAt(j);
                        }
                    }
                    yield return true;
                }
                for (int j = 0; j < Events.Count; j++)
                {
                    if (Events[j].TriggerTime >= 1)
                    {
                        Events[j].TEvent.Invoke();
                        Events.RemoveAt(j);
                    }
                }
                //Ensure requirement met even if clip finished
                if (step.CompleteStepCheck.Length > 0)
                {
                    t = 0;
                    while (step.CompleteStepCheck != TutEventCompleted)
                    {
                        t += Time.deltaTime;
                        if (t > 15 && step.ExtraPrompt != null)
                        {
                            t = 0;
                            audio.PlayAudioclip(step.ExtraPrompt);
                            loc = ScriptLocalization.Get("Tutorial/" + step.ExtraSubtitleID);
                            if (loc.Length < 1)
                            {
                                loc = step.ExtraSubtitleID;
                                Debug.Log("Could not find Localized Text: Tutorial/" + step.ExtraSubtitleID);
                            }
                            Subtitles.Show(loc, step.clip.length);
                        }
                        yield return true;
                    }
                }
                TryHideScroll();
            }    
        }
        yield return new WaitForSeconds(T.EndDelay);
        Reset();
    }

    #region Utility

    public void TryHideScroll()
    {
        if (Scroll.showing)
            Scroll.Hide();
    }

    public void ShowScroll(string ScrollID)
    {
        foreach(var v in ScrollImages)
        {
            if(v.ID == ScrollID)
            {
                ScrollDisplay.ScrollImage t = v.GetImage(AppBase.v.controls);
                if (t != null)
                    Scroll.Show(t);
                else
                    Debug.Log("No " + AppBase.v.controls + " image for " + ScrollID);
            }
        }
    }

    public void EventCompleted(string promptID)
    {
        if(inTutorial)
        {
            TutEventCompleted = promptID;
        }
    }

    void SetPaths()
    {
        splineMove move = Owl.GetComponent<splineMove>();
        foreach (var v in move.events)
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

    #endregion

    #region Special Functions

    UnityAction<OrbAbility> OrbSummon;
    UnityAction ArrowGrabbed;
    UnityAction OpenedWrist;
    UnityAction<Teleporter> LeftStartTP;
    void InitListeners()
    {
        OrbSummon = new UnityAction<OrbAbility>(OrbCreated);
        ArrowGrabbed = new UnityAction(delegate {EventCompleted("ARROW_GRABBED"); });
        OpenedWrist = new UnityAction(delegate { EventCompleted("OPENED_WRIST"); });
        LeftStartTP = new UnityAction<Teleporter>(delegate { ResetSystem(); });
    }

    void OrbCreated(OrbAbility orb)
    {
        EventCompleted("ORB_SUMMONED");
        orb.OnThrow.AddListener(delegate { EventCompleted("ORB_THROWN"); });
    }

    public void SetupListeners()
    {
        OrbManager.instance.OnCreate.AddListener(OrbSummon);
        Quiver.instance.OnGrabbedArrow.AddListener(ArrowGrabbed);
        GameCanvas.instance.OnPanelOpen.AddListener(OpenedWrist);
    }

    public void ClearListeners()
    {
        if(OrbManager.instance != null && OrbSummon != null)
            OrbManager.instance.OnCreate.RemoveListener(OrbSummon);
        if (Quiver.instance != null && ArrowGrabbed != null)
            Quiver.instance.OnGrabbedArrow.RemoveListener(ArrowGrabbed);
        if (GameCanvas.instance != null && OpenedWrist != null)
            GameCanvas.instance.OnPanelOpen.RemoveListener(OpenedWrist);
    }

    public bool CheckSpecialCase(string caseID)
    {
        if(caseID == "NO_COSMETICS")
        {
            int numCosmetics = Cosmetics.Titles.Count + Cosmetics.WingIDs.Count;
            return numCosmetics < 2;
        }
        if(caseID == "MANY_COSMETICS")
        {
            int numCosmetics = Cosmetics.Titles.Count + Cosmetics.WingIDs.Count;
            return numCosmetics > 5;
        }
        if(caseID == "FEW_ITEMS")
        {
            int numItems = Armory.instance.Options.Count;
            return numItems < 10;
        }
        return false;
    }

    public void SpecialAction(string actionID)
    {
        if(actionID == "RESET_ORB_CD")
            OrbManager.instance.ResetCooldown();
        if(actionID == "DISABLE_QUIVER")
            Quiver.instance.Disabled = true;
        if(actionID == "ENABLE_QUIVER")
            Quiver.instance.Disabled = false;
        if (actionID == "OPEN_MENU" && !ToggleMenu.instance.isOpen())
            ToggleMenu.instance.Toggle();
        if(actionID == "ENSURE_ORB")
        {
            if (Armory.currentOutfit.Gloves == null || Armory.currentOutfit.Gloves.GetEffect().EffectID == 0)
                Armory.instance.EquipItem(Armory.BaseItem(ItemType.Gloves), false);
        }
        if(actionID == "CLEAR_TPS")
        {
            ToggleBaseTeleporters(false);
        }
    }

    #endregion

    #region Custom Classes

    [System.Serializable]
    public class TutorialSequence
    {
        public string Title;
        public Teleporter tpStart;
        public bool LeaveResets = true;
        public GameObject path;
        public TutorialStep[] steps;
        public float EndDelay;

        public override string ToString()
        {
            if (Title != null)
                return Title;
            return base.ToString();
        }
    }

    [System.Serializable]
    public class TutorialStep
    {
        public float StartDelay;
        public AudioClip clip;
        public string SubtitleID;
        [TextArea]
        public string SubtitleText;
        public TutorialEvent[] events;
        [Header("Optional")]
        public bool PathMoveNext;
        public string ScrollID;
        public string SpecialCaseStart;
        public string CompleteStepCheck;
        [AdvancedInspector.Inspect("HasCheck")]
        public AudioClip ExtraPrompt;
        [AdvancedInspector.Inspect("HasCheck")]
        public string ExtraSubtitleID;

        bool HasCheck()
        {
            return CompleteStepCheck != null && CompleteStepCheck.Length > 0;
        }

        public override string ToString()
        {
            if (SubtitleID != null)
                return SubtitleID;
            return base.ToString();
        }
    }

    [System.Serializable]
    public class TutorialEvent
    {
        public UnityEvent TEvent;
        [Range(0, 1)]
        public float TriggerTime;
    }

    [System.Serializable]
    public class ScrollDisplay
    {
        public string ID;
        public ScrollImage[] Images;

        public ScrollImage GetImage(ControllerType c)
        {
            for(int i=0; i<Images.Length; i++)
            {
                if (Images[i].Controller == c)
                    return Images[i];
            }
            if (Images.Length > 0)
                return Images[0];
            return null;
        }

        public override string ToString()
        {
            if (ID != null)
                return ID;
            return base.ToString();
        }

        [System.Serializable]
        public class ScrollImage
        {
            public ControllerType Controller;
            public Texture2D image;
            public Texture2D illum;

            public override string ToString()
            {
                return Controller.ToString();
            }
        }
    }

    #endregion

}
