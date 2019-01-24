using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
	public AlarmManager AlarmManager;
	public DebugManager DebugManager;
	public PlayerHUDManager PlayerHUDManager;
	public PlayerSpawnManager PlayerSpawnManager;
	public PlayerRoomTransitionManager PlayerTransitionManager;

	void Awake()
	{
		AlarmManager = GetComponentInChildren<AlarmManager>();
		DebugManager = GetComponentInChildren<DebugManager>();
		PlayerHUDManager = GetComponentInChildren<PlayerHUDManager>();
		PlayerSpawnManager = GetComponentInChildren<PlayerSpawnManager>();
		PlayerTransitionManager = GetComponentInChildren<PlayerRoomTransitionManager>();
	}
}
