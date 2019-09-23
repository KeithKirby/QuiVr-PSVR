using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;

public class GameBase : MonoBehaviour
{
    public enum PlayerLocationType
    {
        Hub,
        GameplayArea
    }

    public static GameBase instance;
    PhotonView v;

    public Teleporter Watchtower;
    public Text BaseText;
    public GameObject DetailHolder;
    public Text DetailText;
    public Health FinalBase;
    public AnimationCurve DifficultyCurve;
    public AnimationCurve EliteChance;
    //public EnemyStream[] Streams;
    //public Gate[] Gates;
    public float NewGameTime;
    public AnimationCurve EnemyForces;
    public GameObject ReviveEffect;
    public GameObject KillEffect;
    float DiffMult = 1f;
    public AnimationCurve ReviveReqCurve;
    public ScoreManager SPScore;
    public ScoreManager MPScore;
    public AnimationCurve ComboScoreVal;

    [Header("In-Game Values")]
    public bool inGame;
    public ObscuredFloat Difficulty;
    [HideInInspector]
    public int LastSuccess;
    [HideInInspector]
    public int PlayerDeaths;
    [HideInInspector]
    public int EnemiesSpawned;
    [HideInInspector]
    public int EnemyReq;
    [HideInInspector]
    public float IntroTimeTaken;
    float introStart;
    public bool resumed;
    public long startTime;

    public Health CurrentTarget;
    public EnemyStream CurrentStream;

    const int MIN_TIME = 60;
    [HideInInspector]
    public ObscuredFloat timeFromLastEnd;

    public PlayerLocationType PlayerLocation
    {
        set
        {
            if (_playerLocation != value)
            {
                _playerLocation = value;
                switch(_playerLocation)
                {
                    case PlayerLocationType.Hub:
                        //PS4Plus.Inst.IsGamePlayActive = false;
                        break;
                    case PlayerLocationType.GameplayArea:
                        //PS4Plus.Inst.IsGamePlayActive = true;
                        break;
                }
            }
        }
        get
        {
            return _playerLocation;
        }
    }
    PlayerLocationType _playerLocation;

    private ObscuredBool CheatDetected;

