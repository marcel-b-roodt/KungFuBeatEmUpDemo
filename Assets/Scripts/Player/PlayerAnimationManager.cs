using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

	private Animator playerAnimator;

	public Animator PlayerAnimator
	{
		get
		{
			if (playerAnimator == null)
				playerAnimator = GetComponent<Animator>();
			return playerAnimator;
		}
	}

	internal void ResetAnimatorParameters()
	{
		SetAnimationBool(AnimationCodes.Blocking, false);
		SetAnimationBool(AnimationCodes.ChargingAttack, false);
		SetAnimationBool(AnimationCodes.JumpKicking, false);
		SetAnimationBool(AnimationCodes.SlideKicking, false);
		ResetAnimationSpeed();
	}

	internal void ResetAnimationSpeed()
	{
		if (PlayerAnimator != null)
			PlayerAnimator.speed = 1;
	}

	internal void ChangeAnimationSpeed(float multiplier)
	{
		PlayerAnimator.speed = multiplier;
	}

	#region Setters
	internal void SetMovement(float value)
	{
		SetAnimationFloat(AnimationCodes.MovementInput, value);
	}

	internal void SetWalking(bool value)
	{
		SetAnimationBool(AnimationCodes.Walking, value);
	}

	internal void SetCrouch(bool value)
	{
		SetAnimationBool(AnimationCodes.Crouching, value);
	}

	internal void SetSlide(bool value)
	{
		SetAnimationBool(AnimationCodes.Sliding, value);
	}

	internal void ChargeUpAttack()
	{
		SetAnimationBool(AnimationCodes.ChargingAttack, true);
	}

	internal void ExecuteBasicAttack()
	{
		SetAnimationBool(AnimationCodes.ChargingAttack, false);
		SetAnimationInteger(AnimationCodes.BasicAttackIndex, UnityEngine.Random.Range(0, 2));
	}

	internal void ExecuteChargeAttack() //TODO - Make a charged attack animation
	{
		SetAnimationBool(AnimationCodes.ChargingAttack, false);
		SetAnimationInteger(AnimationCodes.BasicAttackIndex, UnityEngine.Random.Range(0, 2));
	}

	internal void ExecuteBlock()
	{
		SetAnimationBool(AnimationCodes.Blocking, true);
	}

	internal void ExecuteJumpKick()
	{
		SetAnimationBool(AnimationCodes.JumpKicking, true);
	}

	internal void ExecuteSlideKick()
	{
		SetAnimationBool(AnimationCodes.SlideKicking, true);
	}
	#endregion

	#region HelperMethods
	private void SetAnimationBool(string name, bool value)
	{
		PlayerAnimator?.SetBool(name, value);
	}

	private void SetAnimationInteger(string name, int value)
	{
		PlayerAnimator?.SetInteger(name, value);
	}

	private void SetAnimationFloat(string name, float value)
	{
		PlayerAnimator?.SetFloat(name, value);
	}

	private void SetAnimationTrigger(string name)
	{
		PlayerAnimator?.SetTrigger(name);
	}
	#endregion

	private static class AnimationCodes
	{
		public const string MovementInput = "MovementInput";
		public const string Crouching = "Crouching";
		public const string Walking = "Walking";
		public const string Sliding = "Sliding";
		public const string Jumping = "Jumping";
		public const string Hanging = "Hanging";
		public const string ClimbingUp = "ClimbingUp";
		public const string Vaulting = "Vaulting";
		public const string SteppingUp = "SteppingUp";

		public const string Blocking = "Blocking";
		public const string ChargingAttack = "ChargingAttack";
		public const string BasicAttackIndex = "BasicAttackIndex";
		public const string JumpKicking = "JumpKicking";
		public const string SlideKicking = "SlideKicking";
	}
}
