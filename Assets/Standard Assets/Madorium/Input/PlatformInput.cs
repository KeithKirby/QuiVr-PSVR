using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PS4
using UnityEngine.PS4;
#endif

namespace Madorium.Input
{
    public class KeyState
    {
        protected enum PressState
        {
            Up,
            Pressing,
            Down,
            Releasing
        }

        public delegate void OnPressed();
        public event OnPressed Pressed;

        public KeyState()
        {
            Inputs = new List<KeyInput>();
        }

        public KeyState(List<KeyInput> inputs)
        {
            Inputs = inputs;
        }

        public void Update()
        {
            bool pressed = false;
            for (int i = 0; i < Inputs.Count; ++i)
                pressed = pressed || Inputs[i].UpdatePress();
            switch (_state)
            {
                case PressState.Up:
                    if (pressed)
                    {
                        _state = PressState.Pressing;
                        if (null != Pressed)
                            Pressed();
                    }
                    break;
                case PressState.Pressing:
                    if (pressed)
                        _state = PressState.Down;
                    else
                        _state = PressState.Releasing;
                    break;
                case PressState.Down:
                    if (!pressed)
                        _state = PressState.Releasing;
                    break;
                case PressState.Releasing:
                    if (pressed)
                    {
                        _state = PressState.Pressing;
                        if (null != Pressed)
                            Pressed();
                    }
                    else
                        _state = PressState.Up;
                    break;
            }
        }

        public bool Down { get { return PressState.Pressing == _state; } }
        public bool Up { get { return PressState.Releasing == _state; } }
        public bool Hold { get { return PressState.Down == _state || PressState.Pressing == _state; } }
        public List<KeyInput> Inputs = new List<KeyInput>();

        protected PressState _state;
    }

    public class KeyInput
    {
        public virtual bool UpdatePress() { return false; }
    }

    public class KeyInput_Move : KeyInput
    {
        public KeyInput_Move(int slot, int index, MoveGetButtons button)
        {
            _buttonId = (int)button;
            _slot = slot;
            _index = index;
        }

        override public bool UpdatePress()
        {
#if UNITY_PS4 && !UNITY_EDITOR
            return (PS4Input.MoveGetButtons(_slot, _index) & _buttonId) != 0;
#endif
            return false;
        }

        int _slot;
        int _index;
        int _buttonId;
    }

    
    public class KeyInput_SteamVR : KeyInput
    {
        public KeyInput_SteamVR(int slot, int index, Valve.VR.EVRButtonId btn)
        {
            _buttonId = btn;
            _index = index;
        }

        override public bool UpdatePress()
        {
            var vrcontroller = SteamVR_Controller.Input(_index);
            var pressed = vrcontroller.GetPress(_buttonId);
            if (pressed)
                return true;
            else
                return false;
        }

        int _index;
        Valve.VR.EVRButtonId _buttonId;
    }

    public class KeyInput_Keyboard : KeyInput
    {
        public KeyInput_Keyboard(KeyCode key) { _key = key; }
        override public bool UpdatePress()
        {
            if(UnityEngine.Input.GetKey(_key))
                return true;
            else
                return false;
        }
        KeyCode _key;
    }
}