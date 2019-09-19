using Madorium.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput_PadControl : KeyInput
{
    public bool Pressed { get; set; }
    override public bool UpdatePress()
    {
        return Pressed;
    }
}

public class PadControl : MonoBehaviour {

    public static bool PCGrab = false;

    public Transform PlayerRig;
    public float LookTurnRateX = 50;
    public float LookTurnRateY = 50;
    public float DeadZone = 0.1f;
    public GameObject LeftController;
    public GameObject RightController;

    public GameObject LeftIdleLocator;
    public GameObject RightIdleLocator;

    public GameObject QuiverLocator;

    public GameObject BowDraw;
    public GameObject DrawStartLocator;
    public GameObject DrawEndLocator;

    public KeyInput_PadControl GrabKey;

    private ArrowGrabSequence _arrowGrabSequence;

    void Start()
    {
        _arrowGrabSequence = GetComponent<ArrowGrabSequence>();
        GrabKey = new KeyInput_PadControl();
        var ps4Input = PS4InputEx.GetInst();
        ps4Input.contollerState[0].Trigger.Inputs.Add(GrabKey);
    }

    void OnDestroy()
    {
        if (null != GrabKey)
        {
            var ps4Input = PS4InputEx.GetInst();
            ps4Input.contollerState[0].Trigger.Inputs.Remove(GrabKey);
            GrabKey = null;
        }
    }

    void EquipItemReward()
    {
        var reward = FindObjectOfType<ItemReward>();
        if (null == reward)
            return;
        reward.Equip();
    }

    void DeclineItemReward()
    {
        var reward = FindObjectOfType<ItemReward>();
        if (null == reward)
            return;
        reward.Reset();
    }

    // Update is called once per frame
    void Update () {
#if ENABLE_PAD_CONTROL
        var lStickX = Input.GetAxis("HorizontalMove");
        var lStickY = Input.GetAxis("VerticalMove");
        float turnX = lStickX * LookTurnRateX * Time.unscaledDeltaTime;
        float turnY = lStickY * LookTurnRateY * Time.unscaledDeltaTime;

#if ENABLE_CHEATS
        if(Input.GetButtonDown("L1"))
        {
            DeclineItemReward();
        }
        if(Input.GetButtonDown("R1"))
        {
            EquipItemReward();
        }
#endif

        var tfm = PlayerRig.transform;
        if (turnX != 0 || turnY != 0)
        {
            PlayerRig.transform.Rotate(new Vector3(0, 1, 0), turnX, Space.World);
            PlayerRig.transform.Rotate(PlayerRig.transform.right, turnY, Space.World);
            _arrowGrabSequence.OnMoved();
        }
#endif
    }
}