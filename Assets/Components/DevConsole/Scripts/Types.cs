using System;
using UnityEngine.Events;

namespace DevConsole
{
    /// <summary>
    /// This attribute can be used on public static methods to register them as commands with the console
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommand : Attribute
    {
        public string name;
        public string description;
        public string paramsDescription;

        /// <summary>
        /// Creates a description of a console command
        /// </summary>
        /// <param name="name">Command name for the console. If not specified, the method's name will be used</param>
        /// <param name="description">Optional description text, will be shown in the console help</param>
        /// <param name="paramsDescription">Optional string describing the parameters, if any</param>
        public ConsoleCommand(string name = null, string description = null, string paramsDescription = null)
        {
            this.name = name;
            this.description = description;
            this.paramsDescription = paramsDescription;
        }
    }

    /// <summary>
    /// Represents a console command that executes a UnityEvent on some GameObject
    /// </summary>
    [Serializable]
    public struct GameObjectConsoleCommand
    {
        public string name;
        public string description;
        public UnityEvent unityEvent;
    }
}
