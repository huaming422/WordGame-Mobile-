using System.Collections.Generic;
using UnityEngine;

public class AttachData<T>
{
	private static Dictionary<Transform, T> allAttaches;

	public static void Attach(Transform obj, T data)
	{
		if (!(obj == null))
		{
			if (allAttaches == null)
			{
				allAttaches = new Dictionary<Transform, T>();
			}
			allAttaches[obj] = data;
		}
	}

	public static T GetData(Transform obj)
	{
		T value = default(T);
		if (obj == null || allAttaches == null)
		{
			return value;
		}
		allAttaches.TryGetValue(obj, out value);
		return value;
	}

	public static void Destroy()
	{
		if (allAttaches != null)
		{
			allAttaches.Clear();
			allAttaches = null;
		}
	}
}
