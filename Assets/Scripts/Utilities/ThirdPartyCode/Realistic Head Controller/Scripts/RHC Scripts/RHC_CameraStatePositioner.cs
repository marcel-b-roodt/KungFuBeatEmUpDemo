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
 * This class is a state positioner. Meaning it is used to change the position & rotation of the head/camera, according to the player state. It takes event inputs from the class RHC_EventManager.
 * The event caster class RHC_EventManager has logic to fire off event that says "Player has changed it's Stance/Movement state to this: ". This class listens to that,
 * and accordingly fires off the necessary Coroutine to interpolate the head orientation. You can implement your own state machines & managers. You can still use this class
 * freely & easily as long as you subscribe the method EVO_StanceStateChanged to your own event.
 *
 * This class supports 5 orientations in default:
 * - Default : The position & rotation of the head while standing.
 * - Air: The position & rotation of the head when jumped.
 * - Air Down: The position & rotation of the head when landed. (instant)
 * - Crouch: The position & rotation of the head while crouching.
 * - Prone: The position & rotation of the head while proning.
 *
 * The coolest part is, it's really easy to implement new states according to your own player controller. Just look at the script, copy & paste variables & methods, refactor, and you would be good to go.
 * You can always ask to me about any issue/implementation from the contact informations, including live chat, that were mentioned above.
 *
 */

// Libraries
using UnityEngine;
using System.Collections;

public class RHC_CameraStatePositioner : MonoBehaviour
{
	#region Variable(s)

	/* GENERAL */
	public float f_PosInterpolationSpeed;           // Speed used while processing position interpolations.
	public float f_RotInterpolationSpeed;           // Speed used while processing rotation interpolations.
	private Coroutine co_ChangeState;               // Coroutine to change orientation state.
	private bool b_WasAired;                        // Logic flag used to trigger jump state interpolation.
	private Transform t_This;                       // This transform.

	/* DEFAULT ORIENTATION */
	public Vector3 v3_DefaultPosition;              // Initial position of this transform.
	public Vector3 v3_DefaultRotation;              // Initial ROTer of this transform.    

	/* AIR ORIENTATION */
	public Vector3 v3_AirPosition;                  // Target position to interpolate to when jumped.
	public Vector3 v3_AirRotation;                  // Target rotation to interpolate to when jumped.
	public float f_GoDownFromAirPosSpeed;           // Speed used to interpolate the position of the head from air orientation to "down" orientation. "Down" orientation is the position&rotation to interpolate when "landed".
	public float f_GoDownFromAirRotSpeed;           // Speed used to interpolate the rotation of the head from air orientation to "down" orientation. "Down" orientation is the position&rotation to interpolate when "landed".

	/* AIRDOWN ORIENTATION */
	public Vector3 v3_AirDownPosition;              // Target position to interpolate to when landed.
	public Vector3 v3_AirDownRotation;              // Target rotation to interpolate to when landed.

	/* CROUCH ORIENTATION */
	public Vector3 v3_CrouchingPosition;            // Target position to interpolate to when crouched.
	public Vector3 v3_CrouchingRotation;            // Target rotation to interpolate to when crouched.

	/* PRONE ORIENTATION */
	public Vector3 v3_CrawlingPosition;              // Target position to interpolate to when proned.
	public Vector3 v3_CrawlingRotation;              // Target position to interpolate to when proned.

	/* RECOVERY ORIENTATION */
	public Vector3 v3_RecoveryPosition;            // Target position to interpolate to when crouched.
	public Vector3 v3_RecoveryRotation;            // Target rotation to interpolate to when crouched.

	/* CROUCH RECOVERY ORIENTATION */
	public Vector3 v3_CrouchRecoveryPosition;            // Target position to interpolate to when crouched.
	public Vector3 v3_CrouchRecoveryRotation;            // Target rotation to interpolate to when crouched.

	#endregion

	#region Initializer(s)

	void Awake()
	{
		// Initialize variables.
		t_This = transform;
		v3_DefaultPosition = t_This.localPosition;
		v3_DefaultRotation = t_This.localEulerAngles;
	}

