using Madorium.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class MoveControllerState
{
    public KeyState MenuA = new KeyState();
    public KeyState MenuB = new KeyState();
    public KeyState CrossButton = new KeyState(); // X on PS4
    public KeyState CircleButton = new KeyState(); // O on PS4

    public KeyState Zap = new KeyState();

    public KeyState Trigger = new KeyState();

    public Vector3 Velocity = new Vector3();
    public Vector3 LastPosition = new Vector3();


    public void Init()
    {
    }

    public void Update()
    {
        MenuA.Update();
        MenuB.Update();
        CrossButton.Update(); // X on PS4
        CircleButton.Update(); // O on PS4
        Zap.Update();
        Trigger.Update();        
    }

    public void UpdatePosition(Vector3 position, float dt)
    {   
        var newPosition = position;
        Velocity = newPosition - LastPosition;
        var scl = 1 / dt; // Multiply up so velocity is per second
        Velocity.Scale(new Vector3(scl, scl, scl));
        LastPosition = position;
    }
}

public class PS4InputEx : MonoBehaviour
{
    static PS4InputEx s_input;
    public MoveControllerState[] contollerState = new MoveControllerState[] { new MoveControllerState(), new MoveControllerState() };

    public static PS4InputEx GetInst() { return s_input; }

    public static float GrabRight()
    {
        float gr;
#if UNITY_EDITOR
        gr = Input.GetAxis("GrabRight");
        if(gr==0)
        {
            gr = 0;
        }
        else
        {
            gr = (gr + 1.0f) / 2.0f;
        }
#else
        gr = Input.GetAxis("GrabRightPS4");
        gr = -gr;
#endif
        return gr;
    }

    void Awake()
    {
        if (null == s_input)
        {
            s_input = this;
#if UNITY_PS4
            // Right hand            
            contollerState[0].MenuA = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 0, MoveGetButtons.Square), new KeyInput_Keyboard(KeyCode.Alpha1) });
            contollerState[0].MenuB = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 0, MoveGetButtons.Triangle), new KeyInput_Keyboard(KeyCode.Alpha2) });
            contollerState[0].CrossButton = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 0, MoveGetButtons.Cross), new KeyInput_Keyboard(KeyCode.Alpha3) });
            contollerState[0].CircleButton = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 0, MoveGetButtons.Circle), new KeyInput_Keyboard(KeyCode.Alpha4) });

            contollerState[0].Zap = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 0, MoveGetButtons.Zap), new KeyInput_Keyboard(KeyCode.Z) });            
            contollerState[0].Trigger = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 0, MoveGetButtons.Trigger), new KeyInput_Keyboard(KeyCode.C) });

            // Left hand
            contollerState[1].MenuA = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 1, MoveGetButtons.Square), new KeyInput_Keyboard(KeyCode.Q) });
            contollerState[1].MenuB = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 1, MoveGetButtons.Triangle), new KeyInput_Keyboard(KeyCode.W) });
            contollerState[1].CrossButton = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 1, MoveGetButtons.Cross), new KeyInput_Keyboard(KeyCode.E) });
            contollerState[1].CircleButton = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 1, MoveGetButtons.Circle), new KeyInput_Keyboard(KeyCode.R) });

            contollerState[1].Zap = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 1, MoveGetButtons.Zap), new KeyInput_Keyboard(KeyCode.X) });
            contollerState[1].Trigger = new KeyState(new List<KeyInput> { new KeyInput_Move(0, 1, MoveGetButtons.Trigger), new KeyInput_Keyboard(KeyCode.V) });
#else
            contollerState[0].MenuA = new KeyState(new List<KeyInput> { new KeyInput_Keyboard(KeyCode.Alpha2) });
            contollerState[0].MenuB = new KeyState(new List<KeyInput> { new KeyInput_Keyboard(KeyCode.Alpha1) });
            contollerState[0].Zap = new KeyState(new List<KeyInput> { new KeyInput_SteamVR(0, 3,Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger), new KeyInput_Keyboard(KeyCode.Z) });
            contollerState[1].Zap = new KeyState(new List<KeyInput> { new KeyInput_SteamVR(0, 4, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger), new KeyInput_Keyboard(KeyCode.X) });
            contollerState[0].Trigger = new KeyState(new List<KeyInput> { new KeyInput_Keyboard(KeyCode.C) });
            contollerState[1].Trigger = new KeyState(new List<KeyInput> { new KeyInput_Keyboard(KeyCode.V) });
