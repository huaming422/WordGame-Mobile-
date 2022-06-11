using System;
using System.Collections;
using System.Collections.Generic;

public static class MiniJsonExtensions
{
	public static string ToJson(this IDictionary obj, bool indent = false, bool sortKey = false)
	{
		return MiniJSON.JsonEncode(obj, indent, sortKey);
	}

	public static string ToJson(this Dictionary<string, string> obj, bool indent = false, bool sortKey = false)
	{
		return MiniJSON.JsonEncode(obj, indent, sortKey);
	}

	public static string ToJson(this ArrayList obj, bool indent = false, bool sortKey = false)
	{
		return MiniJSON.JsonEncode(obj, indent, sortKey);
	}

	public static ArrayList DecodeArrayJosn(this string json)
	{
		return MiniJSON.JsonDecode(json) as ArrayList;
	}

	public static Hashtable DecodeJson(this string json)
	{
		return MiniJSON.JsonDecode(json) as Hashtable;
	}

	public static bool GetBool(this Hashtable data, string key, bool defalut = false)
	{
		if (data == null || !data.ContainsKey(key))
		{
			return defalut;
		}
		return Convert.ToBoolean(data[key]);
	}

	public static int GetInt(this Hashtable data, string key)
	{
		if (data == null || !data.ContainsKey(key))
		{
			return -1;
		}
		return Convert.ToInt32(data[key]);
	}

	public static long GetLong(this Hashtable data, string key)
	{
		if (data == null || !data.ContainsKey(key))
		{
			return -1L;
		}
		return Convert.ToInt64(data[key]);
	}

	public static string GetString(this Hashtable data, string key)
	{
		if (data == null || !data.ContainsKey(key))
		{
			return null;
		}
		return data[key] as string;
	}

	public static Hashtable GetHashtable(this Hashtable data, string key)
	{
		if (data == null || !data.ContainsKey(key))
		{
			return null;
		}
		return data[key] as Hashtable;
	}

	public static ArrayList GetArrayList(this Hashtable data, string key)
	{
		if (data == null || !data.ContainsKey(key))
		{
			return null;
		}
		return data[key] as ArrayList;
	}

	public static int[] GetIntArray(this Hashtable data, string key)
	{
		if (data == null || !data.ContainsKey(key))
		{
			return null;
		}
		ArrayList arrayList = data[key] as ArrayList;
		if (arrayList == null)
		{
			return null;
		}
		int[] array = new int[arrayList.Count];
		for (int i = 0; i < arrayList.Count; i++)
		{
			array[i] = Convert.ToInt32(arrayList[i]);
		}
		return array;
	}
}
