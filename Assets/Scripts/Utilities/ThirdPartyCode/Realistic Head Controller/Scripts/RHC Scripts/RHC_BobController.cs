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
 * This class is used to control head bob movement while moving. It works coordinately with the RHC_CameraStatePositioner class. It uses two listener methods, EVO_StanceStateChanged & EVO_MovementStateChanged.
 * These 2 methods are subscribed to the event, which is fired off when player walks, runs, crouches, jumps etc. in RHC_EventManager, in order to decide what kind of head bob should be applied to the transform.
 * It uses Mathf.Sin to create smoothly interpolated bobbing movement and a Coroutine to smoothly transform between "moving" and "idling". 
 *
 * The class works in an interleaved way. It checks whether the player is moving or not. If moving, whether it is running or walking. And also it checks whether the player is in stand, crouch or crawl state.
 * Then it decides what kind of bobbing it should apply.
 * According to the example player states in the package, this class supports 6 different ways of bobbing:
 *
 * - Walk Bob: Standing & walking(Player is walking while it's in stand state).
 * - Run Bob: Standing & running(Player is running while it's in stand state).
 * - Crouch Walk Bob: Crouching & walking.
 * - Crouch Run Bob: Crouching & running.
 * - Crawl Slow Bob: crawling & walking(crawling slowly).
 * - Crawl Fast Bob: crawling & running(crawling fast).
 *
 *
 * The coolest part is, it's really easy to implement new states according to your own player controller. Just look at the script, copy & paste variables & methods, refactor, and you would be good to go. 
 * Check the Video Tutorials for more info!
 * You can always ask to me about any issue/implementation from the contact informations, including live chat, that were mentioned above.
 * 
 */

// Libraries
using UnityEngine;
using System.Collections;

public class RHC_BobController : MonoBehaviour
{
    #region Variable(s)

    /* ENUMERATIONS */

    public enum Preset
    {
        NONE, WALK_Light, WALK_Medium, WALK_Heavy, WALK_Carrying, WALK_Injured, WALK_Dizzy, WALK_Drunk, WALK_Push,
        RUN_Light, RUN_Medium, RUN_Heavy, RUN_Carrying, RUN_Injured, RUN_Drunk, RUN_Push,
        CRAWL_Slow, CRAWL_Fast
    };

    public RHC_BobPresets _WalkBobPreset;
    public RHC_BobPresets _RunBobPreset;
    public RHC_BobPresets _CrouchWalkBobPreset;
    public RHC_BobPresets _CrouchRunBobPreset;
    public RHC_BobPresets _CrawlSlowBobPreset;
    public RHC_BobPresets _CrawlFastBobPreset;

    public RHC_BobPresets _RecordedWalkBobPreset;
    public RHC_BobPresets _RecordedRunBobPreset;
    public RHC_BobPresets _RecordedCrouchWalkBobPreset;
    public RHC_BobPresets _RecordedCrouchRunBobPreset;
    public RHC_BobPresets _RecordedCrawlSlowBobPreset;
    public RHC_BobPresets _RecordedCrawlFastBobPreset;

    public enum BobSettings { WalkSettings, RunSettings, CrouchWalkSettings, CrouchRunSettings, CrawlSlowSettings, CrawlFastSettings };       // Choose which settings to edit from the inspector.
    public BobSettings _settings;

    public enum BobStyle { OnlyPosition, OnlyRotation, Both };                  // Choose what to affect on the target transform while walking.
    public BobStyle _WalkBobStyle;

    public BobStyle _RunBobStyle;

    public BobStyle _CrouchWalkBobStyle;

    public BobStyle _CrouchRunBobStyle;

    public BobStyle _CrawlSlowBobStyle;

    public BobStyle _CrawlFastBobStyle;

    /* WALK - POSITION SETTINGS */
    public Vector3 v3_WALK_POS_Amounts;                                         // Intensity of position interpolation while walk bobbing.
    public Vector3 v3_WALK_POS_Speed;                                           // Speed of position interpolation while walk bobbing.
    public float f_WALK_POS_Smooth;                                             // Position interpolation smooth while walk bobbing.

    /* WALK - ROTATION SETTINGS */
    public Vector3 v3_WALK_ROT_Amounts;                                         // Intensity of rotation interpolation while walking.
    public Vector3 v3_WALK_ROT_Speed;                                           // Speed of rotation interpolation while walking.
    public float f_WALK_ROT_Smooth;                                             // Rotation interpolation smooth while walk bobbing.

    /* RUN - POSITION SETTINGS */
    public Vector3 v3_RUN_POS_Amounts;                                          // Intensity of position interpolation while running.
    public Vector3 v3_RUN_POS_Speed;                                            // Speed of position interpolation while running.
    public float f_RUN_POS_Smooth;                                              // Position interpolation smooth while running.

    /* RUN - ROTATION SETTINGS */
    public Vector3 v3_RUN_ROT_Amounts;                                          // Intensity of rotation interpolation while running.
    public Vector3 v3_RUN_ROT_Speed;                                            // Speed of rotation interpolation while running.
    public float f_RUN_ROT_Smooth;												// Rotation interpolation smooth while running.

    /* CROUCH WALK - POSITION SETTINGS */
    public Vector3 v3_CRCW_POS_Amounts;                                         // Intensity of position interpolation while walking & crouching.
    public Vector3 v3_CRCW_POS_Speed;                                           // Speed of position interpolation while walking & crouching.
    public float f_CRCW_POS_Smooth;                                             // Position interpolation smooth while walking & crouching?

    /* CROUCH WALK - ROTATION SETTINGS */
    public Vector3 v3_CRCW_ROT_Amounts;                                         // Intensity of rotation interpolation while walking & crouching.
    public Vector3 v3_CRCW_ROT_Speed;                                           // Speed of rotation interpolation while walking & crouching.
    public float f_CRCW_ROT_Smooth;                                             // Rotation interpolation smooth while walking & crouching.

    /* CROUCH RUN - POSITION SETTINGS */
    public Vector3 v3_CRCR_POS_Amounts;                                         // Intensity of position interpolation while running & crouching.
    public Vector3 v3_CRCR_POS_Speed;                                           // Speed of position interpolation while running & crouching.
    public float f_CRCR_POS_Smooth;                                             // Position interpolation smooth while running & crouching.

    /* CROUCH RUN - ROTATION SETTINGS */
    public Vector3 v3_CRCR_ROT_Amounts;                                         // Intensity of rotation interpolation while running & crouching.
    public Vector3 v3_CRCR_ROT_Speed;                                           // Speed of rotation interpolation while running & crouching.
    public float f_CRCR_ROT_Smooth;                                             // Rotation interpolation smooth while running & crouching.

    /* CRAWL SLOW - POSITION SETTINGS */
    public Vector3 v3_CRAWLS_POS_Amounts;                                       // Intensity of position interpolation while crawling & walking(crawling slowly).
    public Vector3 v3_CRAWLS_POS_Speed;                                         // Speed of position interpolation while crawling & walking(crawling slowly).
    public float f_CRAWLS_POS_Smooth;                                           // Position interpolation smooth while crawling & walking(crawling slowly).

    /* CRAWL SLOW - ROTATION  SETTINGS */
    public Vector3 v3_CRAWLS_ROT_Amounts;                                       // Intensity of rotation interpolation while crawling & walking(crawling slowly).
    public Vector3 v3_CRAWLS_ROT_Speed;                                         // Speed of rotation interpolation while crawling & walking(crawling slowly).
    public float f_CRAWLS_ROT_Smooth;                                           //Rotation interpolation smooth while crawling & walking(crawling slowly).

    /* CRAWL FAST - POSITION SETTINGS */
    public Vector3 v3_CRAWLF_POS_Amounts;                                       // Intensity of position interpolation while crawling & running(crawling fast).
    public Vector3 v3_CRAWLF_POS_Speed;                                         // Speed of position interpolation while crawling & running(crawling fast).
    public float f_CRAWLF_POS_Smooth;                                           // Position interpolation smooth while crawling & running(crawling fast).

    /* CRAWL FAST - ROTATION SETTINGS */
    public Vector3 v3_CRAWLF_ROT_Amounts;                                       // Intensity of rotation interpolation while crawling & running(crawling fast).
    public Vector3 v3_CRAWLF_ROT_Speed;                                         // Speed of rotation interpolation while crawling & running(crawling fast).
    public float f_CRAWLF_ROT_Smooth;                                           // Rotation interpolation smooth while crawling & running(crawling fast).

    /* GENERAL SETTINGS */
    public float f_ResetSpeed;                                                  // How fast will the target transform reset to it's original position & rotation when no movement is present.
    public bool b_MovementStateChanged = false;                                 // Flag used in state change logic.
    private bool b_EnableBobbing = false;                                       // Is bobbing enabled? Used for controlling logic based on event calls.    
    private bool b_PositionAffected;                                            // Controls whether to interpolate the position or not while resetting.
    private bool b_RotationAffected;                                            // Controls whether to interpolate the rotation or not while resetting.
    private Transform t_Target;                                                 // Target transform to affect (this transform by Awake).
    private Vector3 v3_InitialTargetROT;                                        // Initial target rotation.
    private Vector3 v3_InitialTargetPOS;                                        // Initial target position.
    private Coroutine co_ResetHeadOrientation;                                  // Coroutine used for resetting head orientation to it's original position & rotation.

    private delegate void DELVO_CurrentBobbing();                               // Delegate method. When any bobbing should be done, this is executed. The registered method to it would be changed by event listener method.
    private DELVO_CurrentBobbing bobbingMethod;

    private RHC_EventManager.StanceState en_CurrentStanceState;                 // Storage for current Player Stance State.
    private RHC_EventManager.MovementState en_CurrentMovementState;             // Storage for current Player Movement State.

    #endregion

    #region Initializer(s)

    void Awake()
    {
        t_Target = transform;

        // Store initial rotation and position of the target transform.
        v3_InitialTargetROT = t_Target.localEulerAngles;
        v3_InitialTargetPOS = t_Target.localPosition;

        // Set the position & rotation controller booleans according to the user choice of affection for interpolation on the target transform.
        if (_WalkBobStyle == BobStyle.OnlyPosition || _WalkBobStyle == BobStyle.Both || _RunBobStyle == BobStyle.OnlyPosition || _RunBobStyle == BobStyle.Both)
            b_PositionAffected = true;

        if (_WalkBobStyle == BobStyle.OnlyRotation || _WalkBobStyle == BobStyle.Both || _RunBobStyle == BobStyle.OnlyRotation || _RunBobStyle == BobStyle.Both)
            b_RotationAffected = true;
    }

    void OnEnable()
    {
        // Subscribe listener methods to the events in RHC_EventManager.
        // This means that whenever player changes it's movement state(notmoving, walking, running) and it's stance state(standing, crouching, crawling) these methods will be called accordingly.
        RHC_EventManager.movementStateChanged += EVO_MovementStateChanged;
        RHC_EventManager.stanceStateChanged += EVO_StanceStateChanged;
        RHC_EventManager.bobChangedCustom += EVO_BobChangedCustom;
        RHC_EventManager.bobChangedPreset += EVO_BobChangedPreset;
    }

    #endregion

    #region Control(s)

    void Update()
    {
        // If bobbing is enabled.
        if (b_EnableBobbing)
        {
            // If movement state has changed via events, false the bool for the sake of the logic, and then stop if a reset coroutine is in present.
            if (b_MovementStateChanged)
            {
                b_MovementStateChanged = false;

                if (co_ResetHeadOrientation != null)
                    StopCoroutine(co_ResetHeadOrientation);
            }
            // Call the corresponding bobbing method.
            bobbingMethod();
        }
    }

    /// <summary> Bobbing method for if the player is standing and walking.</summary>
    void WalkBob()
    {
        // Check the choice of bob style and apply bobbing accordingly.
        if (_WalkBobStyle == BobStyle.OnlyPosition)
        {
            // Declare & initialize the inner components of the target vector for position. 
            // Use Mathf.Sin to create smooth ping-pong bobbing variables according to the tweakable settings.
            float posTargetX = Mathf.Sin(Time.time * v3_WALK_POS_Speed.x) * v3_WALK_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_WALK_POS_Speed.y) * v3_WALK_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_WALK_POS_Speed.z) * v3_WALK_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_WALK_POS_Smooth);
        }
        else if (_WalkBobStyle == BobStyle.OnlyRotation)
        {
            // Declare & initialize the inner components of the target vector for rotation. 
            // Use Mathf.Sin to create smooth ping-pong bobbing variables according to the tweakable settings.
            float ROTTargetX = Mathf.Sin(Time.time * v3_WALK_ROT_Speed.x) * v3_WALK_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_WALK_ROT_Speed.y) * v3_WALK_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_WALK_ROT_Speed.z) * v3_WALK_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vector using spherical linear interpolation.
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_WALK_ROT_Smooth);
        }
        else
        {
            // Declare & initialize the inner components of the target vector for position. 
            // Use Mathf.Sin to create smooth ping-pong bobbing variables according to the tweakable settings.
            float posTargetX = Mathf.Sin(Time.time * v3_WALK_POS_Speed.x) * v3_WALK_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_WALK_POS_Speed.y) * v3_WALK_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_WALK_POS_Speed.z) * v3_WALK_POS_Amounts.z;

            // Declare & initialize the target vector position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Declare & initialize the inner components of the target vector for rotation. 
            // Use Mathf.Sin to create smooth ping-pong bobbing variables according to the tweakable settings.
            float ROTTargetX = Mathf.Sin(Time.time * v3_WALK_ROT_Speed.x) * v3_WALK_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_WALK_ROT_Speed.y) * v3_WALK_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_WALK_ROT_Speed.z) * v3_WALK_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vectors using normal & spherical linear interpolations.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_WALK_POS_Smooth);
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_WALK_ROT_Smooth);
        }
    }

    /// <summary> Bobbing method for if the player is standing and running.</summary>
    void RunBob()
    {
        // Check the choice of bob style and apply bobbing accordingly.
        if (_RunBobStyle == BobStyle.OnlyPosition)
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_RUN_POS_Speed.x) * v3_RUN_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_RUN_POS_Speed.y) * v3_RUN_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_RUN_POS_Speed.z) * v3_RUN_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = new Vector3(posTargetX, posTargetY, posTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_RUN_POS_Smooth);
        }
        else if (_RunBobStyle == BobStyle.OnlyRotation)
        {
            // Declare & initialize the inner components of the target vector for rotation. 
            float ROTTargetX = Mathf.Sin(Time.time * v3_RUN_ROT_Speed.x) * v3_RUN_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_RUN_ROT_Speed.y) * v3_RUN_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_RUN_ROT_Speed.z) * v3_RUN_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vector using spherical linear interpolation.
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_RUN_ROT_Smooth);
        }
        else
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_RUN_POS_Speed.x) * v3_RUN_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_RUN_POS_Speed.y) * v3_RUN_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_RUN_POS_Speed.z) * v3_RUN_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = new Vector3(posTargetX, posTargetY, posTargetZ);

            // Declare & initialize the inner components of the target vector for rotation. 
            float ROTTargetX = Mathf.Sin(Time.time * v3_RUN_ROT_Speed.x) * v3_RUN_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_RUN_ROT_Speed.y) * v3_RUN_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_RUN_ROT_Speed.z) * v3_RUN_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vectors using normal & spherical linear interpolations.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_RUN_POS_Smooth);
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_RUN_ROT_Smooth);
        }
    }

    /// <summary> Bobbing method for if the player is crouching and walking.</summary>
    void CrouchWalkBob()
    {
        // Check the choice of bob style and apply bobbing accordingly.
        if (_CrouchWalkBobStyle == BobStyle.OnlyPosition)
        {
            // Declare & initialize the inner components of the target vector for position.  
            float posTargetX = Mathf.Sin(Time.time * v3_CRCW_POS_Speed.x) * v3_CRCW_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRCW_POS_Speed.y) * v3_CRCW_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRCW_POS_Speed.z) * v3_CRCW_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRCW_POS_Smooth);
        }
        else if (_CrouchWalkBobStyle == BobStyle.OnlyRotation)
        {
            // Declare & initialize the inner components of the target vector for rotation. 
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRCW_ROT_Speed.x) * v3_CRCW_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRCW_ROT_Speed.y) * v3_CRCW_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRCW_ROT_Speed.z) * v3_CRCW_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vector using spherical linear interpolation.
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRCW_ROT_Smooth);
        }
        else
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_CRCW_POS_Speed.x) * v3_CRCW_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRCW_POS_Speed.y) * v3_CRCW_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRCW_POS_Speed.z) * v3_CRCW_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Declare & initialize the inner components of the target vector for rotation.
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRCW_ROT_Speed.x) * v3_CRCW_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRCW_ROT_Speed.y) * v3_CRCW_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRCW_ROT_Speed.z) * v3_CRCW_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vectors using normal & spherical linear interpolations.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRCW_POS_Smooth);
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRCW_ROT_Smooth);
        }
    }

    /// <summary> Bobbing method for if the player is crouching and running.</summary>
    void CrouchRunBob()
    {
        // Check the choice of bob style and apply bobbing accordingly.
        if (_CrouchRunBobStyle == BobStyle.OnlyPosition)
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_CRCR_POS_Speed.x) * v3_CRCR_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRCR_POS_Speed.y) * v3_CRCR_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRCR_POS_Speed.z) * v3_CRCR_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRCR_POS_Smooth);
        }
        else if (_CrouchRunBobStyle == BobStyle.OnlyRotation)
        {
            // Declare & initialize the inner components of the target vector for rotation. 
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRCR_ROT_Speed.x) * v3_CRCR_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRCR_ROT_Speed.y) * v3_CRCR_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRCR_ROT_Speed.z) * v3_CRCR_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRCR_ROT_Smooth);
        }
        else
        {
            // Declare & initialize the inner components of the target vector for position.
            float posTargetX = Mathf.Sin(Time.time * v3_CRCR_POS_Speed.x) * v3_CRCR_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRCR_POS_Speed.y) * v3_CRCR_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRCR_POS_Speed.z) * v3_CRCR_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Declare & initialize the inner components of the target vector for rotation. 
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRCR_ROT_Speed.x) * v3_CRCR_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRCR_ROT_Speed.y) * v3_CRCR_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRCR_ROT_Speed.z) * v3_CRCR_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vectors using normal & spherical linear interpolations.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRCR_POS_Smooth);
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRCR_ROT_Smooth);
        }
    }

    /// <summary> Bobbing method for if the player is crawling and walking(crawling slowly).</summary>
    void CrawlSlowBob()
    {
        // Check the choice of bob style and apply bobbing accordingly.
        if (_CrawlSlowBobStyle == BobStyle.OnlyPosition)
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_CRAWLS_POS_Speed.x) * v3_CRAWLS_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRAWLS_POS_Speed.y) * v3_CRAWLS_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRAWLS_POS_Speed.z) * v3_CRAWLS_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRAWLS_POS_Smooth);
        }
        else if (_CrawlSlowBobStyle == BobStyle.OnlyRotation)
        {
            // Declare & initialize the inner components of the target vector for rotation.
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRAWLS_ROT_Speed.x) * v3_CRAWLS_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRAWLS_ROT_Speed.y) * v3_CRAWLS_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRAWLS_ROT_Speed.z) * v3_CRAWLS_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRAWLS_ROT_Smooth);
        }
        else
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_CRAWLS_POS_Speed.x) * v3_CRAWLS_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRAWLS_POS_Speed.y) * v3_CRAWLS_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRAWLS_POS_Speed.z) * v3_CRAWLS_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Declare & initialize the inner components of the target vector for rotation.
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRAWLS_ROT_Speed.x) * v3_CRAWLS_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRAWLS_ROT_Speed.y) * v3_CRAWLS_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRAWLS_ROT_Speed.z) * v3_CRAWLS_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vectors using normal & spherical linear interpolations.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRAWLS_POS_Smooth);
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRAWLS_ROT_Smooth);
        }
    }

    /// <summary> Bobbing method for if the player is crawling and walking(crawling fast).</summary>
    void CrawlFastBob()
    {
        // Check the choice of bob style and apply bobbing accordingly.
        if (_CrawlFastBobStyle == BobStyle.OnlyPosition)
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_CRAWLF_POS_Speed.x) * v3_CRAWLF_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRAWLF_POS_Speed.y) * v3_CRAWLF_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRAWLF_POS_Speed.z) * v3_CRAWLF_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRAWLF_POS_Smooth);
        }
        else if (_CrawlFastBobStyle == BobStyle.OnlyRotation)
        {
            // Declare & initialize the inner components of the target vector for rotation.
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRAWLF_ROT_Speed.x) * v3_CRAWLF_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRAWLF_ROT_Speed.y) * v3_CRAWLF_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRAWLF_ROT_Speed.z) * v3_CRAWLF_ROT_Amounts.z;

            // Declare & initialize the target vector for rotation.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vector using linear interpolation.
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRAWLF_ROT_Smooth);

        }
        else
        {
            // Declare & initialize the inner components of the target vector for position. 
            float posTargetX = Mathf.Sin(Time.time * v3_CRAWLF_POS_Speed.x) * v3_CRAWLF_POS_Amounts.x;
            float posTargetY = Mathf.Sin(Time.time * v3_CRAWLF_POS_Speed.y) * v3_CRAWLF_POS_Amounts.y;
            float posTargetZ = Mathf.Sin(Time.time * v3_CRAWLF_POS_Speed.z) * v3_CRAWLF_POS_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 posTarget = v3_InitialTargetPOS + new Vector3(posTargetX, posTargetY, posTargetZ);

            // Declare & initialize the inner components of the target vector for rotation.
            float ROTTargetX = Mathf.Sin(Time.time * v3_CRAWLF_ROT_Speed.x) * v3_CRAWLF_ROT_Amounts.x;
            float ROTTargetY = Mathf.Sin(Time.time * v3_CRAWLF_ROT_Speed.y) * v3_CRAWLF_ROT_Amounts.y;
            float ROTTargetZ = Mathf.Sin(Time.time * v3_CRAWLF_ROT_Speed.z) * v3_CRAWLF_ROT_Amounts.z;

            // Declare & initialize the target vector for position.
            Vector3 ROTTarget = v3_InitialTargetROT + new Vector3(ROTTargetX, ROTTargetY, ROTTargetZ);

            // Apply the target vectors using normal & spherical linear interpolations.
            t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_CRAWLF_POS_Smooth);
            t_Target.localRotation = Quaternion.Lerp(t_Target.localRotation, Quaternion.Euler(ROTTarget), Time.deltaTime * f_CRAWLF_ROT_Smooth);
        }
    }

    #endregion

    #region Coroutine(s)

    /// <summary> Coroutine for resetting the head orientation.</summary>
    public IEnumerator CO_Reset()
    {
        float i = 0.0f;
        float rate = f_ResetSpeed;

        // Store the current rotation to use inside the loop as an initial point for linear interpolation.
        Quaternion currentQ = t_Target.localRotation;
        // Store the current position to use inside the loop as an initial point for linear interpolation.
        Vector3 currentPos = t_Target.localPosition;

        while (i < 1.0f)    // Run the coroutine for a desired time which is altered by the speed given.
        {
            // Apply position resetting if the position was altered, and rotation resetting if the rotation was altered.
            i += Time.deltaTime * rate;
            if (b_RotationAffected)
                t_Target.localRotation = Quaternion.Lerp(currentQ, Quaternion.Euler(v3_InitialTargetROT), i);
            if (b_PositionAffected)
                t_Target.localPosition = Vector3.Lerp(currentPos, v3_InitialTargetPOS, i);

            yield return null;
        }
    }

    #endregion

    #region ExternallySet

    /// <summary> Listens to the movementStateChanged event in RHC_EventManager. Makes necessary changes and sets bobbing delegate to according function. Or calls reset coroutine if necessary.</summary>
    void EVO_MovementStateChanged(RHC_EventManager.MovementState currentMovementState)
    {
        // Store the player movement state for the comparison logic.
        en_CurrentMovementState = currentMovementState;

        // If player jumped, disable bobbing, if any reset coroutine is in present, stop it and call a new one, then return.
        if (en_CurrentStanceState == RHC_EventManager.StanceState.OnAir)
        {
            b_EnableBobbing = false;

            if (co_ResetHeadOrientation != null)
            {
                StopCoroutine(co_ResetHeadOrientation);
                co_ResetHeadOrientation = StartCoroutine(CO_Reset());
            }
            return;
        }
        // If player started to idle, disable bobbing, if any reset coroutine is in present, stop it and call a new one.
        if (currentMovementState == RHC_EventManager.MovementState.Idling)
        {
            b_EnableBobbing = false;

            if (co_ResetHeadOrientation != null)
                StopCoroutine(co_ResetHeadOrientation);

            co_ResetHeadOrientation = StartCoroutine(CO_Reset());
        }
        // If player started to walk, enable bobbing, and set the delegate method to necessary bobbing method, so that Update function can bob accordingly.
        else if (currentMovementState == RHC_EventManager.MovementState.Walking)
        {
            b_EnableBobbing = true;
            b_MovementStateChanged = true;

            // Check stance state and set the necessary method.
            if (en_CurrentStanceState == RHC_EventManager.StanceState.Standing)
                bobbingMethod = WalkBob;
            else if (en_CurrentStanceState == RHC_EventManager.StanceState.Crouching)
                bobbingMethod = CrouchWalkBob;
            else
                bobbingMethod = CrawlSlowBob;
        }
        // If player started to run (depending on the controller, maybe started to move faster than default movement speed), enable bobbing, and set the delegate method to necessary bobbing method, so that Update function can bob accordingly.
        else if (currentMovementState == RHC_EventManager.MovementState.Running)
        {
            b_EnableBobbing = true;
            b_MovementStateChanged = true;

            // Check stance state and set the necessary method.
            if (en_CurrentStanceState == RHC_EventManager.StanceState.Standing)
                bobbingMethod = RunBob;
            else if (en_CurrentStanceState == RHC_EventManager.StanceState.Crouching)
                bobbingMethod = CrouchRunBob;
            else
                bobbingMethod = CrawlFastBob;
        }
    }

    /// <summary> Listens to the stanceStateChanged event in RHC_EventManager. Makes necessary changes and sets bobbing delegate to according function. Or calls reset coroutine if necessary.</summary>
    void EVO_StanceStateChanged(RHC_EventManager.StanceState currentStanceState)
    {
        // Store the player stance state for the comparison logic.
        en_CurrentStanceState = currentStanceState;

        // If player is not moving, disable bobbing, if any reset coroutine is in present, stop it and call a new one, then return.
        if (en_CurrentMovementState == RHC_EventManager.MovementState.Idling)
        {
            b_EnableBobbing = false;

            if (co_ResetHeadOrientation != null)
            {
                StopCoroutine(co_ResetHeadOrientation);
                co_ResetHeadOrientation = StartCoroutine(CO_Reset());
            }
            return;
        }
        // If player standed, make the comparisons regarding whether it's walking or running, then set the necessary method to be used in Update.
        if (en_CurrentStanceState == RHC_EventManager.StanceState.Standing)
        {
            if (en_CurrentMovementState == RHC_EventManager.MovementState.Walking)
            {
                b_EnableBobbing = true;
                b_MovementStateChanged = true;
                bobbingMethod = WalkBob;    // Set the delegate method.
            }
            else if (en_CurrentMovementState == RHC_EventManager.MovementState.Running)
            {
                b_EnableBobbing = true;
                b_MovementStateChanged = true;
                bobbingMethod = RunBob;    // Set the delegate method.
            }
        }
        // If player crouched, make the comparisons regarding whether it's walking or running, then set the necessary method to be used in Update.
        else if (en_CurrentStanceState == RHC_EventManager.StanceState.Crouching)
        {
            if (en_CurrentMovementState == RHC_EventManager.MovementState.Walking)
            {
                b_EnableBobbing = true;
                b_MovementStateChanged = true;
                bobbingMethod = CrouchWalkBob;  // Set the delegate method.
            }
            else if (en_CurrentMovementState == RHC_EventManager.MovementState.Running)
            {
                b_EnableBobbing = true;
                b_MovementStateChanged = true;
                bobbingMethod = CrouchRunBob;   // Set the delegate method.
            }
        }
    }

    /// <summary> Listens to the bobChangedPreset event in RHC_EventManager. Adjusts the sent setting's variables as the sent preset parameter.</summary>
    void EVO_BobChangedPreset(RHC_BobController.BobSettings settingToChange, RHC_BobPresets presetToChange)
    {
        // Receive new preset data according to the input parameter preset.
 

        if (settingToChange == BobSettings.WalkSettings)
        {
            _WalkBobPreset = presetToChange;
            _WalkBobStyle = presetToChange.m_BobStyle;
            v3_WALK_POS_Amounts = presetToChange.v3_PosAmount;
            v3_WALK_ROT_Amounts = presetToChange.v3_RotAmount;
            v3_WALK_POS_Speed = presetToChange.v3_PosSpeed;
            v3_WALK_ROT_Speed = presetToChange.v3_RotSpeed;
            f_WALK_POS_Smooth = presetToChange.f_PosSmooth;
            f_WALK_ROT_Smooth = presetToChange.f_RotSmooth;
        }
        else if (settingToChange == BobSettings.RunSettings)
        {
            _RunBobPreset = presetToChange;
            _RunBobStyle = presetToChange.m_BobStyle;
            v3_RUN_POS_Amounts = presetToChange.v3_PosAmount;
            v3_RUN_ROT_Amounts = presetToChange.v3_RotAmount;
            v3_RUN_POS_Speed = presetToChange.v3_PosSpeed;
            v3_RUN_ROT_Speed = presetToChange.v3_RotSpeed;
            f_RUN_POS_Smooth = presetToChange.f_PosSmooth;
            f_RUN_ROT_Smooth = presetToChange.f_RotSmooth;
        }
        else if (settingToChange == BobSettings.CrouchWalkSettings)
        {
            _CrouchWalkBobPreset = presetToChange;
            _CrouchWalkBobStyle = presetToChange.m_BobStyle;
            v3_CRCW_POS_Amounts = presetToChange.v3_PosAmount;
            v3_CRCW_ROT_Amounts = presetToChange.v3_RotAmount;
            v3_CRCW_POS_Speed = presetToChange.v3_PosSpeed;
            v3_CRCW_ROT_Speed = presetToChange.v3_RotSpeed;
            f_CRCW_POS_Smooth = presetToChange.f_PosSmooth;
            f_CRCW_ROT_Smooth = presetToChange.f_RotSmooth;
        }
        else if (settingToChange == BobSettings.CrouchRunSettings)
        {
            _CrouchRunBobPreset = presetToChange;
            _CrouchRunBobStyle = presetToChange.m_BobStyle;
            v3_CRCR_POS_Amounts = presetToChange.v3_PosAmount;
            v3_CRCR_ROT_Amounts = presetToChange.v3_RotAmount;
            v3_CRCR_POS_Speed = presetToChange.v3_PosSpeed;
            v3_CRCR_ROT_Speed = presetToChange.v3_RotSpeed;
            f_CRCR_POS_Smooth = presetToChange.f_PosSmooth;
            f_CRCR_ROT_Smooth = presetToChange.f_RotSmooth;
        }
        else if (settingToChange == BobSettings.CrawlSlowSettings)
        {
            _CrawlSlowBobPreset = presetToChange;
            _CrawlSlowBobStyle = presetToChange.m_BobStyle;
            v3_CRAWLS_POS_Amounts = presetToChange.v3_PosAmount;
            v3_CRAWLS_ROT_Amounts = presetToChange.v3_RotAmount;
            v3_CRAWLS_POS_Speed = presetToChange.v3_PosSpeed;
            v3_CRAWLS_ROT_Speed = presetToChange.v3_RotSpeed;
            f_CRAWLS_POS_Smooth = presetToChange.f_PosSmooth;
            f_CRAWLS_ROT_Smooth = presetToChange.f_RotSmooth;
        }
        else if (settingToChange == BobSettings.CrawlFastSettings)
        {
            _CrawlFastBobPreset = presetToChange;
            _CrawlFastBobStyle = presetToChange.m_BobStyle;
            v3_CRAWLF_POS_Amounts = presetToChange.v3_PosAmount;
            v3_CRAWLF_ROT_Amounts = presetToChange.v3_RotAmount;
            v3_CRAWLF_POS_Speed = presetToChange.v3_PosSpeed;
            v3_CRAWLF_ROT_Speed = presetToChange.v3_RotSpeed;
            f_CRAWLF_POS_Smooth = presetToChange.f_PosSmooth;
            f_CRAWLF_ROT_Smooth = presetToChange.f_RotSmooth;
        }
    }
    /// <summary> Listens to the bobChangedCustom event in RHC_EventManager. Sets the sent setting's variables according to the sent parameters.</summary>
    void EVO_BobChangedCustom(RHC_BobController.BobSettings settingToChange, RHC_BobController.BobStyle bobStyle, Vector3 posAmount, Vector3 posSpeed, Vector3 rotAmount, Vector3 rotSpeed, float posSmooth, float rotSmooth)
    {
        if (settingToChange == BobSettings.WalkSettings)
        {
            _WalkBobStyle = bobStyle;
            v3_WALK_POS_Amounts = posAmount;
            v3_WALK_ROT_Amounts = rotAmount;
            v3_WALK_POS_Speed = posSpeed;
            v3_WALK_ROT_Speed = rotSpeed;
            f_WALK_POS_Smooth = posSmooth;
            f_WALK_ROT_Smooth = rotSmooth;
        }
        else if (settingToChange == BobSettings.RunSettings)
        {
            _RunBobStyle = bobStyle;
            v3_RUN_POS_Amounts = posAmount;
            v3_RUN_ROT_Amounts = rotAmount;
            v3_RUN_POS_Speed = posSpeed;
            v3_RUN_ROT_Speed = rotSpeed;
            f_RUN_POS_Smooth = posSmooth;
            f_RUN_ROT_Smooth = rotSmooth;
        }
        else if (settingToChange == BobSettings.CrouchWalkSettings)
        {
            _CrouchWalkBobStyle = bobStyle;
            v3_CRCW_POS_Amounts = posAmount;
            v3_CRCW_ROT_Amounts = rotAmount;
            v3_CRCW_POS_Speed = posSpeed;
            v3_CRCW_ROT_Speed = rotSpeed;
            f_CRCW_POS_Smooth = posSmooth;
            f_CRCW_ROT_Smooth = rotSmooth;
        }
        else if (settingToChange == BobSettings.CrouchRunSettings)
        {
            _CrouchRunBobStyle = bobStyle;
            v3_CRCR_POS_Amounts = posAmount;
            v3_CRCR_ROT_Amounts = rotAmount;
            v3_CRCR_POS_Speed = posSpeed;
            v3_CRCR_ROT_Speed = rotSpeed;
            f_CRCR_POS_Smooth = posSmooth;
            f_CRCR_ROT_Smooth = rotSmooth;
        }
        else if (settingToChange == BobSettings.CrawlSlowSettings)
        {
            _CrawlSlowBobStyle = bobStyle;
            v3_CRAWLS_POS_Amounts = posAmount;
            v3_CRAWLS_ROT_Amounts = rotAmount;
            v3_CRAWLS_POS_Speed = posSpeed;
            v3_CRAWLS_ROT_Speed = rotSpeed;
            f_CRAWLS_POS_Smooth = posSmooth;
            f_CRAWLS_ROT_Smooth = rotSmooth;
        }
        else if (settingToChange == BobSettings.CrawlFastSettings)
        {
            _CrawlFastBobStyle = bobStyle;
            v3_CRAWLF_POS_Amounts = posAmount;
            v3_CRAWLF_ROT_Amounts = rotAmount;
            v3_CRAWLF_POS_Speed = posSpeed;
            v3_CRAWLF_ROT_Speed = rotSpeed;
            f_CRAWLF_POS_Smooth = posSmooth;
            f_CRAWLF_ROT_Smooth = rotSmooth;
        }
    }

    #endregion

    #region Finalizer(s)

    void OnDisable()
    {
        // Make sure that no coroutine objects are yielding when this component is disabled.
        StopAllCoroutines();

        // Reset orientation.
        t_Target.localPosition = v3_InitialTargetPOS;
        t_Target.localEulerAngles = v3_InitialTargetROT;

        // Unsubscribe the event listener methods from the movementStateChanged & stanceStateChanged events fired off from RHC_EventManager.
        RHC_EventManager.movementStateChanged -= EVO_MovementStateChanged;
        RHC_EventManager.stanceStateChanged -= EVO_StanceStateChanged;
        RHC_EventManager.bobChangedCustom -= EVO_BobChangedCustom;
        RHC_EventManager.bobChangedPreset -= EVO_BobChangedPreset;
    }

    #endregion



}










