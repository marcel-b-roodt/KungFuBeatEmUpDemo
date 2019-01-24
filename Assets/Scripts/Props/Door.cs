using UnityEngine;
using System.Collections;
using SensorToolkit;

public class Door : MonoBehaviour
{
	public enum DoorOpeningType
	{
		OpenVertically,
		OpenHorizontally
	}

	public enum DoorSecurityLevel
	{
		None = 0,
		Level1 = 1, 
		Level2 = 2,
		Level3 = 3,
		Level4 = 4,
		Level5 = 5,
		Level6 = 6,
		Level7 = 7
	}

	public Sensor ObjectSensor;
	public DoorOpeningType OpeningType;
	public DoorSecurityLevel SecurityLevel;
	public GameObject LeftDoor;
	public GameObject RightDoor;
	public float SlideAmount;
	public float Speed;

	Vector3 leftStart;
	Vector3 rightStart;


	void Start()
	{
		leftStart = LeftDoor.transform.localPosition;
		if (RightDoor != null)
			rightStart = RightDoor.transform.localPosition;

		StartCoroutine(ClosingState());
	}

	IEnumerator ClosingState()
	{
		Start:

		var nearestEntity = ObjectSensor.GetNearest();
		if (nearestEntity != null)
		{
			//Handle Door Security Level here Logic here
			StartCoroutine(OpeningState()); yield break;
		}

		LeftDoor.transform.localPosition = Vector3.Lerp(LeftDoor.transform.localPosition, leftStart, Time.deltaTime * Speed);
		if (RightDoor != null)
			RightDoor.transform.localPosition = Vector3.Lerp(RightDoor.transform.localPosition, rightStart, Time.deltaTime * Speed);

		yield return null;
		goto Start;
	}

	IEnumerator OpeningState()
	{
		Start:

		if (ObjectSensor.GetNearest() == null)
		{
			StartCoroutine(ClosingState()); yield break;
		}

		LeftDoor.transform.localPosition = Vector3.Lerp(LeftDoor.transform.localPosition, 
			leftStart - (OpeningType == DoorOpeningType.OpenHorizontally ? Vector3.right : Vector3.down) * SlideAmount, Time.deltaTime * Speed);
		if (RightDoor != null)
			RightDoor.transform.localPosition = Vector3.Lerp(RightDoor.transform.localPosition, 
			rightStart + (OpeningType == DoorOpeningType.OpenHorizontally ? Vector3.right : Vector3.down) * SlideAmount, Time.deltaTime * Speed);

		yield return null;
		goto Start;
	}
}