    void Awake()
    {
        v = GetComponent<PhotonView>();
        instance = this;
        ArrowEffects.EffectsDisabled = false;
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    IEnumerator Start()
    {
        while (null == PlayerProfile.Profile)
            yield return null;

        /*
        Parse.ParseConfig conf = Parse.ParseConfig.CurrentConfig;
        conf.TryGetValue<float>("DiffMult", out DiffMult);
        if (DiffMult <= 0)
            DiffMult = 1;
            */
        DiffMult = PlayerProfile.Profile.DifficultyMultiplier;
        yield return true;
        StartGame();
        InvokeRepeating("SyncStats", 1f, 1f);
    }

    void SyncStats()
    {
        Statistics.UpdateNetworkValues();
    }

    public void OnCheatDetected()
    {
        CheatDetected = true;
        Statistics.AddValue("CheatCaught", 1);
        Statistics.SaveStatistics();
        AppBase.v.cheated = true;
        if (PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void EndGame()
    {
        if (!inGame)
            return;
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            inGame = false;
            if (PhotonNetwork.inRoom)
                v.RPC("EndGameSettings", PhotonTargets.Others, !FinalBase.isDead(), (int)Difficulty);
            bool won = !FinalBase.isDead();
            EndGameSettings(won, (int)Difficulty);
            KillAll();
            if (PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
            {
                PlatformSetup.instance.IncrementVersionValue("MPGames", 1);
                PlatformSetup.instance.IncrementVersionValue("MPDifficulty", (int)Difficulty);
            }
            else
            {
                PlatformSetup.instance.IncrementVersionValue("SPGames", 1);
                PlatformSetup.instance.IncrementVersionValue("SPDifficulty", (int)Difficulty);

                PlayerPrefs.SetInt("GameInProgress", -1);
                PlayerPrefs.Save();
            }
        }
    }

    [AdvancedInspector.Inspect]
    public void ForceEndGame()
    {
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            v.RPC("EndGame", PhotonTargets.MasterClient);
        else
            EndGame();
    }

    [AdvancedInspector.Inspect]
    public void StartGame()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            Altered = false;
            inGame = true;
            if (PhotonNetwork.inRoom)
                v.RPC("StartGameSettings", PhotonTargets.Others);
            StartGameSettings();
        }
    }

    bool startedIntro;
    bool endedIntro;
    bool startedDifficulty;
    public void StartIntro() // After shot portal from hub
    {
        if (!startedIntro)
        {
            startedIntro = true;
            introStart = Time.time;
        }
    }

    public bool inIntro()
    {
        return startedIntro && !endedIntro;
    }

    public void EndIntro()
    {
        endedIntro = true;
        if (OwlMgr.instance != null)
            OwlMgr.instance.DisableOwl();
    }

    void EndObsvr(float finalScore, float difficulty)
    {
#if !NOANALYTICS
        ObserVR_CustomEvent evt;
        if (PhotonNetwork.inRoom)
        {
            evt = ObserVR.ObserVR_Events.CustomEvent("MP_GAME_END")
                .AddParameter("players", PhotonNetwork.room.PlayerCount);
        }
        else
        {
            evt = ObserVR.ObserVR_Events.CustomEvent("SOLO_GAME_END");
        }
        evt.AddParameter("gate", (int)(difficulty / 100));
        evt.AddParameter("score", finalScore);
        evt.AddParameter("enemiesKilled", Statistics.GetCurrentInt("EnmKilled"));
        evt.AddParameter("accuracy", Statistics.GetCurrentInt("Accuracy"));
        evt.AddParameter("critPercent", Statistics.GetCurrentInt("CritPerc"));
        evt.EndParameters();
#endif
    }

    void CheckDifficulty()
    {
        if (!startedDifficulty && Difficulty > 0)
        {
            multiplePlayers = false;
            if (PhotonNetwork.inRoom)
            {
                multiplePlayers = PhotonNetwork.otherPlayers.Length > 0;
            }
#if OBSERVR_SVR
            ObserVR_CustomEvent evt;
            if (PhotonNetwork.inRoom)
            {
                evt = ObserVR.ObserVR_Events.CustomEvent("MP_GAME_START")
                    .AddParameter("players", PhotonNetwork.room.PlayerCount);
            }
            else
            {
                evt = ObserVR.ObserVR_Events.CustomEvent("SOLO_GAME_START");
            }
            evt.AddParameter("Mask", Armory.currentOutfit.Helmet.GetName(false));
            evt.AddParameter("Chest", Armory.currentOutfit.Chest.GetName(false));
            evt.AddParameter("Gloves", Armory.currentOutfit.Gloves.GetName(false));
            evt.AddParameter("Quiver", Armory.currentOutfit.Quiver.GetName(false));
            evt.AddParameter("Arrow", Armory.currentOutfit.Arrow.GetName(false));
            evt.EndParameters();
#endif

            startedDifficulty = true;
            startTime = (long)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            OnStartDifficulty.Invoke();
            
        }
    }

    [PunRPC]
    public void IncreaseDifficulty(float amt)
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            Difficulty += amt;
            CreatureManager.EnemyDifficulty = Difficulty;
        }
        else if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("IncreaseDifficulty", PhotonTargets.MasterClient, amt);
    }

    public void EnterGameplayArea()
    {
        PlayerLocation = PlayerLocationType.GameplayArea;
    }

    public void UsedHubPortal()
    {
        PlayerLocation = PlayerLocationType.Hub;
    }

    bool multiplePlayers;
    [PunRPC]
    void StartGameSettings()
    {
        Statistics.ClearCurrent();
        if(DetailText != null)
        {
            if(DetailHolder != null)
                DetailHolder.SetActive(false);
            DetailText.text = "";
            BaseText.text = "";
        }
        TileManager.instance.TileDB.usedTiles = new List<EncounterTile>();
        EnemiesSpawned = 0;
        PlayerDeaths = 0;
        Difficulty = 0;
        resumed = false;
        CreatureManager.EnemyDifficulty = Difficulty;
        LastSuccess = 0;
        numBosses = 0;
        PowerupManager.instance.ClearPowerup();
        ArrowEffects.EffectsDisabled = false;
        startedDifficulty = false;
        startedIntro = false;
        endedIntro = false;
        inGame = true;
        Altered = false;
        FinalBase.Revive(true);
        if (TileManager.instance != null && (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient))
        {
            TileManager.instance.BuildWeeklySeed(); //Should probably make more elegant for different map types
        }
        TeleporterManager.instance.Reset();
        EventManager.Reset();
        //TeleporterManager.instance.SetPowerupRandom();
        if (TelePlayer.instance != null && TelePlayer.instance.currentNode != null)
        {
            if(!AppBase.v.NoLobby && EnvironmentManager.curEnv != EnvironmentType.Olympus)
                Watchtower.ForceUse();
            //else
                //Watchtower.ForceUse();
        }            
        foreach (var v in StreamManager.instance.AllStreams)
        {
            v.ResetValues();
            if (v.StreamOnStart)
                v.BeginStream();
        }
        if(GateManager.instance != null)
        {
            foreach (var v in GateManager.instance.AllGates)
            {
                if (TileManager.instance == null || !TileManager.instance.Infinite || v == TileManager.instance.BaseGate)
                    v.ResetGate();
                v.ResetReq();
            }
        }
        if (OwlMgr.instance != null)
        {
            OwlMgr.instance.Reset();
            OwlMgr.instance.EnableOwl();
        }
        GameOrb.Reset();
        OnStartGame.Invoke();
    }

    public void NewGameClear()
    {
        if(Difficulty < 10)
            Statistics.ClearCurrent();
    }

    [PunRPC]
    void EndGameSettings(bool won, int finalDifficulty)
    {
        DebugTimed.Log("Game Completed -- [Won: " + won + "]");
        PS4Plus.Inst.IsGamePlayActive = false;
        if (AppBase.v.isBeta)
            Achievement.EarnAchievement("BETA_TEST");
        Difficulty = finalDifficulty;
        CreatureManager.EnemyDifficulty = Difficulty;
        inGame = false;
        PowerupOrb.RemoveAll();
        TeleporterManager.instance.Invoke("Reset", 0.75f);
        TileManager.instance.TileDB.usedTiles = new List<EncounterTile>();
        ContinuePanel.Close();
        Settings.Set("FirstTime", false);
        if (!won)
        {
            if (!EnvironmentManager.instance.UseDynamicMusic)
                GameplayAudio.PlayLose();
            BaseText.text = "Enemy Repelled!";
            if (finalDifficulty > 199 && !ItemGenerator.ItemOnLoss(Difficulty) && CanGetReward()) //remove false
            {
                int motes = ItemGenerator.GetResource(Difficulty);
                Armory.instance.GiveResource(motes);
                if (MoteParticles.instance != null)
                    MoteParticles.instance.SpawnMotes(motes);
                timeFromLastEnd = 0;
            }
            else if (CanGetReward())
            {
                fffff();
                timeFromLastEnd = 0;
            }
        }
        else
        {
            if(!EnvironmentManager.instance.UseDynamicMusic)
                GameplayAudio.PlayWin();
            BaseText.text = "Enemy Repelled!";
            if (CanGetReward())
            {
                fffff();
                timeFromLastEnd = 0;
            }
        }
        int score = CalculateScore(finalDifficulty);
        EndObsvr(score, finalDifficulty);
        if(CheatDetected)
        {
            Debug.Log("Score could not be submitted - Cheats Used");
        }
        else if (Altered)
        {
            Debug.Log("Score could not be submitted - Altered");
        }
        else if ((!PhotonNetwork.inRoom || !multiplePlayers) && SPScore != null)
        {
            Debug.Log("Changing Score: " + score);
            SPScore.ChangeScore(score);
            SPScore.SubmitIfNewHigh();
            if (Statistics.GetInt("HighestDiffSP") < score)
            {
                Statistics.SetValue("HighestDiffSP", score);
            }
        }
        else if (PhotonNetwork.inRoom)
        {
            SubmitMPScore(score);
            if (Statistics.GetInt("HighestDiffMP") < score)
            {
                Statistics.SetValue("HighestDiffMP", score);
            }
        }
            



        OnEndGame.Invoke();
        foreach (var v in GateManager.instance.AllGates)
        {
            v.DisableGate();
        }
        foreach (var v in StreamManager.instance.AllStreams)
        {
            v.EndStream();
        }
        EnemyStream.completed = false;
        CreatureManager.bossOut = false;
        TeleporterManager.instance.Invoke("Reset", 5);
        if (FinalBase.currentHP < 10)
            FinalBase.currentHP = 1;
        StopAllCoroutines();
        Statistics.SaveStatistics();
        StartCoroutine("NewGameTimed", score);
        
    }

    int CalculateScore(int diff)
    {
        int bonusRestores = Mathf.Clamp(GateManager.instance.highestGateRestores * 5, 0, 20);
        GamePanel.PlayerTuple teamAcc = GamePanel.BestVal(GamePanel.CurrentValues(PlayerMetric.Accuracy));
        GamePanel.PlayerTuple teamCrit = GamePanel.BestVal(GamePanel.CurrentValues(PlayerMetric.CritPerc));
        GamePanel.PlayerTuple longShot = GamePanel.BestVal(GamePanel.CurrentValues(PlayerMetric.LongShot));
        GamePanel.PlayerTuple teamCombo = GamePanel.BestVal(GamePanel.CurrentValues(PlayerMetric.HighCombo));
        int AccScore = (int)((teamAcc.value/100f) * 20f);
        int CritScore = (int)((teamCrit.value / 35f) * 20f);
        int LongScore = (int)(Mathf.Clamp(((longShot.value - 80)/120) * 20f, 0, 20));
        int ComboScore = (int)(Mathf.Clamp(ComboScoreVal.Evaluate(teamCombo.value), 0, 20));
        int finalScore = diff + bonusRestores + AccScore + CritScore + LongScore + ComboScore;
        if (DetailHolder != null)
            DetailHolder.SetActive(true);
        DetailText.text = "<color=white>";
        if(PhotonNetwork.inRoom && PhotonNetwork.countOfPlayers > 1)
            DetailText.text += "Team Score: " + finalScore;
        else
            DetailText.text += "Score: " + finalScore;
        DetailText.text += "</color><size=42><size=12>\n\n</size>";
        DetailText.text += "Base: <color=white>" + diff;
        if (bonusRestores > 1)
            DetailText.text += "</color>\nBonus Restores: <color=white>" + bonusRestores;
        if(PhotonNetwork.inRoom && PhotonNetwork.countOfPlayers > 1)
        {
            DetailText.text += "</color>\nAccuracy (<size=28><color=#60c2e5>" + teamAcc.name + "</color></size> - " + (int)(teamAcc.value) + "): <color=white>" + AccScore;
            DetailText.text += "</color>\nCrit Percent (<size=28><color=#60c2e5>" + teamCrit.name + "</color></size> - " + (int)(teamCrit.value) + "): <color=white>" + CritScore;
            DetailText.text += "</color>\nLongest Shot (<size=28><color=#60c2e5>" + longShot.name + "</color></size> - " + (int)longShot.value + "): <color=white>" + LongScore;
            DetailText.text += "</color>\nBest Combo (<size=28><color=#60c2e5>" + teamCombo.name + "</color></size> - " + (int)teamCombo.value + "): <color=white>" + ComboScore + "</color>\n</size>";
        }
        else
        {
            DetailText.text += "</color>\nAccuracy (" + (int)(teamAcc.value) + "): <color=white>" + AccScore;
            DetailText.text += "</color>\nCrit Percent (" + (int)(teamCrit.value) + "): <color=white>" + CritScore;
            DetailText.text += "</color>\nLongest Shot (" + (int)longShot.value + "): <color=white>" + LongScore;
            DetailText.text += "</color>\nBest Combo (" + (int)teamCombo.value + "): <color=white>" + ComboScore + "</color>\n</size>";
        }
        return finalScore;
    }

    [HideInInspector]
    public bool Altered;
    void CheckCommands()
    {
        if(inGame && !DevConsole.DevConsoleUI.isActive())
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.UpArrow))
            {
                AlterGame();
                IncreaseDifficulty(100);
            }
            else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.DownArrow))
            {
                AlterGame();
                IncreaseDifficulty(-100);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G))
            {
                SkipGate();
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
            {
                AlterGame();
                KillAll();
            }

            //if (Input.GetKey(KeyCode.K) )
            //{
                //Debug.Log("K pressed");
                //DestroyGate();
            //}

#if UNITY_PS4 && ENABLE_CHEATS
            var move1 = PS4InputEx.GetMove(1);
            if (move1.CrossButton.Down) // Not circle
            {
                Debug.Log("Move1CrossButton.Down");
                SkipGate();
            }

            if(Input.GetButtonDown("L2")) // Cheat on controller
            {
                Debug.Log("L2 pressed");
                SkipGate();
            }
            //if (Input.GetButtonDown("L1"))
            //{
                //Debug.Log("L1 pressed");
                //DestroyGate();
            //}
#endif
        }
    }

    void SkipGate()
    {
        if (CurrentStream != null && CurrentStream.RestoreGate != null)
        {
            AlterGame();
            if (CurrentStream.RestoreGate.isDestroyed())
                CurrentStream.RestoreGate.ForceRestore();
            else if (!CurrentStream.RestoreGate.isClosed())
                CurrentStream.RestoreGate.ForceClose();

        }
    }

    void DestroyGate()
    {
        if (CurrentTarget != null)
        {
            AlterGame();
            CurrentTarget.Kill();
        }
    }

    void AlterGame()
    {
        Altered = true;
        if (PhotonNetwork.inRoom)
            GetComponent<PhotonView>().RPC("AlterNetwork", PhotonTargets.Others);
    }

    [PunRPC]
    void AlterNetwork()
    {
        Altered = true;
    }

    public void KillAll(List<Creature> ignore = null)
    {
        foreach (var v in CreatureManager.AllEnemies())
        {
            if (v != null && !v.GetComponent<Health>().isDead() && (ignore == null || !ignore.Contains(v)))
            {
                v.Kill();
            }
        }
    }

    IEnumerator NewGameTimed(int scoreReached)
    {
        float third = NewGameTime / 3f;
        float t1 = third;
        float t2 = 2 * third;
        while (t1 > 0)
        {
            t1 -= Time.deltaTime;
            yield return true;
        }
        while (t2 > 0)
        {
            t2 -= Time.deltaTime;
            //DetailText.text = "Difficulty Conquered: " + scoreReached + "\nNew Game in: " + (int)t2;
            yield return true;
        }
        DetailText.text = "";
        StartGame();
    }

    private void fffff()
    {
        int r = ItemGenerator.GetRatity(Difficulty);
        var itemGen = GameObject.FindObjectOfType<ItemGenerator>();
        itemGen.GetRandomItem(r, ItemGenerator.Encrypt("" + r), EndGrantItem);
    }

    [AdvancedInspector.Inspect]
    void DebugDropLoot()
    {
        Difficulty = 100;
        fffff();
    }

    void EndGrantItem(ArmorOption item)
    {
        if (ItemReward.instance != null && !Armory.instance.HasDuplicate(item))
            ItemReward.instance.SetupReward(item);
        Armory.instance.AddItemFromGenerator(item);
    }

    public void AddDeath()
    {
        PlayerDeaths++;
        if (PhotonNetwork.inRoom)
            v.RPC("AddDeathNetwork", PhotonTargets.Others);
    }

    [AdvancedInspector.Inspect]
    void ClearGame()
    {
        ClearGame(false);
    }

    [AdvancedInspector.Inspect]
    void ForceKillAllEnemies()
    {
        AlterGame();
        KillAll();
    }

    public void ClearGame(bool startNew = false)
    {
        inGame = false;
        foreach (var v in StreamManager.instance.AllStreams)
        {
            v.EndStream();
        }
        TeleporterManager.instance.Reset();
        foreach (var s in StreamManager.instance.AllStreams)
        {
            s.KillAll();
        }
        foreach (var v in GateManager.instance.AllGates)
        {
            v.ResetGate();
        }
        TileManager.instance.TileDB.usedTiles = new List<EncounterTile>();
        StopAllCoroutines();
        OnClearGame.Invoke();
        if (startNew)
            StartGameSettings();
    }

    public void TryGiveRevive(int val, Vector3 pos)
    {
        if (CurrentStream != null && CurrentStream.RestoreGate != null && CurrentStream.RestoreGate.isDestroyed())
        {
            CurrentStream.RestoreGate.AddRevReq(val);
            ReviveEffectNetwork(pos);
            if (PhotonNetwork.inRoom)
                GetComponent<PhotonView>().RPC("ReviveEffectNetwork", PhotonTargets.Others, pos);
        }
    }

    public int GetReviveReq()
    {
        if (CurrentStream != null && CurrentStream.RestoreGate != null && CurrentStream.RestoreGate.isDestroyed())
        {
            int req = (int)ReviveReqCurve.Evaluate(Difficulty/100f);//150 + ((int)(Difficulty/100f) * 45);
            if (PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
            {
                int plrNum = PhotonNetwork.playerList.Length;
                float[] diffMults = { 1f, 1.6f, 2f, 2.25f };//{ 1f, 1.6f, 2.5f, 3.2f };
                req = (int)(req * diffMults[plrNum - 1]);
            }
            return req;//CurrentStream.RestoreGate.CalculateReviveReq();
        }
        return -1;
    }

    [PunRPC]
    void ReviveEffectNetwork(Vector3 pos)
    {
        if (CurrentStream != null && CurrentStream.RestoreGate != null)
        {
            GameObject o = (GameObject)Instantiate(ReviveEffect, pos, Quaternion.identity);
            o.GetComponent<EffectSettings>().Target = CurrentStream.RestoreGate.gameObject;
        }
    }

    public void TickDifficulty()
    {
        if (CurrentTarget != null)
        {
            float dfc = (Time.deltaTime / 5f);
            dfc *= DiffMult;
            //dfc *= Mathf.Clamp((CurrentTarget.currentHP / CurrentTarget.maxHP), 0.5f, 1);
            if (TileManager.instance != null)
                Difficulty += dfc * (GateManager.instance.CurrentGateIndex()+1);
            else
                Difficulty += dfc * DifficultyCurve.Evaluate(Difficulty) * EnemyDB.v.DifficultyTimeMult;
            CreatureManager.EnemyDifficulty = Difficulty;
        }
    }

    public int GetEnemyForces()
    {
        int forces = (int)EnemyForces.Evaluate(Difficulty);
        if (PhotonNetwork.inRoom && PhotonNetwork.playerList.Length > 1)
            forces = (int)(forces * (0.5 + (PhotonNetwork.playerList.Length * 0.75f)));
        forces += (int)(Difficulty / 20f);
        EnemyReq = forces;
        return forces;
    }

    [HideInInspector]
    public int numBosses;
    public bool ShouldSpawnBoss()
    {
        bool timer = instance.EnemiesSpawned >= instance.GetEnemyForces();
        if (timer)
            return true;
        else
        {
            if (numBosses == 0 && Difficulty >= 500)
                return true;
            if (numBosses == 1 && Difficulty >= 1000)
                return true;
        }
        return false;
    }

    public static bool CanGetReward()
    { 
        if(instance != null)
        {			
            return  
                    instance.timeFromLastEnd >= MIN_TIME &&
                    !instance.CheatDetected &&
                    !instance.Altered &&
                    !AppBase.v.OfflineMode;
        }
        return true;
    }

    void DevKillAll()
    {
        KillPulse(Vector3.up*0.2f);
    }

    public void KillPulse(Vector3 pos, List<Creature> ignore=null)
    {
        KillAll(ignore);
        if(KillEffect != null)
        {
            GameObject ne = (GameObject)Instantiate(KillEffect, pos, KillEffect.transform.rotation);
            ne.SetActive(true);
        }
    }

    void SubmitMPScore(int score)
    {
        if(PhotonNetwork.isMasterClient && MPScore != null)// && PhotonNetwork.playerList.Length > 1)
        {
#if UNITY_PS4
            string[] vals = ScoreDatabase.Inst.GetMPIdAndComment();
#else
            string[] vals = ScoreDatabase.GetMPValues();
#endif
            //Debug.LogFormat("SubmitMPScore({0}) MPVals {1} {2}", score, vals[0], vals[1]);
            MPScore.ChangeScoreMP(score, vals[0], vals[1]);
        }
        else
        {
            //Debug.LogFormat("SubmitMPScore({0}) Fail", score);
        }
    }

    [PunRPC]
    public void RefreshMPScores(bool success, int score)
    {
        if(success)
        {
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            {
                GetComponent<PhotonView>().RPC("RefreshMPScores", PhotonTargets.Others, success, score);
            }
            MPScore.GetComponent<AAScoreUI>().CheckTopMP(true, score);
        }
    }

#region Syncronizaton
    public float updateRate;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting && syncing)
        {
            syncing = false;
            updcd = updateRate;

            stream.SendNext(inGame);
            stream.SendNext((float)Difficulty);
            stream.SendNext(PlayerDeaths);
            stream.SendNext(EnemiesSpawned);
            stream.SendNext(LastSuccess);
        }
        else if(stream.isReading)
        {
            inGame = (bool)stream.ReceiveNext();
            Difficulty = (float)stream.ReceiveNext();
            CreatureManager.EnemyDifficulty = Difficulty;
            PlayerDeaths = (int)stream.ReceiveNext();
            EnemiesSpawned = (int)stream.ReceiveNext();
            LastSuccess = (int)stream.ReceiveNext();
        }
    }

    float updcd;
    bool syncing;
    public bool PlayingBase = true;
    void Update()
    {
        if(PlayingBase)
        {
            CheckCommands();
            timeFromLastEnd += Time.unscaledDeltaTime;
            updcd -= Time.deltaTime;
            if (updcd < 0 && !syncing)
            {
                if(PhotonNetwork.isMasterClient && !v.isMine)
                {
                    v.RequestOwnership();
                }
                syncing = true;
            }
            IntroTimeTaken = 0;
            if (startedIntro && !endedIntro)
                IntroTimeTaken = Time.time - introStart;
            CreatureManager.ClearInvalid();
            if (!startedDifficulty && Difficulty > 0)
                CheckDifficulty();
            else if (startedDifficulty && PhotonNetwork.inRoom && !multiplePlayers)
                multiplePlayers = PhotonNetwork.otherPlayers.Length > 0;
            if (CheatDetected && !PhotonNetwork.inRoom)
                Difficulty = 0;
            CreatureManager.EnemyDifficulty = Difficulty;
            bool gameplayActive = inGame && Difficulty > 0;
            if (PS4Plus.Inst)
                PS4Plus.Inst.IsGamePlayActive = gameplayActive;
        }
    }

    void OnJoinedRoom()
    {
        timeFromLastEnd = MIN_TIME;
        if(!PhotonNetwork.isMasterClient)
            ClearGame(false);
        else
            ClearGame(true);
    }

    void OnLeftRoom()
    {
        timeFromLastEnd = MIN_TIME;
        ClearGame(true);
    }

    public static bool RollElite()
    {
        float f = Random.Range(0, 100f);
        float chance = 0;
        if(instance != null)
            chance = 100f * instance.EliteChance.Evaluate(instance.Difficulty);
        return f < chance;
    }

    [PunRPC]
    void AddDeathNetwork()
    {
        PlayerDeaths++;
    }

#endregion

    public UnityEvent OnEndGame;
    public UnityEvent OnStartGame;
    public UnityEvent OnClearGame;
    public UnityEvent OnStartDifficulty;
}