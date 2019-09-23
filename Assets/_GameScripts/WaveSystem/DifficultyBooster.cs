using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DifficultyBooster : MonoBehaviour {

    public int Value = 1;
    public int GateNumber;
    public UnityEvent OnIncrease;

    public ScoreManager SPScore;
    public ScoreManager MPScore;

    [BitStrap.Button]
	public void IncreaseDifficulty()
    {
        if(GameBase.instance != null && GameBase.instance.Difficulty <= 0)
        {
            if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            {
                int myTopScore = SPScore.HighScore / 100;
                if (PhotonNetwork.inRoom)
                    myTopScore = MPScore.HighScore / 100;
                int setDiff = Mathf.Clamp(myTopScore - 10, 5, 50);
                Debug.Log("[Box Skip] Setting to Gate: " + setDiff + " (Top Score: " + myTopScore + ")");
                Boost(setDiff);
            }
            OnIncrease.Invoke();
        }
    }

    public void Boost(int difficulty)
    {
        StartCoroutine("DoBoost", difficulty);
    }

    IEnumerator DoBoost(int difficulty)
    {
        GameBase.instance.CurrentStream.RestoreGate.ForceClose();
        GameBase.instance.IncreaseDifficulty(((difficulty - 2) * 100));
        yield return true;
        yield return true;
        GameBase.instance.CurrentStream.RestoreGate.ForceRestore();
    }

    /*
    IEnumerator CloseGates()
    {
        for(int i=0; i<GateNumber; i++)
        {
            if (GameBase.instance.CurrentStream.RestoreGate.isDestroyed())
                GameBase.instance.CurrentStream.RestoreGate.ForceRestore();
            else if (!GameBase.instance.CurrentStream.RestoreGate.isClosed())
                GameBase.instance.CurrentStream.RestoreGate.ForceClose();
            yield return true;
            yield return true;
        }
    }
    */
}
