namespace TapjoyUnity.Internal.UnityCompat
{
	internal class SceneManagerCompat
	{
		internal static TapjoyAction<SceneCompat, SceneCompat> activeSceneChanged;

		internal static TapjoyAction<SceneCompat, int> sceneLoaded;

		internal static TapjoyAction<SceneCompat> sceneUnloaded;

		internal static int sceneCount
		{
			get
			{
				if (UnityDependency.sceneCount != null)
				{
					return UnityDependency.sceneCount();
				}
				return 0;
			}
		}

		internal static SceneCompat GetActiveScene()
		{
			if (UnityDependency.GetActiveScene != null)
			{
				return UnityDependency.GetActiveScene();
			}
			return SceneCompat.NONE;
		}

		internal static SceneCompat GetSceneAt(int index)
		{
			if (UnityDependency.GetSceneAt != null)
			{
				return UnityDependency.GetSceneAt(index);
			}
			return SceneCompat.NONE;
		}
	}
}
