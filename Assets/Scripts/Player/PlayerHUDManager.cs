using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDManager : MonoBehaviour
{
	public Text StateText;

	private AlarmManager AlarmManager;

	void Start()
	{
		AlarmManager = Helpers.GetManagers().AlarmManager;
	}

	void Update()
	{
		StateText.text = GetStateText();
	}

	private string GetStateText()
	{
		return AlarmManager.GetAlarmUIText();
	}
}
