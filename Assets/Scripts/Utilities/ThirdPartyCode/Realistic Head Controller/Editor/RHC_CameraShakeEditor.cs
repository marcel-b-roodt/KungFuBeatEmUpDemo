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
 * Class Description: Editor class for IE_CameraShake class.
 *
 */

// Libraries
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RHC_CameraShake))]
public class RHC_CameraShakeEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Set target.
        RHC_CameraShake selfTarget = (RHC_CameraShake)target;
        serializedObject.Update();


        // Set script variable field.
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);


        // Set label style.
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.skin.label.fontStyle = FontStyle.Bold;

        // Hit Shake Settings
        selfTarget.b_RecoilShakeFoldout = EditorGUILayout.Foldout(selfTarget.b_RecoilShakeFoldout, new GUIContent("Recoil Shake"));
        if (selfTarget.b_RecoilShakeFoldout)
        {
            selfTarget.b_UseRecoilShake = EditorGUILayout.Toggle(new GUIContent("Use Recoil Shake"), selfTarget.b_UseRecoilShake);
            if (selfTarget.b_UseRecoilShake)
            {
                selfTarget.v3_RecoilShake_MinRot = EditorGUILayout.Vector3Field("Min Rotation", selfTarget.v3_RecoilShake_MinRot);
                selfTarget.v3_RecoilShake_MaxRot = EditorGUILayout.Vector3Field("Max Rotation", selfTarget.v3_RecoilShake_MaxRot);
                selfTarget.f_RecoilShake_UpSpeed = EditorGUILayout.FloatField("Up Speed", selfTarget.f_RecoilShake_UpSpeed);
                selfTarget.f_RecoilShake_DownSpeed = EditorGUILayout.FloatField("Down Speed", selfTarget.f_RecoilShake_DownSpeed);
                selfTarget.b_RecoilShake_UseIncreasingPower = EditorGUILayout.Toggle(new GUIContent("Use Increasing Power"), selfTarget.b_RecoilShake_UseIncreasingPower);

                if (selfTarget.b_RecoilShake_UseIncreasingPower)
                {
                    EditorGUI.indentLevel++;
                    selfTarget.f_RecoilShake_StartPower = EditorGUILayout.FloatField("Start Power", selfTarget.f_RecoilShake_StartPower);
                    selfTarget.f_RecoilShake_EndPower = EditorGUILayout.FloatField("End Power", selfTarget.f_RecoilShake_EndPower);
                    selfTarget.f_RecoilShake_Acceleration = EditorGUILayout.Slider("Acceleration", selfTarget.f_RecoilShake_Acceleration, 0.0f, 20.0f);
                    selfTarget.f_RecoilShake_Deceleration = EditorGUILayout.Slider("Deceleration", selfTarget.f_RecoilShake_Deceleration, 0.0f, 20.0f);
                    EditorGUI.indentLevel--;
                }
            }
        }

        // Generic Shake Settings
        selfTarget.b_GenericShakeFoldout = EditorGUILayout.Foldout(selfTarget.b_GenericShakeFoldout, new GUIContent("Generic Shakes"));
        if (selfTarget.b_GenericShakeFoldout)
        {
            selfTarget.b_UseGenericShake = EditorGUILayout.Toggle(new GUIContent("Use Generic Shake"), selfTarget.b_UseGenericShake);

            if (selfTarget.b_UseGenericShake)
            {
                selfTarget.f_GenericShakeSmooth = EditorGUILayout.FloatField("Shake Smooth", selfTarget.f_GenericShakeSmooth);

                if (selfTarget.f_GenericShakeSmooth == 0.0f)
                    EditorGUILayout.HelpBox("Shake Smooth is 0! No movement will be observed!", MessageType.Warning);

                selfTarget.f_GenericShakeResetSpeed = EditorGUILayout.FloatField("Reset Speed", selfTarget.f_GenericShakeResetSpeed);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add New Shake Type"))
                {
                    RHC_CameraShake.RHC_GenericShakeData genericShake = new RHC_CameraShake.RHC_GenericShakeData();
                    selfTarget._GenericShakes.Add(genericShake);
                    genericShake.s_Name = "New Shake " + selfTarget._GenericShakes.Count;
                }
                if (GUILayout.Button("Remove Last Shake Type"))
                {
                    if (selfTarget._GenericShakes.Count != 0)
                        selfTarget._GenericShakes.RemoveAt(selfTarget._GenericShakes.Count - 1);
                }
                if (GUILayout.Button("Reset List"))
                    selfTarget._GenericShakes.Clear();

                EditorGUILayout.EndHorizontal();

                selfTarget.b_DistanceShakeListEnabled = EditorGUILayout.Foldout(selfTarget.b_DistanceShakeListEnabled, new GUIContent("Registered Shake Types"));
                if (selfTarget.b_DistanceShakeListEnabled)
                {
                    if (selfTarget._GenericShakes.Count == 0)
                        EditorGUILayout.HelpBox("No Shakes are added yet!", MessageType.Info);
                    else
                    {
                        EditorGUI.indentLevel++;
                        for (int x = 0; x < selfTarget._GenericShakes.Count; x++)
                        {
                            selfTarget._GenericShakes[x].b_FoldoutActive = EditorGUILayout.Foldout(selfTarget._GenericShakes[x].b_FoldoutActive, new GUIContent(selfTarget._GenericShakes[x].s_Name));
                            if (selfTarget._GenericShakes[x].b_FoldoutActive)
                            {
                                EditorGUILayout.LabelField(selfTarget._GenericShakes[x].s_Name, EditorStyles.boldLabel);
                                selfTarget._GenericShakes[x].s_Name = EditorGUILayout.TextField("Name: ", selfTarget._GenericShakes[x].s_Name);
                                selfTarget._GenericShakes[x].v3_PosAmount = EditorGUILayout.Vector3Field("Position Amounts", selfTarget._GenericShakes[x].v3_PosAmount);
                                selfTarget._GenericShakes[x].v3_PosSpeed = EditorGUILayout.Vector3Field("Position Speeds", selfTarget._GenericShakes[x].v3_PosSpeed);
                                EditorGUILayout.Space();
                                selfTarget._GenericShakes[x].v3_RotAmount = EditorGUILayout.Vector3Field("Rotation Amounts", selfTarget._GenericShakes[x].v3_RotAmount);
                                selfTarget._GenericShakes[x].v3_RotSpeed = EditorGUILayout.Vector3Field("Rotation Speeds", selfTarget._GenericShakes[x].v3_RotSpeed);
                                EditorGUILayout.Space();
                                selfTarget._GenericShakes[x].f_ShakeTime = EditorGUILayout.FloatField("Shake Time", selfTarget._GenericShakes[x].f_ShakeTime);
                                selfTarget._GenericShakes[x].f_ApplyDistance = EditorGUILayout.FloatField("Apply Distance", selfTarget._GenericShakes[x].f_ApplyDistance);
                                selfTarget._GenericShakes[x]._Preset = EditorGUILayout.ObjectField("Preset", selfTarget._GenericShakes[x]._Preset, typeof(RHC_GenericShakePreset), true) as RHC_GenericShakePreset;

                                if (selfTarget._GenericShakes[x]._Preset != null && selfTarget._GenericShakes[x]._Preset != selfTarget._GenericShakes[x]._RecordedPreset)
                                {
                                    selfTarget._GenericShakes[x]._RecordedPreset = selfTarget._GenericShakes[x]._Preset;
                                    selfTarget._GenericShakes[x].s_Name = selfTarget._GenericShakes[x]._Preset.s_Name;
                                    selfTarget._GenericShakes[x].v3_PosAmount = selfTarget._GenericShakes[x]._Preset.v3_PosAmount;
                                    selfTarget._GenericShakes[x].v3_PosSpeed = selfTarget._GenericShakes[x]._Preset.v3_PosSpeed;
                                    selfTarget._GenericShakes[x].v3_RotAmount = selfTarget._GenericShakes[x]._Preset.v3_RotAmount;
                                    selfTarget._GenericShakes[x].v3_RotSpeed = selfTarget._GenericShakes[x]._Preset.v3_RotSpeed;
                                    selfTarget._GenericShakes[x].f_ShakeTime = selfTarget._GenericShakes[x]._Preset.f_ShakeTime;
                                    selfTarget._GenericShakes[x].f_ApplyDistance = selfTarget._GenericShakes[x]._Preset.f_ApplyDistance;
                                }
                            

                                EditorGUILayout.BeginHorizontal();

                                if (GUILayout.Button(new GUIContent("Apply To Preset")) && selfTarget._GenericShakes[x]._Preset)
                                {
                                    selfTarget._GenericShakes[x]._Preset.s_Name = selfTarget._GenericShakes[x].s_Name;
                                    selfTarget._GenericShakes[x]._Preset.v3_PosAmount = selfTarget._GenericShakes[x].v3_PosAmount;
                                    selfTarget._GenericShakes[x]._Preset.v3_PosSpeed = selfTarget._GenericShakes[x].v3_PosSpeed;
                                    selfTarget._GenericShakes[x]._Preset.v3_RotAmount = selfTarget._GenericShakes[x].v3_RotAmount;
                                    selfTarget._GenericShakes[x]._Preset.v3_RotSpeed = selfTarget._GenericShakes[x].v3_RotSpeed;
                                    selfTarget._GenericShakes[x]._Preset.f_ShakeTime = selfTarget._GenericShakes[x].f_ShakeTime;
                                    selfTarget._GenericShakes[x]._Preset.f_ApplyDistance = selfTarget._GenericShakes[x].f_ApplyDistance;
                                    AssetDatabase.SaveAssets();
                                    EditorUtility.SetDirty(selfTarget._GenericShakes[x]._Preset);
                                }

                                if (GUILayout.Button(new GUIContent("Revert To Preset")) && selfTarget._GenericShakes[x]._Preset)
                                {
                                    selfTarget._GenericShakes[x].s_Name = selfTarget._GenericShakes[x]._Preset.s_Name;
                                    selfTarget._GenericShakes[x].v3_PosAmount = selfTarget._GenericShakes[x]._Preset.v3_PosAmount;
                                    selfTarget._GenericShakes[x].v3_PosSpeed = selfTarget._GenericShakes[x]._Preset.v3_PosSpeed;
                                    selfTarget._GenericShakes[x].v3_RotAmount = selfTarget._GenericShakes[x]._Preset.v3_RotAmount;
                                    selfTarget._GenericShakes[x].v3_RotSpeed = selfTarget._GenericShakes[x]._Preset.v3_RotSpeed;
                                    selfTarget._GenericShakes[x].f_ShakeTime = selfTarget._GenericShakes[x]._Preset.f_ShakeTime;
                                    selfTarget._GenericShakes[x].f_ApplyDistance = selfTarget._GenericShakes[x]._Preset.f_ApplyDistance;
                                }

                                if (GUILayout.Button(new GUIContent("Remove This")))
                                    selfTarget._GenericShakes.RemoveAt(x);

                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUI.indentLevel++;
                    }
                }

            }
        }
        EditorUtility.SetDirty(selfTarget);
        serializedObject.ApplyModifiedProperties();
    }

    // Returns new struct according to the preset. 
    RHC_CameraShake.RHC_GenericShakeData GetPreset(int index)
    {
        RHC_CameraShake.RHC_GenericShakeData preset = new RHC_CameraShake.RHC_GenericShakeData();

        // Short Distance Explosion 
        if (index == 1)
        {
            preset.s_Name = "Short Distance Explosion";
            preset.v3_PosAmount = new Vector3(0.05f, 0.2f, 0.02f);
            preset.v3_PosSpeed = new Vector3(4, 7, 4);
            preset.v3_RotAmount = new Vector3(5, 0, 5);
            preset.v3_RotSpeed = new Vector3(8, 0, 16);
            preset.f_ApplyDistance = 15.0f;
            preset.f_ShakeTime = 3.5f;
            preset.i_PresetChoiceIndex = index;
        }

        return preset;
    }
}
