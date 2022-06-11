using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
	public int coinCount = 100;

	public int brilliance;

	public bool playerBuyRemoveAd;

	public int rateGameSate;

	public int nowDiyPicCount;

	public int openLevel = 1;

	private static PlayerData _instance;

	public static PlayerData i
	{
		get
		{
			if (_instance == null)
			{
				string @string = AccountDataManager.GetString("datakey_1", string.Empty);
				if (string.IsNullOrEmpty(@string))
				{
					_instance = new PlayerData();
					return _instance;
				}
				_instance = JsonUtility.FromJson<PlayerData>(@string);
			}
			return _instance;
		}
	}

	public static void Save()
	{
		if (_instance != null)
		{
			string value = JsonUtility.ToJson(_instance);
			AccountDataManager.SetString("datakey_1", value);
		}
	}

	public static void Destroy()
	{
		_instance = null;
	}
}
