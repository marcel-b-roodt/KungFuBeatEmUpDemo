using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IAttackable
{
	public float MaxHealth;
	[ReadOnly] public float Health;

	public GameObject PlayerHUD;

	//private PlayerMovementStateMachine playerMovementStateMachine;

	private float knockbackRecoveryFraction = 3f;
	private float currentKnockbackRecoveryTime = 0f;
	private float staggerKnockbackVelocity = 2f;
	private float currentStaggerRecoveryTime = 0.1f;

	public HealthState healthState { get; private set; }

	public string stateName { get { return healthState.ToString(); } }

	void Awake()
	{
		healthState = HealthState.FreeMoving;
		Health = MaxHealth;
	}

	void Start()
	{
		PlayerHUD = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD);
	}

	public void ReceiveAttack(float damage)
	{
		TakeDamage(damage);
	}

	public void ReceiveStaggerAttack(float damage, Vector3 staggerDirection, float staggerRecoveryTime)
	{
		BecomeStaggered();

		currentStaggerRecoveryTime = staggerRecoveryTime;
		//animator.SetBool("Staggered", true);
		//playerMovementStateMachine.moveDirection += staggerDirection * staggerKnockbackVelocity;

		TakeDamage(damage);
	}

	public void ReceiveKnockbackAttack(float damage, Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime)
	{
		BecomeKnockedBack();

		currentKnockbackRecoveryTime = knockbackTime;
		//animator.SetBool("KnockedBack", true);
		//playerMovementStateMachine.moveDirection += knockbackDirection * knockbackVelocity;

		TakeDamage(damage);
	}

	#region HealthState
	internal void TakeDamage(float damage)
	{
		Health -= damage;

		if (Health <= 0)
			Die();
	}

	internal virtual void Die()
	{
		healthState = HealthState.Dead;
	}
	#endregion

	#region PlayerState
	public bool IsDead()
	{
		return healthState == HealthState.Dead;
	}

	public bool IsKnockedBack()
	{
		return healthState == HealthState.KnockedBack;
	}

	internal bool IsStaggered()
	{
		return healthState == HealthState.Staggered;
	}

	internal bool IsFreeMoving()
	{
		return healthState == HealthState.FreeMoving;
	}

	public void BecomeFreeMoving()
	{
		healthState = HealthState.FreeMoving;
	}

	public void BecomeKnockedBack()
	{
		healthState = HealthState.KnockedBack;
	}

	internal void BecomeStaggered()
	{
		healthState = HealthState.Staggered;
	}
	#endregion
}
