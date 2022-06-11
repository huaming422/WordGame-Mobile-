using System;
using System.Reflection;
using UnityEngine;

namespace DG.Tweening.Core
{
	internal static class Utils
	{
		private static Assembly[] _loadedAssemblies;

		private static readonly string[] _defAssembliesToQuery = new string[2] { "Assembly-CSharp", "Assembly-CSharp-firstpass" };

		internal static Vector3 Vector3FromAngle(float degrees, float magnitude)
		{
			float f = degrees * ((float)Math.PI / 180f);
			return new Vector3(magnitude * Mathf.Cos(f), magnitude * Mathf.Sin(f), 0f);
		}

		internal static float Angle2D(Vector3 from, Vector3 to)
		{
			Vector2 right = Vector2.right;
			to -= from;
			float num = Vector2.Angle(right, to);
			if (Vector3.Cross(right, to).z > 0f)
			{
				num = 360f - num;
			}
			return num * -1f;
		}

		internal static bool Vector3AreApproximatelyEqual(Vector3 a, Vector3 b)
		{
			if (Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y))
			{
				return Mathf.Approximately(a.z, b.z);
			}
			return false;
		}

		internal static Type GetLooseScriptType(string typeName)
		{
			for (int i = 0; i < _defAssembliesToQuery.Length; i++)
			{
				Type type = Type.GetType(string.Format("{0}, {1}", typeName, _defAssembliesToQuery[i]));
				if (type != null)
				{
					return type;
				}
			}
			if (_loadedAssemblies == null)
			{
				_loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			}
			for (int j = 0; j < _loadedAssemblies.Length; j++)
			{
				Type type2 = Type.GetType(string.Format("{0}, {1}", typeName, _loadedAssemblies[j].GetName()));
				if (type2 != null)
				{
					return type2;
				}
			}
			return null;
		}
	}
}
