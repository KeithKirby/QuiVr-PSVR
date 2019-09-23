using UnityEngine;

public class EloRating
{
    /// 
    /// Updates the scores in the passed matchup. 
    /// 
    /// The Matchup to update
    /// Whether User 1 was the winner (false if User 2 is the winner)
    /// The desired Diff
    /// The desired KFactor
    /// 
    public static void UpdateScores(Matchup matchup, bool user1WonMatch, int winningPlayerMatchCount)
    {
        double kFactor = GetKFactor(user1WonMatch ? matchup.User1Score : matchup.User2Score, 25, winningPlayerMatchCount);
        double est1 = 1 / System.Convert.ToDouble(1 + 10 ^ (matchup.User2Score - matchup.User1Score) / 25);
        double est2 = 1 / System.Convert.ToDouble(1 + 10 ^ (matchup.User1Score - matchup.User2Score) / 25);
        int sc1 = 0;
        int sc2 = 0;
        if (user1WonMatch)
            sc1 = 1;
        else
            sc2 = 1;
        Debug.Log((user1WonMatch ? "P1" : "P2") + " won the match");
        matchup.User1Score = (int)(System.Math.Round(matchup.User1Score + kFactor * (sc1 - System.Math.Abs(est1))));
        matchup.User2Score = (int)(System.Math.Round(matchup.User2Score + kFactor * (sc2 - System.Math.Abs(est2))));
        Debug.Log("New ELO: P1 - " + matchup.User1Score + " :: P2 - " + matchup.User2Score);
    }

    static double GetKFactor(int winnerRating, int scoreDifference, int winningPlayerMatchCount)
    {
        var kFactor = 32.0;

        if (winningPlayerMatchCount >= 11 && winningPlayerMatchCount < 21)
            kFactor = kFactor * .75;
        else if (winningPlayerMatchCount < 11)
            kFactor = kFactor * .5;
        return kFactor + scoreDifference;
    }

    /// 
    /// Updates the scores in the match, using default Diff and KFactors (400, 100)
    /// 
    /// The Matchup to update
    /// Whether User 1 was the winner (false if User 2 is the winner)
   // public static void UpdateScores(Matchup matchup, bool user1WonMatch)
    //{
        //UpdateScores(matchup, user1WonMatch, 100, 10);
    //}

    public class Matchup
    {
        public int User1Score { get; set; }
        public int User2Score { get; set; }
    }

}