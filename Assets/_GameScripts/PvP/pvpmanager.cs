using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
//using Parse;
using UnityEngine.UI;
public class pvpmanager : MonoBehaviour {

    PhotonView v;
    public static pvpmanager instance;

    public int EnemyBudget;
    public int EnemyDifficulty;
    public int BaseResourceGen;
    public ArmorOption BaseGloves;

    [Header("Game Completion")]
    public Transform LeaveOrb;
    public Text EndText;
    public AudioClip EndClip;

    [Header("In Game Values")]
    public bool PlayingPVP;
    public bool inGame;
    public bool gameCompleted;
    public float Handicap;
    public ObscuredFloat myResource;
    public PunTeams.Team myTeam;
    public ObscuredInt RedTeamEnm;
    public ObscuredInt BlueTeamEnm;
    public float TimeElapsed;
    public int allowedEnms;

    [Header("Team 1")]
    public EnemyStream t1Stream;
    public Health t1Health;
    public List<PvPPlayer> t1Users;

    [Header("Team 2")]
    public EnemyStream t2Stream;
    public Health t2Health;
    public List<PvPPlayer> t2Users;

    #region Life Cycle

    void Awake()
    {
        v = GetComponent<PhotonView>();
        instance = this;
        PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
        t1Users = new List<PvPPlayer>();
        t2Users = new List<PvPPlayer>();
#if !NOANALYTICS
        RemoteSettings.Updated += new RemoteSettings.UpdatedEventHandler(HandleRemoteSettings);
#endif
    }

    void HandleRemoteSettings()
    {
#if !NOANALYTICS
        EnemyDifficulty = RemoteSettings.GetInt("PVP_ENEMY_DIFFICULTY", EnemyDifficulty);
        BaseResourceGen = RemoteSettings.GetInt("PVP_RESOURCE_GEN", BaseResourceGen);
#endif
        if (PvPPanel.instance != null)
        {
            PvPPanel.instance.UpdateButtons();
        }
    }

    void Update()
    {
        if(inGame)
        {
            CreatureManager.ClearInvalid();
            CreatureManager.EnemyDifficulty = EnemyDifficulty;
        }
    }

#endregion

#region Game Progress
    public void SetupTeams()
    {

    }

    public void StartGameDelayed(float delay)
    {
        Invoke("StartGame", delay);
    }

    [AdvancedInspector.Inspect]
    public void StartGame()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            Debug.Log("Starting PvP Game");
            if (PhotonNetwork.inRoom)
                v.RPC("StartGameSettings", PhotonTargets.Others);
            StartGameSettings();
        }
    }

    Outfit realOutfit;
    [PunRPC]
    public void StartGameSettings()
    {
        Statistics.ClearCurrent();
        EndText.text = "";
        realOutfit = Armory.currentOutfit;
        Armory.currentOutfit = new Outfit();
        Armory.currentOutfit.Gloves = BaseGloves;
        if(PlayerArmor.instance != null)
            PlayerArmor.instance.ForceRefresh();
        InvokeRepeating("TickProgress", 1f, 1f);
        inGame = true;
        Handicap = CalculateHandicap();
    }

    public void RefreshTeamVals()
    {
        if (myTeam == PunTeams.Team.blue)
        {
            TeleporterManager.instance.currentArea = 0;
            EnvironmentManager.ChangeEnvImmediate(EnvironmentType.Desert);
        }            
        else
        {
            TeleporterManager.instance.currentArea = 1;
            EnvironmentManager.ChangeEnvImmediate(EnvironmentType.Snow);
        }         
        TeleporterManager.instance.ChangeArea();

    }

    [AdvancedInspector.Inspect]
    public void EndGame()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            Debug.Log("Ending Game");
            if (PhotonNetwork.inRoom)
                v.RPC("EndGameSettings", PhotonTargets.Others);
            EndGameSettings();
        }
    }

    [PunRPC]
    public void EndGameSettings()
    {
        inGame = false;
        gameCompleted = true;
        CancelInvoke("TickProgress");
        KillAll();
        Armory.currentOutfit = realOutfit;
        if (PlayerArmor.instance != null)
            PlayerArmor.instance.ForceRefresh();
        bool blueWon = true;
        string endTxt = "~~Game Over~~\n";
        if(t1Health.isDead()) //Blue Team
        {
            blueWon = false;
        }
        else if(t2Health.isDead()) //Red Team
        {

        }
        else
        {
            endTxt += "<size=80>(Opponent Disconnected)</size>\n";
            blueWon = myTeam == PunTeams.Team.blue;
        }
        bool myTeamWon = ((blueWon && myTeam == PunTeams.Team.blue) || (!blueWon && myTeam == PunTeams.Team.red));
        GameplayAudio.PlayLose();
        endTxt += "<size=180><color=";
        endTxt += myTeamWon ? "#00ff00ff>Victory!" : "#ff0000ff>Defeat!";
        endTxt += "</color></size>";
        EndText.text = endTxt;
        if(PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            HandleELO(blueWon);
        }
        StartCoroutine("SwitchBack", myTeamWon);
    }

    IEnumerator SwitchBack(bool teamWon)
    {
        float t = 15f;
        while (t > 0)
        {
            t -= Time.deltaTime;
            if(t < 10)
            {
                string endTxt = "~~Game Over~~\n";
                endTxt += "<size=180><color=";
                endTxt += teamWon ? "#00ff00ff>Victory!" : "#ff0000ff>Defeat!";
                endTxt += "</color></size>\n";
                endTxt += "Leaving in " + (int)t;
                EndText.text = endTxt;
            }
            yield return true;
        }
        GameTypeManager.instance.ChangeToCanyon();
    }

    public void TickProgress()
    {
        if(PlayingPVP && inGame)
        {
            TimeElapsed += 1f;
            myResource += TickResource();
        }
    }

    float TickResource()
    {
        return BaseResourceGen * Handicap;
    }

    void HandleELO(bool blueWon)
    {
        EloRating.Matchup mPrev = new EloRating.Matchup();
        mPrev.User1Score = AvgMMR(t1Users);
        mPrev.User2Score = AvgMMR(t2Users);
        EloRating.Matchup mNew = new EloRating.Matchup();
        mNew.User1Score = mPrev.User1Score;
        mNew.User2Score = mPrev.User2Score;
        EloRating.UpdateScores(mNew, blueWon, (blueWon?AvgGamesPlayed(t1Users):AvgGamesPlayed(t2Users)));
        int t1Diff = mNew.User1Score - mPrev.User1Score;
        int t2Diff = mNew.User2Score - mPrev.User2Score;
        UpdateMMRDB(t1Diff, t2Diff);
    }
