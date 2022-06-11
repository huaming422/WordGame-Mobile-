using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SomeExtend
{
	public static T Sum<T>(this T[] src, Func<T, T, T> func)
	{
		if (src == null || src.Length < 1)
		{
			return default(T);
		}
		T val = default(T);
		for (int i = 0; i < src.Length; i++)
		{
			val = func(val, src[i]);
		}
		return val;
	}

	public static T Sum<T>(this List<T> src, Func<T, T, T> func)
	{
		if (src == null || src.Count < 1)
		{
			return default(T);
		}
		T val = default(T);
		for (int i = 0; i < src.Count; i++)
		{
			val = func(val, src[i]);
		}
		return val;
	}

	public static List<T> ToList<T>(this T[] src)
	{
		if (src == null)
		{
			return null;
		}
		List<T> list = new List<T>(src.Length);
		for (int i = 0; i != src.Length; i++)
		{
			list.Add(src[i]);
		}
		return list;
	}

	public static List<T> MyClone<T>(this List<T> src)
	{
		if (src == null)
		{
			return null;
		}
		List<T> list = new List<T>(src.Count);
		list.AddRange(src);
		return list;
	}

	public static T[] MyClone<T>(this T[] src, Func<T, T> dothing = null)
	{
		if (src == null)
		{
			return null;
		}
		T[] array = new T[src.Length];
		for (int i = 0; i < src.Length; i++)
		{
			if (dothing == null)
			{
				array[i] = src[i];
			}
			else
			{
				array[i] = dothing(src[i]);
			}
		}
		return array;
	}

	public static T[] MyClone<T>(this T[] src)
	{
		if (src == null)
		{
			return null;
		}
		T[] array = new T[src.Length];
		Array.Copy(src, array, src.Length);
		return array;
	}

	public static int Max<T>(this List<T> src, Func<T, T, bool> IsChange)
	{
		if (src == null || src.Count < 1)
		{
			return -1;
		}
		T arg = src[0];
		int result = 0;
		for (int i = 0; i < src.Count; i++)
		{
			if (IsChange(arg, src[i]))
			{
				result = i;
				arg = src[i];
			}
		}
		return result;
	}

	public static int Max<T>(this T[] src, Func<T, T, bool> IsChange)
	{
		if (src == null || src.Length < 1)
		{
			return -1;
		}
		T arg = src[0];
		int result = 0;
		for (int i = 0; i < src.Length; i++)
		{
			if (IsChange(arg, src[i]))
			{
				result = i;
				arg = src[i];
			}
		}
		return result;
	}

	public static T[] GetByCount<T>(this List<T> src, int count, bool delet = false)
	{
		if (src == null || src.Count == 0)
		{
			return null;
		}
		T[] array = new T[count];
		if (count >= src.Count)
		{
			src.CopyTo(array);
			if (delet)
			{
				src.Clear();
			}
			return array;
		}
		for (int i = 0; i < count; i++)
		{
			array[i] = src[i];
		}
		if (delet)
		{
			src.RemoveRange(0, count);
		}
		return array;
	}

	public static bool IsEmpty<T>(this List<T> src)
	{
		if (src == null || src.Count == 0)
		{
			return true;
		}
		return false;
	}

	public static bool IsEmpty<T>(this T[] src)
	{
		if (src == null || src.Length == 0)
		{
			return true;
		}
		return false;
	}

	public static bool HaveRepeat<T>(this List<T> src)
	{
		if (src == null || src.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < src.Count; i++)
		{
			int num = src.IndexOf(src[i]);
			if (num != i)
			{
				return true;
			}
		}
		return false;
	}

	public static bool HaveRepeat<T>(this T[] src)
	{
		if (src == null || src.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < src.Length; i++)
		{
			int num = Array.IndexOf(src, src[i]);
			if (num != i)
			{
				return true;
			}
		}
		return false;
	}

	public static bool HaveRepeat<T>(this List<T> src, Func<T, T, bool> action)
	{
		if (src == null || src.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < src.Count; i++)
		{
			int num = src.FindIndex((T n) => action(n, src[i]));
			if (num != i)
			{
				return true;
			}
		}
		return false;
	}

	public static bool HaveRepeat<T>(this T[] src, Func<T, T, bool> action)
	{
		if (src == null || src.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < src.Length; i++)
		{
			int num = Array.FindIndex(src, (T n) => action(n, src[i]));
			if (num != i)
			{
				return true;
			}
		}
		return false;
	}

	public static void MyInvoke(this UnityAction action)
	{
		if (action != null)
		{
			action();
		}
	}

	public static void MyInvoke<T>(this UnityAction<T> action, T obj)
	{
		if (action != null)
		{
			action(obj);
		}
	}

	public static bool HaveRepeat(this string src)
	{
		if (string.IsNullOrEmpty(src))
		{
			return false;
		}
		for (int i = 0; i < src.Length; i++)
		{
			if (src.IndexOf(src[i]) != i)
			{
				return true;
			}
		}
		return false;
	}

	public static void ActiveChils(this Transform go, bool isAcive)
	{
		if (!(go == null))
		{
			for (int i = 0; i < go.childCount; i++)
			{
				go.GetChild(i).gameObject.SetActive(isAcive);
			}
		}
	}
}
