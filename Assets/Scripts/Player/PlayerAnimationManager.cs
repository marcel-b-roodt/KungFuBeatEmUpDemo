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
		SetAnimationBool(AnimationCodes.BasicAttacking, false);
		SetAnimationBool(AnimationCodes.Grappling, false);
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

	internal void SetHanging(bool value)
	{
		SetAnimationBool(AnimationCodes.Hanging, value);
	}

	internal void SetClimbingUp(bool value)
	{
		SetAnimationBool(AnimationCodes.ClimbingUp, value);
	}

	internal void SetVaulting(bool value)
	{
		SetAnimationBool(AnimationCodes.Vaulting, value);
	}

	internal void SetSteppingUp(bool value)
	{
		SetAnimationBool(AnimationCodes.SteppingUp, value);
	}

	internal void ExecuteGrapple()
	{
		SetAnimationBool(AnimationCodes.Grappling, true);
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

		public const string BasicAttacking = "BasicAttacking";
		public const string Grappling = "Grappling";
	}
}
