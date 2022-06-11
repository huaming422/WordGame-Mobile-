using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Util
{
	private static List<string> luaPaths = new List<string>();

	public static string persistentDataPath
	{
		get
		{
			string text = AppConst.AppName.ToLower();
			if (Application.isMobilePlatform)
			{
				return Application.persistentDataPath + "/" + text + "/";
			}
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				int num = Application.dataPath.LastIndexOf('/');
				return Application.dataPath.Substring(0, num + 1) + text + "/";
			}
			return "c:/" + text + "/";
		}
	}

	public static string streamingAssetsPath
	{
		get
		{
			return Application.streamingAssetsPath + "/";
		}
	}

	public static bool NetAvailable
	{
		get
		{
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}

	public static bool IsWifi
	{
		get
		{
			return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
		}
	}

	public static float randomValue
	{
		get
		{
			long ticks = DateTime.Now.Ticks;
			while (DateTime.Now.Ticks == ticks)
			{
			}
			int seed = (int)(DateTime.Now.Ticks << 32 >> 32);
			System.Random random = new System.Random(seed);
			return (float)random.Next(0, 10000) / 10000f;
		}
	}

	public static int Int(object o)
	{
		return Convert.ToInt32(o);
	}

	public static float Float(object o)
	{
		return (float)Math.Round(Convert.ToSingle(o), 2);
	}

	public static long Long(object o)
	{
		return Convert.ToInt64(o);
	}

	public static int Random(int min, int max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	public static float Random(float min, float max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	public static string Uid(string uid)
	{
		int num = uid.LastIndexOf('_');
		return uid.Remove(0, num + 1);
	}

	public static long GetTime()
	{
		TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
		return (long)timeSpan.TotalMilliseconds;
	}

	public static T Get<T>(GameObject go, string subnode) where T : Component
	{
		if (go != null)
		{
			Transform transform = go.transform.Find(subnode);
			if (transform != null)
			{
				return transform.GetComponent<T>();
			}
		}
		return (T)null;
	}

	public static T Get<T>(Transform go, string subnode) where T : Component
	{
		if (go != null)
		{
			Transform transform = go.Find(subnode);
			if (transform != null)
			{
				return transform.GetComponent<T>();
			}
		}
		return (T)null;
	}

	public static T Get<T>(Component go, string subnode) where T : Component
	{
		return go.transform.Find(subnode).GetComponent<T>();
	}

	public static T Add<T>(GameObject go) where T : Component
	{
		if (go != null)
		{
			T[] components = go.GetComponents<T>();
			for (int i = 0; i < components.Length; i++)
			{
				if ((UnityEngine.Object)components[i] != (UnityEngine.Object)null)
				{
					UnityEngine.Object.Destroy(components[i]);
				}
			}
			return go.gameObject.AddComponent<T>();
		}
		return (T)null;
	}

	public static T Add<T>(Transform go) where T : Component
	{
		return Add<T>(go.gameObject);
	}

	public static GameObject Child(GameObject go, string subnode)
	{
		return Child(go.transform, subnode);
	}

	public static GameObject Child(Transform go, string subnode)
	{
		Transform transform = go.Find(subnode);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	public static GameObject Peer(GameObject go, string subnode)
	{
		return Peer(go.transform, subnode);
	}

	public static GameObject Peer(Transform go, string subnode)
	{
		Transform transform = go.parent.Find(subnode);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	public static string Md5String(string source)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.UTF8.GetBytes(source);
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes, 0, bytes.Length);
		mD5CryptoServiceProvider.Clear();
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public static string md5file(string file)
	{
		try
		{
			FileStream fileStream = new FileStream(file, FileMode.Open);
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] array = mD.ComputeHash(fileStream);
			fileStream.Close();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("md5file() fail, error:" + ex.Message);
		}
	}

	public static void ClearChild(Transform go)
	{
		if (!(go == null))
		{
			for (int num = go.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(go.GetChild(num).gameObject);
			}
		}
	}

	public static void ClearMemory()
	{
		GC.Collect();
		Resources.UnloadUnusedAssets();
	}

	public static string GetFilePathToWWWPath(string path)
	{
		if (path.Contains("file://"))
		{
			return path;
		}
		return "file:///" + path;
	}

	public static string GetFileText(string path)
	{
		return File.ReadAllText(path);
	}

	public static void Log(string str)
	{
		if (AppConst.IsOpenLog)
		{
			Debug.Log(str);
		}
	}

	public static void LogWarning(string str)
	{
		if (AppConst.IsOpenLog)
		{
			Debug.LogWarning(str);
		}
	}

	public static void LogError(string str)
	{
		if (AppConst.IsOpenLog)
		{
			Debug.LogError(str);
		}
	}
}
