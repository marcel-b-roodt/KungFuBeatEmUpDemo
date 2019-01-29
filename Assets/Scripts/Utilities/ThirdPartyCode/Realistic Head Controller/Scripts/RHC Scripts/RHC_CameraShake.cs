/**
 * Realistic Head Controller
 * Copyright (c) 2017-Present, Inan Evin, Inc. All rights reserved.
 * Author: Inan Evin
 * https://www.inanevin.com/realistic-head-controller
 * contact: inanevin@gmail.com
 * live chat: https://discord.gg/eZAz3KW
 *
 * Feel free to ask about the package and talk about recommendations!
 *
 *
 * Class Description:
 *
 * This class is used to control shake the head orientation. It has 2 features:
 * - Hit Shake
 * - Generic Shake
 *
 * Hit Shake: It is used to recoil the head when the player is hit somehow, or even to simulate firing recoil. Hit event would be fired from RHC_EventManager by calling the static method PSVO_CallRecoil() from anywhere. After that, 
 * head would be manipulated according to the variables under Hit Shake Foldout.
 *
 * Generic Shake: It is used to shake the head when a near Generic occurs. It can be used to simulate explosions, earthquakes, even hit effects. Manipulation is pretty easy. Shake events would be fired
 * from RHC_EventManager by calling the static method PSVO_CallGenericShake(Vector3 v). It takes a Vector3 type of variable as a parameter, to indicate the source position of the Generic. According to the
 * source position, method in this class calculates the distance to the position, and decides what kind of generic shake to be applied.
 * You can add/remove generic shakes from the inspector!
 *
 * Shakes are done using Mathf.Sin function. So that, for example if you set the shake variable of an effect as Vector3(5,10,2), then the head will shake between x: -5 & 5, y: -10 & 10, z: -2 & 2 according to the other tweakable
 * variables such as power or speed.
 *
 */


