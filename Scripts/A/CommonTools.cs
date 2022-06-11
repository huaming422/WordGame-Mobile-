using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CommonTools
{
	public static void Swap<T>(ref T a, ref T b)
	{
		T val = a;
		a = b;
		b = val;
	}

	public static void Swap<T>(List<T> l, int a, int b)
	{
		if (a >= 0 && b >= 0 && l != null && a < l.Count && b < l.Count && a != b)
		{
			T value = l[a];
			l[a] = l[b];
			l[b] = value;
		}
	}

	public static void Swap<T>(T[] l, int a, int b)
	{
		if (a >= 0 && b >= 0 && l != null && a < l.Length && b < l.Length && a != b)
		{
			T val = l[a];
			l[a] = l[b];
			l[b] = val;
		}
	}

	public static void ExceptList<T>(List<T> src, List<T> exc)
	{
		ExceptList(src, exc.ToArray());
	}

	public static void ExceptList<T>(List<T> src, T[] exc)
	{
		if (src == null || exc == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < src.Count - num; i++)
		{
			if (Array.IndexOf(exc, src[i]) != -1)
			{
				Swap(src, i, src.Count - 1 - num);
				i--;
				num++;
			}
		}
		if (num != 0)
		{
			src.RemoveRange(src.Count - num, num);
		}
	}

	public static void RandomConfuseList<T>(List<T> src, int s = 0)
	{
		if (src != null)
		{
			for (int i = s; i < src.Count; i++)
			{
				int a = UnityEngine.Random.Range(s, src.Count);
				Swap(src, a, i);
			}
		}
	}

	public static void RandomConfuseList<T>(T[] src, int s = 0)
	{
		if (src != null)
		{
			for (int i = s; i < src.Length; i++)
			{
				int a = UnityEngine.Random.Range(s, src.Length);
				Swap(src, a, i);
			}
		}
	}

	public static int RandomGetIndexByProbability(float[] p)
	{
		if (p == null || p.Length < 1)
		{
			return 0;
		}
		float num = 0f;
		float num2 = UnityEngine.Random.Range(0f, p.Sum((float a, float b) => a + b));
		for (int i = 0; i < p.Length; i++)
		{
			num += p[i];
			if (num2 <= num)
			{
				return i;
			}
		}
		return 0;
	}

	public static T[] RandomGet<T>(T[] src, int c)
	{
		if (src == null || src.Length < 1 || c <= 0)
		{
			return null;
		}
		if (c >= src.Length)
		{
			return src;
		}
		T[] array = new T[c];
		src = src.MyClone();
		RandomConfuseList(src);
		Array.Copy(src, array, array.Length);
		return array;
	}

	public static int[] RandomAlloc(int sum, int c, int max = -1, int min = 1)
	{
		if (max == -1)
		{
			max = sum;
		}
		if (c <= 0 || sum <= 0 || min <= 0)
		{
			return null;
		}
		if (max * c < sum || min * c > sum)
		{
			return null;
		}
		int[] array = new int[c];
		for (int i = 1; i != c; i++)
		{
			int num = sum - (c - i) * min;
			num = ((max <= num) ? max : num);
			int num2 = sum - num * (c - i);
			num2 = ((num2 <= min) ? min : num2);
			array[i - 1] = UnityEngine.Random.Range(num2, num + 1);
			sum -= array[i - 1];
		}
		array[c - 1] = sum;
		return array;
	}

	public static int[] RandomCreateIndex(int c, int min, int max)
	{
		if (max - min < c || c < 1)
		{
			return null;
		}
		int num = max - min;
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = min + i;
		}
		RandomConfuseList(array);
		int[] array2 = new int[c];
		Array.Copy(array, array2, c);
		return array2;
	}

	public static void DistinctList<T>(List<T> src)
	{
		int num = 0;
		for (int i = 0; i < src.Count - 1 - num; i++)
		{
			for (int j = i + 1; j < src.Count - num; j++)
			{
				if (src[i].Equals(src[j]))
				{
					Swap(src, j, src.Count - 1 - num);
					j--;
					num++;
				}
			}
		}
		if (num != 0)
		{
			src.RemoveRange(src.Count - num, num);
		}
	}

	public static bool IntersectAB<T>(T[] a, T[] b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		for (int i = 0; i < b.Length; i++)
		{
			if (Array.IndexOf(a, b[i]) != -1)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IntersectAB<T>(List<T> a, List<T> b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		for (int i = 0; i < b.Count; i++)
		{
			if (a.IndexOf(b[i]) != -1)
			{
				return true;
			}
		}
		return false;
	}

	public static List<T> GetIntersectAB<T>(List<T> a, List<T> b, Func<T, T, bool> math = null)
	{
		if (a == null || b == null)
		{
			return null;
		}
		List<T> list = new List<T>();
		for (int i = 0; i < b.Count; i++)
		{
			for (int j = 0; j < a.Count; j++)
			{
				if (math != null)
				{
					if (math(b[i], a[j]))
					{
						list.Add(b[i]);
						break;
					}
				}
				else if (b[i].Equals(a[j]))
				{
					list.Add(b[i]);
					break;
				}
			}
		}
		return list;
	}

	public static bool IsBetween(int s, int mi, int ma)
	{
		return s >= mi && s <= ma;
	}

	public static bool IsSuperEqual<T>(T a, T b, Dictionary<T, List<T>> dic)
	{
		if (a.Equals(b))
		{
			return true;
		}
		if (dic == null || dic.Count < 0)
		{
			return false;
		}
		List<T> list = null;
		List<T> list2 = null;
		if (dic.ContainsKey(a))
		{
			list = dic[a];
		}
		if (dic.ContainsKey(b))
		{
			list2 = dic[b];
		}
		if (list == null && list2 == null)
		{
			return false;
		}
		if (list != null && list.IndexOf(b) != -1)
		{
			return true;
		}
		if (list2 != null && list2.IndexOf(a) != -1)
		{
			return true;
		}
		if (list != null && list2 != null)
		{
			IntersectAB(list, list2);
		}
		return false;
	}

	public static bool IsSuperEqualAngle<T>(T t, T nt, Dictionary<T, List<T>> dic)
	{
		if (t.Equals(nt))
		{
			return true;
		}
		if (dic == null || dic.Count < 0)
		{
			return false;
		}
		if (!dic.ContainsKey(t))
		{
			return false;
		}
		List<T> list = dic[t];
		if (list == null || list.Count < 1)
		{
			return false;
		}
		if (list.IndexOf(nt) != -1)
		{
			return true;
		}
		return false;
	}

	public static int LerpInt(int a, int b, float n)
	{
		if (n <= 0f)
		{
			return a;
		}
		if (n >= 1f)
		{
			return b;
		}
		return Mathf.RoundToInt((1f - n) * (float)a + (float)b * n);
	}

	public static long LerpLong(long a, long b, float n)
	{
		if (n <= 0f)
		{
			return a;
		}
		if (n >= 1f)
		{
			return b;
		}
		return Mathf.RoundToInt((1f - n) * (float)a + (float)b * n);
	}

	public static bool IsSameDay(DateTime time1, DateTime time2)
	{
		return time1.Year == time2.Year && time1.Month == time2.Month && time1.Day == time2.Day;
	}

	public static int GetIntervalDay(DateTime time1, DateTime time2)
	{
		DateTime dateTime = new DateTime(time1.Year, time1.Month, time1.Day);
		DateTime dateTime2 = new DateTime(time2.Year, time2.Month, time2.Day);
		return (int)(dateTime2 - dateTime).TotalDays;
	}

	public static int[] VesrionCode(string src)
	{
		if (string.IsNullOrEmpty(src))
		{
			return null;
		}
		string[] array = src.Split('.');
		int[] array2 = new int[src.Length];
		for (int i = 0; i < array.Length; i++)
		{
			int result = 0;
			if (int.TryParse(array[i], out result))
			{
				array2[i] = result;
				continue;
			}
			return null;
		}
		return array2;
	}

	public static bool NeedUpdate(string local, string server)
	{
		if (string.IsNullOrEmpty(server))
		{
			return false;
		}
		if (string.IsNullOrEmpty(local))
		{
			return true;
		}
		int[] array = VesrionCode(local);
		int[] array2 = VesrionCode(server);
		int num = Mathf.Min(array.Length, array2.Length);
		for (int i = 0; i < num; i++)
		{
			if (array[i] < array2[i])
			{
				return true;
			}
		}
		return false;
	}

	public static string Unicode2String(string source)
	{
		return new Regex("\\\\u([0-9A-F]{4})", RegexOptions.IgnoreCase).Replace(source, (Match x) => Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)).ToString());
	}
}