#endregion

#region Game Mechanics

#endregion

#region Utility

    public void SetTeam()
    {
        if(PhotonNetwork.inRoom)
        {
            if(PhotonNetwork.playerList.Length <= 2)
            {
                if (PhotonNetwork.isMasterClient)
                    TeamExtensions.SetTeam(PhotonNetwork.player, PunTeams.Team.blue);
                else
                    TeamExtensions.SetTeam(PhotonNetwork.player, PunTeams.Team.red);
            }
            else
            {
                for(int i=0; i< PhotonNetwork.playerList.Length; i++)
                {
                    if(PhotonNetwork.playerList[i] == PhotonNetwork.player)
                    {
                        if(i%2 == 0)
                            TeamExtensions.SetTeam(PhotonNetwork.player, PunTeams.Team.blue);
                        else
                            TeamExtensions.SetTeam(PhotonNetwork.player, PunTeams.Team.red);
                    }
                }
            }
        }
        myTeam = TeamExtensions.GetTeam(PhotonNetwork.player);
        SendMMRInfo();
        RefreshTeamVals();
    }

    void SendMMRInfo()
    {
        /*
        if (ParseUser.CurrentUser != null)
        {
            string un = ParseUser.CurrentUser.Username;
            int MMR = 800;
            int GP = 1;
            if (ParseUser.CurrentUser.ContainsKey("MMR"))
                MMR = ParseUser.CurrentUser.Get<int>("MMR");
            if (ParseUser.CurrentUser.ContainsKey("PVPGames"))
                GP = ParseUser.CurrentUser.Get<int>("PVPGames");
            if (PhotonNetwork.inRoom)
                v.RPC("NetworkMMR", PhotonTargets.Others, un, (int)myTeam, MMR, GP);
            NetworkMMR(un, (int)myTeam, MMR, GP);
        }
        */

        var profile = PlayerProfile.Profile;
        if (PhotonNetwork.inRoom)
            v.RPC("NetworkMMR", PhotonTargets.Others, profile.Username, (int)myTeam, profile.MatchmakingRank, profile.GamesPlayed);
        NetworkMMR(profile.Username, (int)myTeam, profile.MatchmakingRank, profile.GamesPlayed);
    }

    int AvgMMR(List<PvPPlayer> plrs)
    {
        int mmr = 800;
        if(plrs.Count > 0)
        {
            foreach(var v in plrs)
            {
                mmr += v.MMR;
            }
            mmr = (int)((float)mmr / (float)plrs.Count);
        }
        return mmr;
    }

    int AvgGamesPlayed(List<PvPPlayer> plrs)
    {
        int gp = 10;
        if (plrs.Count > 0)
        {
            foreach (var v in plrs)
            {
                gp += v.GamesPlayed;
            }
            gp = (int)((float)gp+1 / (float)plrs.Count);
        }
        return gp+1;
    }

    void UpdateMMRDB(int t1diff, int t2diff)
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("t1Diff", t1diff);
        values.Add("t2Diff", t2diff);
        List<string> t1un = new List<string>();
        List<string> t2un = new List<string>();
        foreach(var v in t1Users)
        {
            t1un.Add(v.username);
        }
        foreach (var v in t2Users)
        {
            t2un.Add(v.username);
        }
        values.Add("t1un", t1un.ToArray());
        values.Add("t2un", t2un.ToArray());
        /* TODO -- Create Cloud Code Function for MMR Updates
        ParseCloud.CallFunctionAsync<string>("UpdateMMR", values).ContinueWith(t =>
        {
            if (!t.IsFaulted && !t.IsCanceled)
            {
                Debug.Log("Updated ELO Values")
            }
            else
            {
                Debug.Log("Unsuccessful Request: " + t.Exception.InnerExceptions);
            }
        });
        */
    }

    float CalculateHandicap()
    {
        /*
        int OppMMR = (myTeam == PunTeams.Team.blue ? AvgMMR(t2Users) : AvgMMR(t1Users));
        int myMMR = 800;        
        if (ParseUser.CurrentUser.ContainsKey("MMR"))
            myMMR = ParseUser.CurrentUser.Get<int>("MMR");
        if(OppMMR > 100)
            return Mathf.Clamp(((float)OppMMR/myMMR), 0.25f, 3f);
        return 1;
        */

        var profile = PlayerProfile.Profile;
        int OppMMR = (myTeam == PunTeams.Team.blue ? AvgMMR(t2Users) : AvgMMR(t1Users));        
        if (OppMMR > 100)
            return Mathf.Clamp(((float)OppMMR / profile.MatchmakingRank), 0.25f, 3f);
        return 1;
    }

    public Health GetMyGate()
    {
        if (myTeam == PunTeams.Team.blue)
            return t1Health;
        else if (myTeam == PunTeams.Team.red)
            return t2Health;
        return null;
    }

    void SetLeaveOrbPosition()
    {
        Vector3 wp = PlayerHead.instance.transform.position + (PlayerHead.instance.transform.TransformDirection(Vector3.forward) * 0.5f);
        wp = new Vector3(wp.x, PlayerHead.instance.transform.position.y - 0.25f, wp.z);
        LeaveOrb.transform.position = Vector3.Lerp(LeaveOrb.position, wp, Time.unscaledDeltaTime * 2f);
        LeaveOrb.LookAt(PlayerHead.instance.transform.position);
    }