// Libraries
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RHC_CameraShake : MonoBehaviour
{
    #region Variable(s)

    /* GENERAL VARIABLES */

    private Transform t_This;                           // This transform.
    private Vector3 v3_InitialLocalPos;                 // Initial local position of this transform.
    private Vector3 v3_InitialLocalROT;                 // Initial local ROTer of this transform.
    private Coroutine co_ResetHeadOrientation;          // Coroutine used to reset the head after shakes.
    // List of shakes that were registered from the inspector.
    public List<RHC_GenericShakeData> _GenericShakes = new List<RHC_GenericShakeData>();
    public bool b_DistanceShakeListEnabled;                     // Logic flag used in the Editor script of this class to foldin/foldout the hit shake list.

    /* Recoil SHAKE VARIABLES */

    public bool b_RecoilShakeFoldout;                      // Logic flag used in the Editor script of this class to foldin/foldout the hit shake settings.
    public bool b_UseRecoilShake;                          // Will hit shake effect be used?
    public Vector3 v3_RecoilShake_MinRot;                  // Minimum hit shake rotation.                
    public Vector3 v3_RecoilShake_MaxRot;                  // Maximum hit shake rotation.
    public float f_RecoilShake_UpSpeed;                    // Speed used to interpolate TO the calculated hit shake rotation.
    public float f_RecoilShake_DownSpeed;                  // Speed used to interpolate FROM the calculated hit shake rotation to initial rotation.
    public bool b_RecoilShake_UseIncreasingPower;          // If this is true, when the player gets multiple hits, each hit will increase the shake power, thus head will shake more according to the power variables.
    public float f_RecoilShake_StartPower;                 // If increasing power is enabled, then the power will start from this value, and climb up to the end value as the player keeps getting hit.
    public float f_RecoilShake_EndPower;                   // If increasing power is enabled, then the power will start from start value and climb up to this value as the player keeps getting hit.
    public float f_RecoilShake_Acceleration;               // If increasing power is enabled, power will climb up using this acceleration value as the player keeps getting hit.
    public float f_RecoilShake_Deceleration;               // If increasing power is enabled, power will climb down using this acceleration value when the player stops getting hit.
    private float f_RecoilShake_InitialStartPower;         // If increasing power is enabled, this value is used to record initial start power, so that we know where to lerp once player stops getting hit.
    private Coroutine co_RecoilShake;                      // Coroutine used for hit shake function.
    private Coroutine co_ResetRecoilShakePower;            // Coroutine used to reset the hit shake power if increasing power on Hit Shake Settings is enabled.

    /* Distance SHAKE VARIABLES */

    public bool b_GenericShakeFoldout;                   // Logic flag used in the Editor script of this class to foldin/foldout the Distance shake settings.
    public bool b_UseGenericShake;                       // Will Generic shake effect be used?
    public float f_GenericShakeSmooth = 0.2f;            // Smoothing variable used for interpolating the head according to the shakes.         
    public float f_GenericShakeResetSpeed;               // Speed used to reset the head orientation after any Distance shake has finished interpolating.

    /* Generic SHAKE VARIABLES */

    private Coroutine co_SineShake;                     // Coroutine used for Generic shakes & Generic shake function.

    // Data struct for Generic shakes.
    [System.SerializableAttribute]
    public class RHC_GenericShakeData
    {
        public string s_Name;                           // Name of this shake type.
        public Vector3 v3_PosAmount;                    // Position shake.
        public Vector3 v3_RotAmount;                    // Rotation shake.
        public Vector3 v3_PosSpeed;                     // Position shake speed.
        public Vector3 v3_RotSpeed;                     // Rotation shake speed.
        public float f_ShakeTime;                       // Shake time.
        public float f_ApplyDistance;                   // If the distance to a particular Generic is smaller than this variable's value, then this Generic would be applied. Overrides the ones with higher distance value.
        public int i_PresetChoiceIndex;                 // Which preset does this shake type use? 0: NoPreset
        public bool b_FoldoutActive;                    // Logic flag used in the Editor script of this class to foldin/foldout a particular Generic hit shake.
        public RHC_GenericShakePreset _Preset;         // Preset for Generic shake.
        public RHC_GenericShakePreset _RecordedPreset;

    }
    #endregion

    #region Initializer(s)

    void Awake()
    {
        // Initialize variables.
        f_RecoilShake_InitialStartPower = f_RecoilShake_StartPower;
        t_This = transform;
        v3_InitialLocalPos = t_This.localPosition;
        v3_InitialLocalROT = t_This.localEulerAngles;
    }

    void OnEnable()
    {
        // If hit shake is to be used, subscribe it's method to the event in RHC_EventManager.
        if (b_UseRecoilShake)
            RHC_EventManager.recoilShake += EVO_Recoil;

        // If Generic shake is to be used, subscribe it's method to the event in RHC_EventManager.
        if (b_UseGenericShake)
        {
            RHC_EventManager.genericShake += EVO_GenericShake;
            RHC_EventManager.customShake += EVO_CustomShake;
            RHC_EventManager.customShakePreset += EVO_CustomShakePreset;
        }
    }

    #endregion

    #region Coroutine(s)

    /// <summary> Coroutine used for resetting the head orientation after any kind of shake effect had finished. </summary>
    IEnumerator CO_ResetHeadOrientation(float speed)
    {
        // Record current position & rotation to be used inside Vector3.Lerp & Quaternion.Slerp.
        Vector3 currentPosition = t_This.localPosition;
        Quaternion currentRot = t_This.localRotation;

        float rate = speed;

        /* Calculate average difference of magnitude to the initial head orientation from current orientation in order to determine constant speed to be used in while loop.
        float posDiff = (v3_InitialLocalPos - currentPosition).magnitude;
        float rotDiff = (v3_InitialLocalROT - t_This.localROTerAngles).magnitude;
        float avgDiff = (Mathf.Abs(posDiff) + Mathf.Abs(rotDiff)) / 2;
        rate *= avgDiff */

        float i = 0.0f;

        // Interpolate the head to it's initial orientation.
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            t_This.localPosition = Vector3.Lerp(currentPosition, v3_InitialLocalPos, i);
            t_This.localRotation = Quaternion.Slerp(currentRot, Quaternion.Euler(v3_InitialLocalROT), i);
            yield return null;
        }
    }

    /// <summary> Coroutine used for generic shakes. </summary>
    IEnumerator CO_SineShake(Vector3 posAmount, Vector3 posSpeed, Vector3 rotAmount, Vector3 rotSpeed, float time)
    {
        float i = 0.0f;
        float power = 1.0f;
        // Record current head orientation to use inside Vector3.Lerp & Quaternion.Slerp.
        Vector3 currentPos = t_This.localPosition;
        Vector3 currentROTer = t_This.localEulerAngles;
        while (i < time)
        {
            i += Time.deltaTime;
            power = (time - i) / time;

            // Declare & init Sine variables, they are added on the recorded head orientation.   
            Vector3 posSine = currentPos + new Vector3(Mathf.Sin(Time.time * posSpeed.x) * posAmount.x, Mathf.Sin(Time.time * posSpeed.y) * posAmount.y, Mathf.Sin(Time.time * posSpeed.z) * posAmount.z) * power;
            Vector3 rotSine = currentROTer + new Vector3(Mathf.Sin(Time.time * rotSpeed.x) * rotAmount.x, Mathf.Sin(Time.time * rotSpeed.y) * rotAmount.y, Mathf.Sin(Time.time * rotSpeed.z) * rotAmount.z) * power;

            // Interpolate the head.
            t_This.localPosition = Vector3.Lerp(t_This.localPosition, posSine, Time.deltaTime * f_GenericShakeSmooth);
            t_This.localRotation = Quaternion.Slerp(t_This.localRotation, Quaternion.Euler(rotSine), Time.deltaTime * f_GenericShakeSmooth);
            yield return null;
        }

        // If reset head orientation coroutine is in present, meaning triggered somehow, stop it and start a new one.
        if (co_ResetHeadOrientation != null)
            StopCoroutine(co_ResetHeadOrientation);

        co_ResetHeadOrientation = StartCoroutine(CO_ResetHeadOrientation(f_GenericShakeResetSpeed));
    }

    /// <summary> Coroutine used for Recoil shake. </summary>
    private IEnumerator CO_RecoilShake(Vector3 targetRot)
    {
        float i = 0.0f;
        // Record initial rotation to use in the Quaternion.Slerp.
        Quaternion currentRot = t_This.localRotation;

        // Interpolate.
        while (i < 1.0f)
        {
            i += Time.deltaTime * f_RecoilShake_UpSpeed;
            t_This.localRotation = Quaternion.Slerp(currentRot, Quaternion.Euler(targetRot), i);
            yield return null;
        }

        // If increasing power is enabled, start decreasing the power after an hit. ( It would have been increased before getting into this coroutine. )
        if (b_RecoilShake_UseIncreasingPower)
        {
            if (co_ResetRecoilShakePower != null)
                StopCoroutine(co_ResetRecoilShakePower);

            co_ResetRecoilShakePower = StartCoroutine(CO_ResetRecoilShakePower());
        }

        // If reset head orientation coroutine is in present, meaning triggered somehow, stop it and start a new one.
        if (co_ResetHeadOrientation != null)
            StopCoroutine(co_ResetHeadOrientation);

        co_ResetHeadOrientation = StartCoroutine(CO_ResetHeadOrientation(f_RecoilShake_DownSpeed));
    }

    /// <summary> Coroutine used for resetting the hit shake power which would have been increased just before interpolating the head. </summary>
    private IEnumerator CO_ResetRecoilShakePower()
    {
        float i = 0.0f;
        // Record initial power to use in the Mathf.Lerp
        float current = f_RecoilShake_StartPower;

        // Interpolate.
        while (i < 1.0f)
        {
            i += Time.deltaTime * f_RecoilShake_Deceleration;
            f_RecoilShake_StartPower = Mathf.Lerp(current, f_RecoilShake_InitialStartPower, i);
            yield return null;
        }

    }

    #endregion

    #region ExternallySet

    /// <summary>
    /// This method would be registered to the static Generic event in RHC_EventManager, if Generic shake is enabled.
    /// Meaning that each time the event is fired from the RHC_EventManager, this function would be called.
    /// </summary>
    private void EVO_GenericShake(Vector3 sourcePosition)
    {
        if (_GenericShakes.Count == 0)
        {
            Debug.LogError("No Shake Data is registered from the inspector to: " + this.gameObject.name);
            return;
        }
        // Record the distance to the Generic position.
        float distance = Vector3.Distance(sourcePosition, t_This.position);


        // Iterate through the list of all shake types to select the correct shake depending on the distance.
        int selectedShakeIndex = -1;
        float difference = 0.0f;
        float prevDiff = 0.0f;
        for (int x = 0; x < _GenericShakes.Count; x++)
        {
            if (distance < _GenericShakes[x].f_ApplyDistance)
            {
                difference = _GenericShakes[x].f_ApplyDistance - distance;

                // If first suitable iteration.
                if (prevDiff == 0.0f)
                {
                    prevDiff = difference;
                    selectedShakeIndex = x;
                }
                else
                {
                    if (difference < prevDiff)
                    {
                        prevDiff = difference;
                        selectedShakeIndex = x;
                    }
                }
            }
        }



        // If no suitable shake is found, return.
        if (selectedShakeIndex == -1)
            return;

        // Declare & init necessary variables according to the selected shake.
        Vector3 posSineAmounts = _GenericShakes[selectedShakeIndex].v3_PosAmount;
        Vector3 posSineSpeeds = _GenericShakes[selectedShakeIndex].v3_PosSpeed;
        Vector3 rotSineAmounts = _GenericShakes[selectedShakeIndex].v3_RotAmount;
        Vector3 rotSineSpeeds = _GenericShakes[selectedShakeIndex].v3_RotSpeed;
        float time = _GenericShakes[selectedShakeIndex].f_ShakeTime;

        // If currently a hit shake or sine shake(Generic shake) coroutine is in present, stop them and start a new hit shake coroutine.
        if (co_RecoilShake != null)
            StopCoroutine(co_RecoilShake);

        if (co_SineShake != null)
            StopCoroutine(co_SineShake);

        if (co_ResetHeadOrientation != null)
            StopCoroutine(co_ResetHeadOrientation);


        co_SineShake = StartCoroutine(CO_SineShake(posSineAmounts, posSineSpeeds, rotSineAmounts, rotSineSpeeds, time));
    }

    ///<summary>
    /// Use this function to implement your own shakes. 
    ///</summary>
    ///<param name="posShakeAmount"> Positional Shake Amount </param>
    ///<param name="posShakeSpeed"> Positional Shake Speed </param>
    ///<param name="posShakeAmount"> Rotational Shake Amount </param>
    ///<param name="posShakeSpeed"> Rotational Shake Speed </param>
    ///<param name ="shakeTime"> Total Shake Time </param>
    ///<returns></returns>
    private void EVO_CustomShake(Vector3 posShakeAmount, Vector3 posShakeSpeed, Vector3 rotShakeAmount, Vector3 rotShakeSpeed, float shakeTime)
    {
        // Stop the coroutines and call a new sine shake.
        if (co_RecoilShake != null)
            StopCoroutine(co_RecoilShake);

        if (co_SineShake != null)
            StopCoroutine(co_SineShake);

        co_SineShake = StartCoroutine(CO_SineShake(posShakeAmount, posShakeSpeed, rotShakeAmount, rotShakeSpeed, shakeTime));
    }

    ///<summary>
    /// Use this function to implement your own shakes without distance sorting.
    ///</summary>
    ///<param name="genericShakeData"> The shake preset that you want to play. </param>
    ///<returns></returns>
    private void EVO_CustomShakePreset(RHC_GenericShakePreset genericShakeData)
    {
        // Stop the coroutines and call a new sine shake.
        if (co_RecoilShake != null)
            StopCoroutine(co_RecoilShake);

        if (co_SineShake != null)
            StopCoroutine(co_SineShake);

        co_SineShake = StartCoroutine(CO_SineShake(genericShakeData.v3_PosAmount, genericShakeData.v3_PosSpeed, genericShakeData.v3_RotAmount, genericShakeData.v3_RotSpeed, genericShakeData.f_ShakeTime));
    }

    /// <summary>
    /// This method would be registered to the static Generic event in RHC_EventManager, if Generic shake is enabled.
    /// Meaning that each time the event is fired from the RHC_EventManager, this function would be called.
    /// </summary>
    private void EVO_Recoil()
    {
        // Randomize target rotation according to the min & max variables set in inspector.
        Vector3 targetRot = new Vector3(
            Random.Range(v3_RecoilShake_MinRot.x, v3_RecoilShake_MaxRot.x),
            Random.Range(v3_RecoilShake_MinRot.y, v3_RecoilShake_MaxRot.y),
            Random.Range(v3_RecoilShake_MinRot.z, v3_RecoilShake_MaxRot.z)
        );

        float power = 1.0f;

        // If increasing power is enabled, if current coroutine to decrease power is in present, stop it, and increase power according to the acceleration. Also limit it from going more than the end power.
        if (b_RecoilShake_UseIncreasingPower)
        {
            if (co_ResetRecoilShakePower != null)
                StopCoroutine(co_ResetRecoilShakePower);

            power = f_RecoilShake_StartPower;
            f_RecoilShake_StartPower += f_RecoilShake_Acceleration;

            if (f_RecoilShake_StartPower > f_RecoilShake_EndPower)
                f_RecoilShake_StartPower = f_RecoilShake_EndPower;
        }

        // Multiply target rotation by current power. (either 1.0f, or increased power)
        targetRot *= power;

        // If a sine shake(Generic shake) or a hit shake coroutine is in present, stop it, and start a new hit shake coroutine.
        if (co_SineShake != null)
            StopCoroutine(co_SineShake);

        if (co_RecoilShake != null)
            StopCoroutine(co_RecoilShake);

        if (co_ResetHeadOrientation != null)
            StopCoroutine(co_ResetHeadOrientation);


        co_RecoilShake = StartCoroutine(CO_RecoilShake(targetRot));
    }

 

    #endregion

    #region Finalizer(s)

    void OnDisable()
    {
        // Make sure that no coroutine objects are yielding when this component is disabled.
        StopAllCoroutines();

        // If hit shake is enabled, unsubscribe it's method from the hit shake event in RHC_EventManager.
        if (b_UseRecoilShake)
            RHC_EventManager.recoilShake -= EVO_Recoil;

        // If Generic shake is enabled, unsubscribe it's methods from the Generic shake event in RHC_EventManager.
        if (b_UseGenericShake)
        {
            RHC_EventManager.genericShake -= EVO_GenericShake;
            RHC_EventManager.customShake -= EVO_CustomShake;
            RHC_EventManager.customShakePreset -= EVO_CustomShakePreset;

        }
    }

    #endregion
}
