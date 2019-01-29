/**
 * Realistic Head Controller
 * Copyright (c) 2017-Present, Inan Evin, Inc. All rights reserved.
 * Author: Inan Evin
 * www.inanevin.com
 * contact: inanevin@gmail.com
 * live chat: https://discord.gg/eZAz3KW
 *
 * Feel free to ask about the package and talk about recommendations!
 *
 *
 * Class Description: Custom editor class for the IE_BobController class.
 *
 */

// Libraries
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RHC_BobController))]
public class RHC_BobControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Set target.
        RHC_BobController selfTarget = (RHC_BobController)target;
        serializedObject.Update();

        // Set target's variable field.
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);

        // Declare & init properties.
        SerializedProperty selectSettings = serializedObject.FindProperty("_settings");
        SerializedProperty WalkBobStyle = serializedObject.FindProperty("_WalkBobStyle");
        SerializedProperty RunBobStyle = serializedObject.FindProperty("_RunBobStyle");
        SerializedProperty CrouchWalkBobStyle = serializedObject.FindProperty("_CrouchWalkBobStyle");
        SerializedProperty CrouchRunBobStyle = serializedObject.FindProperty("_CrouchRunBobStyle");
        SerializedProperty CrawlSlowBobStyle = serializedObject.FindProperty("_CrawlSlowBobStyle");
        SerializedProperty CrawlFastBobStyle = serializedObject.FindProperty("_CrawlFastBobStyle");

        /*SerializedProperty WalkBobPreset = serializedObject.FindProperty("_WalkBobPreset");
        SerializedProperty RunBobPreset = serializedObject.FindProperty("_RunBobPreset");
        SerializedProperty CrouchWalkBobPreset = serializedObject.FindProperty("_CrouchWalkBobPreset");
        SerializedProperty CrouchRunBobPreset = serializedObject.FindProperty("_CrouchRunBobPreset");
        SerializedProperty CrawlSlowBobPreset = serializedObject.FindProperty("_CrawlSlowBobPreset");
        SerializedProperty CrawlFastBobPreset = serializedObject.FindProperty("_CrawlFastBobPreset");*/

        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        selfTarget.f_ResetSpeed = EditorGUILayout.FloatField("Reset Speed", selfTarget.f_ResetSpeed);

        EditorGUILayout.LabelField("Bob Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select to Edit", EditorStyles.label);
        EditorGUILayout.PropertyField(selectSettings, GUIContent.none, true);
        EditorGUILayout.EndHorizontal();

        // Standing & walking settings.
        if (selfTarget._settings == RHC_BobController.BobSettings.WalkSettings)
        {

            // Preset selection.
            selfTarget._WalkBobPreset = EditorGUILayout.ObjectField("Preset", selfTarget._WalkBobPreset, typeof(RHC_BobPresets), true) as RHC_BobPresets;

            // If a new preset is selected, set the variables.
            if (selfTarget._WalkBobPreset != selfTarget._RecordedWalkBobPreset)
            {
                selfTarget._RecordedWalkBobPreset = selfTarget._WalkBobPreset;
                RHC_BobPresets selectedPreset = selfTarget._WalkBobPreset;
                selfTarget._WalkBobStyle = selectedPreset.m_BobStyle;
                selfTarget.v3_WALK_POS_Amounts = selectedPreset.v3_PosAmount;
                selfTarget.v3_WALK_POS_Speed = selectedPreset.v3_PosSpeed;
                selfTarget.v3_WALK_ROT_Amounts = selectedPreset.v3_RotAmount;
                selfTarget.v3_WALK_ROT_Speed = selectedPreset.v3_RotSpeed;
                selfTarget.f_WALK_POS_Smooth = selectedPreset.f_PosSmooth;
                selfTarget.f_WALK_ROT_Smooth = selectedPreset.f_RotSmooth;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bob Style", EditorStyles.label);
            EditorGUILayout.PropertyField(WalkBobStyle, GUIContent.none, true);
            EditorGUILayout.EndHorizontal();

            if (selfTarget._WalkBobStyle == RHC_BobController.BobStyle.OnlyPosition)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_WALK_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_WALK_POS_Amounts);
                selfTarget.v3_WALK_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_WALK_POS_Speed);
                selfTarget.f_WALK_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_WALK_POS_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._WalkBobStyle == RHC_BobController.BobStyle.OnlyRotation)
            {
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_WALK_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_WALK_ROT_Amounts);
                selfTarget.v3_WALK_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_WALK_ROT_Speed);
                selfTarget.f_WALK_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_WALK_ROT_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._WalkBobStyle == RHC_BobController.BobStyle.Both)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_WALK_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_WALK_POS_Amounts);
                selfTarget.v3_WALK_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_WALK_POS_Speed);
                selfTarget.f_WALK_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_WALK_POS_Smooth);
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_WALK_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_WALK_ROT_Amounts);
                selfTarget.v3_WALK_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_WALK_ROT_Speed);
                selfTarget.f_WALK_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_WALK_ROT_Smooth);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent("Apply To Preset")) && selfTarget._WalkBobPreset)
            {
                selfTarget._WalkBobPreset.v3_PosAmount = selfTarget.v3_WALK_POS_Amounts;
                selfTarget._WalkBobPreset.v3_PosSpeed = selfTarget.v3_WALK_POS_Speed;
                selfTarget._WalkBobPreset.v3_RotAmount = selfTarget.v3_WALK_ROT_Amounts;
                selfTarget._WalkBobPreset.v3_RotSpeed = selfTarget.v3_WALK_ROT_Speed;
                selfTarget._WalkBobPreset.f_PosSmooth = selfTarget.f_WALK_POS_Smooth;
                selfTarget._WalkBobPreset.f_RotSmooth = selfTarget.f_WALK_ROT_Smooth;

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(selfTarget._WalkBobPreset);
            }

            if (GUILayout.Button(new GUIContent("Revert To Preset")) && selfTarget._WalkBobPreset)
            {
                selfTarget.v3_WALK_POS_Amounts = selfTarget._WalkBobPreset.v3_PosAmount;
                selfTarget.v3_WALK_POS_Speed = selfTarget._WalkBobPreset.v3_PosSpeed;
                selfTarget.v3_WALK_ROT_Amounts = selfTarget._WalkBobPreset.v3_RotAmount;
                selfTarget.v3_WALK_ROT_Speed = selfTarget._WalkBobPreset.v3_RotSpeed;
                selfTarget.f_WALK_POS_Smooth = selfTarget._WalkBobPreset.f_PosSmooth;
                selfTarget.f_WALK_ROT_Smooth = selfTarget._WalkBobPreset.f_RotSmooth;
            }
        }
        // Standing & running settings.
        else if (selfTarget._settings == RHC_BobController.BobSettings.RunSettings)
        {
            // Preset selection.
            selfTarget._RunBobPreset = EditorGUILayout.ObjectField("Preset", selfTarget._RunBobPreset, typeof(RHC_BobPresets), true) as RHC_BobPresets;

            // If a new preset is selected, set the variables.
            if (selfTarget._RunBobPreset != selfTarget._RecordedRunBobPreset)
            {
                selfTarget._RecordedRunBobPreset = selfTarget._RunBobPreset;
                RHC_BobPresets selectedPreset = selfTarget._RunBobPreset;
                selfTarget._RunBobStyle = selectedPreset.m_BobStyle;
                selfTarget.v3_RUN_POS_Amounts = selectedPreset.v3_PosAmount;
                selfTarget.v3_RUN_POS_Speed = selectedPreset.v3_PosSpeed;
                selfTarget.v3_RUN_ROT_Amounts = selectedPreset.v3_RotAmount;
                selfTarget.v3_RUN_ROT_Speed = selectedPreset.v3_RotSpeed;
                selfTarget.f_RUN_POS_Smooth = selectedPreset.f_PosSmooth;
                selfTarget.f_RUN_ROT_Smooth = selectedPreset.f_RotSmooth;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bob Style", EditorStyles.label);
            EditorGUILayout.PropertyField(RunBobStyle, GUIContent.none, true);
            EditorGUILayout.EndHorizontal();

            if (selfTarget._RunBobStyle == RHC_BobController.BobStyle.OnlyPosition)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_RUN_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_RUN_POS_Amounts);
                selfTarget.v3_RUN_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_RUN_POS_Speed);
                selfTarget.f_RUN_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_RUN_POS_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._RunBobStyle == RHC_BobController.BobStyle.OnlyRotation)
            {
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_RUN_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_RUN_ROT_Amounts);
                selfTarget.v3_RUN_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_RUN_ROT_Speed);
                selfTarget.f_RUN_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_RUN_ROT_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._RunBobStyle == RHC_BobController.BobStyle.Both)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_RUN_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_RUN_POS_Amounts);
                selfTarget.v3_RUN_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_RUN_POS_Speed);
                selfTarget.f_RUN_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_RUN_POS_Smooth);

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_RUN_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_RUN_ROT_Amounts);
                selfTarget.v3_RUN_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_RUN_ROT_Speed);
                selfTarget.f_RUN_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_RUN_ROT_Smooth);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent("Apply To Preset")) && selfTarget._RunBobPreset)
            {
                selfTarget._RunBobPreset.v3_PosAmount = selfTarget.v3_RUN_POS_Amounts;
                selfTarget._RunBobPreset.v3_PosSpeed = selfTarget.v3_RUN_POS_Speed;
                selfTarget._RunBobPreset.v3_RotAmount = selfTarget.v3_RUN_ROT_Amounts;
                selfTarget._RunBobPreset.v3_RotSpeed = selfTarget.v3_RUN_ROT_Speed;
                selfTarget._RunBobPreset.f_PosSmooth = selfTarget.f_RUN_POS_Smooth;
                selfTarget._RunBobPreset.f_RotSmooth = selfTarget.f_RUN_ROT_Smooth;

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(selfTarget._RunBobPreset);
            }

            if (GUILayout.Button(new GUIContent("Revert To Preset")) && selfTarget._RunBobPreset)
            {
                selfTarget.v3_RUN_POS_Amounts = selfTarget._RunBobPreset.v3_PosAmount;
                selfTarget.v3_RUN_POS_Speed = selfTarget._RunBobPreset.v3_PosSpeed;
                selfTarget.v3_RUN_ROT_Amounts = selfTarget._RunBobPreset.v3_RotAmount;
                selfTarget.v3_RUN_ROT_Speed = selfTarget._RunBobPreset.v3_RotSpeed;
                selfTarget.f_RUN_POS_Smooth = selfTarget._RunBobPreset.f_PosSmooth;
                selfTarget.f_RUN_ROT_Smooth = selfTarget._RunBobPreset.f_RotSmooth;
            }

        }
        // Crouching & walking settings.
        else if (selfTarget._settings == RHC_BobController.BobSettings.CrouchWalkSettings)
        {
            // Preset selection.
            selfTarget._CrouchWalkBobPreset = EditorGUILayout.ObjectField("Preset", selfTarget._CrouchWalkBobPreset, typeof(RHC_BobPresets), true) as RHC_BobPresets;

            // If a new preset is selected, set the variables.
            if (selfTarget._CrouchWalkBobPreset != selfTarget._RecordedCrouchWalkBobPreset)
            {
                selfTarget._RecordedCrouchWalkBobPreset = selfTarget._CrouchWalkBobPreset;
                RHC_BobPresets selectedPreset = selfTarget._CrouchWalkBobPreset;
                selfTarget._CrouchWalkBobStyle = selectedPreset.m_BobStyle;
                selfTarget.v3_CRCW_POS_Amounts = selectedPreset.v3_PosAmount;
                selfTarget.v3_CRCW_POS_Speed = selectedPreset.v3_PosSpeed;
                selfTarget.v3_CRCW_ROT_Amounts = selectedPreset.v3_RotAmount;
                selfTarget.v3_CRCW_ROT_Speed = selectedPreset.v3_RotSpeed;
                selfTarget.f_CRCW_POS_Smooth = selectedPreset.f_PosSmooth;
                selfTarget.f_CRCW_ROT_Smooth = selectedPreset.f_RotSmooth;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bob Style", EditorStyles.label);
            EditorGUILayout.PropertyField(CrouchWalkBobStyle, GUIContent.none, true);
            EditorGUILayout.EndHorizontal();

            if (selfTarget._CrouchWalkBobStyle == RHC_BobController.BobStyle.OnlyPosition)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCW_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCW_POS_Amounts);
                selfTarget.v3_CRCW_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCW_POS_Speed);
                selfTarget.f_CRCW_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCW_POS_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._CrouchWalkBobStyle == RHC_BobController.BobStyle.OnlyRotation)
            {
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCW_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCW_ROT_Amounts);
                selfTarget.v3_CRCW_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCW_ROT_Speed);
                selfTarget.f_CRCW_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCW_ROT_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._CrouchWalkBobStyle == RHC_BobController.BobStyle.Both)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCW_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCW_POS_Amounts);
                selfTarget.v3_CRCW_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCW_POS_Speed);
                selfTarget.f_CRCW_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCW_POS_Smooth);

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCW_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCW_ROT_Amounts);
                selfTarget.v3_CRCW_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCW_ROT_Speed);
                selfTarget.f_CRCW_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCW_ROT_Smooth);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent("Apply To Preset")) && selfTarget._CrouchWalkBobPreset)
            {
                selfTarget._CrouchWalkBobPreset.v3_PosAmount = selfTarget.v3_CRCW_POS_Amounts;
                selfTarget._CrouchWalkBobPreset.v3_PosSpeed = selfTarget.v3_CRCW_POS_Speed;
                selfTarget._CrouchWalkBobPreset.v3_RotAmount = selfTarget.v3_CRCW_ROT_Amounts;
                selfTarget._CrouchWalkBobPreset.v3_RotSpeed = selfTarget.v3_CRCW_ROT_Speed;
                selfTarget._CrouchWalkBobPreset.f_PosSmooth = selfTarget.f_CRCW_POS_Smooth;
                selfTarget._CrouchWalkBobPreset.f_RotSmooth = selfTarget.f_CRCW_ROT_Smooth;

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(selfTarget._CrouchWalkBobPreset);
            }

            if (GUILayout.Button(new GUIContent("Revert To Preset")) && selfTarget._CrouchWalkBobPreset)
            {
                selfTarget.v3_CRCW_POS_Amounts = selfTarget._CrouchWalkBobPreset.v3_PosAmount;
                selfTarget.v3_CRCW_POS_Speed = selfTarget._CrouchWalkBobPreset.v3_PosSpeed;
                selfTarget.v3_CRCW_ROT_Amounts = selfTarget._CrouchWalkBobPreset.v3_RotAmount;
                selfTarget.v3_CRCW_ROT_Speed = selfTarget._CrouchWalkBobPreset.v3_RotSpeed;
                selfTarget.f_CRCW_POS_Smooth = selfTarget._CrouchWalkBobPreset.f_PosSmooth;
                selfTarget.f_CRCW_ROT_Smooth = selfTarget._CrouchWalkBobPreset.f_RotSmooth;
            }
        }
        // Crouching & running settings.
        else if (selfTarget._settings == RHC_BobController.BobSettings.CrouchRunSettings)
        {
            // Preset selection.
            selfTarget._CrouchRunBobPreset = EditorGUILayout.ObjectField("Preset", selfTarget._CrouchRunBobPreset, typeof(RHC_BobPresets), true) as RHC_BobPresets;

            // If a new preset is selected, set the variables.
            if (selfTarget._CrouchRunBobPreset != selfTarget._RecordedCrouchRunBobPreset)
            {
                selfTarget._RecordedCrouchRunBobPreset = selfTarget._CrouchRunBobPreset;
                RHC_BobPresets selectedPreset = selfTarget._CrouchRunBobPreset;
                selfTarget._CrouchRunBobStyle = selectedPreset.m_BobStyle;
                selfTarget.v3_CRCR_POS_Amounts = selectedPreset.v3_PosAmount;
                selfTarget.v3_CRCR_POS_Speed = selectedPreset.v3_PosSpeed;
                selfTarget.v3_CRCR_ROT_Amounts = selectedPreset.v3_RotAmount;
                selfTarget.v3_CRCR_ROT_Speed = selectedPreset.v3_RotSpeed;
                selfTarget.f_CRCR_POS_Smooth = selectedPreset.f_PosSmooth;
                selfTarget.f_CRCR_ROT_Smooth = selectedPreset.f_RotSmooth;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bob Style", EditorStyles.label);
            EditorGUILayout.PropertyField(CrouchRunBobStyle, GUIContent.none, true);
            EditorGUILayout.EndHorizontal();

            if (selfTarget._CrouchRunBobStyle == RHC_BobController.BobStyle.OnlyPosition)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCR_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCR_POS_Amounts);
                selfTarget.v3_CRCR_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCR_POS_Speed);
                selfTarget.f_CRCR_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCR_POS_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._CrouchRunBobStyle == RHC_BobController.BobStyle.OnlyRotation)
            {
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCR_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCR_ROT_Amounts);
                selfTarget.v3_CRCR_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCR_ROT_Speed);
                selfTarget.f_CRCR_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCR_ROT_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._CrouchRunBobStyle == RHC_BobController.BobStyle.Both)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCR_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCR_POS_Amounts);
                selfTarget.v3_CRCR_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCR_POS_Speed);
                selfTarget.f_CRCR_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCR_POS_Smooth);

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRCR_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRCR_ROT_Amounts);
                selfTarget.v3_CRCR_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRCR_ROT_Speed);
                selfTarget.f_CRCR_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRCR_ROT_Smooth);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent("Apply To Preset")) && selfTarget._CrouchRunBobPreset)
            {
                selfTarget._CrouchRunBobPreset.v3_PosAmount = selfTarget.v3_WALK_POS_Amounts;
                selfTarget._CrouchRunBobPreset.v3_PosSpeed = selfTarget.v3_WALK_POS_Speed;
                selfTarget._CrouchRunBobPreset.v3_RotAmount = selfTarget.v3_WALK_ROT_Amounts;
                selfTarget._CrouchRunBobPreset.v3_RotSpeed = selfTarget.v3_WALK_ROT_Speed;
                selfTarget._CrouchRunBobPreset.f_PosSmooth = selfTarget.f_WALK_POS_Smooth;
                selfTarget._CrouchRunBobPreset.f_RotSmooth = selfTarget.f_WALK_ROT_Smooth;

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(selfTarget._CrouchRunBobPreset);
            }

            if (GUILayout.Button(new GUIContent("Revert To Preset")) && selfTarget._CrouchRunBobPreset)
            {
                selfTarget.v3_CRCR_POS_Amounts = selfTarget._CrouchRunBobPreset.v3_PosAmount;
                selfTarget.v3_CRCR_POS_Speed = selfTarget._CrouchRunBobPreset.v3_PosSpeed;
                selfTarget.v3_CRCR_ROT_Amounts = selfTarget._CrouchRunBobPreset.v3_RotAmount;
                selfTarget.v3_CRCR_ROT_Speed = selfTarget._CrouchRunBobPreset.v3_RotSpeed;
                selfTarget.f_CRCR_POS_Smooth = selfTarget._CrouchRunBobPreset.f_PosSmooth;
                selfTarget.f_CRCR_ROT_Smooth = selfTarget._CrouchRunBobPreset.f_RotSmooth;
            }
        }
        // Proning & walking settings.
        else if (selfTarget._settings == RHC_BobController.BobSettings.CrawlSlowSettings)
        {
            // Preset selection.
            selfTarget._CrawlSlowBobPreset = EditorGUILayout.ObjectField("Preset", selfTarget._CrawlSlowBobPreset, typeof(RHC_BobPresets), true) as RHC_BobPresets;

            // If a new preset is selected, set the variables.
            if (selfTarget._CrawlSlowBobPreset != selfTarget._RecordedCrawlSlowBobPreset)
            {
                selfTarget._RecordedCrawlSlowBobPreset = selfTarget._CrawlSlowBobPreset;
                RHC_BobPresets selectedPreset = selfTarget._CrawlSlowBobPreset;
                selfTarget._CrawlSlowBobStyle = selectedPreset.m_BobStyle;
                selfTarget.v3_CRAWLS_POS_Amounts = selectedPreset.v3_PosAmount;
                selfTarget.v3_CRAWLS_POS_Speed = selectedPreset.v3_PosSpeed;
                selfTarget.v3_CRAWLS_ROT_Amounts = selectedPreset.v3_RotAmount;
                selfTarget.v3_CRAWLS_ROT_Speed = selectedPreset.v3_RotSpeed;
                selfTarget.f_CRAWLS_POS_Smooth = selectedPreset.f_PosSmooth;
                selfTarget.f_CRAWLS_ROT_Smooth = selectedPreset.f_RotSmooth;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bob Style", EditorStyles.label);
            EditorGUILayout.PropertyField(CrawlSlowBobStyle, GUIContent.none, true);
            EditorGUILayout.EndHorizontal();

            if (selfTarget._WalkBobStyle == RHC_BobController.BobStyle.OnlyPosition)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLS_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLS_POS_Amounts);
                selfTarget.v3_CRAWLS_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLS_POS_Speed);
                selfTarget.f_CRAWLS_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLS_POS_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._WalkBobStyle == RHC_BobController.BobStyle.OnlyRotation)
            {
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLS_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLS_ROT_Amounts);
                selfTarget.v3_CRAWLS_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLS_ROT_Speed);
                selfTarget.f_CRAWLS_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLS_ROT_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._WalkBobStyle == RHC_BobController.BobStyle.Both)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLS_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLS_POS_Amounts);
                selfTarget.v3_CRAWLS_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLS_POS_Speed);
                selfTarget.f_CRAWLS_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLS_POS_Smooth);

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLS_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLS_ROT_Amounts);
                selfTarget.v3_CRAWLS_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLS_ROT_Speed);
                selfTarget.f_CRAWLS_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLS_ROT_Smooth);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent("Apply To Preset")) && selfTarget._CrawlSlowBobPreset)
            {
                selfTarget._CrawlSlowBobPreset.v3_PosAmount = selfTarget.v3_CRAWLS_POS_Amounts;
                selfTarget._CrawlSlowBobPreset.v3_PosSpeed = selfTarget.v3_CRAWLS_POS_Speed;
                selfTarget._CrawlSlowBobPreset.v3_RotAmount = selfTarget.v3_CRAWLS_ROT_Amounts;
                selfTarget._CrawlSlowBobPreset.v3_RotSpeed = selfTarget.v3_CRAWLS_ROT_Speed;
                selfTarget._CrawlSlowBobPreset.f_PosSmooth = selfTarget.f_CRAWLS_POS_Smooth;
                selfTarget._CrawlSlowBobPreset.f_RotSmooth = selfTarget.f_CRAWLS_ROT_Smooth;

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(selfTarget._CrawlSlowBobPreset);
            }

            if (GUILayout.Button(new GUIContent("Revert To Preset")) && selfTarget._CrawlSlowBobPreset)
            {
                selfTarget.v3_CRAWLS_POS_Amounts = selfTarget._CrawlSlowBobPreset.v3_PosAmount;
                selfTarget.v3_CRAWLS_POS_Speed = selfTarget._CrawlSlowBobPreset.v3_PosSpeed;
                selfTarget.v3_CRAWLS_ROT_Amounts = selfTarget._CrawlSlowBobPreset.v3_RotAmount;
                selfTarget.v3_CRAWLS_ROT_Speed = selfTarget._CrawlSlowBobPreset.v3_RotSpeed;
                selfTarget.f_CRAWLS_POS_Smooth = selfTarget._CrawlSlowBobPreset.f_PosSmooth;
                selfTarget.f_CRAWLS_ROT_Smooth = selfTarget._CrawlSlowBobPreset.f_RotSmooth;
            }
        }
        // Proning & running settings.
        else if (selfTarget._settings == RHC_BobController.BobSettings.CrawlFastSettings)
        {
            // Preset selection.
            selfTarget._CrawlFastBobPreset = EditorGUILayout.ObjectField("Preset", selfTarget._CrawlFastBobPreset, typeof(RHC_BobPresets), true) as RHC_BobPresets;

            // If a new preset is selected, set the variables.
            if (selfTarget._CrawlFastBobPreset != selfTarget._RecordedCrawlFastBobPreset)
            {
                selfTarget._RecordedCrawlFastBobPreset = selfTarget._CrawlFastBobPreset;
                RHC_BobPresets selectedPreset = selfTarget._CrawlFastBobPreset;
                selfTarget._CrawlFastBobStyle = selectedPreset.m_BobStyle;
                selfTarget.v3_CRAWLF_POS_Amounts = selectedPreset.v3_PosAmount;
                selfTarget.v3_CRAWLF_POS_Speed = selectedPreset.v3_PosSpeed;
                selfTarget.v3_CRAWLF_ROT_Amounts = selectedPreset.v3_RotAmount;
                selfTarget.v3_CRAWLF_ROT_Speed = selectedPreset.v3_RotSpeed;
                selfTarget.f_CRAWLF_POS_Smooth = selectedPreset.f_PosSmooth;
                selfTarget.f_CRAWLF_ROT_Smooth = selectedPreset.f_RotSmooth;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bob Style", EditorStyles.label);
            EditorGUILayout.PropertyField(CrawlFastBobStyle, GUIContent.none, true);
            EditorGUILayout.EndHorizontal();

            if (selfTarget._CrawlFastBobStyle == RHC_BobController.BobStyle.OnlyPosition)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLF_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLF_POS_Amounts);
                selfTarget.v3_CRAWLF_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLF_POS_Speed);
                selfTarget.f_CRAWLF_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLF_POS_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._CrawlFastBobStyle == RHC_BobController.BobStyle.OnlyRotation)
            {
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLF_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLF_ROT_Amounts);
                selfTarget.v3_CRAWLF_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLF_ROT_Speed);
                selfTarget.f_CRAWLF_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLF_ROT_Smooth);
                EditorGUI.indentLevel--;
            }
            else if (selfTarget._CrawlFastBobStyle == RHC_BobController.BobStyle.Both)
            {
                EditorGUILayout.LabelField("Position");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLF_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLF_POS_Amounts);
                selfTarget.v3_CRAWLF_POS_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLF_POS_Speed);
                selfTarget.f_CRAWLF_POS_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLF_POS_Smooth);

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Rotation");
                EditorGUI.indentLevel++;
                selfTarget.v3_CRAWLF_ROT_Amounts = EditorGUILayout.Vector3Field("Amounts", selfTarget.v3_CRAWLF_ROT_Amounts);
                selfTarget.v3_CRAWLF_ROT_Speed = EditorGUILayout.Vector3Field("Speed", selfTarget.v3_CRAWLF_ROT_Speed);
                selfTarget.f_CRAWLF_ROT_Smooth = EditorGUILayout.FloatField("Smooth", selfTarget.f_CRAWLF_ROT_Smooth);
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent("Apply To Preset")) && selfTarget._CrawlFastBobPreset)
            {
                selfTarget._CrawlFastBobPreset.v3_PosAmount = selfTarget.v3_CRAWLF_POS_Amounts;
                selfTarget._CrawlFastBobPreset.v3_PosSpeed = selfTarget.v3_CRAWLF_POS_Speed;
                selfTarget._CrawlFastBobPreset.v3_RotAmount = selfTarget.v3_CRAWLF_ROT_Amounts;
                selfTarget._CrawlFastBobPreset.v3_RotSpeed = selfTarget.v3_CRAWLF_ROT_Speed;
                selfTarget._CrawlFastBobPreset.f_PosSmooth = selfTarget.f_CRAWLF_POS_Smooth;
                selfTarget._CrawlFastBobPreset.f_RotSmooth = selfTarget.f_CRAWLF_ROT_Smooth;

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(selfTarget._CrawlFastBobPreset);
            }

            if (GUILayout.Button(new GUIContent("Revert To Preset")) && selfTarget._CrawlFastBobPreset)
            {
                selfTarget.v3_CRAWLF_POS_Amounts = selfTarget._CrawlFastBobPreset.v3_PosAmount;
                selfTarget.v3_CRAWLF_POS_Speed = selfTarget._CrawlFastBobPreset.v3_PosSpeed;
                selfTarget.v3_CRAWLF_ROT_Amounts = selfTarget._CrawlFastBobPreset.v3_RotAmount;
                selfTarget.v3_CRAWLF_ROT_Speed = selfTarget._CrawlFastBobPreset.v3_RotSpeed;
                selfTarget.f_CRAWLF_POS_Smooth = selfTarget._CrawlFastBobPreset.f_PosSmooth;
                selfTarget.f_CRAWLF_ROT_Smooth = selfTarget._CrawlFastBobPreset.f_RotSmooth;
            }
        }

        // Save & Apply.
        EditorUtility.SetDirty(selfTarget);
        serializedObject.ApplyModifiedProperties();
    }

    void VO_ApplyToPreset(RHC_BobPresets preset)
    {

        EditorUtility.SetDirty(preset);
    }

}
