using System;
using UnityEngine;

public class PlayerAttackStateMachine : MonoBehaviour
{
//	public const float JUMP_KICK_ALLOWANCE_TIME = 0.2f;

//	private PlayerStatus playerStatus;
//	private PlayerAnimationManager playerAnimationManager;
//	private PlayerAttackManager playerAttackManager;
//	private PlayerInputManager playerInputManager;
//	private PlayerInteractionManager playerInteractionManager;
//	private PlayerMovementStateMachine playerMovementStateMachine;

//	public float MaxChargeAttackDamageMultiplier = 2.5f;
//	public float ChargeAttackMinimumChargePercentage = 0.3f;
//	public float ChargeAttackLungeMinimumChargePercentage = 0.6f;

//	//Attack Motion Times - Convert these to frames?
//	public float BlockMotionTime = 0.5f;
//	public float BasicAttackMotionTime = 0.3f;
//	public float AttackFullyChargedMotionTime = 2.5f;
//	public float ChargeAttackMotionTime = 0.6f;
//	public float JumpKickAttackMotionTime = 15f;
//	public float SlideKickAttackMotionTime = 0.6f;

//	//Attack Cooldown Times
//	public float BlockCooldown = 0.3f;
//	public float BasicAttackCooldown = 0.1f;
//	public float JumpKickAttackCooldown = 0.5f;
//	public float SlideKickAttackCooldown = 0.5f;

//	private float attackChargePercentage = 0f;
//	private bool blockInputHandled = false;

//	public Enum CurrentState { get { return currentState; } private set { ChangeState(); currentState = value; } }

//	public float TimeSinceEnteringCurrentState { get { return Time.time - timeEnteredState; } }

//	#region PropertyGetters

//	public float DamageMultiplier { get { return attackChargePercentage * MaxChargeAttackDamageMultiplier; } }

//	public float AttackChargePercentage { get { return attackChargePercentage; } }

//	#endregion

//	private void ChangeState()
//	{
//		lastState = state.currentState;
//		timeEnteredState = Time.time;
//	}

//	void Awake()
//	{
//		playerStatus = GetComponent<PlayerStatus>();
//		playerAnimationManager = GetComponent<PlayerAnimationManager>();
//		playerAttackManager = GetComponent<PlayerAttackManager>();
//		playerInputManager = GetComponent<PlayerInputManager>();
//		playerInteractionManager = GetComponent<PlayerInteractionManager>();
//		playerMovementStateMachine = GetComponent<PlayerMovementStateMachine>();
//		CurrentState = PlayerAttackState.Idle;
//	}

//	protected override void EarlyGlobalSuperUpdate()
//	{

//	}

//	protected override void LateGlobalSuperUpdate()
//	{
//		if (Input.GetButtonUp(InputCodes.SecondaryFire))
//			blockInputHandled = false;

//		//Debug.Log($"Time in state: {TimeSinceEnteringCurrentState}");
//	}

//	#region AttackStates

//	#region Idle
//	void Idle_EnterState()
//	{
//		playerAttackManager.ClearEnemiesHit();
//		playerAnimationManager.ResetAnimatorParameters();
//	}

//	void Idle_SuperUpdate()
//	{
//		if (playerMovementStateMachine.IsRecovering)
//			return;

//		if (playerInputManager.Current.PrimaryFireInput)
//		{
//			Attack();
//			return;
//		}

//		if (playerInputManager.Current.SecondaryFireInput && !blockInputHandled)
//		{
//			CurrentState = PlayerAttackState.Blocking;
//			return;
//		}

//		if (playerInputManager.Current.InteractInput)
//		{
//			playerInteractionManager.Interact();
//			return;
//		}
//	}
//	#endregion

//	#region Blocking
//	void Blocking_EnterState()
//	{
//		blockInputHandled = true;
//		playerAnimationManager.ExecuteBlock();
//	}

//	void Blocking_SuperUpdate()
//	{
//		if (TimeSinceEnteringCurrentState >= BlockMotionTime)
//		{
//			CurrentState = PlayerAttackState.Idle;
//			return;
//		}
//	}

//	#endregion

//	#region BasicAttacking
//	void BasicAttacking_EnterState()
//	{
//		playerAnimationManager.ExecuteBasicAttack();
//		playerAttackManager.BasicAttack(); //Move this to a connecting frame in the animation
//	}

//	void BasicAttacking_SuperUpdate()
//	{
//		if (TimeSinceEnteringCurrentState >= BasicAttackMotionTime)
//		{
//			CurrentState = PlayerAttackState.Idle;
//			return;
//		}
//	}
//	#endregion

//	#region ChargingAttack
//	void ChargingAttack_EnterState()
//	{
//		playerAnimationManager.ChargeUpAttack();
//		attackChargePercentage = 0f;
//	}

//	void ChargingAttack_SuperUpdate()
//	{
//		attackChargePercentage = Mathf.Min(TimeSinceEnteringCurrentState / AttackFullyChargedMotionTime, 1f);
//		playerAnimationManager.ChangeAnimationSpeed(Mathf.Lerp(0.5f, 1f, attackChargePercentage));

//		if (!playerInputManager.Current.PrimaryFireInput && attackChargePercentage < ChargeAttackMinimumChargePercentage)
//		{
//			CurrentState = PlayerAttackState.BasicAttacking;
//			return;
//		}

//		if (attackChargePercentage >= ChargeAttackMinimumChargePercentage && !playerInputManager.Current.PrimaryFireInput)
//		{
//			CurrentState = PlayerAttackState.ChargeAttacking;
//			return;
//		}

//		//Debug.Log($"Attack Charge Percentage: {attackChargePercentage}");
//	}

//	void ChargingAttack_ExitState()
//	{
//		playerAnimationManager.ResetAnimationSpeed();
//	}
//	#endregion

//	#region ChargeAttacking
//	void ChargeAttacking_EnterState()
//	{
//		if (attackChargePercentage >= ChargeAttackLungeMinimumChargePercentage)
//		{
//			if (playerMovementStateMachine.InCrouchingState)
//				playerMovementStateMachine.CrouchLunge();
//			else
//				playerMovementStateMachine.Lunge();
//		}

//		playerAnimationManager.ExecuteChargeAttack();
//	}

//	void ChargeAttacking_SuperUpdate()
//	{
//		if (TimeSinceEnteringCurrentState >= ChargeAttackMotionTime && (PlayerMovementState)playerMovementStateMachine.CurrentState != PlayerMovementState.Lunging)
//		{
//			CurrentState = PlayerAttackState.Idle;
//			return;
//		}
//	}
//	#endregion

//	#region JumpKicking
//	void JumpKicking_EnterState()
//	{
//		playerAnimationManager.ExecuteJumpKick();
//	}

//	void JumpKicking_SuperUpdate()
//	{
//		if (TimeSinceEnteringCurrentState >= JumpKickAttackMotionTime || (PlayerMovementState)playerMovementStateMachine.CurrentState != PlayerMovementState.Jumping)
//		{
//			CurrentState = PlayerAttackState.Idle;
//			return;
//		}
//	}
//	#endregion

//	#region SlideKicking
//	void SlideKicking_EnterState()
//	{
//		playerAnimationManager.ExecuteSlideKick();
//	}

//	void SlideKicking_SuperUpdate()
//	{
//		if (TimeSinceEnteringCurrentState >= SlideKickAttackMotionTime || (PlayerMovementState)playerMovementStateMachine.CurrentState != PlayerMovementState.Sliding)
//		{
//			CurrentState = PlayerAttackState.Idle;
//			return;
//		}
//	}
//	#endregion

//	#endregion

//	internal void Attack()
//	{
//		if (playerInteractionManager.HoldingObject)
//		{
//			playerInteractionManager.Throw();
//			//Animate player throw?
//		}
//		else
//		{
//			if ((PlayerMovementState)playerMovementStateMachine.CurrentState == PlayerMovementState.Sliding)
//			{
//				CurrentState = PlayerAttackState.SlideKicking;
//				return;
//			}
//			else if ((PlayerMovementState)playerMovementStateMachine.CurrentState == PlayerMovementState.Jumping
//				&& playerMovementStateMachine.TimeSinceEnteringCurrentState < JUMP_KICK_ALLOWANCE_TIME
//				&& playerMovementStateMachine.LocalMovementIsForwardFacing)
//			{
//				CurrentState = PlayerAttackState.JumpKicking;
//				return;
//			}
//			else
//			{
//				CurrentState = PlayerAttackState.ChargingAttack;
//				return;
//			}
//		}
//	}
}

//public enum PlayerAttackState
//{
//	Idle,
//	Blocking,
//	BasicAttacking,
//	ChargingAttack,
//	ChargeAttacking,
//	JumpKicking,
//	SlideKicking
//}