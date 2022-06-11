using System;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening.Core
{
	public static class DOTweenExternalCommand
	{
		public static event Action<PathOptions, Tween, Quaternion, Transform> SetOrientationOnPath;

		internal static void Dispatch_SetOrientationOnPath(PathOptions options, Tween t, Quaternion newRot, Transform trans)
		{
			if (DOTweenExternalCommand.SetOrientationOnPath != null)
			{
				DOTweenExternalCommand.SetOrientationOnPath(options, t, newRot, trans);
			}
		}
	}
}
