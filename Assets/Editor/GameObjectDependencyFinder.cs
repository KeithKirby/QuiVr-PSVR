using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class GameObjectDependencyFinder : EditorWindow
{
    static GameObject obj = null;

    [MenuItem("Utils/GameObjectDependencyFinder")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GameObjectDependencyFinder window = (GameObjectDependencyFinder)EditorWindow.GetWindow(typeof(GameObjectDependencyFinder));
        window.Show();
    }

    void OnGUI()
    {
        obj = EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Find Dependency", obj, typeof(GameObject)) as GameObject;

        if (obj)
        {
            Object[] roots = new Object[] { obj };

            if (GUI.Button(new Rect(3, 25, position.width - 6, 20), "Check Dependencies"))
                Selection.objects = EditorUtility.CollectDependencies(roots);


            if (GUI.Button(new Rect(3, 55, position.width - 6, 20), "Dump"))
            {
                var deps = EditorUtility.CollectDependencies(roots);
                foreach (var obj in deps)
                {
                    if (obj is UnityEngine.MonoBehaviour)
                    {
                        UnityEngine.MonoBehaviour mbox = (UnityEngine.MonoBehaviour)obj;
                        if (null != mbox)
                        {
                            var mb = mbox.transform;
                            var path = mb.name;
                            while (mb.parent != null)
                            {
                                path = mb.parent.name + "->" + path;
                                mb = mb.parent;
                            }
                            Debug.LogFormat("{0}", path);
                        }
                    }
                }
            }
        }
        else
            EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20), "Missing:", "Select an object first");
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
