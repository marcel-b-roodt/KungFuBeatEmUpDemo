using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour, IAttackable
{
	[ReadOnly]
	public bool grabbed;

	#region AttackProperties
	public static float ThrownInteractableDamageVelocityThreshold = 5f;
	public static float ThrownInteractableDamage = 18f;
	public static float ThrownInteractableKnockbackVelocity = 25f;
	public static float ThrownInteractableKnockbackTime = 0.2f;
	#endregion

	private Transform grabbedLocation;
	private new Collider collider;
	private new Rigidbody rigidbody;

	public void Start()
	{
		grabbed = false;
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
	}

	public void Update()
	{
		if (grabbed)
		{
			transform.position = grabbedLocation.position;
		}
	}

	public void ReceiveAttack(float damage)
	{
		//Needs Interactable status to take damage
	}

	public void ReceiveStaggerAttack(float damage, Vector3 staggerDirection, float staggerTime)
	{
		//Needs Interactable status to take damage
	}

	public void ReceiveKnockbackAttack(float damage, Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime)
	{
		this.rigidbody.velocity = knockbackDirection * knockbackVelocity / rigidbody.mass; //Something like this
	}

	public void Grab(Transform grabbedLocation)
	{
		grabbed = true;
		rigidbody.isKinematic = true;
		collider.enabled = false;
		this.grabbedLocation = grabbedLocation;
	}

	public void Drop()
	{
		grabbed = false;
		rigidbody.isKinematic = false;
		collider.enabled = true;
		this.grabbedLocation = null;
	}

	public void OnCollisionEnter(Collision collision)
	{
		var attackableComponent = collision.gameObject.GetAttackableComponent();
		if (attackableComponent != null && collision.gameObject.tag != Helpers.Tags.Player)
		{
			var direction = (collision.gameObject.transform.position - transform.position).normalized;
			var currentVelocity = rigidbody.velocity.magnitude;

			if (currentVelocity >= ThrownInteractableDamageVelocityThreshold)
				attackableComponent.ReceiveKnockbackAttack(ThrownInteractableDamage, direction, ThrownInteractableKnockbackVelocity, ThrownInteractableKnockbackTime);
		}
	}
}
