using UnityEngine;
using System.Collections;
//using Parse;

public class SingleUseAnalytics : MonoBehaviour {

	public string Class;
	public string rowID;
	public string key;

	public bool SendOnStart;
	public bool RequireSingleplayer;
	public bool RequireMultiplayer;

#if !NOANALYTICS
	bool needSetKey;

	void Update()
	{
		if (needSetKey) {
			needSetKey = false;
			PlayerPrefs.SetInt(key, 1);
			PlayerPrefs.Save();
		}
	}

	// Use this for initialization
	void Start () {
		if (SendOnStart) {
			Send ();
		}
    }

    public void Send()
	{
        if ((PhotonNetwork.inRoom && RequireSingleplayer) || (!PhotonNetwork.inRoom && RequireMultiplayer))
			return;
		if (!PlayerPrefs.HasKey (key)) 
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery(Class);
            try
            {
                query.GetAsync(rowID).ContinueWith(t =>
                {
                    if (!t.IsFaulted && t.Exception == null)
                    {
                        ParseObject val = t.Result;
                        val.Increment(key);
                        val.SaveAsync();
                        needSetKey = true;
                    }
                });
            }
            catch
            {
                Debug.LogError("Failed to update anonymized analytics values");
            }
		}
	}
#endif
}
