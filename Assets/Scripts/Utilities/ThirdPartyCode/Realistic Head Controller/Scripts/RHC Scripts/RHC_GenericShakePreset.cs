using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "InanEvin/RealisticHeadController/GenericShakePreset")]
public class RHC_GenericShakePreset : ScriptableObject {

    public string s_Name;                           // Name of this shake type.
    public Vector3 v3_PosAmount;                    // Position shake.
    public Vector3 v3_PosSpeed;                     // Position shake speed.
    public Vector3 v3_RotAmount;                    // Rotation shake.
    public Vector3 v3_RotSpeed;                     // Rotation shake speed.
    public float f_ShakeTime;                       // Shake time.
    public float f_ApplyDistance;                   // If the distance to a particular Generic is smaller than this variable's value, then this Generic would be applied. Overrides the ones with higher distance value.
}
