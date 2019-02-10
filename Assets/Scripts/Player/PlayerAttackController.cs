using System;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
	public const float JumpKickAllowanceTime = 0.2f;
	public const float SlideKickAllowanceTime = 0.2f;

	public float MaxChargeAttackDamageMultiplier = 2.5f;
	public float ChargeAttackMinimumChargePercentage = 0.3f;
	public float ChargeAttackLungeMinimumChargePercentage = 0.6f;

	//Attack Motion Times - Convert these to frames?
	public float BlockMotionTime = 0.5f;
	public float BasicAttackMotionTime = 0.3f;
	public float AttackFullyChargedMotionTime = 2.5f;
	public float ChargeAttackMotionTime = 0.6f;
	public float JumpKickAttackMotionTime = 15f;
	public float SlideKickAttackMotionTime = 0.6f;

	//Attack Cooldown Times
	public float BlockCooldown = 0.3f;
	public float BasicAttackCooldown = 0.1f;
	public float JumpKickAttackCooldown = 0.5f;
	public float SlideKickAttackCooldown = 0.5f;

	private float attackChargePercentage = 0f;
	private bool blockInputHandled = false;

	public PlayerAttackState CurrentPlayerState { get; private set; }
	public PlayerAttackState PreviousCharacterState { get; private set; }
	public float TimeEnteredState { get; private set; }
	public float ZoomLevelFromMovementState { get { return 1; } }
	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	private PlayerStatus playerStatus;
	private PlayerAnimationManager playerAnimationManager;
	private PlayerAttackManager playerAttackManager;
	private PlayerInteractionManager playerInteractionManager;
	private PlayerMovementController playerMovementController;

	private Player.PlayerInputs _bufferedInputs;

	private PlayerMovementState MovementState { get { return playerMovementController.CurrentPlayerState; } }
	private float TimeSinceEnteringMovementState { get { return playerMovementController.TimeSinceEnteringState; } }
	private bool IsRecovering { get { return playerMovementController.FallRecoveryStatus != FallRecoveryStatus.Default; } }

	void Awake()
	{
		playerStatus = GetComponent<PlayerStatus>();
		playerAnimationManager = GetComponent<PlayerAnimationManager>();
		playerAttackManager = GetComponent<PlayerAttackManager>();
		playerInteractionManager = GetComponent<PlayerInteractionManager>();
		playerMovementController = GetComponent<PlayerMovementController>();

		TransitionToState(PlayerAttackState.Idle);
	}

	public void TransitionToState(PlayerAttackState newState)
	{
		PreviousCharacterState = CurrentPlayerState;
		OnStateExit(PreviousCharacterState, newState);
		CurrentPlayerState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousCharacterState);
	}

	public void OnStateEnter(PlayerAttackState state, PlayerAttackState fromState)
	{
		switch (state)
		{
			case PlayerAttackState.Idle:
				{
					playerAttackManager.ClearEnemiesHit();
					playerAnimationManager.ResetAnimatorParameters();
					break;
				}
			case PlayerAttackState.Blocking:
				{
					blockInputHandled = true;
					//playerAnimationManager.ExecuteBlock();
					break;
				}
			case PlayerAttackState.BasicAttacking:
				{
					//playerAnimationManager.ExecuteBasicAttack();
					playerAttackManager.BasicAttack(); //Move this to a connecting frame in the animation
					break;
				}
			case PlayerAttackState.ChargingAttack:
				{
					//playerAnimationManager.ChargeUpAttack();
					attackChargePercentage = 0f;
					break;
				}
			case PlayerAttackState.ChargeAttacking:
				{
					if (attackChargePercentage >= ChargeAttackLungeMinimumChargePercentage)
					{
						//if (playerMovementStateMachine.InCrouchingState)
						//	playerMovementStateMachine.CrouchLunge();
						//else
						//	playerMovementStateMachine.Lunge();
					}

					//playerAnimationManager.ExecuteChargeAttack();
					break;
				}
			case PlayerAttackState.JumpKicking:
				{
					break;
				}
			case PlayerAttackState.SlideKicking:
				{
					break;
				}
		}
	}

	public void OnStateExit(PlayerAttackState state, PlayerAttackState toState)
	{
		switch (state)
		{

		}
	}

	internal void SetInputs(ref Player.PlayerInputs inputs)
	{
		_bufferedInputs = inputs;

		switch (CurrentPlayerState)
		{
			case PlayerAttackState.Idle:
				{
					if (!IsRecovering)
					{
						if (inputs.PrimaryFire)
						{
							if (MovementState == PlayerMovementState.Sliding && TimeSinceEnteringMovementState <= SlideKickAllowanceTime)
							{
								TransitionToState(PlayerAttackState.SlideKicking);
							}
						}

						if (inputs.SecondaryFire)
						{

						}

						if (inputs.Interact)
						{

						}
					}
					break;
				}
		}
	}

	//Pre-movement update
	public void BeforeCharacterUpdate(float deltaTime)
	{

	}

	//Post-movement update
	public void AfterCharacterUpdate(float deltaTime)
	{
		//Check if the MovementController is still in the sames state that it should be.
		//Update the State Machine here if the movement no longer allows the attack state
		switch (CurrentPlayerState)
		{
			case PlayerAttackState.Idle:
				{
					break;
				}
			case PlayerAttackState.SlideKicking:
				{

					break;
				}
		}
	}

	public void OnGroundHit()
	{

	}

	public void OnMovementHit()
	{

	}

	public void OnLanded()
	{

	}

	public void OnLeaveStableGround()
	{

	}

	//	#region AttackStates

	//	#region Idle
	//	void Idle_SuperUpdate()
	//	{
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

public enum PlayerAttackState
{
	Idle,
	Blocking,
	BasicAttacking,
	ChargingAttack,
	ChargeAttacking,
	JumpKicking,
	SlideKicking
}