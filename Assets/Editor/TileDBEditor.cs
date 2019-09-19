using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Rotorz.ReorderableList;

[CustomEditor(typeof(TileSet))]
public class TileDBEditor : Editor {

    SerializedProperty links;
    SerializedProperty gates;
    SerializedProperty endcaps;

    void OnEnable()
    {
        links = serializedObject.FindProperty("Links");
        gates = serializedObject.FindProperty("Gates");
        endcaps = serializedObject.FindProperty("EndCaps");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ReorderableListGUI.Title("Links");
        ReorderableListGUI.ListField(links);
        serializedObject.ApplyModifiedProperties();
    }
}
