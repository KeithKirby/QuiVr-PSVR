using UnityEngine;
using UnityEngine.VR;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class SetupManager : MonoBehaviour
{
    Animator stateMachine;

    void Start()
    {
        stateMachine = GetComponent<Animator>();

#if UNITY_PS4
        // Register the callback needed to detect resetting the HMD
        Utility.onSystemServiceEvent += OnSystemServiceEvent;
#endif

        if (UnityEngine.XR.XRSettings.enabled == false)
        {
            stateMachine.SetTrigger("Need HMD Setup");
        }
        else
        {
            stateMachine.SetTrigger("Start Instructions");
            VRManager.instance.BeginVRSetup();
        }
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = stateMachine.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.HMD Setup"))
        {
            if(UnityEngine.XR.XRSettings.enabled == true)
                stateMachine.SetTrigger("Start Instructions");
            else if (Input.GetButtonDown("Fire1"))
                VRManager.instance.SetupHMDDevice();
        }
    }

    public void FinishedUIInteraction()
    {
        stateMachine.SetTrigger("Recentering");
    }

    public void ProgressToMainMenu()
    {
        FindObjectOfType<SceneSwitcher>().SwitchToScene("PSVRExample_MainMenu");
    }

#if UNITY_PS4
    void OnSystemServiceEvent(Utility.sceSystemServiceEventType eventType)
    {
        if (stateMachine == null)
            return;

        AnimatorStateInfo stateInfo = stateMachine.GetCurrentAnimatorStateInfo(0);

        if (eventType == Utility.sceSystemServiceEventType.ResetVrPosition && stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Recentering"))
        {
            stateMachine.SetTrigger("Finished");
        }
    }
#endif
}
