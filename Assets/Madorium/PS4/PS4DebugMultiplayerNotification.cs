using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PS4DebugMultiplayerNotification : MonoBehaviour {

    public Text PlayingRealtime;
    public Text LoggedIn;
    public Text SignedIn;
    public Text MultiplePlayersActive;
    public Text GameplayActive;
    public Text ArcheryActive;

    public Text InGame;
    public Text Difficulty;

    // Update is called once per frame
    void Update ()
    {
        var plus = PS4Plus.Inst;
        if (null != plus)
        {
            PlayingRealtime.text = string.Format("IsPlayingRealtime: {0}", plus.IsPlayingRealtime);
            LoggedIn.text = string.Format("IsUserLoggedIn: {0}", plus.IsUserLoggedIn);
            SignedIn.text = string.Format("IsUserSignedIn: {0}", plus.IsUserSignedIn);
            MultiplePlayersActive.text = string.Format("IsMultiplePlayersActive: {0}", plus.IsMultiplePlayersActive);
            GameplayActive.text = string.Format("IsGamePlayActive: {0}", plus.IsGamePlayActive);
            ArcheryActive.text = string.Format("IsArcheryGameActive: {0}", plus.IsArcheryGameActive);
        }
        else
        {
            PlayingRealtime.text = string.Format("IsPlayingRealtime: Not initialised");
            LoggedIn.text = string.Format("IsUserLoggedIn: Not initialised");
            SignedIn.text = string.Format("IsUserSignedIn: Not initialised");
            MultiplePlayersActive.text = string.Format("IsMultiplePlayersActive: Not initialised");
            GameplayActive.text = string.Format("IsGamePlayActive: Not initialised");
            ArcheryActive.text = string.Format("IsArcheryGameActive: Not initialised");
        }

        var gameBase = GameBase.instance;
        if (null != gameBase)
        {
            InGame.text = string.Format("InGame: {0}", gameBase.inGame);
            Difficulty.text = string.Format("Difficulty: {0}", gameBase.Difficulty);
        }
        else
        {
            InGame.text = string.Format("InGame: Not initialised");
            Difficulty.text = string.Format("Difficulty: Not initialised");
        }
    }
}
