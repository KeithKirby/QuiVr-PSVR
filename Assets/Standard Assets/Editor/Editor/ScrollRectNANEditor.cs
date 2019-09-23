#if UNITY_EDITOR && UNITY_5_6 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class ScrollRectNANEditor : EditorWindow
{
    static ScrollRectNANEditor _window;

    [MenuItem("Window/ScrollRect (5.6) Bug Fixer/Open Window #F9")]
    public static void Open()
    {
        if (_window != null)
            EditorWindow.GetWindow<ScrollRectNANEditor>().Close();
        //Show existing window instance. If one doesn't exist, make one. 
        _window = EditorWindow.GetWindow<ScrollRectNANEditor>("NAN Fixer", true, typeof(SceneView)) as ScrollRectNANEditor;
    }

    [MenuItem("Window/ScrollRect (5.6) Bug Fixer/Fix Now")]
    public static void FixNow()
    {
        Open();
        _window.FixBug();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Fix Now", EditorStyles.toolbarButton))
            FixBug();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
    }

    void FixBug()
    {
        if (_window == null)
        {
            FixNow();
            return;
        }

        ScrollRect[] scrolls = FindObjectsOfTypeAll<ScrollRect>();
        //ScrollRect[] scrolls = FindObjectsOfType(typeof(ScrollRect)) as ScrollRect[];	// Doesn't allow for inactive gameobjects. 
        if (scrolls == null || scrolls.Length == 0)
        {
            if (_window != null)
                _window.ShowNotification(new GUIContent("No ScrollRects have been found."));
        }
        else
        {
            for (int i = 0; i < scrolls.Length; i++)
                scrolls[i].enabled = false;

            for (int i = 0; i < scrolls.Length; i++)
            {
                ScrollRect scroll = scrolls[i];
                if (scroll.movementType == ScrollRect.MovementType.Clamped)
                {
                    scroll.movementType = ScrollRect.MovementType.Elastic;
                    scroll.elasticity = 0;
                }
                if (scroll.content != null)
                    scroll.content.anchoredPosition = FixIfNaN(scroll.content.anchoredPosition);
            }

            for (int i = 0; i < scrolls.Length; i++)
                scrolls[i].enabled = true;

            if (_window != null)
                _window.ShowNotification(new GUIContent(scrolls.Length + " instances have been successfully fixed ;)"));
        }
    }

    public static T[] FindObjectsOfTypeAll<T>()
    {
        List<T> results = new List<T>();
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            var s = EditorSceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    results.AddRange(go.GetComponentsInChildren<T>(true));
                }
            }
        }
        return results.ToArray();
    }

    Vector2 FixIfNaN(Vector2 v)
    {
        if(float.IsNaN(v.x))
            v.x = 0;
        if (float.IsNaN(v.y))
            v.y = 0;
        return v;
    }

    Vector3 FixIfNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
            v.x = 0;
        if (float.IsNaN(v.y))
            v.y = 0;
        if (float.IsNaN(v.z))
            v.z = 0;
        return v;
    }
}
#endif