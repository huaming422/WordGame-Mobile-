using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class AccountDataManager
{
	private static string nowUserId;

	private static string datafileName = "gameData.gb";

	private static string dataFilePath;

	private static Hashtable dataCache;

	private static bool inited;

	private static byte key = 5;

	public static bool isInit
	{
		get
		{
			return inited;
		}
	}

	private static string fileDir
	{
		get
		{
			return accountDataDir + nowUserId + "/";
		}
	}

	public static string accountDataDir
	{
		get
		{
			return AppConst.DataDir + "AccountData/";
		}
	}

	public static void Init(string useId, string fileName = null)
	{
		if (inited)
		{
			return;
		}
		nowUserId = useId;
		if (!string.IsNullOrEmpty(fileName))
		{
			datafileName = fileName;
		}
		dataFilePath = fileDir + datafileName;
		if (!Directory.Exists(fileDir))
		{
			Directory.CreateDirectory(fileDir);
		}
		if (!File.Exists(dataFilePath))
		{
			using (File.Create(dataFilePath))
			{
			}
		}
		byte[] array = File.ReadAllBytes(dataFilePath);
		if (array.Length == 0)
		{
			dataCache = new Hashtable();
		}
		else
		{
			string @string = Encoding.UTF8.GetString(DecodeAndEnCodeData(array));
			dataCache = @string.DecodeJson();
		}
		inited = true;
	}

	private static byte[] DecodeAndEnCodeData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return data;
		}
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = (byte)(data[i] ^ key);
		}
		return data;
	}

	public static void SetInt(string key, int value, bool isSave = true)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		if (dataCache == null)
		{
			Debug.LogWarning("AccountDataManager not init");
			return;
		}
		dataCache[key] = value;
		if (isSave)
		{
			SaveData();
		}
	}

	public static int GetInt(string key)
	{
		return Convert.ToInt32(dataCache[key]);
	}

	public static int GetInt(string key, int defaultValue)
	{
		if (dataCache == null)
		{
			return defaultValue;
		}
		if (!dataCache.ContainsKey(key))
		{
			return defaultValue;
		}
		return Convert.ToInt32(dataCache[key]);
	}

	public static void SetFloat(string key, float value, bool isSave = true)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		if (dataCache == null)
		{
			Debug.LogWarning("AccountDataManager not init");
			return;
		}
		dataCache[key] = value;
		if (isSave)
		{
			SaveData();
		}
	}

	public static float GetFloat(string key)
	{
		return (float)Convert.ToDouble(dataCache[key]);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		if (dataCache == null)
		{
			return defaultValue;
		}
		if (!dataCache.ContainsKey(key))
		{
			return defaultValue;
		}
		return (float)Convert.ToDouble(dataCache[key]);
	}

	public static void SetString(string key, string value, bool isSave = true)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		if (dataCache == null)
		{
			Debug.LogWarning("AccountDataManager not init");
			return;
		}
		dataCache[key] = value;
		if (isSave)
		{
			SaveData();
		}
	}

	public static string GetString(string key)
	{
		return dataCache[key] as string;
	}

	public static string GetString(string key, string defaultValue)
	{
		if (dataCache == null)
		{
			return defaultValue;
		}
		if (!dataCache.ContainsKey(key))
		{
			return defaultValue;
		}
		return dataCache[key] as string;
	}

	public static bool HasKey(string key)
	{
		if (dataCache == null)
		{
			return false;
		}
		return dataCache.ContainsKey(key);
	}

	public static void SaveData()
	{
		if (dataCache != null && dataCache.Count != 0)
		{
			string s = dataCache.ToJson();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			File.WriteAllBytes(dataFilePath, DecodeAndEnCodeData(bytes));
		}
	}

	public static void DeleteKey(string key, bool isSave = true)
	{
		if (dataCache != null && dataCache.Count != 0 && dataCache.ContainsKey(key))
		{
			dataCache.Remove(key);
			if (isSave)
			{
				SaveData();
			}
		}
	}

	public static void DeleteAll()
	{
		if (!inited)
		{
			if (File.Exists(dataFilePath))
			{
				File.Delete(dataFilePath);
			}
		}
		else if (dataCache != null && dataCache.Count != 0)
		{
			dataCache.Clear();
			if (File.Exists(dataFilePath))
			{
				File.Delete(dataFilePath);
			}
			inited = false;
			Init(nowUserId);
		}
	}

	public static void Clearn()
	{
		if (Directory.Exists(accountDataDir))
		{
			Directory.Delete(accountDataDir, true);
		}
		Directory.CreateDirectory(accountDataDir);
	}

	public static void Close()
	{
		if (inited)
		{
			SaveData();
			inited = false;
		}
	}
}
