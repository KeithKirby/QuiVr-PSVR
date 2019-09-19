using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DevConsole
{
    public class DevConsoleUI : MonoBehaviour
    {
        public float height;
        public KeyCode hotkey = KeyCode.BackQuote;

        public RectTransform window;
        public Text textArea;    
        public InputField inputField;

        [Range(0, 100)]
        public int opacityPercent = 100;
    
        bool active = false;

        public bool Active { get { return active; } }

        DevConsole console;

        private string partialCommand;

        public static DevConsoleUI instance;

        private void Start()
        {
            height = window.rect.height;
            console = DevConsole.Instance;
        }

        void Awake()
        {
            window.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            /* Set transparency for all Image elements. Using the multiple-component function
             * since there should be two Image components, one for the console window itself
             * and anothr for the ScrollRect's mask */
            float alpha = 255 * opacityPercent / 100.0f;
            foreach (Image img in window.GetComponentsInChildren<Image>())
            {
                Color c = img.color;
                img.color = new Color(c.r, c.g, c.b, alpha / 255.0f);
            }
            /* Load settings, connect the GUI to the implementation */
            Color backgroundColor = textArea.transform.parent.GetComponent<Image>().color;
            DevConsole.settings = gameObject.GetComponent<ConsoleSettings>();
            textArea.color = DevConsole.settings.otherColor;
            inputField.GetComponent<InputField>().textComponent.color = DevConsole.settings.commandColor;
            console = DevConsole.Instance;
            DevConsole.settings.bgColorString = "#" + ColorUtility.ToHtmlStringRGB(backgroundColor);
            DevConsole.settings.otherColorString = "#" + ColorUtility.ToHtmlStringRGB(DevConsole.settings.otherColor);
            console.OnScrollbackChanged += UpdateTextArea;
            UpdateTextArea(console.FullText);
            instance = this;
        }

        float t = 0;
        void Update()
        {
            if (Input.GetKeyDown(hotkey) && !(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)))
            {
                if (active) Hide(); else Show();
                active = !active;
            }

            if (active)//&& inputField.isFocused)
            {
                if(!inputField.isFocused && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                    CheckAddText();
                else if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    if (Input.GetKeyDown(KeyCode.V))
                        inputField.text += GUIUtility.systemCopyBuffer;
                    else if (Input.GetKeyDown(KeyCode.C))
                        GUIUtility.systemCopyBuffer = inputField.text;
                }
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    SubmitText();
                }
                else if(Input.GetKey(KeyCode.Backspace) && inputField.text.Length > 0)
                {
                    t -= Time.unscaledDeltaTime;
                    if(t <= 0)
                    {
                        inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                        t = 0.1f;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    string previous = console.PreviousCommand();
                    if (previous != null)
                    {
                        inputField.text = previous;
                        inputField.MoveTextEnd(false);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    string next = console.NextCommand();
                    if (next != null)
                    {
                        inputField.text = next;
                        inputField.MoveTextEnd(false);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Tab))
                {
                    var options = console.GetAutocompleteOptions(inputField.text.Trim());
                    if (options.Count == 1)
                    {
                        inputField.text = options[0] + " ";
                        inputField.MoveTextEnd(false);
                    }
                    else if (options.Count > 1)
                    {
                        var commonPart = options[0].Substring(0, options.Min(s => s.Length)).TakeWhile((c, idx) => options.All(s => s[idx] == c)).ToArray();
                        inputField.text = new string(commonPart);
                        inputField.MoveTextEnd(false);
                    }
                }
            }
        }

        public static bool isActive()
        {
            if(instance != null)
            {
                return instance.Active;
            }
            return false;
        }

        void CheckAddText()
        {
            if(Input.GetKeyDown(KeyCode.Backspace) && inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                t = 1;
                return;
            }
            string charToAdd = "";
            if (Input.GetKeyDown(KeyCode.Space)) charToAdd = " ";
            else if(Input.GetKeyDown(KeyCode.A)) charToAdd = "a";
            else if (Input.GetKeyDown(KeyCode.B)) charToAdd = "b";
            else if (Input.GetKeyDown(KeyCode.C)) charToAdd = "c";
            else if (Input.GetKeyDown(KeyCode.D)) charToAdd = "d";
            else if (Input.GetKeyDown(KeyCode.E)) charToAdd = "e";
            else if (Input.GetKeyDown(KeyCode.F)) charToAdd = "f";
            else if (Input.GetKeyDown(KeyCode.G)) charToAdd = "g";
            else if (Input.GetKeyDown(KeyCode.H)) charToAdd = "h";
            else if (Input.GetKeyDown(KeyCode.I)) charToAdd = "i";
            else if (Input.GetKeyDown(KeyCode.J)) charToAdd = "j";
            else if (Input.GetKeyDown(KeyCode.K)) charToAdd = "k";
            else if (Input.GetKeyDown(KeyCode.L)) charToAdd = "l";
            else if (Input.GetKeyDown(KeyCode.M)) charToAdd = "m";
            else if (Input.GetKeyDown(KeyCode.N)) charToAdd = "n";
            else if (Input.GetKeyDown(KeyCode.O)) charToAdd = "o";
            else if (Input.GetKeyDown(KeyCode.P)) charToAdd = "p";
            else if (Input.GetKeyDown(KeyCode.Q)) charToAdd = "q";
            else if (Input.GetKeyDown(KeyCode.R)) charToAdd = "r";
            else if (Input.GetKeyDown(KeyCode.S)) charToAdd = "s";
            else if (Input.GetKeyDown(KeyCode.T)) charToAdd = "t";
            else if (Input.GetKeyDown(KeyCode.U)) charToAdd = "u";
            else if (Input.GetKeyDown(KeyCode.V)) charToAdd = "v";
            else if (Input.GetKeyDown(KeyCode.W)) charToAdd = "w";
            else if (Input.GetKeyDown(KeyCode.X)) charToAdd = "x";
            else if (Input.GetKeyDown(KeyCode.Y)) charToAdd = "y";
            else if (Input.GetKeyDown(KeyCode.Z)) charToAdd = "z";
            else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) charToAdd = "0";
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) charToAdd = "1";
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) charToAdd = "2";
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) charToAdd = "3";
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) charToAdd = "4";
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) charToAdd = "5";
            else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) charToAdd = "6";
            else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) charToAdd = "7";
            else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) charToAdd = "8";
            else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) charToAdd = "9";
            else if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) charToAdd = "+";
            else if (Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.KeypadPeriod)) charToAdd = ".";
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                charToAdd = charToAdd.ToUpper();
            if (charToAdd.Length > 0)
                inputField.text += charToAdd;
        }

        void OnDestroy()
        {
            console.OnScrollbackChanged -= UpdateTextArea;
        }

        private void Show()
        {
            window.gameObject.SetActive(true);
            //inputField.ActivateInputField();
            inputField.text = "";   
            //inputField.Select();
            window.anchoredPosition = new Vector2(window.anchoredPosition.x, 0);
        }

        private void Hide()
        {
            inputField.text = "";
            window.gameObject.SetActive(false);
        }

        
        public void SubmitText()
        {
            if (inputField.text != "")
            {
                console.RunCommand(inputField.text);
            }

            inputField.text = "";
            //inputField.Select();
            //inputField.ActivateInputField();
        }
        
        void UpdateTextArea(string fullLog)
        {
            string[] pts = fullLog.Split('\n');
            int minIndex = 0;
            if (pts.Length > 200)
                minIndex = pts.Length - 199;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool ok = false;
            for(int i=minIndex; i<pts.Length; i++)
            {
                bool canWrite = true;
                if(!ok)
                {
                    if (pts[i].Contains("<color="))
                        ok = true;
                    else if(pts[i].Contains("</color>"))
                    {
                        canWrite = false;
                        ok = true;
                    }
                }
                if(canWrite)
                    sb.Append(pts[i] + "\n");
            }
            textArea.text = sb.ToString();
        }

    }
}
