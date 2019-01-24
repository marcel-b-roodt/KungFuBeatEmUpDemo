using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyAI: MonoBehaviour
{
	[ReadOnly] public bool isAlerted;
	[ReadOnly] public bool wasAttacked;
	public Transform Destination; //Change this for where the character wants to move
	public Vector3 TargetLookPosition;

	private NavMeshPath _path;
	private Vector3[] _pathCorners = new Vector3[16];
	private Vector3 _lastValidDestination;
	private EnemyMovementController _enemyController;

	private Animator animator;
	private EnemyStatus status;
	private NavMeshAgent navAgent;

	public virtual void Awake()
	{
		animator = gameObject.GetComponentInChildren<Animator>();
		status = gameObject.GetComponent<EnemyStatus>();
		navAgent = gameObject.GetComponent<NavMeshAgent>();
		navAgent.isStopped = true;

		_enemyController = gameObject.GetComponent<EnemyMovementController>();
		_path = new NavMeshPath();
	}

	public virtual void Update()
	{
		var enemyInputs = HandleCharacterNavigation();
		SetControllerInputs(ref enemyInputs);
	}

	public void SetControllerInputs(ref EnemyMovementInputs enemyInputs)
	{
		_enemyController.SetInputs(ref enemyInputs);
	}

	public virtual void Attack(float damage)
	{

	}

	public void ReceiveAttack(float damage)
	{
		status.TakeDamage(damage);
	}

	public List<IAttackable> CheckInstantFrameHitboxForPlayer(BoxCollider hitbox, out bool hasEnemy)
	{
		Vector3 size = hitbox.size / 2;
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		ExtDebug.DrawBox(hitbox.transform.position + hitbox.transform.forward * 0.5f, size, hitbox.transform.rotation, Color.blue);
		int layerMask = LayerMask.GetMask(Helpers.Layers.Player);
		Collider[] colliders = Physics.OverlapBox(hitbox.transform.position + hitbox.transform.forward * 0.5f, size, hitbox.transform.rotation, layerMask);
		var results = new List<IAttackable>();

		foreach (Collider collider in colliders)
		{
			if (collider.tag == Helpers.Tags.Player)
			{
				var attackableComponent = collider.gameObject.GetAttackableComponent();
				if (attackableComponent != null)
					results.Add(attackableComponent);
			}
		}

		hasEnemy = results.Count > 0;
		return results;
	}

	internal virtual void Die()
	{
		navAgent.isStopped = true;
	}

	public struct EnemyMovementInputs
	{
		public Vector3 MoveVector;
		public Vector3 TargetLookPosition;
	}

	private EnemyMovementInputs HandleCharacterNavigation()
	{
		EnemyMovementInputs enemyInputs = new EnemyMovementInputs();

		if (NavMesh.CalculatePath(_enemyController.transform.position, Destination.position, NavMesh.AllAreas, _path))
		{
			_lastValidDestination = Destination.position;
		}
		else
		{
			NavMesh.CalculatePath(_enemyController.transform.position, _lastValidDestination, NavMesh.AllAreas, _path);
		}

		int cornersCount = _path.GetCornersNonAlloc(_pathCorners);
		if (cornersCount > 1)
		{
			enemyInputs.MoveVector = (_pathCorners[1] - _enemyController.transform.position).normalized;
		}
		else
		{
			enemyInputs.MoveVector = Vector3.zero;
		}

		enemyInputs.TargetLookPosition = TargetLookPosition;
		return enemyInputs;
	}
}
