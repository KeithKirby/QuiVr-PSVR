using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace DevConsole
{
    public class ConsoleSettings : MonoBehaviour
    {
        [Header("Unity logging")]
        [Space(8)]
        public bool logUnityErrors = true;
        public bool logUnityWarnings = true;
        public bool logUnityInfos = true;

        [Header("Output colors")]
        [Space(8)]
        public Color errorColor = Color.red;
        public Color warningColor = Color.yellow;
        public Color infoColor = Color.gray;
        public Color otherColor = Color.black;
        public Color commandColor = Color.black;
        [HideInInspector]
        public string bgColorString;
        [HideInInspector]
        public string otherColorString;

        [Header("Output settings")]
        [TextArea]
        public string firstOutput = @"QCons DevConsole by Capricorn Software. Support: unity@capricornsoftware.se
Type 'help' to view available commands. Press Tab to invoke auto-completion.";

        [Space(8)]
        public List<GameObjectConsoleCommand> gameObjectCommands;
    }
}
