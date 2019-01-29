using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "InanEvin/RealisticHeadController/BobPreset")]
public class RHC_BobPresets : ScriptableObject {

    public string s_Name;
    public RHC_BobController.BobStyle m_BobStyle;
    public Vector3 v3_PosAmount;
    public Vector3 v3_PosSpeed;
    public Vector3 v3_RotAmount;
    public Vector3 v3_RotSpeed;
    public float f_PosSmooth;
    public float f_RotSmooth;
}
