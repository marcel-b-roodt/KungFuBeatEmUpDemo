
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
	private Camera playerCamera;
	private BoxCollider playerHitbox;

	private Interactable heldObject;
	private Transform grabbedLocation;

	public float playerThrowPower = 25f;

	void Awake()
	{
		grabbedLocation = Helpers.FindObjectInChildren(gameObject, "GrabbedLocation").transform;
	}

	private void Start()
	{
		playerCamera = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).GetComponent<Camera>();
		playerHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHitbox).GetComponent<BoxCollider>();
	}

	public bool HoldingObject { get; private set; } = false;

	public void Interact()
	{
		if (HoldingObject)
		{
			Drop();
		}
		else
		{
			GrabInteractableInHitbox();
		}
	}

	public void GrabInteractableInHitbox()
	{
		Vector3 size = playerHitbox.size / 2;
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		ExtDebug.DrawBox(playerHitbox.transform.position + playerHitbox.transform.forward * 0.5f, size, playerHitbox.transform.rotation, Color.blue);
		int layerMask = LayerMask.GetMask(Helpers.Layers.Interactable);
		Collider[] colliders = Physics.OverlapBox(playerHitbox.transform.position + playerHitbox.transform.forward * 0.5f, size, playerHitbox.transform.rotation, layerMask);

		foreach (Collider collider in colliders)
		{
			Interactable newLiftedObject = collider.gameObject.GetComponent<Interactable>();
			if (newLiftedObject == null)
				continue;
			else
			{
				Grab(newLiftedObject);
				return;
			}
		}
	}

	private void Grab(Interactable targetObject)
	{
		HoldingObject = true;
		heldObject = targetObject;
		heldObject.Grab(grabbedLocation);
	}

	public void Drop()
	{
		HoldingObject = false;
		heldObject.grabbed = false;
		heldObject.Drop();
	}

	public void Throw()
	{
		HoldingObject = false;
		heldObject.grabbed = false;
		heldObject.Drop();
		Ray screenRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
		heldObject.GetComponent<Rigidbody>().velocity = screenRay.direction * playerThrowPower; //Factor in the weight of the object
	}
}
