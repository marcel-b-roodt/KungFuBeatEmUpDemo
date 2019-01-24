using System;
using UnityEngine;

public class PlayerAttackStateMachine : MonoBehaviour//SuperStateMachine
{
	//private PlayerStatus playerStatus;
	//private PlayerAnimationManager playerAnimationManager;
	//private PlayerAttackManager playerAttackManager;
	////private PlayerInputManager playerInputManager;
	//private PlayerInteractionManager playerInteractionManager;
	////private PlayerMovementStateMachine playerMovementStateMachine;

	//public float MaxChargeAttackDamageMultiplier = 2.5f;
	//public float ChargeAttackMinimumChargePercentage = 0.3f;
	//public float ChargeAttackLungeMinimumChargePercentage = 0.6f;

	////Attack Motion Times - Convert these to frames?
	//public float BasicAttackMotionTime = 0.3f;
	//public float GrappleMotionTime = 1.2f;

	////Attack Cooldown Times
	//public float BasicAttackCooldown = 0.1f;

	//public Enum CurrentState { get { return currentState; } private set { ChangeState(); currentState = value; } }

	//public float TimeSinceEnteringCurrentState { get { return Time.time - timeEnteredState; } }

	//#region PropertyGetters
	//public bool CanAttack
	//{
	//	get
	//	{
	//		return true;
	//		//return !((PlayerMovementState)playerMovementStateMachine.CurrentState == PlayerMovementState.Sliding ||
	//		//		(PlayerMovementState)playerMovementStateMachine.CurrentState == PlayerMovementState.Jumping ||
	//		//		(PlayerMovementState)playerMovementStateMachine.CurrentState == PlayerMovementState.Falling);
	//	}
	//}
	//#endregion

	//private void ChangeState()
	//{
	//	lastState = state.currentState;
	//	timeEnteredState = Time.time;
	//}

	//void Awake()
	//{
	//	playerStatus = GetComponent<PlayerStatus>();
	//	playerAnimationManager = GetComponent<PlayerAnimationManager>();
	//	playerAttackManager = GetComponent<PlayerAttackManager>();
	//	//playerInputManager = GetComponent<PlayerInputManager>();
	//	playerInteractionManager = GetComponent<PlayerInteractionManager>();
	//	//playerMovementStateMachine = GetComponent<PlayerMovementStateMachine>();
	//	CurrentState = PlayerAttackState.Idle;
	//}

	//protected override void EarlyGlobalSuperUpdate()
	//{

	//}

	//protected override void LateGlobalSuperUpdate()
	//{
	//	//Debug.Log($"Time in state: {TimeSinceEnteringCurrentState}");
	//}

	//#region AttackStates

	//#region Idle
	//void Idle_EnterState()
	//{
	//	playerAnimationManager.ResetAnimatorParameters();
	//	playerAttackManager.ClearEnemiesHit();
	//}

	//void Idle_SuperUpdate()
	//{
	//	//if (playerMovementStateMachine.IsRecovering)
	//	//	return;

	//	//if (playerInputManager.Current.PrimaryFireInput)
	//	//{
	//	//	PrimaryAttack();
	//	//	return;
	//	//}

	//	//if (playerInputManager.Current.SecondaryFireInput)
	//	//{
	//	//	Grapple();
	//	//	return;
	//	//}

	//	//if (playerInputManager.Current.InteractInput)
	//	//{
	//	//	playerInteractionManager.Interact();
	//	//	return;
	//	//}
	//}
	//#endregion

	//#region BasicAttacking
	//void BasicAttacking_EnterState()
	//{
	//	//playerAnimationManager.ExecuteBasicAttack();
	//	playerAttackManager.BasicAttack(); //Move this to a connecting frame in the animation
	//}

	//void BasicAttacking_SuperUpdate()
	//{
	//	if (TimeSinceEnteringCurrentState >= BasicAttackMotionTime)
	//	{
	//		CurrentState = PlayerAttackState.Idle;
	//		return;
	//	}
	//}
	//#endregion

	//#region Grappling
	//void Grappling_EnterState()
	//{
	//	playerAnimationManager.ExecuteGrapple();
	//	playerAttackManager.Grapple(); //Grab the target we wish to grapple.
	//}

	//void Grappling_SuperUpdate()
	//{
	//	if (TimeSinceEnteringCurrentState >= GrappleMotionTime)
	//	{
	//		CurrentState = PlayerAttackState.Idle;
	//		return;
	//	}

	//	//Make Grapple Attempt

	//	//Determine which type of grapple we need to apply to the player from the AttackManager.
	//	//See which move we should perform. Let the enemy have a chance to counter it if they know what we're doing.
	//	//Perform the attack
	//}
	//#endregion

	//#endregion

	//internal void PrimaryAttack()
	//{
	//	if (playerInteractionManager.HoldingObject)
	//	{
	//		playerInteractionManager.Throw();
	//		//Animate player throw?
	//	}
	//	else
	//	{
	//		if (CanAttack)
	//		{
	//			CurrentState = PlayerAttackState.BasicAttacking;
	//			return;
	//		}
	//	}
	//}

	//internal void Grapple()
	//{
	//	if (playerInteractionManager.HoldingObject)
	//	{
	//		playerInteractionManager.Drop();
	//		//Animate player drop?
	//	}
	//	else
	//	{
	//		if (CanAttack)
	//		{

	//			CurrentState = PlayerAttackState.Grappling;
	//			return;
	//		}
	//	}
	//}
}

public enum PlayerAttackState
{
	Idle,
	Blocking,
	BasicAttacking,
	Grappling
}