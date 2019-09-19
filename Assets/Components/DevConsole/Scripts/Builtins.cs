using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace DevConsole
{
    public static class Builtins
    {

        /*
        [ConsoleCommand(description = "Prints its parameters back to the console", paramsDescription = "[str, ...]")]
        public static string echo(params string[] args)
        {
            return string.Join(" ", args);
        }

        [ConsoleCommand(description = "Runs all commands from a given file", paramsDescription = "filename")]
        public static string run(params string[] args)
        {
            if (args.Length != 1)
            {
                return "Filename required!";
            }
            try
            {
                string[] lines = System.IO.File.ReadAllLines(@args[0]);
                foreach(string line in lines)
                {
                    DevConsole.Instance.RunCommand(line);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "";
        }

            [ConsoleCommand(description = "Save the console to a HTML file. Optional parameter sets HTML background color",
            paramsDescription = "filename [bgcolor]")]
        public static string savehtml(params string[] args)
        {
            if (args.Length < 1)
            {
                return "Filename required!";
            }
            string bgColor = "";
            if (args.Length == 2)
            {
                Regex colorRegex = new Regex("^#?[0-f][0-f][0-f][0-f][0-f][0-f]$", RegexOptions.IgnoreCase);
                if (colorRegex.Match(args[1]).Success)
                {
                    bgColor = args[1];
                    if (!bgColor.StartsWith("#"))
                    {
                        bgColor = "#" + bgColor;
                    }
                }
                else
                {
                    return args[1] + " is not a valid color string!";
                }
            }
            string html = DevConsole.Instance.GetHtml(bgColor);
            try
            {
                System.IO.File.WriteAllText(@args[0], html);
                return "Saved to " + args[0];
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        */

        [ConsoleCommand(description = "Print Unity, .NET and operating system version info")]
        public static string envinfo()
        {
            string s = "Unity version: " + Application.unityVersion + Environment.NewLine;
            s += ".NET environment: " + Environment.Version;
            s += " (" + Environment.OSVersion + ")" + Environment.NewLine;
            return s;
        }

        [ConsoleCommand(description = "Save the console to a plain text file", paramsDescription = "filename")]
        public static string savetxt(params string[] args)
        {
            if (args.Length != 1)
            {
                return "Filename required!";
            }
            string raw = DevConsole.Instance.GetRawText();
            try
            {
                System.IO.File.WriteAllText(@args[0], raw);
                return "Saved to " + args[0];
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }     
        
        [ConsoleCommand(description = "Clears the console")]
        public static string clear()
        {
            DevConsole.Instance.Clear();
            return "";
        }

        [ConsoleCommand(description = "Pauses the game")]
        public static string pause()
        {
            if (ToggleMenu.instance != null && !ToggleMenu.instance.isOpen())
            {
                if (!PhotonNetwork.inRoom)
                {
                    ToggleMenu.instance.Toggle();
                    return "Game Paused";
                }
                else
                    return "Can't pause in multiplayer";
            }
            else if (ToggleMenu.instance.isOpen())
                return "Game already paused";
            else
                return "Can't pause right now";
        }

        [ConsoleCommand(description = "Resumes a paused game")]
        public static string resume()
        {
            if (ToggleMenu.instance != null && ToggleMenu.instance.isOpen())
            {
                if (!PhotonNetwork.inRoom)
                {
                    ToggleMenu.instance.Toggle();
                    return "Game Resumed";
                }
                else
                    return "Game was not paused, ignoring command";
            }
            return "Game was not paused, ignoring command";
        }

        [ConsoleCommand(description = "Attempts login to user profile", paramsDescription = "UserID PIN")]
        public static string login(params string[] args)
        {
            if (args.Length < 2)
                return "UserID and PIN required";
            if (User.ArcadeUser != null)
                return "Already logged into profile, please log out first";
            else
            {
                User.Login(args[0], args[1], null);
                return "Attempting profile login: " + args[0];
            }
        }

        [ConsoleCommand(description = "Logs out of User Profile")]
        public static string logout()
        {
            if (User.ArcadeUser != null)
            {
                string un = User.ArcadeUser.ArcadeName;
                User.LeaveArcade();
                return "Logging out of profile: " + un;
            }
            return "No profile logged in";
        }

        [ConsoleCommand(description = "Changes a given player setting to the specified value", paramsDescription = "Key Value")]
        public static string changeSetting(params string[] args)
        {
            if (args.Length < 2)
                return "Setting Key and Value required";
            else
            {
                string key = args[0];
                string val = args[1];
                Settings.Set(key, val);
                return "Setting [" + key + "] set to: " + val;
            }
        }
    }
}
