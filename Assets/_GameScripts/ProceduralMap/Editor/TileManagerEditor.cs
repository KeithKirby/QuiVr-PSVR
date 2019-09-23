using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BitStrap;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : ReorderableArrayEditor {

    private ButtonAttributeHelper helper = new ButtonAttributeHelper();
    bool ViewEditor = true;
    bool ViewRuntime = true;

	void OnEnable()
    {
        useReorderableArrays = true;
        useNestedReorderableArrays = true;
        helper.Init(target);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TileManager targ = (TileManager)target;
        GUIStyle guiStyle = EditorStyles.foldout;
        FontStyle previousStyle = guiStyle.fontStyle;
        guiStyle.fontStyle = FontStyle.Bold;
        ViewEditor = EditorGUILayout.Foldout(ViewEditor, "Editor Options", guiStyle);
        if (ViewEditor)
        {
            EditorGUI.indentLevel++;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("Tiles"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TileDB"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SkyDB"));
            DrawProperty(serializedObject.FindProperty("ConnectingPaths"), false);
            DrawProperty(serializedObject.FindProperty("StartZoneTPs"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("noNavBuilding"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BaseGate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BaseStream"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BaseGateTP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("PathDiffByDist"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EventTileFreq"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FrontCap"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BackCap"));
            EditorGUI.indentLevel--;
        }
        ViewRuntime = EditorGUILayout.Foldout(ViewRuntime, "Runtime Values", guiStyle);
        if (ViewRuntime)
        {
            EditorGUI.indentLevel++;
            bool inf = EditorGUILayout.PropertyField(serializedObject.FindProperty("Infinite"));
            if(!inf)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Gates"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CurrentSeed"));
            DrawProperty(serializedObject.FindProperty("CurrentTiles"), false);
            DrawProperty(serializedObject.FindProperty("CurrentStreams"), false);
            DrawProperty(serializedObject.FindProperty("CombinedPaths"), false);
            DrawProperty(serializedObject.FindProperty("CurrentGates"), false);
            DrawProperty(serializedObject.FindProperty("VisibilityZones"), false);
            helper.DrawButtons();
            EditorGUI.indentLevel--;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
