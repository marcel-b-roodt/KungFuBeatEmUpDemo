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
 * This class is used to manage player events. In player events, there are 4 events registered by default:
 * -movementStateChanged
 * -stanceStateChanged
 * -recoilShake
 * -genericShake
 * -customShake
 *
 * **********
 * MovementStateChanged & StanceStateChanged: These 2 events are written for player state machines. Advanced Head & Camera Movement uses interleaved logic to decide what kind of head behaviour should be applied, in the means of
 * positioning & rotationing the camera, and bobbing the camera, according to these 2 events. The logic is like this:
 * Player may be standing up straight. While standing, it may just idle(doesn't move), walk or run.
 * Player may be crouching. While crouching, it may just idle(doesn't move), walk while crouching, or run while crouching.
 * Player may be proning. While proning, it may just idle(doesn't move), walk while proning -by this we mean crawling slowly-, or run while proning -meaning crawling fast-.
 *
 * So as you can see, 2 main categories are implemented for player state machine logic. First MovementStates, which means if player's idling, walking or running. Second, StanceStates, which means if player's standing up,
 * crouching or proning.
 *
 * These two events would be fired with defined player states on the player controlling classes, and then the other scripts will do the rest! 
 * Listener(s): 
 * 1) RHC_BobController -> Listens to movementStateChanged & stanceStateChanged. 
 * 2) RHC_CameraStatePositioner -> Listens to stanceStateChanged.
 *
 *
 * When using your own custom player controller, just remember to fire call static methods PSVO_MovementStateChanged & PSVO_StanceStateChanged accordingly. The scripts will handle the rest! 
 * More detail about implementation is explained in the docs!
 * **********
 *
 * **********
 * recoilShake: This event would be fired off whenever you want to simulate hit shake(camera recoil) when the player is hit or simulate firing recoil. Just call PSVO_RecoilShake() static method from anywhere!
 * The variables on the RHC_CameraShake class gives the shake effect accordingly.
 * Listener(s):
 * 1) RHC_CameraShake -> Listens to recoilShake.
 * **********
 *
 * **********
 * genericShake: This event would be fired off whenever you want to simulate generic shakes on the camera, like explosions or earthquakes. Just call PSVO_CallGenericShake(Vector3 sourcePosition) static method from anywhere!
 * Listener(s):
 * 1) RHC_CameraShake -> Listens to genericShake & customShake & recoilShake.
 *
 * **********
 * customShake: This event would be fired off whenever you want to simulate generic shakes on the camera, like explosions or earthquakes, by using the input parameters. Just call PSVO_CallCustomShake(Vector3 posShakeAmount, Vector3 posShakeSpeed, Vector3 rotShakeAmount, Vector3 rotShakeSpeed, float * shakeTime) static method from anywhere!
 * Listener(s):
 * 1) RHC_CameraShake -> Listens to customShake & customShake & recoilShake.
 */

using UnityEngine;

public static class RHC_EventManager
{
    public enum MovementState { Idling, Walking, Running, Sliding, JumpKicking, Lunging };                                  // Movement states.
    public static MovementState en_CurrentMovementState;

    public enum StanceState { Standing, Crouching, OnAir, Recovering, CrouchRecovering };                                // Stance states.
    public static StanceState en_CurrentStanceState;

    public delegate void MovementStateActions(MovementState currentMovementState);           // Movement state actions, 1 event is defined as default(movementStateChanged). Would be called when player stops moving, starts walking, running etc.
    public static MovementStateActions movementStateChanged;

    public delegate void StanceStateActions(StanceState currentMovementState);               // Stance state actions, 1 event is defined as default(stanceStateChanged). Would be called when player stands up, crouches, prones etc.
    public static StanceStateActions stanceStateChanged;

    public delegate void PlayerActions();                                                           // Player actions, 1 event is defined as default(recoilShake). Would be called whenever you want to simulate hit shakes(camera recoil).
    public static event PlayerActions recoilShake;

    public delegate void GenericShakes(Vector3 pos);                                             // Distance sorted generic shake actions, 1 event is defined as default(somethingExploded). Would be called whenever you want to simulate explosion shakes.
    public static event GenericShakes genericShake;


