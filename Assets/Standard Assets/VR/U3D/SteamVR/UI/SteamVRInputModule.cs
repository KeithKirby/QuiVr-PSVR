#if DEBUG
#define LOG_CONSOLE_SteamVRInputModule
#endif
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace U3D.SteamVR.UI
{
    public class SteamVRInputModule : BaseInputModule
    {
        #region log stuff
        [System.Diagnostics.Conditional("LOG_CONSOLE_SteamVRInputModule")]
        void Log(string format, params object[] args)
        {
            Debug.LogFormat(this.GetType().Name + "\t" + format, args);
        }
        #endregion

        SteamVR_ControllerManager _controllerManager;
        SteamVR_ControllerManager m_controllerManager
        {
            get { return _controllerManager ?? (_controllerManager = FindObjectOfType<SteamVR_ControllerManager>()); }
        }
        SteamVRPointer[] __trackedControllers = null;
        SteamVRPointer[] m_trackedControllers
        {
            get
            {
                if(__trackedControllers == null)
                {
                    __trackedControllers = GetComponents<SteamVRPointer>();
                }
                return __trackedControllers;
            }
        }

        public override void Process()
        {
            foreach (SteamVRPointer i in m_trackedControllers)
            {
                i.Process();
            }
        }
    }
}