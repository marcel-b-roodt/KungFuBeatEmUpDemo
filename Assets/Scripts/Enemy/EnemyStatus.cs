using System;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyStatus : MonoBehaviour
{

	[SerializeField]
	protected float Health;

	private EnemyAI enemyAI;

	public HealthState state { get; private set; }

	public string stateName { get { return state.ToString(); } }

	void Start () {
		enemyAI = GetComponent<EnemyAI>();
		state = HealthState.FreeMoving;
	}

	#region HealthState
	internal void TakeDamage(float damage)
	{
		Health -= damage;
		enemyAI.wasAttacked = true;

		if (Health <= 0)
			Die();
	}

	internal virtual void Die()
	{
		state = HealthState.Dead;
		Destroy(gameObject);
	}
	#endregion

	#region AIState
	public bool IsDead()
	{
		return state == HealthState.Dead;
	}

	public bool IsKnockedBack()
	{
		return state == HealthState.KnockedBack;
	}

	internal bool IsStaggered()
	{
		return state == HealthState.Staggered;
	}

	internal bool IsFreeMoving()
	{
		return state == HealthState.FreeMoving;
	}

	public void BecomeFreeMoving()
	{
		state = HealthState.FreeMoving;
	}

	public void BecomeKnockedBack()
	{
		state = HealthState.KnockedBack;
	}

	internal void BecomeStaggered()
	{
		state = HealthState.Staggered;
	}
	#endregion
}