#endregion

#region Enemies

    public void SpawnEnemy(int enemyID, int elite = 0, int pid = -1)
    {
        if (PhotonNetwork.isMasterClient)
            NetworkSpawnEnemy((int)myTeam, enemyID, elite, pid);
        else
            v.RPC("NetworkSpawnEnemy", PhotonTargets.MasterClient, (int)myTeam, enemyID, elite, pid);
    }

    [PunRPC]
    void NetworkSpawnEnemy(int team, int enemyID, int elite=0, int pid=-1)
    {
        if ((PunTeams.Team)team == PunTeams.Team.blue)
            t1Stream.SpawnEnemy(enemyID, pid, elite, (PunTeams.Team)team);
        else
            t2Stream.SpawnEnemy(enemyID, pid, elite, (PunTeams.Team)team);
    }

    public float FindClosestEnemy(Vector3 pos)
    {
        float x = float.MaxValue;
        foreach (var v in CreatureManager.AllEnemies())
        {
            if (v != null)
            {
                float dist = Vector3.Distance(pos, v.transform.position);
                if (dist < x)
                    x = dist;
            }
        }
        return x;
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

    public void LoseEnemy(PunTeams.Team team)
    {
        if (team == PunTeams.Team.blue)
            BlueTeamEnm--;
        else if (team == PunTeams.Team.red)
            RedTeamEnm--;
    }

    public void AddEnm(PunTeams.Team team)
    {
        if (team == PunTeams.Team.blue)
            BlueTeamEnm++;
        else if (team == PunTeams.Team.red)
            RedTeamEnm++;
    }

    public int AllowedEnm()
    {
        allowedEnms = 0;
        if (myTeam == PunTeams.Team.red)
            allowedEnms =  EnemyBudget - RedTeamEnm;
        else if (myTeam == PunTeams.Team.blue)
            allowedEnms = EnemyBudget - BlueTeamEnm;
        return allowedEnms;
    }
#endregion

#region Networking

    void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        if (PhotonNetwork.isMasterClient && PlayingPVP)
        {
            Debug.Log("Player Number: " + PhotonNetwork.playerList.Length);
            if (PhotonNetwork.playerList.Length >= 2)
                StartGame();
        }
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer plr)
    {
        if(inGame)
        {
            EndGame();
        }
    }

    [PunRPC]
    void NetworkMMR(string username, int team, int MMR, int gamesPlayed)
    {
        PvPPlayer p = new PvPPlayer();
        p.username = username;
        p.team = (PunTeams.Team)team;
        p.MMR = MMR;
        p.GamesPlayed = gamesPlayed;
        if (p.team == PunTeams.Team.blue && !t1Users.Contains(p))
            t1Users.Add(p);
        else if(!t2Users.Contains(p))
            t2Users.Add(p);

    }
#endregion

    [System.Serializable]
    public class PvPPlayer
    {
        public string username;
        public int MMR;
        public int GamesPlayed;
        public PunTeams.Team team;

        public override bool Equals(object obj)
        {
            return obj.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            if(username != null)
                return username;
            return "";
        }
    }
}
