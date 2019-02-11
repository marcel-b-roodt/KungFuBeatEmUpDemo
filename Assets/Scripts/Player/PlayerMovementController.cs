using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public enum PlayerMovementState
{
	Default,
	Sliding,
	Lunging
}

public enum FallRecoveryStatus
{
	Default,
	Recovering,
	CrouchRecovering
}

public class PlayerMovementController : BaseCharacterController
{
	[Header("Stable Movement")]
	public float MaxStableRunSpeed = 5f;
	public float MaxStableCrouchRunSpeed = 3f;
	public float MaxStableWalkSpeed = 3f;
	public float MaxStableCrouchWalkSpeed = 2f;
	public float StableMovementSharpness = 12;
	public float OrientationSharpness = 12;
	public float WalkSpeedPercentage = 0.5f;

	[Header("Air Movement")]
	public float MaxAirMoveSpeed = 6f;
	public float AirAccelerationSpeed = 3f;
	public float Drag = 0.1f;

	[Header("Jumping")]
	public bool AllowJumpingWhenSliding = true;
	public const float JumpHeight = 1.3f;
	public float JumpPreGroundingGraceTime = 0f;
	public float JumpPostGroundingGraceTime = 0f;
	public float FallRecoveryThreshold = 2.3f * JumpHeight;
	public float FallCrouchRecoveryThreshold = 2.6f * JumpHeight;
	public float FallRecoveryTime = 0.15f;
	public float FallCrouchRecoveryTime = 0.25f;
	public FallRecoveryStatus FallRecoveryStatus { get; private set; } = FallRecoveryStatus.Default;

	[Header("Sliding")]
	public float SlideVelocityThreshold = 2f;
	public float SlideRotationSpeed = 1.25f;
	public float SlideSpeed = 10f;
	public float MaxSlideTime = 0.6f;
	public float StoppedTime = 0.3f;

	[Header("Misc")]
	public List<Collider> IgnoredColliders;
	public bool OrientTowardsGravity = false;
	public Vector3 Gravity = new Vector3(0, -25f, 0);

	public PlayerMovementState CurrentPlayerState { get; private set; }
	public PlayerMovementState PreviousCharacterState { get; private set; }
	public float TimeEnteredState { get; private set; }
	public float ZoomLevelFromMovementState { get { return 1; } }
	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	private Vector3 _currentSlideVelocity;
	private bool _startedSlide;
	private bool _isSlideStopped;
	private float _timeSinceStopped = 0;

	private Collider[] _probedColliders = new Collider[8];
	private Vector3 _moveInputVector;
	private Vector3 _lookInputVector;
	private bool _jumpRequested = false;
	private bool _jumpConsumed = false;
	private bool _jumpedThisFrame = false;
	private float _jumpStartYPosition = 0f;
	private float _timeSinceJumpRequested = Mathf.Infinity;
	private float _timeSinceLastAbleToJump = 0f;
	private float _fallRecoveryTime = Mathf.Infinity;
	private Vector3 _internalVelocityAdd = Vector3.zero;
	private bool _shouldBeCrouching = false;
	private bool _isCrouching = false;
	private bool _isWalking = false;

	private float _timeLeftStableGround;

	private Player.PlayerInputs _bufferedInputs;

	private PlayerAttackController playerAttackController;
	private PlayerAnimationManager playerAnimationManager;

	public bool IsGrounded { get { return Motor.GroundingStatus.IsStableOnGround; } }
	public float TimeSinceGrounded { get { return IsGrounded ? 0 : Time.time - _timeLeftStableGround; } }
	public bool IsCrouching { get { return _isCrouching; } }
	public bool IsRecovering { get { return FallRecoveryStatus != FallRecoveryStatus.Default; } }
	public bool MustWalk { get { return playerAttackController.CurrentPlayerState == PlayerAttackState.ChargingAttack; } }
	public bool LocalMovementIsForwardFacing { get { return _bufferedInputs.MoveAxisForward > 0; } }

	private void Start()
	{
		playerAttackController = GetComponent<PlayerAttackController>();
		playerAnimationManager = GetComponent<PlayerAnimationManager>();

		TransitionToState(PlayerMovementState.Default);

		if (IgnoredColliders == null)
			IgnoredColliders = new List<Collider>();
	}

