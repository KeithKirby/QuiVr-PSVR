using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrackedPlayStationDevices))]
[CanEditMultipleObjects]
public class TrackedPlayStationDevicesEditor : Editor
{
    SerializedProperty trackedDualShock4Transform;
    SerializedProperty trackedAimTransform;
    SerializedProperty trackedMoveTransformPrimary;
    SerializedProperty trackedMoveTransformSecondary;
    SerializedProperty trackedDualShock4Light;
    SerializedProperty trackedAimLight;
    SerializedProperty trackedMoveLightPrimary;
    SerializedProperty trackedMoveLightSecondary;

    SerializedProperty trackingType;
    SerializedProperty trackerUsageType;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        trackedDualShock4Transform = serializedObject.FindProperty("deviceDualShock4").FindPropertyRelative("transform");
        trackedAimTransform = serializedObject.FindProperty("deviceAim").FindPropertyRelative("transform");
        trackedMoveTransformPrimary = serializedObject.FindProperty("deviceMovePrimary").FindPropertyRelative("transform");
        trackedMoveTransformSecondary = serializedObject.FindProperty("deviceMoveSecondary").FindPropertyRelative("transform");
        trackedDualShock4Light = serializedObject.FindProperty("deviceDualShock4").FindPropertyRelative("light");
        trackedAimLight = serializedObject.FindProperty("deviceAim").FindPropertyRelative("light");
        trackedMoveLightPrimary = serializedObject.FindProperty("deviceMovePrimary").FindPropertyRelative("light");
        trackedMoveLightSecondary = serializedObject.FindProperty("deviceMoveSecondary").FindPropertyRelative("light");
        trackingType = serializedObject.FindProperty("trackingType");
        trackerUsageType = serializedObject.FindProperty("trackerUsageType");
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();
        TrackedPlayStationDevices script = (TrackedPlayStationDevices)target;

        script.trackedDevicesType = (TrackedPlayStationDevices.TrackedDevicesType)EditorGUILayout.EnumMaskPopup(new GUIContent("Trackable Devices"), script.trackedDevicesType);
        TrackedPlayStationDevices.TrackedDevicesType deviceTypes = script.trackedDevicesType;

        EditorGUILayout.PropertyField(trackingType, new GUIContent("Tracking Type"));
        EditorGUILayout.PropertyField(trackerUsageType, new GUIContent("Tracker Usage Type"));

        if ((deviceTypes & TrackedPlayStationDevices.TrackedDevicesType.DualShock4) == TrackedPlayStationDevices.TrackedDevicesType.DualShock4)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("DualShock 4 Controller", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(trackedDualShock4Transform, new GUIContent("Transform"));
            EditorGUILayout.PropertyField(trackedDualShock4Light, new GUIContent("Light Renderer"));
        }

        if ((deviceTypes & TrackedPlayStationDevices.TrackedDevicesType.Aim) == TrackedPlayStationDevices.TrackedDevicesType.Aim)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("PS VR Aim Controller", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(trackedAimTransform, new GUIContent("Transform"));
            EditorGUILayout.PropertyField(trackedAimLight, new GUIContent("Light Renderer"));
        }

        if ((deviceTypes & TrackedPlayStationDevices.TrackedDevicesType.Move) == TrackedPlayStationDevices.TrackedDevicesType.Move)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Move Controller (Primary)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(trackedMoveTransformPrimary, new GUIContent("Transform"));
            EditorGUILayout.PropertyField(trackedMoveLightPrimary, new GUIContent("Light Renderer"));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Move Controller (Secondary)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(trackedMoveTransformSecondary, new GUIContent("Transform"));
            EditorGUILayout.PropertyField(trackedMoveLightSecondary, new GUIContent("Light Renderer"));
        }

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }
}
