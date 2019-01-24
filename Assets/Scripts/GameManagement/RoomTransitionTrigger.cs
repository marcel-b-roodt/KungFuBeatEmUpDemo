using SensorToolkit;
using System.Linq;
using UnityEngine;

public class RoomTransitionTrigger : MonoBehaviour
{
	public RoomTransitionID DoorID;
	public RoomTransitionID DestinationID;

	public bool SpawnOnly = false;

	private Sensor playerSensor;
	private bool transitionActive = false;

	private float startTime;
	private float delayTime = 2;

	void Awake()
	{
		playerSensor = GetComponent<RangeSensor>();
	}

	private void Start()
	{
		startTime = Time.time;
	}

	void Update()
	{
		if (!SpawnOnly)
		{
			if (!transitionActive && Time.time - startTime >= delayTime)
			{
				if (!GetDetectedPlayer())
					transitionActive = true;
			}

			if (transitionActive && GetDetectedPlayer())
			{
				TransitionToLevel();
			}
		}
	}

	public void DisableRoomTransitionTrigger()
	{
		transitionActive = false;
	}

	public void EnableRoomTransitionTrigger()
	{
		transitionActive = true;
	}

	private void TransitionToLevel()
	{
		Helpers.GetManagers().PlayerTransitionManager.TransitionToLevel(DestinationID);
	}

	private GameObject GetDetectedPlayer()
	{
		var entities = playerSensor.GetDetectedByComponent<Player>();
		return entities.FirstOrDefault()?.gameObject;
	}
}
