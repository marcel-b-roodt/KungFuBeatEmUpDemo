using SensorToolkit;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyAI : EnemyAI
{
	public enum EnemyAIState
	{
		Patrolling,
		Investigating,
		Caution,
		Searching,
		Alerted
	}

	public Transform[] Waypoints;
	public float WaypointWaitTime = 3f;

	private int _waypointIndex = 0;

	public TriggerSensor FieldOfViewSensor;

	private EnemyAIState PreviousEnemyState;
	private EnemyAIState CurrentEnemyState;
	private float TimeEnteredState;

	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	private AlarmManager _alarmManager;
	private GameObject _player;
	private Transform _shotSource;

	public override void Awake()
	{
		base.Awake();
		_shotSource = gameObject.FindObjectInChildren("ShotSource").transform;

		Destination = Waypoints[_waypointIndex];
	}

	private void Start()
	{
		_alarmManager = Helpers.GetManagers().AlarmManager;
		_player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);
	}

	public void TransitionToState(EnemyAIState newState)
	{
		PreviousEnemyState = CurrentEnemyState;
		OnStateExit(PreviousEnemyState, newState);
		CurrentEnemyState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousEnemyState);
	}

	public void OnStateEnter(EnemyAIState state, EnemyAIState fromState)
	{
		switch (state)
		{
			case EnemyAIState.Patrolling:
				MoveToNextWaypoint();
				SetLookPosition(Vector3.zero);
				break;
		}
	}

	public void OnStateExit(EnemyAIState state, EnemyAIState toState)
	{
		switch (state)
		{

		}
	}

	public override void Update()
	{
		base.Update();

		if (_alarmManager.IsAlarmState && CurrentEnemyState != EnemyAIState.Alerted)
			TransitionToState(EnemyAIState.Alerted);

		switch (CurrentEnemyState)
		{
			case EnemyAIState.Patrolling:
				break;
			case EnemyAIState.Investigating:
				break;
			case EnemyAIState.Caution:
				break;
			case EnemyAIState.Searching:
				break;
			case EnemyAIState.Alerted:
				break;
		}
	}

	public void MoveToNextWaypoint()
	{
		_waypointIndex++;

		if (_waypointIndex >= Waypoints.Length)
			_waypointIndex = 0;

		Destination = Waypoints[_waypointIndex];
	}

	public void SetLookPosition(Vector3 lookPosition)
	{
		TargetLookPosition = lookPosition;
	}

	private GameObject GetSpottedEnemy()
	{
		var entities = FieldOfViewSensor.GetDetectedByComponent<Player>();
		return entities.FirstOrDefault()?.gameObject;
	}

	public override void Attack(float damage)
	{

	}
}
