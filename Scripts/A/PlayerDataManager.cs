using System;
using UnityEngine;

public class PlayerDataManager
{
	public static bool isAcceptPrivate
	{
		get
		{
			return PlayerPrefs.GetInt("datakey_200", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("datakey_200", value ? 1 : 0);
		}
	}

	public static TimeSpan InstallForNow
	{
		get
		{
			string text = PlayerPrefs.GetString("datakey_202", string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				text = DateTime.Now.Ticks.ToString();
				PlayerPrefs.SetString("datakey_202", text);
			}
			DateTime dateTime = new DateTime(long.Parse(text));
			return DateTime.Now - dateTime;
		}
	}

	public static void ChangeCoin(int count, bool isBroadcaset = true)
	{
		PlayerData.i.coinCount += count;
		PlayerData.Save();
		if (isBroadcaset)
		{
			MessageManger.SendMessage(100);
		}
	}
}
