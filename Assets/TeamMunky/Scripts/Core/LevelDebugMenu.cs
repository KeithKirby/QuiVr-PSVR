using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDebugMenu : MonoBehaviour {

    public string[] SceneNames;

    void OnGUI()
    {
        for(int i=0;i< SceneNames.Length; ++i)
        {
            var s = SceneNames[i];
            if(GUI.Button(new Rect(10, 10 + i * 50, 120, 30), s))
            {
                SceneManager.LoadSceneAsync(i);
            }
        }
    }
}