#endif

            _moveVibration = new PS4MoveVibration[2];
            for(int i=0;i< _moveVibration.Length;++i)
                _moveVibration[i] = new PS4MoveVibration(0,i);
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Skipping registration of duplicate UniversalInput");
        }
    }

    void Update()
    {
        contollerState[0].Update();
        contollerState[1].Update();
        foreach (var v in _moveVibration)
        {
            v.InputUpdate();
        }
    }

    static public bool DebugMenuNextDown { get { return s_input.contollerState[0].MenuA.Down; } }
    static public bool DebugMenuPrevDown { get { return s_input.contollerState[0].MenuB.Down; } }

    static public bool Grab(int controller)
    {
        if (controller < 2)
        {
            if (Application.isEditor)
                return Input.GetMouseButton(controller == 0 ? 1 : 0);
#if UNITY_PS4
            return (PS4Input.MoveGetButtons(0, controller) & (int)MoveGetButtons.Trigger) != 0;
#else
            return false;
#endif
        }
        else
        {
            throw new System.Exception(string.Format("Controller out of range {0}", controller));
        }
    }

    static public bool Zap(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].Zap.Hold : s_input.contollerState[1].Zap.Hold;
    }

    static public bool Trigger(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].Trigger.Hold : s_input.contollerState[1].Trigger.Hold;
    }

    static public bool TriggerDown(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].Trigger.Down : s_input.contollerState[1].Trigger.Down;
    }

    static public bool TriggerUp(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].Trigger.Up : s_input.contollerState[1].Trigger.Up;
    }

    static public bool ZapDown(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].Zap.Down : s_input.contollerState[1].Zap.Down;
    }

    static public bool MoveFwd(int controller)
    {
        if (!CheckInput(controller))
            return false;
        if (Application.isEditor)
                return Input.GetKey(controller == 0 ? KeyCode.U : KeyCode.I);
#if UNITY_PS4
            return (PS4Input.MoveGetButtons(0, controller) & (int)MoveGetButtons.Square) != 0;
#else
            return false;
#endif
    }

    static public bool MoveBck(int controller)
    {
        if (!CheckInput(controller))
            return false;
        if (Application.isEditor)
                return Input.GetKey(controller == 0 ? KeyCode.J : KeyCode.K);
#if UNITY_PS4
        return (PS4Input.MoveGetButtons(0, controller) & (int)MoveGetButtons.Cross) != 0;
#else
        return false;
#endif
    }

    static public bool RotateLeft(int controller)
    {
        if (!CheckInput(controller))
            return false;
        if (Application.isEditor)
            if(controller == 0)
                return Input.GetKey(KeyCode.C);
#if UNITY_PS4
        return (PS4Input.MoveGetButtons(0, controller) & (int)MoveGetButtons.Circle) != 0;
#else
        return false;
#endif
    }

    static public bool RotateRight(int controller)
    {
        if (!CheckInput(controller))
            return false;
        if (Application.isEditor)
            if (controller == 0)
                return Input.GetKey(KeyCode.V);
#if UNITY_PS4
        return (PS4Input.MoveGetButtons(0, controller) & (int)MoveGetButtons.Triangle) != 0;
#else
        return false;
#endif
    }

    static bool CheckInput(int controller)
    {
        if (!s_input)
            return false;
        if (controller < 0 || controller >= 2)
            throw new System.Exception(string.Format("Controller out of range {0}", controller));
        return true;
    }

    static public MoveControllerState GetMove(int controller)
    {
        if (!CheckInput(controller))
            return null;
        return controller == 0 ? s_input.contollerState[0] : s_input.contollerState[1];
    }

    static public bool MenuADown(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].MenuA.Down : s_input.contollerState[1].MenuA.Down;
    }

    static public bool MenuCDown(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].CrossButton.Down : s_input.contollerState[0].CrossButton.Down;
    }

    static public bool MenuDDown(int controller)
    {
        if (!CheckInput(controller))
            return false;
        return controller == 0 ? s_input.contollerState[0].CircleButton.Down : s_input.contollerState[0].CircleButton.Down;
    }

    // intensity from 0-255
    /*
    static public void HapticPulse(uint controller, int durationMicroSec, int intensity)
    {
        if(controller<_moveVibration.Length)
            _moveVibration[controller].HapticPulse(durationMicroSec, intensity);
    }*/

        // Strength from 0-1
    static public void HapticPulse(uint controller, float strength)
    {
        strength = strength * strength;
        int motorStrength = (int)Mathf.Lerp(strength, 164,255);
        int durationMicroSec = 50;
        //Debug.LogFormat("Motor strength:{0}", motorStrength);
        if(controller<_moveVibration.Length)
            _moveVibration[controller].HapticPulse(durationMicroSec, motorStrength);
    }

    static PS4MoveVibration[] _moveVibration;
}