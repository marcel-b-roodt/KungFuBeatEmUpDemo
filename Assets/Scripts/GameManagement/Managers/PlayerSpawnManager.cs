using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
	public RoomTransitionID roomSpawnID;

	public GameObject PlayerPrefab;

	void Start()
	{
		SetUpPlayer();

		SceneManager.sceneLoaded += SetUpPlayerEvent;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= SetUpPlayerEvent;
	}

	public void SetRoomSpawnID(RoomTransitionID newRoomID)
	{
		this.roomSpawnID = newRoomID;
	}

	public void SetUpPlayerEvent(Scene scene, LoadSceneMode mode)
	{
		SetUpPlayer();
	}

	public void SetUpPlayer()
	{
		var roomTransitions = GameObject.FindGameObjectsWithTag(Helpers.Tags.RoomTransition);
		var orderedTransitions = roomTransitions.Select(item => item.GetComponent<RoomTransitionTrigger>()).OrderBy(item => item.name);

		if (roomSpawnID == RoomTransitionID.ErrorRoom_NotSet)
		{
			roomSpawnID = orderedTransitions.FirstOrDefault().GetComponent<RoomTransitionTrigger>().DoorID;
		}

		foreach (var transition in orderedTransitions)
		{
			if (transition.DoorID == roomSpawnID)
			{
				GameObject player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);

				if (!player)
				{
					player = GameObject.Instantiate(PlayerPrefab, transition.transform.position, transition.transform.rotation);
				}

				player.transform.position = transition.transform.position;
				player.transform.rotation = transition.transform.rotation;
				return;
			}			
		}
	}
}
