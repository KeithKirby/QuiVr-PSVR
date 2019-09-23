using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPublisher
{
    // Notifies components when screen setup is active so cameras can be disabled
    public delegate void OnSetupScreenActive(bool show);
    static public event OnSetupScreenActive SetupScreenActive;
    static public bool IsSetupScreenActive
    {
        get
        {
            return _isSetupScreenActive;
        }
        set
        {
            if (_isSetupScreenActive != value)
            {
                _isSetupScreenActive = value;
                if (null != SetupScreenActive)
                    SetupScreenActive(value);
            }
        }
    }
    static public bool _isSetupScreenActive = false;


    public delegate void OnOptionsScreenActive(bool show);
    static public event OnOptionsScreenActive OptionsScreenActive;
    static public bool IsOptionsScreenActive
    {
        get
        {
            return _optionsScreenActive;
        }
        set
        {
            if (_optionsScreenActive != value)
            {
                _optionsScreenActive = value;
                if(null!=OptionsScreenActive)
                    OptionsScreenActive(value);
            }
        }
    }
    static public bool _optionsScreenActive = false;
}