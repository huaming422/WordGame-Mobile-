using UnityEngine;

namespace TapjoyUnity.Internal
{
	internal static class ForegroundRealtimeClock
	{
		private static float realtimeSpentWhilePaused = 0f;

		private static float realtimePaused = 0f;

		internal static float Realtime
		{
			get
			{
				return Time.realtimeSinceStartup - realtimeSpentWhilePaused;
			}
		}

		internal static void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				realtimePaused = Time.realtimeSinceStartup;
				return;
			}
			realtimeSpentWhilePaused += Time.realtimeSinceStartup - realtimePaused;
			realtimePaused = 0f;
		}
	}
}
