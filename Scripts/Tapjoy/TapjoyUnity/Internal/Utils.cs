using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TapjoyUnity.Internal
{
	internal static class Utils
	{
		internal static readonly Version VERSION_5_4 = new Version(5, 4);

		private static Version unityVersion;

		internal static Version UnityVersion
		{
			get
			{
				if (unityVersion == null)
				{
					try
					{
						unityVersion = new Version(Regex.Match(Application.unityVersion, "^[0-9]+(\\.[0-9]+){1,3}").Value);
					}
					catch (Exception)
					{
						unityVersion = new Version();
					}
				}
				return unityVersion;
			}
		}
	}
}
