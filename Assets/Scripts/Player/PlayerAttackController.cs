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
		//TODO: Implement Freeze-frame attacks maybe?
		//Do it on contact with the enemy. Quickly freeze time scale until later
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
					playerAnimationManager.ExecuteBlock();
					break;
				}
			case PlayerAttackState.BasicAttacking:
				{
					playerAnimationManager.ExecuteBasicAttack();
					playerAttackManager.BasicAttack(); //Move this to a connecting frame in the animation
					break;
				}
			case PlayerAttackState.ChargingAttack:
				{
					playerAnimationManager.ChargeUpAttack();
					attackChargePercentage = 0f;
					break;
				}
			case PlayerAttackState.ChargeAttacking:
				{
					if (attackChargePercentage >= ChargeAttackLungeMinimumChargePercentage)
					{
						if (playerMovementController.IsCrouching && playerMovementController.Uppercut())
							playerAnimationManager.ExecuteChargeAttack(); //TODO: Make this a proper uppercutting "jump" attack
						else if (playerMovementController.Lunge())
							playerAnimationManager.ExecuteChargeAttack();
					}
					break;
				}
			case PlayerAttackState.JumpKicking:
				{
					playerAnimationManager.ExecuteJumpKick();
					break;
				}
			case PlayerAttackState.SlideKicking:
				{
					playerAnimationManager.ExecuteSlideKick();
					break;
				}
		}
	}

	public void OnStateExit(PlayerAttackState state, PlayerAttackState toState)
	{
		switch (state)
		{
			case PlayerAttackState.ChargingAttack:
				{
					playerAnimationManager.ResetAnimationSpeed();
					break;
				}
		}
	}

	internal void SetInputs(ref Player.PlayerInputs inputs)
	{
		_bufferedInputs = inputs;

		switch (CurrentPlayerState)
		{
			case PlayerAttackState.Idle:
				{
					if (!playerMovementController.IsRecovering)
					{
						if (inputs.PrimaryFire)
						{
							if (playerInteractionManager.HoldingObject)
							{
								playerInteractionManager.Throw();
								//Animate player throw?
								return;
							}
							else if (MovementState == PlayerMovementState.Sliding && TimeSinceEnteringMovementState <= SlideKickAllowanceTime)
							{
								TransitionToState(PlayerAttackState.SlideKicking);
								return;
							}
							else if (!playerMovementController.IsGrounded
								&& playerMovementController.TimeSinceGrounded < JumpKickAllowanceTime
									 && playerMovementController.LocalMovementIsForwardFacing)
							{
								//Just check that the jump check is correct
								TransitionToState(PlayerAttackState.JumpKicking);
								return;
							}
							else
							{
								TransitionToState(PlayerAttackState.ChargingAttack);
								return;
							}
						}

						if (inputs.SecondaryFire)
						{
							TransitionToState(PlayerAttackState.Blocking);
							return;
						}

						if (inputs.Interact)
						{
							playerInteractionManager.Interact();
							return;
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
			case PlayerAttackState.Blocking:
				{
					if (TimeSinceEnteringState >= BlockMotionTime)
					{
						TransitionToState(PlayerAttackState.Idle);
						return;
					}

					break;
				}
			case PlayerAttackState.ChargingAttack:
				{
					attackChargePercentage = Mathf.Min(TimeSinceEnteringState / AttackFullyChargedMotionTime, 1f);
					playerAnimationManager.ChangeAnimationSpeed(Mathf.Lerp(0.5f, 1f, attackChargePercentage));

					if (!_bufferedInputs.PrimaryFireHold && attackChargePercentage < ChargeAttackMinimumChargePercentage)
					{
						TransitionToState(PlayerAttackState.BasicAttacking);
						return;
					}

					if (attackChargePercentage >= ChargeAttackMinimumChargePercentage && !_bufferedInputs.PrimaryFireHold)
					{
						TransitionToState(PlayerAttackState.ChargeAttacking);
						return;
					}

					//Debug.Log($"Attack Charge Percentage: {attackChargePercentage}");
					break;
				}
			case PlayerAttackState.BasicAttacking:
				{
					if (TimeSinceEnteringState >= BasicAttackMotionTime)
					{
						TransitionToState(PlayerAttackState.Idle);
						return;
					}

					break;
				}
			case PlayerAttackState.ChargeAttacking:
				{
					if (TimeSinceEnteringState >= ChargeAttackMotionTime) //&& (PlayerMovementState)playerMovementStateMachine.CurrentState != PlayerMovementState.Lunging)
					{
						TransitionToState(PlayerAttackState.Idle);
						return;
					}

					break;
				}
			case PlayerAttackState.JumpKicking:
				{
					if (TimeSinceEnteringState >= JumpKickAttackMotionTime || playerMovementController.IsGrounded)
					{
						TransitionToState(PlayerAttackState.Idle);
						return;
					}
					break;
				}
			case PlayerAttackState.SlideKicking:
				{
					if (TimeSinceEnteringState >= SlideKickAttackMotionTime || playerMovementController.CurrentPlayerState != PlayerMovementState.Sliding)
					{
						TransitionToState(PlayerAttackState.Idle);
						return;
					}

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