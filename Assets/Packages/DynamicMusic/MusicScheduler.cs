using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScheduler : MonoBehaviour {

    public static MusicScheduler instance;
	public bool gateLost = false;
	public int enemyCount = 0;
	public int intensityLevel = 1 ;
	public int musicIntensityLevel = 1;

	public AudioSource[] channels;
	public AudioClip[] clips;
	public AudioClip gateLostClip;
	public int defaultClipIndex = 0;
	public int lastClip;
	public int nextClip;
	public float bpm = 70;
	public int beatsPerBar = 4;

	int[][] clipRoutes = new int[][]{
		new int[] { 1, 2 },
		new int[] { 0, 2, 3 },
		new int[] { 0, 1, 3, 5 },
		new int[] { 2, 4, 5, },
		new int[] { 2, 5, 6 },
	 	new int[] { 1, 2, 6 },
		new int[] { 3, 5, 6, 7 },
		new int[] { 8 },
		new int[] { 5, 6, 9 },
		new int[] { 0, 1, 2, 3, 5, 6 }};

	double musicStartTime;
	double lastStartTime;
	double nextStartTime;

	float currentBar;
	double barDuration;

	bool running = false;
	bool clipQueued;
	float timer;

	int flip = 0;

    public float volume = 1f;

    void Awake()
    {
        instance = this;
    }

    public void LostGate()
    {
        gateLost = true;
    }

	// Use this for initialization
	void Start () {
		barDuration = 60d / bpm * beatsPerBar;
		StartMusic ();
	}
	
	// Update is called once per frame
	void Update () 
	{
        float musicVol = 0.8f;
        if (Settings.HasKey("MusicVolume"))
            musicVol = Settings.GetFloat("MusicVolume");
        foreach (var v in channels)
        {
            v.volume = volume * musicVol;
        }
		if (running) 
		{
			UpdateState ();
		}
	}

	void StartMusic ()
	{
		AudioClip firstClip = clips [defaultClipIndex];
		channels [flip].clip = firstClip;
		musicStartTime = AudioSettings.dspTime;
		channels [flip].PlayScheduled (musicStartTime + barDuration);
		lastStartTime = musicStartTime + barDuration;
		double firstClipLength = (double)firstClip.samples / firstClip.frequency;
		nextStartTime = lastStartTime + firstClipLength;
		flip = 1 - flip;
		running = true;
	}

	void CueNextClip (int clipIndex)
	{
		AudioClip next = clips [clipIndex];
		channels [flip].clip = next;
		channels [flip].PlayScheduled (nextStartTime);
		lastStartTime = nextStartTime;	
		nextStartTime += (double)next.samples / next.frequency;
		clipQueued = true;
		lastClip = clipIndex;
		//Debug.Log ("The next clip will be " + clips [clipIndex] + " and will play on channel " + flip);
		flip = 1 - flip;
	}

	void UpdateState ()
	{
		timer += Time.deltaTime;
		if (timer > barDuration/4) 
		{
			
			currentBar += 0.25f;

			UpdateIntensity ();

			if (AudioSettings.dspTime > lastStartTime) 
			{
				clipQueued = false;
			}

			if (!clipQueued && AudioSettings.dspTime > nextStartTime - barDuration/2) 
			{
				StateLogic ();
			}

			if (gateLost) 
			{
				GateLost ();
			}

			timer = 0;
		}
	}

	void GateLost ()
	{
		gateLost = false;
		Debug.Log("Gate lost, playing clip at next bar");
		double remainder = (AudioSettings.dspTime - lastStartTime) % barDuration;
		nextStartTime = AudioSettings.dspTime - remainder + barDuration;
		channels [flip].clip = gateLostClip;
		channels [flip].PlayScheduled (nextStartTime);
		channels [1 - flip].SetScheduledEndTime (nextStartTime);
		lastStartTime = nextStartTime;	
		nextStartTime += (double)gateLostClip.samples / gateLostClip.frequency;
		clipQueued = true;
		flip = 1 - flip;
	}

	void UpdateIntensity ()
	{
        enemyCount = CreatureManager.EnemyNum();

        if (enemyCount < 10) 
		{
			intensityLevel = 1;
		}
		else if (enemyCount >= 10 && enemyCount <= 20)
		{
			intensityLevel = 2;
		}
		else if (enemyCount >= 20)
		{
			intensityLevel = 3;
		}

		if (lastClip < 2) 
		{
			musicIntensityLevel = 1;
		}
		else if (lastClip >= 2 && lastClip <= 5)
		{
			musicIntensityLevel = 2;
		}
		else if (lastClip >= 6 && lastClip <= 8)
		{
			musicIntensityLevel = 3;
		}
	}

	void StateLogic()
	{
		//Debug.Log ("Picking a suitable clip from array ");
		nextClip = clipRoutes [lastClip] [Random.Range (0, clipRoutes [lastClip].Length)];
		if (intensityLevel > musicIntensityLevel) {
			int i=0;
			while (i < 5 && nextClip < lastClip) {
				//Debug.Log ("Clip choice too low for intensity level, picking again.");
				nextClip = clipRoutes [lastClip] [Random.Range (0, clipRoutes [lastClip].Length)];
				i++;
			}
			if(i==5)
			{
				//Debug.Log("Could not pick a higher clip in 5 attempts.");
			}
		} else if (intensityLevel < musicIntensityLevel) 
		{
			int i=0;
			while (i < 5 && nextClip > lastClip) {
				//Debug.Log ("Clip choice too high for intensity level, picking again.");
				nextClip = clipRoutes [lastClip] [Random.Range (0, clipRoutes [lastClip].Length)];
				i++;
			}
			if(i==5)
			{
				//Debug.Log("Could not pick a lower clip in 5 attempts.");
			}
		}

		CueNextClip (nextClip);
	}
}