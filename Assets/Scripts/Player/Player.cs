using UnityEngine;

public class Player : MonoBehaviour
{
	public float MouseSensitivity = 0.01f;
	private PlayerMovementController playerController;
	private PlayerCamera playerCamera;

	private void Awake()
	{
		playerController = gameObject.GetComponent<PlayerMovementController>();
	}

	private void Start()
	{
		playerCamera = GetComponentInChildren<PlayerCamera>();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Tell camera to follow transform
		playerCamera.SetFollowCharacter(playerController);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		HandleCameraInput();
		HandleInputs();
	}

	private void HandleCameraInput()
	{
		// Create the look input vector for the camera
		float mouseLookAxisUp = Input.GetAxisRaw(InputCodes.MouseYInput);
		float mouseLookAxisRight = Input.GetAxisRaw(InputCodes.MouseXInput);
		Vector3 lookInputVector = new Vector3(mouseLookAxisRight * MouseSensitivity, mouseLookAxisUp * MouseSensitivity, 0f);

		// Prevent moving the camera while the cursor isn't locked
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			lookInputVector = Vector3.zero;
		}

		// Apply inputs to the camera
		playerCamera.UpdateWithInput(Time.deltaTime, playerController.ZoomLevelFromMovementState, lookInputVector);
	}

	private void HandleInputs()
	{
		PlayerInputs inputs = new PlayerInputs();

		inputs.MoveAxisForward = Input.GetAxisRaw(InputCodes.VerticalInput);
		inputs.MoveAxisRight = Input.GetAxisRaw(InputCodes.HorizontalInput);
		inputs.CameraRotation = playerCamera.transform.rotation;
		inputs.Walk = Input.GetButton(InputCodes.Walk);
		inputs.Jump = Input.GetButtonDown(InputCodes.Jump);
		inputs.JumpHold = Input.GetButton(InputCodes.Jump);
		inputs.Crouch = Input.GetButtonDown(InputCodes.Crouch);
		inputs.CrouchHold = Input.GetButton(InputCodes.Crouch);

		inputs.PrimaryFire = Input.GetButtonDown(InputCodes.PrimaryFire);
		inputs.PrimaryFireHold = Input.GetButtonDown(InputCodes.PrimaryFire);
		inputs.SecondaryFire = Input.GetButtonDown(InputCodes.SecondaryFire);
		inputs.SecondaryFireHold = Input.GetButtonDown(InputCodes.SecondaryFire);
		inputs.Interact = Input.GetButtonDown(InputCodes.Interact);
		inputs.InteractHold = Input.GetButtonDown(InputCodes.Interact);

		playerController.SetInputs(ref inputs);
	}

	public struct PlayerInputs
	{
		public float MoveAxisRight;
		public float MoveAxisForward;
		public Quaternion CameraRotation;
		public bool Walk;
		public bool Jump;
		public bool JumpHold;
		public bool Crouch;
		public bool CrouchHold;

		public bool PrimaryFire;
		public bool PrimaryFireHold;
		public bool SecondaryFire;
		public bool SecondaryFireHold;
		public bool Interact;
		public bool InteractHold;
	}

	public static class InputCodes
	{
		public const string HorizontalInput = "Horizontal";
		public const string VerticalInput = "Vertical";
		public const string MouseXInput = "Mouse X";
		public const string MouseYInput = "Mouse Y";
		public const string Walk = "Walk";
		public const string Jump = "Jump";
		public const string Crouch = "Crouch";
		public const string PrimaryFire = "PrimaryFire";
		public const string SecondaryFire = "SecondaryFire";
		public const string Interact = "Interact";
	}
}
