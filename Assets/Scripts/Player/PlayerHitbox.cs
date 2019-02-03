using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
	PlayerAttackController playerAttackController;
	PlayerAttackManager playerAttackManager;

	void Start()
	{
		playerAttackController = GetComponentInParent<PlayerAttackController>();
		playerAttackManager = GetComponentInParent<PlayerAttackManager>();
	}

	public void OnTriggerStay(Collider collider)
	{
		var attackableComponent = collider.gameObject.GetAttackableComponent();
		if (attackableComponent != null)
		{
			//switch((PlayerAttackState)playerController.CurrentState)
			//{
			//	case PlayerAttackState.BasicAttacking:
			//		//playerAttackManager.BasicAttack(attackableComponent);
			//		return;
			//	case PlayerAttackState.Grappling:
			//		//playerAttackManager.JumpKick(attackableComponent);
			//		return;
			//	default:
			//		return;
			//}
		}
	}
}
