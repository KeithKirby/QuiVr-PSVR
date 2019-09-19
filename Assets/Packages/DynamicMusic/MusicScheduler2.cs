using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MusicScheduler2 : MonoBehaviour
{
    public bool gateLost = false;
    public int enemyCount = 0;
    [Tooltip("Randomly changes enemy count and resets to 0 if a gate is lost.")]
    public bool simulateEnemies = true;
    public int intensityLevel = 1;
    public int musicIntensityLevel = 1;
    public int tolerance = 2;
    public bool debugMessages = false;
    public AudioClip gateLostClip;
    public int defaultClipIndex = 0;
    public float bpm = 70;
    public int beatsPerBar = 4;
    public int noteLength = 16;
    public AudioSource[] channels;
    public AudioClip[] clips;
    public AudioSource[] hitChannel;
    public AudioClip[] hitClip;
    public static MusicScheduler2 instance;
    int[][] clipRoutes = new int[][]{
        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
        new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
        new int[] { 2, 3, 4, 6, 7, 8, 10, 11, 13, 14, 15 },
        new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
        new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
        new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 14, 15 },
        new int[] { 4, 6, 7, 8, 10, 11, 14, 15 },
        new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
        new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 },
        new int[] { 8, 10, 11, 13, 15, 16, 17, 18, 19 },
        new int[] { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 },
        new int[] { 8, 10, 11, 13, 15, 17, 18, 19, 20, 21, 22 },
        new int[] { 8, 9, 10, 14, 15, 16, 17, 18, 19, 20, 21, 22 },
        new int[] { 8, 10, 11, 13, 15, 17, 18, 19, 20, 21, 22, 23 },
        new int[] { 8, 9, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
        new int[] { 9, 10, 11, 16, 17, 18, 19, 20, 21, 22, 23, 26 },
        new int[] { 9, 10, 11, 16, 17, 18, 19, 20, 21, 22, 23, 25 },
        new int[] { 11, 13, 17, 19, 20, 21, 22, 23, 26 },
        new int[] { 11, 14, 16, 17, 18, 19, 20, 21, 22, 23, 25, 27 },
        new int[] { 13, 17, 18, 19, 20, 21, 23, 24, 26, 27, 28 },
        new int[] { 17, 18, 19, 20, 21, 22, 24, 25, 27, 28 },
        new int[] { 16, 17, 18, 19, 20, 21, 22, 25, 26, 27, 28 },
        new int[] { 16, 17, 19, 20, 21, 25, 26, 27, },
        new int[] { 17, 18, 19, 20, 21, 23, 25, 27, 28 },
        new int[] { 17, 18, 19, 21, 22, 26, 27, 28, 30 },
        new int[] { 20, 22, 25, 26, 27, 29, 30, },
        new int[] { 20, 21, 22, 23, 26, 27, 28, 30, 32 },
        new int[] { 21, 28, 31 },
        new int[] { 27, 32 },
        new int[] { 25, 29, 33 },
        new int[] { 16, 21, 25, 27, 34, 36 },
        new int[] { 16, 21, 25, 28, 33, 35 },
        new int[] { 16, 21, 25, 27, 34, 36 },
        new int[] { 16, 21, 25, 28 }
    };
    int lastClip;
    int nextClip;
    int hitClipIndex;
    int lastHitClip;
    double musicStartTime;
    double lastStartTime;
    double nextStartTime;
    double comboHitPlayTime;
    float currentBar;
    double barDuration;
    double noteDuration;
    bool running = false;
    bool clipQueued;
    float timer;
    int flip = 0;
    int hitFlip = 0;
    public float volume = 0f;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        barDuration = 60d / bpm * beatsPerBar;
        noteDuration = barDuration / noteLength;
        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            UpdateState();
        }
        float vol = volume;
        vol *= Settings.GetFloat("MusicVolume", 0.8f);
        foreach (var v in channels)
            v.volume = vol;
        foreach (var v in hitChannel)
            v.volume = vol;
    }
    void StartMusic()
    {
        AudioClip firstClip = clips[defaultClipIndex];
        channels[flip].clip = firstClip;
        musicStartTime = AudioSettings.dspTime;
        channels[flip].PlayScheduled(musicStartTime + barDuration);
        lastStartTime = musicStartTime + barDuration;
        double firstClipLength = (double)firstClip.samples / firstClip.frequency;
        nextStartTime = lastStartTime + firstClipLength - barDuration;
        flip = 1 - flip;
        running = true;
        StartCoroutine(RandomizeEnemies());
    }
    void CueNextClip(int clipIndex)
    {
        AudioClip next = clips[clipIndex];
        channels[flip].clip = next;
        channels[flip].PlayScheduled(nextStartTime);
        lastStartTime = nextStartTime;
        nextStartTime += (double)next.samples / next.frequency;
        nextStartTime -= barDuration;
        clipQueued = true;
        lastClip = clipIndex;
        if (debugMessages)
            Debug.Log("The next clip will be " + clips[clipIndex].name);
        flip = 1 - flip;
    }
    void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer > barDuration / 4)
        {

            currentBar += 0.25f;
            UpdateIntensity();
            if (AudioSettings.dspTime > lastStartTime)
            {
                clipQueued = false;
            }
            if (!clipQueued && AudioSettings.dspTime > nextStartTime - barDuration / 2)
            {
                StateLogic();
            }
            if (gateLost)
            {
                GateLost();
            }
            timer = 0;
        }
    }
    void GateLost()
    {
        gateLost = false;
        if (debugMessages)
            Debug.Log("Gate lost, playing clip at next bar");
        double remainder = (AudioSettings.dspTime - lastStartTime) % barDuration;
        nextStartTime = AudioSettings.dspTime - remainder + barDuration;
        channels[flip].clip = gateLostClip;
        channels[flip].PlayScheduled(nextStartTime);
        channels[1 - flip].SetScheduledEndTime(nextStartTime);
        lastStartTime = nextStartTime;
        nextStartTime += (double)gateLostClip.samples / gateLostClip.frequency;
        clipQueued = true;
        flip = 1 - flip;
        if (simulateEnemies)
            enemyCount = 0;
    }
    void UpdateIntensity()
    {
        Gate g = GateManager.CurrentGate();
        int enemyNum = CreatureManager.EnemyNum();
        float forceValue = CreatureManager.CurrentForceValue();
        if (enemyNum > 0)
        {
            forceValue += ((GameBase.instance.Difficulty / 100f) - 1) * 0.35f;
            if (g != null)
            {
                Health ghp = g.GetHP();
                if (ghp.currentHP < ghp.maxHP - 1)
                {
                    forceValue += 16 * (1 - (ghp.currentHP / ghp.maxHP));
                }
            }
        }
        forceValue += CreatureManager.CloseToGate() * 3;
        enemyCount = (int)Mathf.Ceil(forceValue);
        intensityLevel = Mathf.Clamp(enemyCount, 0, 32);
        if (CreatureManager.bossOut)
            intensityLevel = 32;
        musicIntensityLevel = lastClip;
    }
    void StateLogic()
    {
        if (debugMessages)
            Debug.Log("Starting clip selection.");
        int highestClip = clipRoutes[lastClip][clipRoutes[lastClip].Length - 1];
        int lowestClip = clipRoutes[lastClip][0];
        if (debugMessages)
            Debug.Log("Lowest possible clip is " + clips[lowestClip].name + " the highest is " + clips[highestClip].name);
        if (intensityLevel <= highestClip && intensityLevel >= lowestClip)
        {
            if (debugMessages)
                Debug.Log("Intensity level is within last clip's range, selecting a random clip.");
            nextClip = clipRoutes[lastClip][Random.Range(0, clipRoutes[lastClip].Length)];
            if (intensityLevel > musicIntensityLevel + tolerance)
            {
                if (debugMessages)
                    Debug.Log("Clip level is too low, changing selection.");
                int i = 0;
                while (i < 5 && nextClip < musicIntensityLevel)
                {
                    nextClip = clipRoutes[lastClip][Random.Range(0, clipRoutes[lastClip].Length)];
                    i++;
                }
                if (i == 5)
                {
                    if (debugMessages)
                        Debug.Log("Could not pick a higher clip in 5 attempts, selection is unchanged.");
                }
            }
            else if (intensityLevel < musicIntensityLevel - tolerance)
            {
                if (debugMessages)
                    Debug.Log("Clip is too high, changing selection");
                int i = 0;
                while (i < 5 && nextClip > musicIntensityLevel)
                {
                    nextClip = clipRoutes[lastClip][Random.Range(0, clipRoutes[lastClip].Length)];
                    i++;
                }
                if (debugMessages && i == 5)
                {
                    Debug.Log("Could not pick a lower clip in 5 attempts, selection is unchanged.");
                }
            }
        }
        else
        {
            if (debugMessages)
                Debug.Log("Intensity is outside of clip's intensity range.");
            if (intensityLevel > musicIntensityLevel)
            {
                if (debugMessages)
                    Debug.Log("Picking highest possible clip");
                nextClip = highestClip;
            }
            else if (intensityLevel < musicIntensityLevel)
            {
                if (debugMessages)
                    Debug.Log("Picking lowest possible clip");
                nextClip = lowestClip;
            }
        }
        CueNextClip(nextClip);
    }
    IEnumerator RandomizeEnemies()
    {
        yield return new WaitForSeconds(7);
        while (simulateEnemies && running)
        {
            int enemiesSpawned = (int)Mathf.Round(Random.Range(0, 4));
            int enemiesKilled = (int)Mathf.Round(Random.Range(0, 3));
            if (musicIntensityLevel > 30)
            {
                enemiesKilled += (int)Mathf.Round(Random.Range(0, 3));
            }
            enemyCount = Mathf.Clamp(enemyCount + (enemiesSpawned - enemiesKilled), 0, 100);
            yield return new WaitForSeconds(2);
        }
    }
    public static void TriggerHit()
    {
        if (instance != null)
            instance.TriggerComboClip();
    }
    public void TriggerComboClip()
    {
        if (AudioSettings.dspTime > comboHitPlayTime + noteDuration)
        {
            if (debugMessages)
                Debug.Log("Combo hit triggered");
            while (hitClipIndex == lastHitClip)
            {
                hitClipIndex = Random.Range(0, hitClip.Length - 1);
            }
            hitChannel[hitFlip].clip = hitClip[hitClipIndex];
            double remainder = (AudioSettings.dspTime - lastStartTime) % noteDuration;
            comboHitPlayTime = AudioSettings.dspTime - remainder + noteDuration;
            hitChannel[hitFlip].PlayScheduled(comboHitPlayTime);
            lastHitClip = hitClipIndex;
            hitFlip = 1 - hitFlip;
        }
        else
        {
            if (debugMessages)
                Debug.Log("Cannot play Combo Hit, still in cooldown");
        }
    }
}