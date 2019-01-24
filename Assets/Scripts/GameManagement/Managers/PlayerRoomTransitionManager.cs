using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRoomTransitionManager : MonoBehaviour
{
	private PlayerSpawnManager playerSpawnManager;

	private void Start()
	{
		playerSpawnManager = Helpers.GetManagers().PlayerSpawnManager;

		SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
		SceneManager.sceneLoaded += UpdateActiveSceneOnLoad;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= UpdateActiveSceneOnLoad;
	}

	public void TransitionToLevel(RoomTransitionID destination)
	{
		var sceneName = destination.ToString().Split('_')[0];
		playerSpawnManager.SetRoomSpawnID(destination);
		//TODO: On game start, set the active scene to the level we are playing.
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
	}

	private void UpdateActiveSceneOnLoad(Scene targetScene, LoadSceneMode mode)
	{
		SceneManager.SetActiveScene(targetScene);
	}
}

public enum RoomTransitionID
{
	//SceneName_UsefulName
	ErrorRoom_NotSet = Int32.MaxValue,
	TestLevel_StartingArea = 9000,
	TestLevel_NorthDoor = 9001,
	TestLevel_SouthDoor = 9002,
	TestLevel_WestDoor = 9003,
	TestLevel2_1 = 9004
}
