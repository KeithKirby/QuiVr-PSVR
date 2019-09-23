using UnityEngine;
using System.Collections;
//using Parse;
using System.Collections.Generic;
using UnityEngine.UI;
using U3D.SteamVR.UI;
using VRTK;
using UnityEngine.Events;
public class FeedbackStart : MonoBehaviour {

    bool inMiddle;
    bool success;
    public string FeedbackClass;
    public string FeedbackID;
    public FeedbackQuestion[] Questions;
    bool answeredCurrent;
    int answerValue;

    public GameObject cancelButton;
	public GameObject closeButton;

    public Text qTitle;
    public Text qQuestion;
    public Transform ButtonHolder;
    public GameObject[] ResponseButtons;

    public Vector3 spos;
    public Vector3[] dpos;
    public Vector3[] tpos;
    public Vector3[] qpos;

    public GameObject ButtonPrefab;

    public UnityEvent OnStartFeedback;
    public UnityEvent OnEndFeedback;

    bool didSubmit;

    List<string> answerIDs;

    public void StartFeedback()
    {
		closeButton.SetActive(false);
        cancelButton.SetActive(true);
        didSubmit = false;
        OnStartFeedback.Invoke();
        var ptrs = FindObjectsOfType<SteamVRPointer>();
        foreach(var v in ptrs)
        {
			v.SetEnabled (true);
        }
        inMiddle = true;
		Quiver q = FindObjectOfType<Quiver> ();
		if (q != null) {
			q.GetComponent<VRTK_InteractableObject> ().isUsable = false;
		}
        StopCoroutine("AskQuestions");
        StartCoroutine("AskQuestions");
    }

    public void CancelFeedback()
    {
        StopCoroutine("AskQuestions");
        inMiddle = false;
        didSubmit = false;
        ClearButtons();
        OnEndFeedback.Invoke();
		Quiver q = FindObjectOfType<Quiver> ();
		if (q != null) {
			q.GetComponent<VRTK_InteractableObject> ().isUsable = true;
		}
		foreach(var v in FindObjectsOfType<SteamVRPointer>())
		{
			v.SetEnabled(false);
		}
    }

    IEnumerator AskQuestions()
    {
        answerIDs = new List<string>();
        foreach (var v in Questions)
        {
            answerValue = -1;
            qTitle.text = v.Title;
            qQuestion.text = v.Question;
            ClearButtons();
            ResponseButtons = new GameObject[v.Options.Length];
            for(var i=0; i<v.Options.Length; i++)
            {
                ResponseButtons[i] = NewButton(i, v.Options[i], v.Options.Length);
            }
            answeredCurrent = false;
            while(!answeredCurrent)
            {
                yield return true;
            }
            if(answerValue > -1 && answerValue < v.OptionsIDs.Length)
                answerIDs.Add(v.OptionsIDs[answerValue]);
        }
        qTitle.text = "Please Wait";
        qQuestion.text = "Submitting Responses...";
        ClearButtons();
        SubmitResponses();
        while(!didSubmit)
        {
            yield return true;
        }
        cancelButton.SetActive(false);
        if (success)
        {
            qTitle.text = "Feedback Submitted";
            qQuestion.text = "Thanks for participating in this test. Your feedback is greatly appreciated!";
        }
        else
        {
            qTitle.text = "Error Occurred";
            qQuestion.text = "We were unable to submit your response at this time. Please try again later!";
        }
        answeredCurrent = false;
		closeButton.SetActive (true);
        while(!answeredCurrent)
        {
            yield return true;
        }
        ClearButtons();
        GetComponentInChildren<EMOpenCloseMotion>().Close(true);
        var ptrs = FindObjectsOfType<SteamVRPointer>();
        foreach (var v in ptrs)
        {
			v.SetEnabled (true);
        }
		Quiver q = FindObjectOfType<Quiver> ();
		if (q != null) {
			q.GetComponent<VRTK_InteractableObject> ().isUsable = true;
		}
        OnEndFeedback.Invoke();
        StopCoroutine("AskQuestions");
    }

    void ClearButtons()
    {
        for(int i=0; i<ResponseButtons.Length; i++)
        {
            Destroy(ResponseButtons[i]);
        }
    }

    public GameObject NewButton(int idx, string value, int total)
    {
        GameObject newButton = (GameObject)Instantiate(ButtonPrefab);
        newButton.transform.SetParent(ButtonHolder);
        newButton.transform.localScale = Vector3.one;
        newButton.transform.localEulerAngles = Vector3.zero;
        if(total == 2)
            newButton.transform.localPosition = dpos[idx];
        else if(total == 3)
            newButton.transform.localPosition = tpos[idx];
        else if (total == 4)
            newButton.transform.localPosition = qpos[idx];
        newButton.transform.localPosition += Vector3.down * 75;
        newButton.GetComponentInChildren<Text>().text = value;
        newButton.GetComponent<Button>().onClick.AddListener(() => AnswerQuestion(idx));
        return newButton;
    }

    public void AnswerQuestion(int idx)
    {
        answerValue = idx;
        answeredCurrent = true;
    }

    public void SubmitResponses()
    {
        // dw - Disable feedback system
        Debug.Log("Submitting Response");
        /*
        ParseQuery<ParseObject> query = ParseObject.GetQuery(FeedbackClass);
        query.GetAsync(FeedbackID).ContinueWith(t =>
        {
            if (!t.IsFaulted && t.Exception == null)
            {
                ParseObject feedback = t.Result;
                foreach (var v in answerIDs)
                {
                    feedback.Increment(v);
                }
                feedback.AddUniqueToList("SubmittedUsers", new[] { PhotonNetwork.player.name });
                if(Parse.ParseUser.CurrentUser != null)
                {
                    Parse.ParseUser.CurrentUser[FeedbackClass] = true;
                    Parse.ParseUser.CurrentUser.SaveAsync();
                }
                feedback.SaveAsync();
                success = true;
                didSubmit = true;
            }
            else
            {
                success = false;
                didSubmit = true;
            }
        });
        */
    }

	public bool inProgress()
    {
        return inMiddle;
    }
}

[System.Serializable]
public class FeedbackQuestion
{
    public string Title;
    public string Question;
    public string[] Options;
    public string[] OptionsIDs;
}