	public void TransitionToState(PlayerMovementState newState)
	{
		PreviousCharacterState = CurrentPlayerState;
		OnStateExit(PreviousCharacterState, newState);
		CurrentPlayerState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousCharacterState);
	}

	public void OnStateEnter(PlayerMovementState state, PlayerMovementState fromState)
	{
		switch (state)
		{
			case PlayerMovementState.Default:
				{
					break;
				}
			case PlayerMovementState.Sliding:
				{
					//TODO: Make slide speed a velocity curve
					_currentSlideVelocity = _moveInputVector * SlideSpeed;
					_startedSlide = true;
					_isSlideStopped = false;
					_timeSinceStopped = 0f;

					HandleCrouching();
					playerAnimationManager.SetSlide(true);
					break;
				}
		}
	}

	public void OnStateExit(PlayerMovementState state, PlayerMovementState toState)
	{
		switch (state)
		{
			case PlayerMovementState.Default:
				{
					break;
				}
			case PlayerMovementState.Sliding:
				{
					if (!Motor.GroundingStatus.IsStableOnGround || !_bufferedInputs.CrouchHold)
						HandleCrouching();

					playerAnimationManager.SetSlide(false);
					break;
				}
		}
	}

	/// <summary>
	/// This is called every frame by PlayerController in order to tell the character what its inputs are
	/// </summary>
	public void SetInputs(ref Player.PlayerInputs inputs)
	{
		_bufferedInputs = inputs;

		// Clamp input
		Vector3 moveInputVector = new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward).normalized;
		playerAnimationManager.SetMovement(inputs.Walk ? moveInputVector.magnitude * WalkSpeedPercentage : moveInputVector.magnitude);

		// Calculate camera direction and rotation on the character plane
		Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
		if (cameraPlanarDirection.sqrMagnitude == 0f)
		{
			cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
		}
		Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

		// Handle state transition from input
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					// Move and look inputs
					_moveInputVector = cameraPlanarRotation * moveInputVector;

					// Jumping input
					if (inputs.Jump)
					{
						_timeSinceJumpRequested = 0f;
						_jumpRequested = true;
					}

					if (inputs.Crouch && Motor.GroundingStatus.IsStableOnGround)
					{
						if (Motor.Velocity.magnitude >= SlideVelocityThreshold)
						{
							TransitionToState(PlayerMovementState.Sliding);
						}
						else
						{
							HandleCrouching();
						}
					}

					if (inputs.Walk || MustWalk)
						_isWalking = true;
					else
						_isWalking = false;

					break;
				}
			case PlayerMovementState.Sliding:
				{
					// Move and look inputs
					_moveInputVector = cameraPlanarRotation * moveInputVector;

					break;
				}
		}

		playerAttackController.SetInputs(ref inputs);
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is called before the character begins its movement update
	/// </summary>
	public override void BeforeCharacterUpdate(float deltaTime)
	{
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					if (FallRecoveryStatus != FallRecoveryStatus.Default)
					{
						_fallRecoveryTime += deltaTime;
					}
					break;
				}
			case PlayerMovementState.Sliding:
				{
					// Update times
					if (_isSlideStopped)
					{
						_timeSinceStopped += deltaTime;
					}
					break;
				}
		}

		playerAttackController.BeforeCharacterUpdate(deltaTime);
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is where you tell your character what its rotation should be right now. 
	/// This is the ONLY place where you should set the character's rotation
	/// </summary>
	public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
					{
						// Smoothly interpolate from current to target look direction
						Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

						// Set the current rotation (which will be used by the KinematicCharacterMotor)
						currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
					}
					if (OrientTowardsGravity)
					{
						// Rotate from current up to invert gravity
						currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
					}
					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is where you tell your character what its velocity should be right now. 
	/// This is the ONLY place where you can set the character's velocity
	/// </summary>
	public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					Vector3 targetMovementVelocity = Vector3.zero;

					if (FallRecoveryStatus != FallRecoveryStatus.Default)
					{
						_moveInputVector = Vector3.zero;
						_jumpRequested = false;
					}

					// Ground movement
					if (Motor.GroundingStatus.IsStableOnGround)
					{
						Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
						if (currentVelocity.sqrMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
						{
							// Take the normal from where we're coming from
							Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
							if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
							{
								effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
							}
							else
							{
								effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
							}
						}

						// Reorient velocity on slope
						currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;

						// Calculate target velocity
						Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
						Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;

						if (_isWalking && _isCrouching)
							targetMovementVelocity = reorientedInput * MaxStableCrouchWalkSpeed;
						else if (!_isWalking && _isCrouching)
							targetMovementVelocity = reorientedInput * MaxStableCrouchRunSpeed;
						else if (_isWalking && !_isCrouching)
							targetMovementVelocity = reorientedInput * MaxStableWalkSpeed;
						else if (!_isWalking && !_isCrouching)
							targetMovementVelocity = reorientedInput * MaxStableRunSpeed;

						// Smooth movement Velocity
						currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
					}
					// Air movement
					else
					{
						// Add move input
						if (_moveInputVector.sqrMagnitude > 0f)
						{
							targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

							// Prevent climbing on un-stable slopes with air movement
							if (Motor.GroundingStatus.FoundAnyGround)
							{
								Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
								targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
							}

							Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
							currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
						}

						// Gravity
						currentVelocity += Gravity * deltaTime;

						// Drag
						currentVelocity *= (1f / (1f + (Drag * deltaTime)));
					}

					// Handle jumping
					_jumpedThisFrame = false;
					_timeSinceJumpRequested += deltaTime;
					if (_jumpRequested)
					{
						// See if we actually are allowed to jump
						if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
						{
							// Calculate jump direction before ungrounding
							Vector3 jumpDirection = Motor.CharacterUp;
							if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
							{
								jumpDirection = Motor.GroundingStatus.GroundNormal;
							}

							// Makes the character skip ground probing/snapping on its next update. 
							Motor.ForceUnground();

							// Add to the return velocity and reset jump state
							currentVelocity += (jumpDirection * CalculateJumpSpeed(JumpHeight, Gravity.magnitude)) - Vector3.Project(currentVelocity, Motor.CharacterUp);
							_jumpRequested = false;
							_jumpConsumed = true;
							_jumpedThisFrame = true;
						}
					}

					// Take into account additive velocity
					if (_internalVelocityAdd.sqrMagnitude > 0f)
					{
						currentVelocity += _internalVelocityAdd;
						_internalVelocityAdd = Vector3.zero;
					}

					break;
				}
			case PlayerMovementState.Sliding:
				{
					if (_isSlideStopped)
					{
						// When stopped, do no velocity handling except gravity
						currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
						currentVelocity += Gravity * deltaTime;
					}
					else
					{
						Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

						// Calculate target velocity
						//bool movingForward = _bufferedInputs.MoveAxisForward > 0;
						Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
						Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;

						// When starting to slide, set velocity to max
						if (_startedSlide)
						{
							currentVelocity = _currentSlideVelocity;
							_startedSlide = false;
						}

						if (reorientedInput.magnitude > 0)
						{
							//TODO: Fix this rotation some time. See if something is better
							currentVelocity = Vector3.RotateTowards(currentVelocity, currentVelocity.magnitude * reorientedInput.normalized, Mathf.Deg2Rad * SlideRotationSpeed, currentVelocity.magnitude);
							currentVelocity = Vector3.Lerp(currentVelocity, currentVelocity.magnitude * reorientedInput.normalized, 1 - Mathf.Exp(-SlideRotationSpeed * deltaTime));
						}
							
						currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 0.01f);
					}
					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is called after the character has finished its movement update
	/// </summary>
	public override void AfterCharacterUpdate(float deltaTime)
	{
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					// Handle jump-related values
					{
						// Handle jumping pre-ground grace period
						if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
						{
							_jumpRequested = false;
						}

						if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
						{
							// If we're on a ground surface, reset jumping values
							if (!_jumpedThisFrame)
							{
								_jumpConsumed = false;
							}
							else
							{
								if (_shouldBeCrouching)
									HandleCrouching();
							}

							_timeSinceLastAbleToJump = 0f;
						}
						else
						{
							// Keep track of time since we were last able to jump (for grace period)
							_timeSinceLastAbleToJump += deltaTime;
						}

						if (IsRecovering)
						{
							switch (FallRecoveryStatus)
							{
								case FallRecoveryStatus.Recovering:
									if (_fallRecoveryTime >= FallRecoveryTime)
										FallRecoveryStatus = FallRecoveryStatus.Default;
									break;
								case FallRecoveryStatus.CrouchRecovering:
									if (_fallRecoveryTime >= FallCrouchRecoveryTime)
										FallRecoveryStatus = FallRecoveryStatus.Default;
									break;
							}
						}
					}

					if (!_bufferedInputs.CrouchHold && _shouldBeCrouching)
						HandleCrouching();

					// Handle uncrouching
					HandleUncrouching();

					break;
				}
			case PlayerMovementState.Sliding:
				{
					// Detect being stopped by elapsed time
					if (!_isSlideStopped && TimeSinceEnteringState > MaxSlideTime)
					{
						_isSlideStopped = true;
					}

					// Detect end of stopping phase and transition back to default movement state
					if (_timeSinceStopped > StoppedTime)
					{
						TransitionToState(PlayerMovementState.Default);
					}
					break;
				}
		}

		playerAttackController.AfterCharacterUpdate(deltaTime);
	}

	public override void PostGroundingUpdate(float deltaTime)
	{
		// Handle landing and leaving ground
		if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
		{
			OnLanded();
		}
		else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
		{
			OnLeaveStableGround();
		}
	}

	public override bool IsColliderValidForCollisions(Collider coll)
	{
		if (IgnoredColliders.Count >= 0)
		{
			return true;
		}

		if (IgnoredColliders.Contains(coll))
		{
			return false;
		}
		return true;
	}

	public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		playerAttackController.OnGroundHit();
	}

	public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Sliding:
				{
					// Detect being stopped by obstructions
					if (!_isSlideStopped && !hitStabilityReport.IsStable && Vector3.Dot(-hitNormal, _currentSlideVelocity.normalized) > 0.5f)
					{
						_isSlideStopped = true;
					}
					break;
				}
		}

		playerAttackController.OnMovementHit();
	}

	public void AddVelocity(Vector3 velocity)
	{
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					_internalVelocityAdd += velocity;
					break;
				}
		}
	}

	public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	protected void OnLanded()
	{
		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					var fallHeight = _jumpStartYPosition - transform.position.y;
					_fallRecoveryTime = 0f;

					if (fallHeight >= FallRecoveryThreshold)
					{
						if (fallHeight >= FallCrouchRecoveryThreshold)
						{
							FallRecoveryStatus = FallRecoveryStatus.CrouchRecovering;
							RHC_EventManager.PSVO_ChangeStanceState(RHC_EventManager.StanceState.CrouchRecovering);
						}
						else
						{
							FallRecoveryStatus = FallRecoveryStatus.Recovering;
							RHC_EventManager.PSVO_ChangeStanceState(RHC_EventManager.StanceState.Recovering);
						}

						if (_bufferedInputs.CrouchHold)
							HandleCrouching();
					}

					break;
				}
		}

		playerAttackController.OnLanded();
	}

	protected void OnLeaveStableGround()
	{
		_timeLeftStableGround = Time.time;

		switch (CurrentPlayerState)
		{
			case PlayerMovementState.Default:
				{
					_jumpStartYPosition = transform.position.y;
					break;
				}
			case PlayerMovementState.Sliding:
				{
					TransitionToState(PlayerMovementState.Default);
					break;
				}
		}

		playerAttackController.OnLeaveStableGround();
	}

	private bool HandleCrouching()
	{
		if (!_shouldBeCrouching)
		{
			_shouldBeCrouching = true;

			if (!_isCrouching)
			{
				RHC_EventManager.PSVO_ChangeStanceState(RHC_EventManager.StanceState.Crouching);
				_isCrouching = true;
				SetCrouchingDimensions();
				playerAnimationManager.SetCrouch(true);
			}

			return true;
		}
		else
		{
			_shouldBeCrouching = false;
			return false;
		}

	}

	private void HandleUncrouching()
	{
		if (_isCrouching && !_shouldBeCrouching)
		{
			// Do an overlap test with the character's standing height to see if there are any obstructions
			SetStandingDimensions();
			if (Motor.CharacterOverlap(
				Motor.TransientPosition,
				Motor.TransientRotation,
				_probedColliders,
				Motor.CollidableLayers,
				QueryTriggerInteraction.Ignore) > 0)
			{
				// If obstructions, just stick to crouching dimensions
				SetCrouchingDimensions();
			}
			else
			{
				// If no obstructions, uncrouch
				RHC_EventManager.PSVO_ChangeStanceState(RHC_EventManager.StanceState.Standing);
				_isCrouching = false;
				playerAnimationManager.SetCrouch(false);
			}
		}
	}

	private float CalculateJumpSpeed(float jumpHeight, float gravity)
	{
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	private void SetCrouchingDimensions()
	{
		Motor.SetCapsuleDimensions(0.3f, 0.9f, 0.45f);
	}

	private void SetStandingDimensions()
	{
		Motor.SetCapsuleDimensions(0.3f, 1.8f, 0.9f);
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawWireSphere(_targetClimbUpPosition, 0.1f);
	//}
}