    public delegate void CustomShake(Vector3 posShakeAmount, Vector3 posShakeSpeed, Vector3 rotShakeAmount, Vector3 rotShakeSpeed, float shakeTime);            // Explosion actions, 1 event is defined as default(somethingExploded). Would be called whenever you want to simulate explosion shakes.
    public static event CustomShake customShake;

    public delegate void CustomShakePreset(RHC_GenericShakePreset preset);                      // Non-Distance sorted generic shake actions.
    public static event CustomShakePreset customShakePreset;

    public delegate void ChangeBobPreset(RHC_BobController.BobSettings settingToChange, RHC_BobPresets presetToChange);
    public static event ChangeBobPreset bobChangedPreset;

    public delegate void ChangeBobCustom(RHC_BobController.BobSettings settingToChange, RHC_BobController.BobStyle bobStyle, Vector3 posAmount, Vector3 posSpeed, Vector3 rotAmount, Vector3 rotSpeed, float posSmooth, float rotSmooth);
    public static event ChangeBobCustom bobChangedCustom;

    /// <summary> Used to fire off movement state change event. </summary>
    /// <param name="changeTo"> Which state is the player changed to? </param>
    public static void PSVO_ChangeMovementState(MovementState changeTo)
    {
        if (changeTo == en_CurrentMovementState)
            return;

        // Set the storage for enumaration.
        en_CurrentMovementState = changeTo;

        // Fire off event.
        if (movementStateChanged != null)
            movementStateChanged(changeTo);
    }

    /// <summary> Used to fire off stance state change event. </summary>
    /// <param name="changeTo"> Which state is the player changed to? </param>
    public static void PSVO_ChangeStanceState(StanceState changeTo)
    {
        // Set the storage for enumaration.
        en_CurrentStanceState = changeTo;

        // Fire off event.
        if (stanceStateChanged != null)
            stanceStateChanged(changeTo);
    }

    /// <summary> Used to fire off generic shakes based on the distance params. </summary>
    /// <param name="changeTo"> Source position of the explosion. </param>
    public static void PSVO_CallGenericShake(Vector3 sourcePosition)
    {
        // Fire off event.
        if (genericShake != null)
            genericShake(sourcePosition);
    }

    /// <summary> Used to fire off hit shakes(camera recoil). </summary>
    public static void PSVO_CallRecoil()
    {
        // Fire off event.
        if (recoilShake != null)
            recoilShake();
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
    public static void PSVO_CallCustomShake(Vector3 posShakeAmount, Vector3 posShakeSpeed, Vector3 rotShakeAmount, Vector3 rotShakeSpeed, float shakeTime)
    {
        if (customShake != null)
            customShake(posShakeAmount, posShakeSpeed, rotShakeAmount, rotShakeSpeed, shakeTime);
    }

    ///<summary>
    /// Use this function to implement your own shakes from a preset. 
    ///</summary>
    ///<param name="preset"> Preset to play the shake from </param>
    ///<returns></returns>
    public static void PSVO_CallCustomShake(RHC_GenericShakePreset preset)
    {
        if (customShakePreset != null)
            customShakePreset(preset);
    }

    ///<summary>
    /// Use this function to change a particular bobbing setting's current preset.
    ///</summary>
    ///<param name="settingToChange"> Which bobbing setting to manipulate. </param>
    ///<param name="posShakeAmount"> Select the preset to change to. </param>
    ///<returns></returns>
    public static void PSVO_ChangeBobPreset(RHC_BobController.BobSettings settingToChange, RHC_BobPresets presetToChange)
    {
        // Fire of event if there are listeners.
        if (bobChangedPreset != null)
            bobChangedPreset(settingToChange, presetToChange);
    }

    public static void PSVO_ChangeBobCustom(RHC_BobController.BobSettings settingToChange, RHC_BobController.BobStyle bobStyle, Vector3 posAmount, Vector3 posSpeed, Vector3 rotAmount, Vector3 rotSpeed, float posSmooth, float rotSmooth)
    {
        // Fire of event if there are listeners.
        if (bobChangedCustom != null)
            bobChangedCustom(settingToChange, bobStyle, posAmount, posSpeed, rotAmount, rotSpeed, posSmooth, rotSmooth);
    }
}
