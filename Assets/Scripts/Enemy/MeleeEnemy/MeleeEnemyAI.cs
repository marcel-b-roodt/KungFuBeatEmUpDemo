using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : EnemyAI
{
    private BoxCollider attackHitbox;

	public override void Awake()
	{
		base.Awake();
        attackHitbox = Helpers.FindObjectInChildren(gameObject, "AttackHitbox").GetComponent<BoxCollider>();
	}

	public override void Update() //Remove this code
	{
		base.Update();
	}

	internal override void Die() //Remove this code
	{
		base.Die();
	}

    public override void Attack(float damage)
    {
        base.Attack(damage);

        bool hasEnemy;
        List<IAttackable> attackableComponents = CheckInstantFrameHitboxForPlayer(attackHitbox, out hasEnemy);

        if (hasEnemy)
        {
            foreach (IAttackable attackable in attackableComponents)
            {
                attackable.ReceiveAttack(damage);
            }
        }
    }
}
