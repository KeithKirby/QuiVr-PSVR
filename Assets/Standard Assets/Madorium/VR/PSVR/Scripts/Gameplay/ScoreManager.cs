using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class PSVRScoreManager : MonoBehaviour
{
	public Text timerText;
	public Image timerDisplay;
	public Text scoreText;
	public Text accuracyText;
	public Text highScoreText;
	public float totalTime = 60f;
	public int scoreValue = 100;
	private int theScore = 0;
	static int highScore = 0;
	private float accuracy = 100;
	private int rollingScore = 0;
	private float currentTime = 0f;
	private bool timerIsRunning = false;
	private TargetsManager targetsManager;
	//private WeaponController[] weaponControls;
	
	// Used for initialization
	void Start ()
	{
		timerText.text = currentTime.ToString("F0");
		scoreText.text = rollingScore.ToString();
		accuracyText.text = accuracy.ToString("F0") + "%";
		highScoreText.text = highScore.ToString();

		targetsManager = FindObjectOfType<TargetsManager>();
		//weaponControls = FindObjectsOfType<WeaponController>();
	}
	
	void Update ()
	{
		if(timerIsRunning)
		{
			if(currentTime <= 0)
				StartCoroutine(EndTimer());
			else
				Timer();
		}

		UpdateScoreDisplay();
	}

	// Countdown the timer, text, and a graphical representation of the timer
	void Timer()
	{
		timerDisplay.fillAmount = currentTime / totalTime;
		timerText.text = currentTime.ToString("F0");
		currentTime -= Time.deltaTime;
	}

	// Once the timer is finished, turn off the weapon(s), stop spawning new targets, and restart the game
	IEnumerator EndTimer()
	{
        timerIsRunning = false;
        currentTime = 0;

        if (theScore > highScore)
			highScore = theScore;

		CalculateAccuracy();
		
		//for(int i=0; i<weaponControls.Length; i++)
			//weaponControls[i].WeaponCanShoot(false);
		
		targetsManager.StopSpawning();

        // Exit to main menu after a delay
        yield return new WaitForSeconds(3f);
		FindObjectOfType<SceneSwitcher>().SwitchToScene("PSVRExample_MainMenu");
	}

    // Keep the score and text display updated
    void UpdateScoreDisplay()
	{
		if(rollingScore < theScore)
		{
			rollingScore = (int)Mathf.Lerp(rollingScore, theScore, Time.deltaTime * 10f);
			rollingScore += 1;
		}
		else if(rollingScore > theScore)
		{
			rollingScore = theScore;
		}

		CalculateAccuracy();

		//scoreText.text = rollingScore.ToString();
		//accuracyText.text = accuracy.ToString("F0") + "%";
		//highScoreText.text = highScore.ToString();
	}

	public void StartTimer ()
	{
		theScore = 0;
		timerIsRunning = true;
		currentTime = totalTime;
	}

	void CalculateAccuracy()
	{
        //if(weaponControls == null)
// return;

        int totalShotsFired = 0;

		//for(int i=0; i<weaponControls.Length; i++)
//			totalShotsFired += weaponControls[i].shotsFired;

		if(theScore != 0) {
			accuracy = ((float)theScore / ((float)scoreValue * (float)totalShotsFired)) * 100f;
		}
	}

	public void IncreaseScore ()
	{
		theScore += scoreValue;
	}

    public void Quit()
    {
        timerIsRunning = false;
        FindObjectOfType<SceneSwitcher>().SwitchToScene("PSVRExample_MainMenu");
    }
}
