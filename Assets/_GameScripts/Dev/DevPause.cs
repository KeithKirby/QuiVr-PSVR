using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor;
#endif

public class DevPause : MonoBehaviour {

#if UNITY_EDITOR
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Equals))
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
    }
 #endif

}
