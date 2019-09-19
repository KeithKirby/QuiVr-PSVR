using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DevConsole
{
    public class DevConsole
    {
        // Command history that can be accessed with the usual convenient arrow keys
        private IList<string> history = new List<string>();
        private int historyIndex = 0;

        // A variety of settings exposed to the Unity Inspector
        public static ConsoleSettings settings;

        public delegate void onScrollbackChangedCallback(string logText);
        /// Event exposed to the UI for when the console scrollback log changes
        public event onScrollbackChangedCallback OnScrollbackChanged;

        // We support three function signatures as console commands, have a delegate type for each of them
        private delegate void consoleVoidAction();
        private delegate void consoleVoidActionWithParams(params string[] args);
        private delegate string consoleStringAction();
        private delegate string consoleStringActionWithParams(params string[] args);
        // Same signature, but a public type in order not to use the consoleAction... names from elsewhere
        // Needed to dynamically add commands because you cannot have a Func<> with params type
        public delegate string functionWithStringParams(params string[] args);

        // Internal description of one console command
        private class CommandDescriptor
        {
            public string desc = "";
            public string parameters = "";
            public consoleStringActionWithParams action;
        }

        private Dictionary<string, CommandDescriptor> commandMap = new Dictionary<string, CommandDescriptor>();

        // Everything that appears in the console scrollback. Limit to ~16k chars, or Unity cannot display more
        private StringBuilder contentBuffer = new StringBuilder();
        private const int MaxContentBufferSize = 15000;

        // Holds the entire console history, which is possibly more than the Unity UI can display
        private string fullContents = "";
                
        public static DevConsole Instance
        {
            get
            {
                if (_instance == null) _instance = new DevConsole();
                return _instance;
            }
        }
        private static DevConsole _instance = null;

        protected DevConsole()
        {
            Application.logMessageReceived += LogFromUnity;
            commandMap["help"] = new CommandDescriptor()
            {
                desc = "Prints all available commands and their usage info.",
                parameters = "[command]",
                action = PrintHelp
            };
            AddAttributeCommands();
            AddGameObjectCommands();
            contentBuffer.Append(settings.firstOutput);
        }

        /// <summary>
        /// Run a console command
        /// </summary>
        /// <param name="command">The commandline with arguments, if any</param>
        public void RunCommand(string command)
        {
            string sanitizedCmd = sanitize(command);
            contentBuffer.Append(string.Format("<color=#{0}>", ColorUtility.ToHtmlStringRGB(settings.commandColor)));
            contentBuffer.Append(Environment.NewLine + "> ");
            contentBuffer.Append(sanitizedCmd);
            contentBuffer.Append("</color>");
            AddToHistory(sanitizedCmd);
            contentBuffer.Append(Environment.NewLine);
            string commandOutput = RunCommandInternal(command);
            if (contentBuffer.Length + commandOutput.Length > MaxContentBufferSize)
            {
                fullContents += contentBuffer.ToString();
                contentBuffer = new StringBuilder();
            }
            contentBuffer.Append(commandOutput + '\n');
            historyIndex = history.Count - 1;
            OnScrollbackChanged(contentBuffer.ToString());
        }

        public string sanitize(string command)
        {
            string newCmd = "";
            string[] args = command.Split(' ');
            if(args[0] == "login" && args.Length == 3)
            {
                args[2] = "****";
            }
            foreach(var v in args)
            {
                newCmd += v + " ";
            }
            return newCmd.Substring(0, newCmd.Length-1);
        }

        private void AddToHistory(string command)
        {
            // Don't add the same command twice in a row to history, so that they
            // are more convenient to scroll past with the up button
            if (history.Count == 0 || history[history.Count - 1] != command)
            {
                history.Add(command);
                historyIndex = history.Count - 1;
            }
        }

        /// <summary>
        /// Internal implementation of running a console command
        /// </summary>
        /// <param name="commandLine">The commandline with arguments, if any</param>
        private string RunCommandInternal(string commandLine)
        {
            string[] argv = commandLine.Split(' ');
            string commandName = argv[0];
            if (commandMap.ContainsKey(commandName))
            {
                consoleStringActionWithParams action = commandMap[commandName].action;
                try
                {
                    if (argv.Length > 1)
                    {
                        return action(PrepareArgumentArray(argv.Skip(1).ToArray()));
                    }
                    else
                    {
                        return action(new string[] { });
                    }
                }
                catch (Exception e)
                {
                    Print(e.Message + Environment.NewLine + Environment.StackTrace, settings.errorColor);
                }
            }
            else
            {
                Print(commandName + ": command not found", settings.errorColor);
            }
            return "";
        }

        /// <summary>
        /// Internal function to put arguments into an array while respecting double quotes
        /// </summary>
        /// <param name="rawArguments">Raw wrray of all arguments</param>
        /// <returns>Array of all arguments, where arguments in double quotes are treated as one argument</returns>
        private string[] PrepareArgumentArray(string[] rawArguments)
        {
            /* This is a fairy naive implementation and not very performant, which should not be a
             * concern since it's parsing short commands.
             * Could be done with a regex but it looks write-only */
            IList<string> results = new List<string>();
            string rawString = string.Join(" ", rawArguments);
            bool escapeRead = false;
            bool openQuote = false;
            StringBuilder sb = new StringBuilder();
            foreach (char c in rawString.ToCharArray())
            {
                if (escapeRead && c != '"')
                {
                    sb.Append('\\');
                }
                if (c == '\\')
                {
                    escapeRead = !escapeRead;
                }
                else if (c == '"')
                {
                    if (escapeRead)
                    {
                        sb.Append('"');
                    }
                    else if (openQuote)
                    {
                        results.Add(sb.ToString());
                        sb = new StringBuilder();
                        openQuote = false;
                    }
                    else if (!escapeRead)
                    {
                        openQuote = true;
                    }
                }
                else if (c == ' ' && !openQuote)
                {
                    if (sb.Length > 0)
                    {
                        results.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                }
                else
                {
                    sb.Append(c);
                }
                if (c != '\\' && escapeRead)
                {
                    escapeRead = false;
                }
            }
            if (sb.Length > 0)
            {
                results.Add(sb.ToString());
            }
            return results.ToArray();
        }

        public static functionWithStringParams CreateDynamicCommandDelegate(functionWithStringParams f)
        {
            return (p) => f(p);
        }

        /// <summary>
        /// Trigger autocompletion
        /// </summary>
        /// <param name="input">Current user input</param>
        /// <returns>An autocomplete suggestion</returns>
        public IList<string> GetAutocompleteOptions(string input)
        {
            var options = commandMap.Keys.Where(cmd => cmd.StartsWith(input)).ToList();
            if (options.Count > 1)
            {
                string s = "\n";
                foreach (string cmd in options)
                {
                    s += cmd.PadRight(cmd.Length + 3);
                }
                Print(s);
            }
            return options;
        }

        /// <summary>
        /// Get the previous command from history
        /// </summary>
        /// <returns>The previous command, or null if none</returns>
        public string PreviousCommand()
        {
            if (historyIndex >= 0 && (historyIndex - 1) < history.Count && history.Count > 0)
            {
                return history[historyIndex--];
            }
            return null;
        }

        /// <summary>
        /// Get the next command from history
        /// </summary>
        /// <returns>The next command, or null if none</returns>
        public string NextCommand()
        {
            if (historyIndex < history.Count && history.Count > 0)
            {
                historyIndex = Math.Min(historyIndex + 1, history.Count - 1);
                return history[historyIndex];
            }
            return null;
        }


        /* The help function is here and not in Builtins because help is quite unique, needing
         * access to the internals of our registered commands */
        /// <summary>
        /// Prints info about all commands, or a specific one
        /// </summary>
        /// <param name="options">If given, print info about a specific command</param>
        /// <returns></returns>
        string PrintHelp(string[] options)
        {
            StringBuilder output = new StringBuilder();
            // No arguments: Print all commands
            if (options.Length == 0)
            {
                output.AppendLine("Type 'help' to see this list. Type 'help name' to find out more about the command 'name'");
                output.AppendLine("Defined commands:");
                var sortedCommands = commandMap.Keys.OrderBy(name => name).ToList();
                int columnWidth = 40;
                foreach (string cmd in sortedCommands)
                {
                    string s = (" " + cmd + " " + commandMap[cmd].parameters).PadRight(columnWidth);
                    string desc = commandMap[cmd].desc;
                    if(desc != null && desc.Length > 0)
                    {
                        output.Append(s);
                        if (desc.Length > 0)
                        {
                            // A guesstimate of 110 characters as a reasonable display length - change if it does not
                            // look good
                            output.Append(desc.PadLeft(desc.Length + 3).Take(110).ToArray());
                        }
                        output.AppendLine();
                    }
                }

            }
            // Print help for a specific command
            else if (commandMap.ContainsKey(options[0]))
            {
                CommandDescriptor command = commandMap[options[0]];
                output.Append(" " + options[0] + " " + command.parameters);
                if (command.desc.Length > 0)
                {
                    output.Append(" : " + command.desc);
                }
                output.AppendLine();
            }
            else
            {
                Print(options[0] + ": command not found", settings.errorColor);
            }
            return output.ToString();
        }
    
        private void LogFromUnity(string message, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    if (settings.logUnityErrors)
                    {
                        Print(message, settings.errorColor);
                        if (stackTrace.Length > 0)
                        {
                            Print(stackTrace, settings.errorColor);
                        }
                    }
                    break;
                case LogType.Warning:
                    if (settings.logUnityWarnings)
                    {
                        Print(message, settings.warningColor);
                        if (stackTrace.Length > 0)
                        {
                            Print(stackTrace, settings.warningColor);
                        }
                    }
                    break;
                case LogType.Log:
                    if (settings.logUnityInfos)
                    {
                        Print(message, settings.infoColor);
                        if (stackTrace.Length > 0)
                        {
                            Print(stackTrace, settings.infoColor);
                        }
                    }
                    break;
            }
        }

        private void Print(string message)
        {
            contentBuffer.Append(message);
            contentBuffer.Append("\n");
            OnScrollbackChanged(contentBuffer.ToString());
        }

        private void Print(string message, Color color)
        {
            contentBuffer.Append(string.Format("<color=#{0}>", ColorUtility.ToHtmlStringRGB(color)));
            contentBuffer.Append(message);
            contentBuffer.AppendLine("</color>");
            if(OnScrollbackChanged != null && contentBuffer != null)
                OnScrollbackChanged(contentBuffer.ToString());
        }

        private void AddAttributeCommands()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var methodAttributeDictionary = (
                      from assembly in assemblies
                      from type in assembly.GetTypes()
                      from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                      where method.IsDefined(typeof(ConsoleCommand), false)
                      select new { m = method, attr = method.GetCustomAttributes(typeof(ConsoleCommand), false).FirstOrDefault() as ConsoleCommand }
                      ).ToDictionary(pair => pair.m, pair => pair.attr);


            foreach (KeyValuePair<MethodInfo, ConsoleCommand> pair in methodAttributeDictionary)
            {
                var method = pair.Key;
                var attr = pair.Value;

                consoleStringActionWithParams actionDelegate = null;
                // First, functions that return void
                if (method.ReturnType == typeof(void))
                {
                    // Signature is void f();
                    if (method.GetParameters().Length == 0)
                    {
                        consoleVoidAction action = Delegate.CreateDelegate(typeof(consoleVoidAction), method, false) as consoleVoidAction;
                        actionDelegate = _ => { action(); return ""; };
                    }
                    // Next, check for signature void f(param string args[]);
                    else if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(string[]) &&
                        Attribute.IsDefined(method.GetParameters()[0], typeof(ParamArrayAttribute)))
                    {
                        consoleVoidActionWithParams action = Delegate.CreateDelegate(typeof(consoleVoidActionWithParams), method, false) as consoleVoidActionWithParams;
                        actionDelegate = _ => { action(); return ""; };
                    }
                }
                else if (method.ReturnType == typeof(string)) // Next, string-returning functions
                {
                    // Signature is string f(params string args[]);
                    if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(string[]) &&
                        Attribute.IsDefined(method.GetParameters()[0], typeof(ParamArrayAttribute)))
                    {
                        actionDelegate = Delegate.CreateDelegate(typeof(consoleStringActionWithParams), method, false) as consoleStringActionWithParams;
                    }
                    //Signature is string f();
                    else if (method.GetParameters().Length == 0)
                    {
                        consoleStringAction action = Delegate.CreateDelegate(typeof(consoleStringAction), method, false) as consoleStringAction;
                        actionDelegate = _ => { return action(); };
                    }
                }
                if (actionDelegate == null)
                {
                    Debug.LogWarning(string.Format("Failed to create action for {0}! The method signature is probably wrong", method.Name));
                    continue;
                }

                if (string.IsNullOrEmpty(attr.name))
                {
                    attr.name = method.Name;
                }

                commandMap[attr.name] = new CommandDescriptor()
                {
                    desc = string.IsNullOrEmpty(attr.description) ? "" : attr.description,
                    parameters = string.IsNullOrEmpty(attr.paramsDescription) ? "" : attr.paramsDescription,
                    action = actionDelegate
                };
              
            }   
        }

        private void AddGameObjectCommands()
        {
            foreach (GameObjectConsoleCommand cmd in settings.gameObjectCommands)
            {
                consoleStringActionWithParams del = (p) => { cmd.unityEvent.Invoke(); return "Dispatched to GameObject"; };
                commandMap[cmd.name] = new CommandDescriptor()
                {
                    desc = cmd.description,
                    action = del
                };
            }
        }

        /// <summary>
        /// Add a command to console dynamically. Prefer using the ConsoleCommand attribute instead
        /// </summary>
        /// <param name="name">Name of the console command</param>
        /// <param name="description">Description of the command</param>
        /// <param name="function">A string-returning function that his command should run</param>
        public void AddCommand(string name, string description, Func<string> function)
        {
            AddCommand(name, description, "", function);
        }

        /// <summary>
        /// Add a command to console dynamically. Prefer using the ConsoleCommand attribute instead
        /// </summary>
        /// <param name="name">Name of the console command</param>
        /// <param name="description">Description of the command</param>
        /// <param name="parameterDescription">Description of the parameters</param>
        /// <param name="function">A string-returning function that his command should run</param>
        public void AddCommand(string name, string description, string parameterDescription, Func<string> function)
        {
            consoleStringActionWithParams del = (p) => function();
            commandMap[name] = new CommandDescriptor()
            {
                desc = description,
                parameters = parameterDescription,
                action = del
            };
        }

        /// <summary>
        /// Add a command to console dynamically. Prefer using the ConsoleCommand attribute instead
        /// </summary>
        /// <param name="name">Name of the console command</param>
        /// <param name="description">Description of the command</param>
        /// <param name="function">A string-returning function that his command should run</param>
        public void AddCommand(string name, string description, Func<string, string> function)
        {
            AddCommand(name, description, "", function);
        }

        /// <summary>
        /// Add a command to console dynamically. Prefer using the ConsoleCommand attribute instead
        /// </summary>
        /// <param name="name">Name of the console command</param>
        /// <param name="description">Description of the command</param>
        /// <param name="parameterDescription">Description of the parameters</param>
        /// <param name="function">A string-returning function that his command should run</param>
        public void AddCommand(string name, string description, string parameterDescription, Func<string, string> function)
        {
            consoleStringActionWithParams del = (p) => function(p.Length > 0 ? p[0] : null);
            commandMap[name] = new CommandDescriptor()
            {
                desc = description,
                parameters = parameterDescription,
                action = del
            };
        }

        /// <summary>
        /// Add a command to console dynamically. Prefer using the ConsoleCommand attribute instead
        /// </summary>
        /// <param name="name">Name of the console command</param>
        /// <param name="description">Description of the command</param>
        /// <param name="function">A string-returning function that his command should run</param>
        public void AddCommand(string name, string description, functionWithStringParams function)
        {
            AddCommand(name, description, "", function);
        }

        /// <summary>
        /// Add a command to console dynamically. Prefer using the ConsoleCommand attribute instead
        /// </summary>
        /// <param name="name">Name of the console command</param>
        /// <param name="description">Description of the command</param>
        /// <param name="parameterDescription">Description of the parameters</param>
        /// <param name="function">A string-returning function that his command should run</param>
        public void AddCommand(string name, string description, string parameterDescription, functionWithStringParams function)
        {
            consoleStringActionWithParams del = (p) => function(p);
            commandMap[name] = new CommandDescriptor()
            {
                desc = description,
                parameters = parameterDescription,
                action = del
            };
        }

        public string FullText { get { return fullContents + contentBuffer.ToString(); } }

        /// <summary>
        /// Get the full text of the console log in raw format, with color tags removed.
        /// </summary>
        /// <returns>Full raw text</returns>
        public string GetRawText()
        {
            Regex colorStart = new Regex("<color=#......>");
            string raw = colorStart.Replace(FullText, "");
            raw = raw.Replace("</color>", "");
            return raw;
        }

        /// <summary>
        /// Get the HTML for the console log. Colors will get saved as CSS-colored text
        /// </summary>
        /// <returns>HTML page source</returns>
        public string GetHtml(string backgroundColor)
        {
            StringBuilder sb = new StringBuilder();
            Regex colorStart = new Regex("<color=(#......)>");
            sb.AppendLine("<html>");
            sb.AppendLine("<title>Console Output</title>");
            Color c = Color.clear;
            if (ColorUtility.TryParseHtmlString(backgroundColor, out c) && c != Color.clear)
            {
                sb.AppendFormat("<body style=\"font-family: Courier New, serif;\" bgcolor=\"{0}\" text=\"{1}\">", backgroundColor, settings.otherColorString);
            }
            else
            {
                sb.AppendFormat("<body style=\"font-family: Courier New, serif;\" bgcolor=\"{0}\" text=\"{1}\">", settings.bgColorString, settings.otherColorString);
            }
            sb.Append(Environment.NewLine);
            string[] lines = FullText.Split('\n');
            string nextColor = "";
            foreach (string line in lines)
            {
                if (colorStart.IsMatch(line))
                {
                    sb.Append("<p>");
                    // Iterate over all matches because there could be in-line color tags
                    string loopLine = line;
                    for (int i = 0; i < colorStart.Matches(line).Count; i++)
                    {
                        string colorString = colorStart.Matches(line)[i].Groups[1].Value;
                        loopLine = (loopLine.Replace(colorStart.Matches(line)[i].Groups[0].Value, "<span style=\"color: " + colorString + "\">"));
                    }
                    //sb.Append("<span style=\"color:" + colorStart.Matches(line)[0].Groups[1] + "\">");
                    //loopLine = loopLine.Replace(colorStart.Matches(line)[0].Groups[0].Value, "");
                    // Outermost color could carry on to another line
                    int colorEndTags = Regex.Matches(loopLine, "</color>").Count;
                    if (colorEndTags < colorStart.Matches(line).Count)
                    {
                        nextColor = colorStart.Matches(line)[0].Groups[1].Value;
                    }
                    else
                    {
                        nextColor = "";
                    }
                    loopLine = loopLine.Replace("</color>", "</span>");
                    sb.Append(loopLine);
                    sb.AppendLine("</p>");
                }
                else
                {
                    sb.Append("<p>");
                    if (nextColor != "" && line.Length > 0)
                    {
                        sb.Append("<span style=\"color:" + nextColor + "\">");
                        nextColor = "";
                    }
                    sb.Append(line.Replace("</color>", "</span>"));
                    sb.AppendLine("</p>");
                }
            }
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public void Clear()
        {
            fullContents += contentBuffer.ToString();
            contentBuffer = new StringBuilder();
        }
    }
}
