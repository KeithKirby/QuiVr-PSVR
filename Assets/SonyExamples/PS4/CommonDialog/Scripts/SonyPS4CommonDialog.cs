using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SonyPS4CommonDialog : MonoBehaviour, IScreen
{
    MenuStack menuStack;
	float waitTime = 0;
	float progressDelay = 0;
	float progressTime = 0;
	string imeText = "こんにちは";

	MenuLayout menuMain;
	MenuLayout menuUserMessage;
	MenuLayout menuSystemMessage1;
	MenuLayout menuErrorMessage;
	MenuLayout menuProgress;
	MenuLayout menuSignIn;
    MenuLayout menuIME;

    void Start()
	{
		menuMain = new MenuLayout(this, 450, 34);
		menuUserMessage = new MenuLayout(this, 450, 34);
		menuSystemMessage1 = new MenuLayout(this, 450, 34);
		menuErrorMessage = new MenuLayout(this, 1000, 34);
		menuProgress = new MenuLayout(this, 450, 34);
		menuSignIn = new MenuLayout(this, 450, 34);
        menuIME = new MenuLayout(this, 450, 34);

        menuStack = new MenuStack();
		menuStack.SetMenu(menuMain);

		Sony.PS4.Dialog.Main.OnLog += OnLog;
		Sony.PS4.Dialog.Main.OnLogWarning += OnLogWarning;
		Sony.PS4.Dialog.Main.OnLogError += OnLogError;

		Sony.PS4.Dialog.Common.OnGotDialogResult += OnGotDialogResult;
		Sony.PS4.Dialog.Ime.OnGotIMEDialogResult += OnGotIMEDialogResult;
		Sony.PS4.Dialog.Signin.OnGotSigninDialogResult += OnGotSigninDialogResult;

		Sony.PS4.Dialog.Main.Initialise();
		
	}

	public void OnEnter() {}
	public void OnExit() {}

	public void Process(MenuStack stack)
	{
		if(stack.GetMenu() == menuMain)
		{
			MenuMain();
		}
		else if (stack.GetMenu() == menuUserMessage)
		{
			MenuUserMessage();
		}
		else if (stack.GetMenu() == menuSystemMessage1)
		{
			MenuSystemMessage1();
		}
		else if (stack.GetMenu() == menuErrorMessage)
		{
			MenuErrorMessage();
		}
		else if (stack.GetMenu() == menuProgress)
		{
			MenuProgress();
		}
		else if (stack.GetMenu() == menuSignIn)
		{
			MenuSignIn();
		}
        else if(stack.GetMenu() == menuIME)
        {
            MenuIME();
        }
	}

    public void MenuMain()
	{
        menuMain.Update();

        if (menuMain.AddItem("IME Dialog"))
        {
            menuStack.PushMenu(menuIME);
        }

        if (menuMain.AddItem("Signin Dialog"))
		{
			menuStack.PushMenu(menuSignIn);
		} 

        if (menuMain.AddItem("User"))
		{
			menuStack.PushMenu(menuUserMessage);
		}

        if (menuMain.AddItem("System 1"))
		{
			menuStack.PushMenu(menuSystemMessage1);
		}

		if (menuMain.AddItem("Progress"))
		{
			menuStack.PushMenu(menuProgress);
		}

        if (menuMain.AddItem("Error"))
		{
			menuStack.PushMenu(menuErrorMessage);
		}
	}

	void MenuUserMessage()
	{
        menuUserMessage.Update();

        if (menuUserMessage.AddItem("Yes No"))
		{
			Sony.PS4.Dialog.Common.ShowUserMessage(Sony.PS4.Dialog.Common.UserMessageType.YESNO, true, "Do Something ?");
		}

        if (menuUserMessage.AddItem("Ok"))
		{
			Sony.PS4.Dialog.Common.ShowUserMessage(Sony.PS4.Dialog.Common.UserMessageType.OK, true, "Do Something ?");
		}

        if (menuUserMessage.AddItem("Ok Cancel"))
		{
			Sony.PS4.Dialog.Common.ShowUserMessage(Sony.PS4.Dialog.Common.UserMessageType.OK_CANCEL, true, "Do Something ?");
		}

        if (menuUserMessage.AddItem("Cancel"))
		{
			Sony.PS4.Dialog.Common.ShowUserMessage(Sony.PS4.Dialog.Common.UserMessageType.CANCEL, true, "Do Something ?");
		}

        if (menuUserMessage.AddItem("No Button"))
		{
			Sony.PS4.Dialog.Common.ShowUserMessage(Sony.PS4.Dialog.Common.UserMessageType.NONE, true, "Do Something ?");
			waitTime = 5;
		}

        if (menuUserMessage.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}

	void MenuSystemMessage1()
	{
        menuSystemMessage1.Update();

        if (menuSystemMessage1.AddItem("EMPTY STORE"))
		{
			Sony.PS4.Dialog.Common.ShowSystemMessage(Sony.PS4.Dialog.Common.SystemMessageType.TRC_EMPTY_STORE, 0);
		}

        if (menuSystemMessage1.AddItem("PSN UGC RESTRICTION"))
		{
			Sony.PS4.Dialog.Common.ShowSystemMessage(Sony.PS4.Dialog.Common.SystemMessageType.TRC_PSN_CHAT_RESTRICTION, 0);
		}

        if (menuSystemMessage1.AddItem("PSN CHAT RESTRICTION"))
		{
			Sony.PS4.Dialog.Common.ShowSystemMessage(Sony.PS4.Dialog.Common.SystemMessageType.TRC_PSN_UGC_RESTRICTION, 0);
		}

        if (menuSystemMessage1.AddItem("WARNING SWITCH TO SIMULVIEW"))
		{
			Sony.PS4.Dialog.Common.ShowSystemMessage(Sony.PS4.Dialog.Common.SystemMessageType.WARNING_SWITCH_TO_SIMULVIEW,  0);
			waitTime = 5;
		}

        if (menuSystemMessage1.AddItem("Camera not connected"))
		{
			Sony.PS4.Dialog.Common.ShowSystemMessage(Sony.PS4.Dialog.Common.SystemMessageType.CAMERA_NOT_CONNECTED,  0);
			waitTime = 5;
		}

        if (menuSystemMessage1.AddItem("privacy settings"))
		{
			Sony.PS4.Dialog.Common.ShowSystemMessage(Sony.PS4.Dialog.Common.SystemMessageType.WARNING_PROFILE_PICTURE_AND_NAME_NOT_SHARED,  0);
			waitTime = 5;
		}

        if (menuSystemMessage1.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}


    void ShowErrorButton(string name, System.UInt32 errorCode, int userId = 0) 
    {
        if (menuErrorMessage.AddItem(name))
        {
            bool success = Sony.PS4.Dialog.Common.ShowErrorMessage( errorCode, userId );
            if (!success)
            {
                OnScreenLog.Add("FAILED to show ERROR: " + name);
            }
        }
    }
	
	void MenuErrorMessage()
	{
        menuErrorMessage.Update();

        ShowErrorButton("Show Error (0x8001000C) for Starting User(0)", 0x8001000C);
        ShowErrorButton("Fail to show Invalid Error (0) for Starting user(0)", 0);
        ShowErrorButton("Fail to show Error (0x8001000C) for Invalid user(-1)", 0x8001000C, -1);
        //ShowErrorButton("valid error, SCE_USER_SERVICE_USER_ID_SYSTEM", 0x8001000C, 255);//SCE_USER_SERVICE_USER_ID_SYSTEM - User ID specified when using a device that cannot be allocated to a specific user
        //ShowErrorButton("valid error, SCE_USER_SERVICE_USER_ID_EVERYONE", 0x8001000C, 254);//SCE_USER_SERVICE_USER_ID_EVERYONE - User ID specified when not limiting common dialog operation to a specific user

        for (int slot = 0; slot < 4; ++slot) {
#if UNITY_2017_2_OR_NEWER
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.GetUsersDetails(slot);
#else
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.PadGetUsersDetails(slot);
#endif			
            if (user.status != 0)
            {
                ShowErrorButton("Valid Error for " + user.userName + " ("+user.userId+")", 0x8001000C, user.userId);
                //ShowErrorButton("Invalid Error for pad: " + slot + " (" + user.userId + ")", 0, user.userId);
            }
        }
        if (menuErrorMessage.AddItem("Back"))
        {
            menuStack.PopMenu();
        }
    }

	void ShowSignInbutton(string name, int userId)
	{
		if (menuSignIn.AddItem(name))
   		{
   			Sony.PS4.Dialog.Signin.Open(userId);
		}
	}

	void MenuSignIn()
	{
		menuSignIn.Update();

        ShowSignInbutton("Sign in starting user (0)", 0);
        ShowSignInbutton("Invalid UserId(-1)", -1);

        for (int slot = 0; slot < 4; ++slot) {
#if UNITY_2017_2_OR_NEWER
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.GetUsersDetails(slot);
#else
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.PadGetUsersDetails(slot);
#endif			
            if (user.status != 0)
            {
				ShowSignInbutton("Sign in " + user.userName, user.userId);
            }
        }

        if (menuSignIn.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
    }
    void ShowIMEButton(string buttonText,  uint userId, string startingText, uint maxLength )
    {
        if (menuIME.AddItem(buttonText))
        {
            Sony.PS4.Dialog.Ime.SceImeDialogParam ImeParam = new Sony.PS4.Dialog.Ime.SceImeDialogParam();
            Sony.PS4.Dialog.Ime.SceImeParamExtended ImeExtendedParam = new Sony.PS4.Dialog.Ime.SceImeParamExtended();

            // Set supported languages, 'or' flags together or set to 0 to support all languages.
            ImeParam.supportedLanguages = Sony.PS4.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_JAPANESE |
                                                Sony.PS4.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_ENGLISH_GB |
                                                Sony.PS4.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_DANISH;

            ImeParam.option = Sony.PS4.Dialog.Ime.Option.MULTILINE;
            ImeParam.title = buttonText+" 日本語";
            ImeParam.maxTextLength = maxLength;// 8;
            ImeParam.inputTextBuffer = startingText;// "Player 1";
            ImeParam.userId = userId;

            Sony.PS4.Dialog.Ime.Open(ImeParam, ImeExtendedParam);
        }
    }
    private void MenuIME()
    {
        menuIME.Update();
        ShowIMEButton("IME for Starting Player", 0 ,"starting", 10);
        for (int slot = 0; slot < 4; ++slot)
        {
#if UNITY_2017_2_OR_NEWER
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.GetUsersDetails(slot);
#else
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.PadGetUsersDetails(slot);
#endif			
            if (user.status != 0)
            {
                ShowIMEButton("IME for " + user.userName, (uint)user.userId, user.userName, (uint)(10+slot));
            }
        }

        if (menuIME.AddItem("Back"))
        {
            menuStack.PopMenu();
        }
    }


    void MenuProgress()
	{
        menuProgress.Update();

        if (menuProgress.AddItem("Progress Bar"))
		{
			Sony.PS4.Dialog.Common.ShowProgressBar("Working");
			progressDelay = 3;
			progressTime = 5;
		}

        if (menuProgress.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}

	void OnGUI()
	{
		MenuLayout activeMenu = menuStack.GetMenu();
		activeMenu.GetOwner().Process(menuStack);
	}

	void OnLog(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(msg.Text);
	}

    void OnLogWarning(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("WARNING: " + msg.Text);
	}

    void OnLogError(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("ERROR: " + msg.Text);
	}

    void OnGotDialogResult(Sony.PS4.Dialog.Messages.PluginMessage msg)
    {
        Sony.PS4.Dialog.Common.CommonDialogResult result = Sony.PS4.Dialog.Common.GetResult();

        OnScreenLog.Add("Dialog result: " + result);
    }
    
    void OnGotIMEDialogResult(Sony.PS4.Dialog.Messages.PluginMessage msg)
    {
		Sony.PS4.Dialog.Ime.ImeDialogResult result = Sony.PS4.Dialog.Ime.GetResult();

        OnScreenLog.Add("IME result: " + result.result);
        OnScreenLog.Add("IME button: " + result.button);
        OnScreenLog.Add("IME text: " + result.text);
		if (result.result == Sony.PS4.Dialog.Ime.EnumImeDialogResult.RESULT_OK)
		{
			imeText = result.text;
            OnScreenLog.Add("IME result.text: " + imeText);
        }
        else
        {
            OnScreenLog.Add("IME result.text: " + result.text);
        }
    }

	void OnGotSigninDialogResult(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{
		Sony.PS4.Dialog.Signin.SigninDialogResult result = Sony.PS4.Dialog.Signin.GetResult();

		OnScreenLog.Add("Signin result: " + result.result);
	}
	
	void Update ()
    {
        Sony.PS4.Dialog.Main.Update();

		// Update system wait dialog.
		if(waitTime > 0)
		{
			waitTime -= Time.deltaTime;
			if (waitTime <= 0)
			{
				waitTime = 0;
				Sony.PS4.Dialog.Common.Close();
			}
		}

		// Update progress dialog.
		if(progressDelay > 0)
		{
			progressDelay -= Time.deltaTime;
			if (progressDelay <= 0)
			{
				progressDelay = 0;
			}
		}
		else if (progressTime > 0)
		{
			progressTime -= Time.deltaTime;
			if (progressTime <= 0)
			{
				progressTime = 0;
				Sony.PS4.Dialog.Common.Close();
			}

			float percent = (5 - progressTime) / 5;
			int intPercent = (int)(percent * 100);
			Sony.PS4.Dialog.Common.SetProgressBarPercent(intPercent);
			Sony.PS4.Dialog.Common.SetProgressBarMessage("Coming Soon - " + intPercent);
		}
	}

}
