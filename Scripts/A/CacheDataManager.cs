using System.Collections.Generic;
using UnityEngine;

public class CacheDataManager : SingleObject<CacheDataManager>
{
	private class CacheObject
	{
		public int key;

		public object obj;

		public float outTime;

		public float addTime;

		public CacheObject(int key, object obj, float outTime, float addTime)
		{
			this.key = key;
			this.obj = obj;
			this.outTime = outTime;
			this.addTime = addTime;
		}
	}

	private int nowCheckIndex;

	private int nowkey;

	private Dictionary<int, CacheObject> cacheObjects = new Dictionary<int, CacheObject>();

	private Dictionary<string, int> userKeyToMykeys = new Dictionary<string, int>();

	private List<int> cacheKeys = new List<int>();

	public int Cache(object obj, float outTime)
	{
		nowkey++;
		CacheObject value = new CacheObject(nowkey, obj, outTime, Time.unscaledTime);
		cacheObjects.Add(nowkey, value);
		cacheKeys.Add(nowkey);
		return nowkey;
	}

	public void Cache(string key, object obj, float outTime)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		if (userKeyToMykeys.ContainsKey(key))
		{
			int key2 = userKeyToMykeys[key];
			if (cacheObjects.ContainsKey(key2))
			{
				CacheObject cacheObject = cacheObjects[key2];
				cacheObject.obj = obj;
				cacheObject.addTime = Time.unscaledTime;
				cacheObject.outTime = outTime;
				return;
			}
			userKeyToMykeys.Remove(key);
		}
		int value = Cache(obj, outTime);
		userKeyToMykeys.Add(key, value);
	}

	private void Update()
	{
		if (cacheObjects.Count != 0)
		{
			nowCheckIndex %= cacheKeys.Count;
			CacheObject cacheObject = cacheObjects[cacheKeys[nowCheckIndex]];
			if (Time.unscaledTime - cacheObject.addTime >= cacheObject.outTime && cacheObject.outTime > 0f)
			{
				RemoveBykey(cacheObject.key);
			}
			nowCheckIndex++;
		}
	}

	public object GetCacheObj(int key)
	{
		if (cacheObjects.ContainsKey(key))
		{
			return cacheObjects[key].obj;
		}
		return null;
	}

	public object GetCacheObj(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		if (userKeyToMykeys.ContainsKey(key))
		{
			object cacheObj = GetCacheObj(userKeyToMykeys[key]);
			if (cacheObj == null)
			{
				userKeyToMykeys.Remove(key);
			}
			return cacheObj;
		}
		return null;
	}

	public void RemoveBykey(int key)
	{
		if (cacheObjects.ContainsKey(key))
		{
			cacheObjects.Remove(key);
			cacheKeys.Remove(key);
		}
	}

	public void RemoveBykey(string key)
	{
		if (!string.IsNullOrEmpty(key) && userKeyToMykeys.ContainsKey(key))
		{
			RemoveBykey(userKeyToMykeys[key]);
			userKeyToMykeys.Remove(key);
		}
	}

	public bool ExitsKey(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (!userKeyToMykeys.ContainsKey(key))
		{
			return false;
		}
		return cacheObjects.ContainsKey(userKeyToMykeys[key]);
	}

	public void Clearn()
	{
		cacheObjects.Clear();
		userKeyToMykeys.Clear();
		cacheKeys.Clear();
	}
}
