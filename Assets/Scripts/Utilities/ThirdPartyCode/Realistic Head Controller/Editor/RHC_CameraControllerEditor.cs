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
 * Class Description: Custom editor class for IE_CameraController class.
 *
 */

// Libraries
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RHC_CameraController))]
public class RHC_CameraControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Set target.
        RHC_CameraController selfTarget = (RHC_CameraController)target;
        serializedObject.Update();

        // Set target's variable field.
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);

        selfTarget.b_RotatesPlayer = EditorGUILayout.Toggle(new GUIContent("Rotate Player"), selfTarget.b_RotatesPlayer);
        if (selfTarget.b_RotatesPlayer)
        {
            EditorGUI.indentLevel++;
            selfTarget.t_Player = EditorGUILayout.ObjectField("Player Transform", selfTarget.t_Player, typeof(Transform), true) as Transform;
            EditorGUI.indentLevel--;
        }

        selfTarget.v2_Sensitivity = EditorGUILayout.Vector2Field("Sensitivity", selfTarget.v2_Sensitivity);
        selfTarget.v2_Smooth = EditorGUILayout.Vector2Field("Smooth", selfTarget.v2_Smooth);
        selfTarget.f_MinY = EditorGUILayout.Slider("Minimum Y", selfTarget.f_MinY, -360.0f, 360.0f);
        selfTarget.f_MaxY = EditorGUILayout.Slider("Maximum Y", selfTarget.f_MaxY, -360.0f, 360.0f);

        // Save & Apply.
        EditorUtility.SetDirty(selfTarget);
        serializedObject.ApplyModifiedProperties();
    }

}
