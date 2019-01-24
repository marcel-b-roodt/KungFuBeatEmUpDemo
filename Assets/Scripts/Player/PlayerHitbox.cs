using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
	PlayerAttackStateMachine playerController;
	PlayerAttackManager playerAttackManager;

	void Start()
	{
		playerController = GetComponentInParent<PlayerAttackStateMachine>();
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
