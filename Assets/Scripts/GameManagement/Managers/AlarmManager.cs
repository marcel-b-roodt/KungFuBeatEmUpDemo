using UnityEngine;
using System.Collections;
using System;

public class AlarmManager : MonoBehaviour
{
	//public Light PointLight;
	public Color AlarmColour;
	public float FlashFrequency;

	private AlarmState PreviousAlarmState;
	private AlarmState CurrentAlarmState;
	private float TimeEnteredState;

	public float TimeSinceEnteringState { get { return Time.time - TimeEnteredState; } }

	public float CautionTime;
	public float EvasionTime;
	public float AlertTime;

	private float currentTime;
	
	public enum AlarmState
	{
		Relaxed,
		Caution,
		Evasion,
		Alert
	}

	public bool IsAlarmState
	{
		get { return CurrentAlarmState == AlarmState.Alert; }
	}

	public void TransitionToState(AlarmState newState)
	{
		PreviousAlarmState = CurrentAlarmState;
		OnStateExit(CurrentAlarmState, newState);
		CurrentAlarmState = newState;
		TimeEnteredState = Time.time;
		OnStateEnter(newState, PreviousAlarmState);
	}

	public void OnStateEnter(AlarmState state, AlarmState fromState)
	{
		currentTime = 0;

		switch (state)
		{
			case AlarmState.Relaxed:
				{
					break;
				}
			case AlarmState.Caution:
				{
					break;
				}
			case AlarmState.Evasion:
				{
					break;
				}
			case AlarmState.Alert:
				{
					//Alert all enemies who must respond. Put them in their Alert State
					//Spawn necessary enemies (maintain minimum number of enemies in the EnemyManager)
					//TODO: Work on enemy state (2 types of enemies - Hunters and Guards)
					//Hunters will chase the player down
					//Guards will protect exit zones or patrol an area
					//Update last player position
					break;
				}
		}
	}

	public void OnStateExit(AlarmState state, AlarmState toState)
	{
		switch (state)
		{
			case AlarmState.Relaxed:
				{
					break;
				}
			case AlarmState.Caution:
				{
					break;
				}
			case AlarmState.Evasion:
				{
					break;
				}
			case AlarmState.Alert:
				{
					break;
				}
		}
	}

	void Update()
	{
		currentTime += Time.deltaTime;

		switch (CurrentAlarmState)
		{
			case AlarmState.Relaxed:
				{
					break;
				}
			case AlarmState.Caution:
				{
					if (currentTime >= CautionTime)
						TransitionToState(AlarmState.Relaxed);
					break;
				}
			case AlarmState.Evasion:
				{
					if (currentTime >= EvasionTime)
						TransitionToState(AlarmState.Caution);
					break;
				}
			case AlarmState.Alert:
				{
					if (currentTime >= AlertTime)
						TransitionToState(AlarmState.Evasion);
					break;
				}
		}
	}

	public void StartAlarm()
	{
		TransitionToState(AlarmState.Alert);
	}

	IEnumerator AlarmRoutine()
	{
		//PointLight.color = AlarmColour;
		//var startIntensity = PointLight.intensity;

		while (true)
		{
			//var intensity = (Mathf.Sin(FlashFrequency * Time.time * Mathf.PI * 2f) + 1f) / 2f * startIntensity;
			//PointLight.intensity = intensity;
			yield return null;
		}
	}

	internal string GetAlarmUIText()
	{
		switch (CurrentAlarmState)
		{
			case AlarmState.Relaxed:
				{
					return "";
				}
			case AlarmState.Caution:
				{
					return $"Caution\n{(99.99 * (1 - currentTime / CautionTime)).ToString("00.##")}";
				}
			case AlarmState.Evasion:
				{
					return $"Evasion\n{(99.99 * (1 - currentTime / EvasionTime)).ToString("00.##")}";
				}
			case AlarmState.Alert:
				{
					return $"Alert\n{(99.99 * (1 - currentTime / AlertTime)).ToString("00.##")}";
				}
			default:
				{
					return "";
				}
		}
	}
}
