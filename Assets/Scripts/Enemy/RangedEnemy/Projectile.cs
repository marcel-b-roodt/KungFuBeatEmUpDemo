using UnityEngine;

public class Projectile : MonoBehaviour {

	public float maxVelocity = 15f;
	public float acceleration = 24f;

	[HideInInspector] public float damage;
	[HideInInspector] public int parentId;

	Rigidbody fireballRigidbody;

	void Awake () 
	{
		fireballRigidbody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
		fireballRigidbody.velocity += transform.forward * acceleration * Time.fixedDeltaTime;
		fireballRigidbody.velocity = Vector3.ClampMagnitude(fireballRigidbody.velocity, maxVelocity);
	}

	void OnTriggerEnter(Collider collider)
	{
		var attackableComponent = collider.gameObject.GetAttackableComponent();
		Debug.Log($"Colliding with {collider.gameObject.name}");

		if (attackableComponent != null && collider.gameObject.GetInstanceID() != parentId)
		{
			attackableComponent.ReceiveAttack(damage);
		}
		else
		{
			Debug.Log("No attackable component on object");
		}

		if (collider.gameObject.GetInstanceID() != parentId)
			Destroy(this.gameObject);
	}
}
