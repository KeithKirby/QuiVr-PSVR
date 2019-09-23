using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class FriendList : MonoBehaviour {

    public Text msgText;

    public GameObject friendPrefab;
    public GameObject listHolder;
    List<GameObject> friendObjects;

    PlatformSetup _plat;

    void Start()
    {
        friendObjects = new List<GameObject>();
        _plat = PlatformSetup.instance;
    }

	public void UpdateList()
    {
        if (_plat == null)
        {
            msgText.text = "Platform Error";
            return;
        }
        msgText.text = "Getting Friends List";
        ClearList();
        StopCoroutine("DoListUpdate");
        StartCoroutine("DoListUpdate");
    }

    IEnumerator DoListUpdate()
    {
        if(_plat.UpdateFriendsList())
        {
            while (!_plat.hasUpdatedFriends())
            {
                yield return true;
            }
            foreach (var v in _plat.Friends)
            {
                AddObject(v);
            }
            msgText.text = "";
        }
        else
        {
            if (PhotonNetwork.inRoom)
                msgText.text = "Can't get friends while in multiplayer";
            else
                msgText.text = "Error getting friends";
        }
        yield return true;
        GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }

    public void AddObject(Friend fInfo)
    {
        GameObject newObj = (GameObject)Instantiate(friendPrefab);
        newObj.transform.SetParent(listHolder.transform);
        newObj.transform.localPosition = Vector3.zero;
        newObj.transform.localScale = Vector3.one;
        newObj.transform.localEulerAngles = Vector3.zero;
        newObj.GetComponent<FriendUI>().Init(fInfo);
        friendObjects.Add(newObj);
    }

    void ClearList()
    {
        for(int i=0; i<friendObjects.Count; i++)
        {
            Destroy(friendObjects[i]);
        }
        friendObjects = new List<GameObject>();
    }


}
