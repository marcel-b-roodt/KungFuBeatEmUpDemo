using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
	#region AttackProperties
	public static float BasicAttackDamage = 5f;
	public static float BasicAttackStaggerTime = 0.2f;
	#endregion

	PlayerAttackStateMachine playerAttackStateMachine;
	PlayerSoundManager playerSoundManager;

	SphereCollider playerHitbox;

	List<IAttackable> enemiesHit = new List<IAttackable>();

	void Awake()
	{
		playerAttackStateMachine = GetComponentInParent<PlayerAttackStateMachine>();
		playerSoundManager = GetComponentInParent<PlayerSoundManager>();
		playerHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHitbox).GetComponent<SphereCollider>();
	}

	public void BasicAttack()
	{
		bool hitEnemy = false;
		List<IAttackable> results = CheckInstantFrameHitboxForEnemies(out hitEnemy);

		foreach (IAttackable attackableComponent in results)
		{
			var damage = BasicAttackDamage + BasicAttackDamage;
			attackableComponent.ReceiveStaggerAttack(damage, transform.forward, BasicAttackStaggerTime);
		}

		if (hitEnemy)
			playerSoundManager.PlayBasicAttackHitSound();
		else
			playerSoundManager.PlayBasicAttackMissSound();
	}

	public void Grapple()
	{
		bool hitEnemy = false;
		List<IAttackable> results = CheckInstantFrameHitboxForEnemies(out hitEnemy);

		var damage = BasicAttackDamage + BasicAttackDamage;
		results.FirstOrDefault()?.ReceiveStaggerAttack(damage, transform.forward, BasicAttackStaggerTime);
	}

	public List<IAttackable> CheckInstantFrameHitboxForEnemies(out bool hasEnemy)
	{
		//Gizmos.color = Color.blue;
		//Gizmos.DrawWireSphere(playerHitbox.transform.position, playerHitbox.radius);
		int layerMask = LayerMask.GetMask(Helpers.Layers.Enemy, Helpers.Layers.Interactable);
		Collider[] colliders = Physics.OverlapSphere(playerHitbox.transform.position, playerHitbox.radius, layerMask);
		var results = new List<IAttackable>();

		foreach (Collider collider in colliders)
		{
			if (collider.tag != Helpers.Tags.Player)
			{
				var attackableComponent = collider.gameObject.GetAttackableComponent();
				if (attackableComponent != null)
					results.Add(attackableComponent);
			}
		}

		hasEnemy = results.Count > 0;
		return results;
	}

	public void ClearEnemiesHit()
	{
		enemiesHit.Clear();
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.blue;
	//	Gizmos.DrawWireSphere(playerHitbox.transform.position, playerHitbox.radius);
	//}
}

public static class PlayerAttackManagerExtensions
{
	public static IAttackable GetAttackableComponent(this GameObject gameObject)
	{
		return gameObject.GetComponentInChildren<IAttackable>();
	}
}
