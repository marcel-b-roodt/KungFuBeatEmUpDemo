using UnityEngine;

public static class Helpers
{
	public class Constants
	{
		public const string ManagerScene = "ManagerScene";
	}

	public class Layers
	{
		public const string Default = "Default";
		public const string Interactable = "Interactable";
		public const string Enemy = "Enemy";
		public const string Player = "Player";
		public const string PlayerHitbox = "PlayerHitbox";
		public const string EnemyHitbox = "EnemyHitbox";
		public const string Projectile = "Projectile";
	}

	public class Tags
	{
		public const string Enemy = "Enemy";
		public const string EnemyHitbox = "EnemyHitbox";
		public const string Environment = "Environment";
		public const string GameManagers = "GameManagers";
		public const string Interactable = "Interactable";
		public const string PlayerCamera = "MainCamera";
		public const string Player = "Player";
		public const string PlayerHUD = "PlayerHUD";
		public const string PlayerHitbox = "PlayerHitbox";
		public const string PlayerMesh = "PlayerMesh";
		public const string RoomTransition = "RoomTransition";
	}

	public static Managers GetManagers()
	{
		return GameObject.FindGameObjectWithTag(Helpers.Tags.GameManagers).GetComponent<Managers>();
	}

	public static GameObject FindObjectInChildren(this GameObject gameObject, string gameObjectName)
	{
		Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform item in children)
		{
			if (item.name == gameObjectName)
			{
				return item.gameObject;
			}
		}

		return null;
	}

	public static GameObject FindTaggedObjectInChildren(this GameObject gameObject, string tag)
	{
		Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform item in children)
		{
			if (item.tag == tag)
			{
				return item.gameObject;
			}
		}

		return null;
	}

	public static void DebugDirectionRay(this Transform transform)
	{
		Debug.DrawRay(transform.position, transform.forward, Color.red, 0.1f);
	}

	public static AngleDirection Direction(float angle)
	{
		return (AngleDirection)(Mathf.Round(angle / 45) * 45f);
	}
}
