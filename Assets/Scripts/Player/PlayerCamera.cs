using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCamera : MonoBehaviour
{
	[Header("Rotation")]
	public bool InvertX = false;
	public bool InvertY = false;
	[Range(-90f, 90f)]
	public float DefaultVerticalAngle = 20f;
	[Range(-90f, 90f)]
	public float MinVerticalAngle = -80f;
	[Range(-90f, 90f)]
	public float MaxVerticalAngle = 80f;
	public float RotationSpeed = 10f;
	public float RotationSharpness = 30f;

	[Header("Obstruction")]
	public float ObstructionCheckRadius = 0.5f;
	public LayerMask ObstructionLayers = -1;
	public float ObstructionSharpness = 10000f;

	public Transform Transform { get; private set; }
	public Vector3 PlanarDirection { get; private set; }
	public PlayerMovementController FollowCharacter { get; set; }

	private List<Collider> _internalIgnoredColliders = new List<Collider>();
	private float _currentDistance;
	private float _targetVerticalAngle;
	private RaycastHit _obstructionHit;
	private int _obstructionCount;
	private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
	private float _obstructionTime;

	private const int MaxObstructions = 32;

	void OnValidate()
	{
		DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
	}

	void Awake()
	{
		Transform = this.transform;

		_targetVerticalAngle = 0f;

		PlanarDirection = Vector3.forward;
	}

	private void Start()
	{
		SetFollowCharacter(GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerMovementController>());
	}

	// Set the transform that the camera will orbit around
	public void SetFollowCharacter(PlayerMovementController character)
	{
		FollowCharacter = character;
		PlanarDirection = FollowCharacter.Motor.CharacterForward;

		// Ignore the character's collider(s) for camera obstruction checks
		_internalIgnoredColliders.Clear();
		_internalIgnoredColliders.AddRange(FollowCharacter.GetComponentsInChildren<Collider>());
	}

	public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
	{
		if (FollowCharacter)
		{
			if (InvertX)
			{
				rotationInput.x *= -1f;
			}
			if (InvertY)
			{
				rotationInput.y *= -1f;
			}

			// Process rotation input
			Quaternion rotationFromInput = Quaternion.Euler(FollowCharacter.Motor.CharacterUp * (rotationInput.x * RotationSpeed));
			PlanarDirection = rotationFromInput * PlanarDirection;
			PlanarDirection = Vector3.Cross(FollowCharacter.Motor.CharacterUp, Vector3.Cross(PlanarDirection, FollowCharacter.Motor.CharacterUp));
			_targetVerticalAngle -= (rotationInput.y * RotationSpeed);
			_targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);

			// Calculate smoothed rotation
			Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowCharacter.Motor.CharacterUp);
			Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
			Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

			// Apply rotation
			Transform.rotation = targetRotation;
		}
	}
}