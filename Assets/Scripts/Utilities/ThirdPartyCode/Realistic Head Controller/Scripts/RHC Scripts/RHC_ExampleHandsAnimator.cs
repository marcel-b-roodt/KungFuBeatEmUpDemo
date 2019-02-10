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
 * This class controls the example FPS hands given with the package. It listens to these 3 events in RHC_EventManager:
 * - distanceShake -> When fired, triggers one of the 3 "guard" animations.!--
 * - bobChangedPreset -> When fired, triggers carrying or pushing animations if the bob preset has changed to one of the default presets, including Carrying or Pushing.
 * - movementStateChanged -> When fired, triggers running animation if the movement state has changed to Running.!--
 *
 */
using UnityEngine;

public class RHC_ExampleHandsAnimator : MonoBehaviour
{

    #region Variable(s)

    public Animator anim_Target;                                                // Target animator.
    public float f_LeftRightShakeOffset;                                        // Offset to determine whether the shake source had occured on the left, right or on front&back.

    // Int hashes for triggers.
    private int i_LeftShake = Animator.StringToHash("GuardL");
    private int i_RightShake = Animator.StringToHash("GuardR");
    private int i_ParallelShake = Animator.StringToHash("GuardLR");
    private int i_Carry1 = Animator.StringToHash("Carry1");
    private int i_Carry2 = Animator.StringToHash("Carry2");
    private int i_Push = Animator.StringToHash("Push");
    private int i_EndCarryPush = Animator.StringToHash("EndCarryPush");
    private int i_RunStarted = Animator.StringToHash("RunStarted");
    private int i_CrawlStarted = Animator.StringToHash("CrawlStarted");
    private int i_RunStopped = Animator.StringToHash("RunStopped");
    private int i_CrawlStopped = Animator.StringToHash("CrawlStopped");

    #endregion

    #region Initializer(s)

    void OnEnable()
    {
        // Subscribe methods to events.
        RHC_EventManager.genericShake += EVO_DistanceShake;
        RHC_EventManager.bobChangedPreset += EVO_BobChangedPreset;
        RHC_EventManager.movementStateChanged += EVO_MovementStateChanged;
        RHC_EventManager.stanceStateChanged += EVO_StanceStateChanged;
    }
    #endregion

    #region ExternallySet

    // Listens to genericShake event in RHC_EventManager and casts the necessary animation accordingly.
    void EVO_DistanceShake(Vector3 position)
    {
        Vector3 crossRes = Vector3.Cross(transform.forward, position - transform.position);
        float direction = Vector3.Dot(crossRes, transform.up);

        if (direction > f_LeftRightShakeOffset)
            anim_Target.SetTrigger(i_RightShake);
        else if (direction < -f_LeftRightShakeOffset)
            anim_Target.SetTrigger(i_LeftShake);
        else
            anim_Target.SetTrigger(i_ParallelShake);
    }
    // Listens to bobChangedPreset event in RHC_EventManager and casts the necessary animation accordingly.
    void EVO_BobChangedPreset(RHC_BobController.BobSettings setting, RHC_BobPresets presetChangedTo)
    {
        anim_Target.ResetTrigger(i_Carry1);
        anim_Target.ResetTrigger(i_Carry2);
        anim_Target.ResetTrigger(i_Push);
        anim_Target.ResetTrigger(i_EndCarryPush);

        if (presetChangedTo.s_Name == "WalkCarrying" || presetChangedTo.s_Name == "RunCarrying")
        {
            int random = Random.Range(0, 2);
            if (random == 0)
                anim_Target.SetTrigger(i_Carry1);
            else
                anim_Target.SetTrigger(i_Carry2);
        }
        else if (presetChangedTo.s_Name == "WalkPush" || presetChangedTo.s_Name == "RunPush")
            anim_Target.SetTrigger(i_Push);
        else
            anim_Target.SetTrigger(i_EndCarryPush);
    }
    // Listens to movementStateChanged event in RHC_EventManager and casts the necessary animation accordingly.
    void EVO_MovementStateChanged(RHC_EventManager.MovementState changedTo)
    {
        if(changedTo == RHC_EventManager.MovementState.Idling)
            anim_Target.SetTrigger(i_CrawlStopped);

        if (RHC_EventManager.en_CurrentStanceState != RHC_EventManager.StanceState.Standing) return;
        anim_Target.ResetTrigger(i_RunStarted);
        anim_Target.ResetTrigger(i_RunStopped);

        if (changedTo == RHC_EventManager.MovementState.Running)
            anim_Target.SetTrigger(i_RunStarted);
        else
            anim_Target.SetTrigger(i_RunStopped);

        anim_Target.SetTrigger(i_CrawlStopped);
    }

    // Listens to stanceStateChanged event in RHC_EventManager and casts the necessary animation accordingly.
    void EVO_StanceStateChanged(RHC_EventManager.StanceState changedTo)
    {
        anim_Target.ResetTrigger(i_CrawlStarted);
        anim_Target.ResetTrigger(i_CrawlStopped);

        //if (changedTo == RHC_EventManager.StanceState.Crawling)
        //    anim_Target.SetTrigger(i_CrawlStarted);
        //else
            anim_Target.SetTrigger(i_CrawlStopped);
            
        anim_Target.SetTrigger(i_RunStopped);
    }

    #endregion

    #region Finalizer(s)

    void OnDisable()
    {
        // Unsubscribe methods from the events.
        RHC_EventManager.genericShake -= EVO_DistanceShake;
        RHC_EventManager.bobChangedPreset -= EVO_BobChangedPreset;
        RHC_EventManager.movementStateChanged -= EVO_MovementStateChanged;
        RHC_EventManager.stanceStateChanged -= EVO_StanceStateChanged;
    }

    #endregion
}
