using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace U3D.SteamVR.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SteamVRPointer), true)]
    class SteamVRPointerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                SteamVRPointer src = target as SteamVRPointer;
                GUILayout.Label("State: " + src.state);
            }
        }
    }
}
