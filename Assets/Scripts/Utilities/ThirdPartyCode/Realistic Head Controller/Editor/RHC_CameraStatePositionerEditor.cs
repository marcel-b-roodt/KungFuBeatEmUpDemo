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
 * Class Description: Custom editor class for IE_CameraStatePositioner class.
 *
 */

// Libraries
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RHC_CameraStatePositioner))]
public class RHC_CameraStatePositionerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Set target.
        RHC_CameraStatePositioner selfTarget = (RHC_CameraStatePositioner)target;
        serializedObject.Update();

        // Set target's variable field.
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);

        // Air transform settings.
        EditorGUILayout.LabelField("Air Transform", EditorStyles.boldLabel);
        selfTarget.v3_AirPosition = EditorGUILayout.Vector3Field("Air Position", selfTarget.v3_AirPosition);
        selfTarget.v3_AirRotation = EditorGUILayout.Vector3Field("Air Rotation", selfTarget.v3_AirRotation);

        // Air down transform settings.
        EditorGUILayout.LabelField("Air Down Transform", EditorStyles.boldLabel);
        selfTarget.v3_AirDownPosition = EditorGUILayout.Vector3Field("Air Down Position", selfTarget.v3_AirDownPosition);
        selfTarget.v3_AirDownRotation = EditorGUILayout.Vector3Field("Air Down Rotation", selfTarget.v3_AirDownRotation);
        selfTarget.f_GoDownFromAirPosSpeed = EditorGUILayout.FloatField("Go Down Position Speed", selfTarget.f_GoDownFromAirPosSpeed);
        selfTarget.f_GoDownFromAirRotSpeed = EditorGUILayout.FloatField("Go Down Rotation Speed", selfTarget.f_GoDownFromAirRotSpeed);

        // Crouching transform settings.
        EditorGUILayout.LabelField("Crouching Transform", EditorStyles.boldLabel);
        selfTarget.v3_CrouchingPosition = EditorGUILayout.Vector3Field("Crouch Position", selfTarget.v3_CrouchingPosition);
        selfTarget.v3_CrouchingRotation = EditorGUILayout.Vector3Field("Crouch Rotation", selfTarget.v3_CrouchingRotation);

        // Crawling transform settings.
        EditorGUILayout.LabelField("Crawling Transform", EditorStyles.boldLabel);
        selfTarget.v3_CrawlingPosition = EditorGUILayout.Vector3Field("Crawl Position", selfTarget.v3_CrawlingPosition);
        selfTarget.v3_CrawlingRotation = EditorGUILayout.Vector3Field("Crawl Rotation", selfTarget.v3_CrawlingRotation);

        // Speeds of interpolations.
        EditorGUILayout.LabelField("Speeds", EditorStyles.boldLabel);
        selfTarget.f_PosInterpolationSpeed = EditorGUILayout.Slider("Position Interpolation", selfTarget.f_PosInterpolationSpeed, 0.0f, 10.0f);
        selfTarget.f_RotInterpolationSpeed = EditorGUILayout.Slider("Rotation Interpolation", selfTarget.f_RotInterpolationSpeed, 0.0f, 10.0f);

        // Saving
        GUILayout.Label("Save");
        
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Air Transform"))
        {
            selfTarget.v3_AirPosition = selfTarget.transform.localPosition;
            selfTarget.v3_AirRotation = selfTarget.transform.localEulerAngles;
        }
        if (GUILayout.Button("Air Down Transform"))
        {
            selfTarget.v3_AirDownPosition = selfTarget.transform.localPosition;
            selfTarget.v3_AirDownRotation = selfTarget.transform.localEulerAngles;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Crouch Transform"))
        {
            selfTarget.v3_CrouchingPosition = selfTarget.transform.localPosition;
            selfTarget.v3_CrouchingRotation = selfTarget.transform.localEulerAngles;
        }
        if (GUILayout.Button("Crawl Transform"))
        {
            selfTarget.v3_CrawlingPosition = selfTarget.transform.localPosition;
            selfTarget.v3_CrawlingRotation = selfTarget.transform.localEulerAngles;
        }
        if (GUILayout.Button("Default Transform"))
        {
            selfTarget.v3_DefaultPosition = selfTarget.transform.localPosition;
            selfTarget.v3_DefaultRotation = selfTarget.transform.localEulerAngles;
        }

        GUILayout.EndHorizontal();

        // Loading
        GUILayout.Label("Load");

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Air Transform"))
        {
            selfTarget.transform.localPosition = selfTarget.v3_AirPosition;
            selfTarget.transform.localEulerAngles = selfTarget.v3_AirRotation;
        }

        if (GUILayout.Button("Air Down Transform"))
        {
            selfTarget.transform.localPosition = selfTarget.v3_AirDownPosition;
            selfTarget.transform.localEulerAngles = selfTarget.v3_AirDownRotation;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Crouch Transform"))
        {
            selfTarget.transform.localPosition = selfTarget.v3_CrouchingPosition;
            selfTarget.transform.localEulerAngles = selfTarget.v3_CrouchingRotation;
        }
        if (GUILayout.Button("Crawl Transform"))
        {
            selfTarget.transform.localPosition = selfTarget.v3_CrawlingPosition;
            selfTarget.transform.localEulerAngles = selfTarget.v3_CrawlingRotation;
        }
        if (GUILayout.Button("Default Transform"))
        {
            selfTarget.transform.localPosition = selfTarget.v3_DefaultPosition;
            selfTarget.transform.localEulerAngles = selfTarget.v3_DefaultRotation;
        }

        GUILayout.EndHorizontal();

        // Save & Apply.
        EditorUtility.SetDirty(selfTarget);
        serializedObject.ApplyModifiedProperties();
    }
}
