using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS4DebugInput : MonoBehaviour
{
    PS4DebugButton[] _buttons;
    int _selected = -1;

    // Use this for initialization
    void Start()
    {
        _buttons = GameObject.FindObjectsOfType<PS4DebugButton>();
        Array.Sort(_buttons, (a,b) => { return a.Sort.CompareTo(b.Sort); });
        if (0 != _buttons.Length)
        {
            for (int i = 0; i < _buttons.Length; ++i)
            {
                _buttons[i].Selected = false;
            }
            SelectButton(0);
        }
	}

    void SelectButton(int i)
    {
        if (_selected != i)
        {
            if (_selected != -1)
                _buttons[_selected].Selected = false;
            _selected = i;
            if (_selected != -1)
                _buttons[_selected].Selected = true;
        }
    }

    PS4DebugButton SelectedButton
    {
        get
        {
            if (_selected >= 0)
                return _buttons[_selected];
            else
                return null;
        }
    }

    void NextButton()
    {
        SelectButton(_selected > 0 ? _selected - 1 : _buttons.Length - 1);        
    }

    void PrevButton()
    {
        SelectButton(_selected + 1 >= _buttons.Length ? 0 : _selected + 1);
    }

    int _dpadY = 0;

    // Update is called once per frame
    void Update () {
#if UNITY_EDITOR
        var dpadYRaw = Input.GetAxis("Joy1Axis8");
#else
        var dpadYRaw = Input.GetAxis("Joy1Axis7");
#endif        
        int dpadY = dpadYRaw > 0.1f ? 1 : dpadYRaw < -0.1f ? -1 : 0;
        if (dpadY != _dpadY)
        {
            _dpadY = dpadY;
            if (_dpadY == 1)
                NextButton();
            if (_dpadY == -1)
                PrevButton();
        }

        var select = Input.GetButtonDown("Circle");
        if (select && SelectedButton)
            SelectedButton.DoPress();
    }
}