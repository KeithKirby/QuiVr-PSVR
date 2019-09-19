using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BitStrap
{
    /// <summary>
    /// Custom editor for all MonoBehaviour scripts in order to draw buttons for all button attributes (<see cref="ButtonAttribute"/>).
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class BehaviourButtonsEditor : Editor
    {
        private ButtonAttributeHelper helper = new ButtonAttributeHelper();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            helper.DrawButtons();
        }

        private void OnEnable()
        {
            helper.Init(target);
        }
    }
}
