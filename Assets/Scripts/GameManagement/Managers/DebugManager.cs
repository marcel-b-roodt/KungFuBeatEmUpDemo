using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

	public Transform[] debugPositions;

	private GameObject player;
	private GameObject[] enemies;

	private AlarmManager alarmManager;

	// Use this for initialization
	void Awake () {
		player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);
		enemies = GameObject.FindGameObjectsWithTag(Helpers.Tags.Enemy);
		alarmManager = GetComponent<AlarmManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.LeftAlt))
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.O))
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.L))
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad0) && debugPositions[0] != null)
			{
				player.transform.position = debugPositions[0].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad1) && debugPositions[1] != null)
			{
				player.transform.position = debugPositions[1].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad2) && debugPositions[2] != null)
			{
				player.transform.position = debugPositions[2].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad3) && debugPositions[3] != null)
			{
				player.transform.position = debugPositions[3].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad4) && debugPositions[4] != null)
			{
				player.transform.position = debugPositions[4].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad5) && debugPositions[5] != null)
			{
				player.transform.position = debugPositions[5].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad6) && debugPositions[6] != null)
			{
				player.transform.position = debugPositions[6].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad7) && debugPositions[7] != null)
			{
				player.transform.position = debugPositions[7].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad8) && debugPositions[8] != null)
			{
				player.transform.position = debugPositions[8].position;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Keypad9) && debugPositions[9] != null)
			{
				player.transform.position = debugPositions[9].position;
				return;
			}
		}
	}
}