	void OnEnable()
	{
		// Register EVO_StanceStateChanged method to the event that would be fired off from RHC_EventManager.
		// This means that the method EVO_StanceStateChanged() would be called whenever the event stanceStateChanged is fired.
		RHC_EventManager.stanceStateChanged += EVO_StanceStateChanged;
	}

	#endregion

	#region Coroutine(s)

	/// <summary> Coroutine used for changing head orientation to corresponding state. </summary>
	IEnumerator CO_ChangeState(Vector3 targetPos, Vector3 targetRot, bool wasOnAirBefore)
	{
		// If we had jumped before, meaning if this instance of the coroutine is called when "landed". Interpolate to AirDown orientation first.
		if (wasOnAirBefore)
		{
			// Declare & init variables.
			float k = 0.0f;
			float l = 0.0f;
			Vector3 currentPos = t_This.localPosition;
			Quaternion currentRot = t_This.localRotation;

			// Interpolate.
			while (k < 1.0f || l < 1.0f)
			{
				k += Time.deltaTime * f_GoDownFromAirPosSpeed;
				l += Time.deltaTime * f_GoDownFromAirRotSpeed;

				t_This.localPosition = Vector3.Lerp(currentPos, v3_AirDownPosition, k);
				t_This.localRotation = Quaternion.Slerp(currentRot, Quaternion.Euler(v3_AirDownRotation), l);
				yield return null;
			}
		}

		// Record the target orientation.
		Vector3 targetPosition = targetPos;
		Vector3 targetRotation = targetRot;

		float i = 0.0f;
		float j = 0.0f;

		// Record the current orientation.
		Vector3 currentPosition = t_This.localPosition;
		Quaternion currentRotation = t_This.localRotation;

		// Interpolate to the target orientation.
		while (i < 1.0f || j < 1.0f)
		{
			i += Time.deltaTime * f_PosInterpolationSpeed;
			j += Time.deltaTime * f_RotInterpolationSpeed;

			t_This.localPosition = Vector3.Lerp(currentPosition, targetPosition, i);
			t_This.localRotation = Quaternion.Slerp(currentRotation, Quaternion.Euler(targetRotation), j);
			yield return null;
		}
	}

	#endregion

	#region ExternallySet

	/// <summary> This method is called whenever stanceStateChanged event is fired from RHC_EventManager. </summary>
	void EVO_StanceStateChanged(RHC_EventManager.StanceState changedTo)
	{
		// If a change state coroutine is currently present, stop it so that we can start a new one according to the target state.
		if (co_ChangeState != null)
			StopCoroutine(co_ChangeState);

		// Check the target state, set b_WasAired variable accordingly and call the coroutine for interpolating the head orientation.
		if (changedTo == RHC_EventManager.StanceState.Standing)
		{
			co_ChangeState = StartCoroutine(CO_ChangeState(v3_DefaultPosition, v3_DefaultRotation, b_WasAired));
			b_WasAired = false;
		}
		else if (changedTo == RHC_EventManager.StanceState.Crouching)
		{
			co_ChangeState = StartCoroutine(CO_ChangeState(v3_CrouchingPosition, v3_CrouchingRotation, b_WasAired));
			b_WasAired = false;
		}
		else if (changedTo == RHC_EventManager.StanceState.Recovering)
		{
			co_ChangeState = StartCoroutine(CO_ChangeState(v3_RecoveryPosition, v3_RecoveryRotation, b_WasAired));
			b_WasAired = false;
		}
		else if (changedTo == RHC_EventManager.StanceState.CrouchRecovering)
		{
			co_ChangeState = StartCoroutine(CO_ChangeState(v3_CrouchRecoveryPosition, v3_CrouchRecoveryRotation, b_WasAired));
			b_WasAired = false;
		}
		else if (changedTo == RHC_EventManager.StanceState.OnAir)
		{
			co_ChangeState = StartCoroutine(CO_ChangeState(v3_AirPosition, v3_AirRotation, false));
			b_WasAired = true;
		}
	}

	#endregion

	#region Finalizer(s)

	void OnDisable()
	{
		// Make sure that no coroutine objects are yielding when this component is disabled.
		StopAllCoroutines();

		// Unsubscribe listener method from the state change event in RHC_EventManager.
		RHC_EventManager.stanceStateChanged -= EVO_StanceStateChanged;
	}

	#endregion
}
