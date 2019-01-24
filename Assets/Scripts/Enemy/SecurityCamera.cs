using UnityEngine;
using System.Collections;
using SensorToolkit;
using System.Linq;

public class SecurityCamera : MonoBehaviour
{
	public enum CameraState
	{
		ScanningLeft,
		ScanningRight,
		Tracking,
		Alert
	}

	private float rotationSpeed;
	public float ScanTime;
	public float WaitTime;
	public float TrackTime;
	public float SearchTime;
	public float ScanArcAngle;
	public Light SpotLight;
	public TriggerSensor FieldOfViewSensor;
	public RangeSensor RangeSensor;
	public Color ScanColour;
	public Color TrackColour;
	public Color AlarmColour;

	Quaternion leftExtreme;
	Quaternion rightExtreme;
	Quaternion targetRotation;

	private CameraState PreviousCameraState;
	private CameraState CurrentCameraState;
	private float TimeEnteredState;

	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	private AlarmManager alarmManager;
	private GameObject player;
	private float waitTimer;
	private float detectionTimer;
	private float searchTimer;

	private bool lostSightOfPlayer = true;

	void Awake()
	{
		leftExtreme = Quaternion.AngleAxis(ScanArcAngle / 2f, Vector3.up) * transform.rotation;
		rightExtreme = Quaternion.AngleAxis(-ScanArcAngle / 2f, Vector3.up) * transform.rotation;
		TransitionToState(CameraState.ScanningRight);

		rotationSpeed = ScanArcAngle / ScanTime;
	}

	private void Start()
	{
		alarmManager = Helpers.GetManagers().AlarmManager;
		player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);
	}

	public void TransitionToState(CameraState newState)
	{
		PreviousCameraState = CurrentCameraState;
		OnStateExit(PreviousCameraState, newState);
		CurrentCameraState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousCameraState);
	}

	public void OnStateEnter(CameraState state, CameraState fromState)
	{
		switch (state)
		{
			case CameraState.ScanningLeft:
				{
					SpotLight.color = ScanColour;
					targetRotation = leftExtreme;
					waitTimer = 0f;
					lostSightOfPlayer = true;
					break;
				}
			case CameraState.ScanningRight:
				{
					SpotLight.color = ScanColour;
					targetRotation = rightExtreme;
					waitTimer = 0f;
					lostSightOfPlayer = true;
					break;
				}
			case CameraState.Tracking:
				{
					SpotLight.color = TrackColour;
					detectionTimer = 0f;
					searchTimer = 0f;
					lostSightOfPlayer = false;
					break;
				}
			case CameraState.Alert:
				{
					SpotLight.color = AlarmColour;
					targetRotation = transform.rotation;
					break;
				}
		}
	}

	public void OnStateExit(CameraState state, CameraState toState)
	{
		switch (state)
		{
			case CameraState.ScanningLeft:
				{
					break;
				}
			case CameraState.ScanningRight:
				{
					break;
				}
			case CameraState.Tracking:
				{
					break;
				}
			case CameraState.Alert:
				{
					break;
				}
		}
	}

	void Update()
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

		if (alarmManager.IsAlarmState && CurrentCameraState != CameraState.Alert)
		{
			if (CurrentCameraState != CameraState.Alert || CurrentCameraState != CameraState.Tracking)
				PreviousCameraState = CurrentCameraState;

			TransitionToState(CameraState.Alert);
		}

		switch (CurrentCameraState)
		{
			case CameraState.ScanningLeft:
				{
					if (GetSpottedEnemy())
						TransitionToState(CameraState.Tracking);

					if (transform.rotation == leftExtreme)
					{
						waitTimer += Time.deltaTime;
						if (waitTimer >= WaitTime)
							TransitionToState(CameraState.ScanningRight);
					}
					break;
				}
			case CameraState.ScanningRight:
				{
					if (GetSpottedEnemy())
						TransitionToState(CameraState.Tracking);

					if (transform.rotation == rightExtreme)
					{
						waitTimer += Time.deltaTime;
						if (waitTimer >= WaitTime)
							TransitionToState(CameraState.ScanningLeft);
					}
					break;
				}
			case CameraState.Tracking:
				{
					if (searchTimer < SearchTime)
					{
						if (RangeSensor.IsDetected(player) && !lostSightOfPlayer)
							targetRotation = LookRotationWithClamp(transform.position - player.transform.position);
						else
							lostSightOfPlayer = true;

						if (FieldOfViewSensor.IsDetected(player))
						{
							searchTimer = 0;
							detectionTimer += Time.deltaTime;
							if (detectionTimer >= TrackTime)
							{
								alarmManager.StartAlarm();
								TransitionToState(CameraState.Alert);
								break;
							}
						}
						else
						{
							searchTimer += Time.deltaTime;
							if (searchTimer >= SearchTime)
							{
								TransitionToState(PreviousCameraState);
								break;
							}
						}
					}
					break;
				}
			case CameraState.Alert:
				{
					if (!alarmManager.IsAlarmState)
					{
						TransitionToState(GetClosestExtremeToView());
						break;
					}
					else
					{
						if (RangeSensor.IsDetected(player))
							targetRotation = LookRotationWithClamp(transform.position - player.transform.position);
						else
							targetRotation = transform.rotation;
						break;
					}
				}
		}
	}

	private GameObject GetSpottedEnemy()
	{
		var entities = FieldOfViewSensor.GetDetectedByComponent<Player>();
		return entities.FirstOrDefault()?.gameObject;
	}

	private CameraState GetClosestExtremeToView()
	{
		var leftExtremeAngle = Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.y, leftExtreme.eulerAngles.y));
		var rightExtremeAngle = Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rightExtreme.eulerAngles.y));

		if (leftExtremeAngle <= rightExtremeAngle)
			return CameraState.ScanningLeft;
		else
			return CameraState.ScanningRight;
	}

	private Quaternion LookRotationWithClamp(Vector3 target)
	{
		var rotation = Quaternion.LookRotation(target, Vector3.up);
		var min = Mathf.Min(leftExtreme.eulerAngles.y, rightExtreme.eulerAngles.y);
		var max = Mathf.Max(leftExtreme.eulerAngles.y, rightExtreme.eulerAngles.y);
		var minDeltaAngle = Mathf.Abs(Mathf.DeltaAngle(rotation.eulerAngles.y, min));
		var maxDeltaAngle = Mathf.Abs(Mathf.DeltaAngle(rotation.eulerAngles.y, max));

		float targetYRotation = rotation.eulerAngles.y;
		if (targetYRotation < min || targetYRotation > max)
		{
			if (minDeltaAngle <= maxDeltaAngle)
				targetYRotation = min;
			else
				targetYRotation = max;
		}

		rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, targetYRotation, rotation.eulerAngles.z);
		return rotation;
	}
}
