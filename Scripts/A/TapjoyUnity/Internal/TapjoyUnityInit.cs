using TapjoyUnity.Internal.UnityCompat;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TapjoyUnity.Internal
{
	public sealed class TapjoyUnityInit : MonoBehaviour
	{
		private static bool initialized;

		private void Awake()
		{
			if (!initialized)
			{
				initialized = true;
				//ApiBindingAndroid.Install();
				SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene newScene)
				{
					UnityDependency.OnActiveSceneChanged(Wrap(oldScene), Wrap(newScene));
				};
				SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode mode)
				{
					UnityDependency.OnSceneLoaded(Wrap(scene), (int)mode);
				};
				SceneManager.sceneUnloaded += delegate(Scene scene)
				{
					UnityDependency.OnSceneUnloaded(Wrap(scene));
				};
				UnityDependency.sceneCount = () => SceneManager.sceneCount;
				UnityDependency.GetActiveScene = () => Wrap(SceneManager.GetActiveScene());
				UnityDependency.GetSceneAt = (int index) => Wrap(SceneManager.GetSceneAt(index));
				UnityDependency.ToJson = JsonUtility.ToJson;
			}
		}

		private static SceneCompat Wrap(Scene scene)
		{
			return new SceneCompat(scene, scene.IsValid(), scene.buildIndex, scene.name, scene.path);
		}
	}
}
