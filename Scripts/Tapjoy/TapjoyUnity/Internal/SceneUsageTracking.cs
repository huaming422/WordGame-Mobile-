using System;
using System.Collections.Generic;
using TapjoyUnity.Internal.UnityCompat;

namespace TapjoyUnity.Internal
{
	internal class SceneUsageTracking
	{
		private abstract class Model
		{
			internal readonly float startRealtime;

			internal readonly List<string> placements = new List<string>();

			protected Model()
			{
				startRealtime = ForegroundRealtimeClock.Realtime;
			}

			internal void AddPlacement(string placement)
			{
				if (!placements.Contains(placement))
				{
					placements.Add(placement);
					placements.Sort();
				}
			}
		}

		private class Act : Model
		{
			internal readonly string actId;

			internal readonly string previousActId;

			internal Act(string actId, string previousActId)
			{
				this.actId = actId;
				this.previousActId = previousActId;
			}
		}

		private class SceneLoad : Model
		{
			internal readonly Act act;

			internal readonly string sceneId;

			internal readonly int loadMode;

			internal SceneLoad(Act act, SceneCompat scene, int loadMode)
			{
				this.act = act;
				sceneId = GetSceneId(scene);
				this.loadMode = loadMode;
			}
		}

		private class SceneActive : Model
		{
			internal readonly string actId;

			internal readonly string previousActId;

			internal readonly string sceneId;

			internal readonly string previousSceneId;

			internal SceneActive(string actId, string previousActId, string sceneId, string previousSceneId)
			{
				this.actId = actId;
				this.previousActId = previousActId;
				this.sceneId = sceneId;
				this.previousSceneId = previousSceneId;
			}
		}

		private class ActDimensions
		{
			public string act;

			public string act_previous;

			public string[] placements;

			internal ActDimensions(Act act)
			{
				this.act = act.actId;
				act_previous = act.previousActId;
				placements = act.placements.ToArray();
			}
		}

		private class SceneLoadDimensions
		{
			public string act;

			public string act_previous;

			public string scene;

			public int scene_load_mode;

			public string[] placements;

			internal SceneLoadDimensions(SceneLoad sceneLoad)
			{
				act = sceneLoad.act.actId;
				act_previous = sceneLoad.act.previousActId;
				scene = sceneLoad.sceneId;
				scene_load_mode = sceneLoad.loadMode;
				placements = sceneLoad.placements.ToArray();
			}
		}

		private class SceneActiveDimensions
		{
			public string act;

			public string act_previous;

			public string scene;

			public string scene_previous;

			public string scene_next;

			public string[] placements;

			internal SceneActiveDimensions(SceneActive sceneActive, SceneCompat newActiveScene)
			{
				act = sceneActive.actId;
				act_previous = sceneActive.previousActId;
				scene = sceneActive.sceneId;
				scene_previous = sceneActive.previousSceneId;
				scene_next = GetSceneId(newActiveScene);
				placements = sceneActive.placements.ToArray();
			}
		}

		private class UsageValues
		{
			public long spent_time;

			internal UsageValues(Model model)
			{
				spent_time = (long)((ForegroundRealtimeClock.Realtime - model.startRealtime) * 1000f);
			}
		}

		private static SceneUsageTracking instance;

		private string lastActId;

		private Act previousAct;

		private Act currentAct;

		private Dictionary<int, SceneLoad> scenes = new Dictionary<int, SceneLoad>();

		private SceneActive currentSceneActive;

		public static bool Enabled
		{
			get
			{
				return instance != null;
			}
			set
			{
				if (value)
				{
					if (instance == null && !(Utils.UnityVersion < Utils.VERSION_5_4) && UnityDependency.ToJson != null)
					{
						instance = new SceneUsageTracking();
						instance.OnEnable();
					}
				}
				else if (instance != null)
				{
					instance.OnDisable();
					instance = null;
				}
			}
		}

		private void OnEnable()
		{
			SceneManagerCompat.activeSceneChanged = (TapjoyAction<SceneCompat, SceneCompat>)Delegate.Combine(SceneManagerCompat.activeSceneChanged, new TapjoyAction<SceneCompat, SceneCompat>(OnActiveSceneChanged));
			SceneManagerCompat.sceneLoaded = (TapjoyAction<SceneCompat, int>)Delegate.Combine(SceneManagerCompat.sceneLoaded, new TapjoyAction<SceneCompat, int>(OnSceneLoaded));
			SceneManagerCompat.sceneUnloaded = (TapjoyAction<SceneCompat>)Delegate.Combine(SceneManagerCompat.sceneUnloaded, new TapjoyAction<SceneCompat>(OnSceneUnloaded));
			TJPlacement.OnShowContentCalled = (TapjoyAction<string>)Delegate.Combine(TJPlacement.OnShowContentCalled, new TapjoyAction<string>(OnPlacementShowContent));
			try
			{
				int sceneCount = SceneManagerCompat.sceneCount;
				for (int i = 0; i < sceneCount; i++)
				{
					OnSceneLoaded(SceneManagerCompat.GetSceneAt(i), -1);
				}
				OnActiveSceneChanged(SceneCompat.NONE, SceneManagerCompat.GetActiveScene());
			}
			catch (Exception)
			{
			}
		}

		private void OnDisable()
		{
			TJPlacement.OnShowContentCalled = (TapjoyAction<string>)Delegate.Remove(TJPlacement.OnShowContentCalled, new TapjoyAction<string>(OnPlacementShowContent));
			SceneManagerCompat.sceneUnloaded = (TapjoyAction<SceneCompat>)Delegate.Remove(SceneManagerCompat.sceneUnloaded, new TapjoyAction<SceneCompat>(OnSceneUnloaded));
			SceneManagerCompat.sceneLoaded = (TapjoyAction<SceneCompat, int>)Delegate.Remove(SceneManagerCompat.sceneLoaded, new TapjoyAction<SceneCompat, int>(OnSceneLoaded));
			SceneManagerCompat.activeSceneChanged = (TapjoyAction<SceneCompat, SceneCompat>)Delegate.Remove(SceneManagerCompat.activeSceneChanged, new TapjoyAction<SceneCompat, SceneCompat>(OnActiveSceneChanged));
			try
			{
				int sceneCount = SceneManagerCompat.sceneCount;
				for (int i = 0; i < sceneCount; i++)
				{
					OnSceneUnloaded(SceneManagerCompat.GetSceneAt(i));
				}
				OnActiveSceneChanged(SceneCompat.NONE, SceneCompat.NONE);
			}
			catch (Exception)
			{
			}
		}

		private static string GetSceneId(SceneCompat scene)
		{
			if (scene.IsValid())
			{
				if ((scene.path ?? "") != "")
				{
					return "" + scene.buildIndex + "::" + scene.path;
				}
				return "" + scene.buildIndex + ":" + scene.name + ":";
			}
			return "";
		}

		private void OnActiveSceneChanged(SceneCompat oldScene, SceneCompat newScene)
		{
			if (currentSceneActive != null)
			{
				TrackActiveSceneChanged(currentSceneActive, newScene);
			}
			if (newScene.IsValid())
			{
				string sceneId = GetSceneId(newScene);
				string previousSceneId = ((currentSceneActive == null) ? null : currentSceneActive.sceneId);
				string actId = ((currentAct == null) ? sceneId : currentAct.actId);
				string previousActId = ((currentAct == null) ? lastActId : currentAct.previousActId);
				currentSceneActive = new SceneActive(actId, previousActId, sceneId, previousSceneId);
			}
			else
			{
				currentSceneActive = null;
			}
		}

		private void OnSceneLoaded(SceneCompat scene, int loadMode)
		{
			int hashCode = scene.GetHashCode();
			if (!scenes.ContainsKey(hashCode))
			{
				string sceneId = GetSceneId(scene);
				if (currentAct == null)
				{
					currentAct = new Act(sceneId, lastActId);
					lastActId = sceneId;
					TrackActStarted(currentAct);
				}
				SceneLoad sceneLoad = new SceneLoad(currentAct, scene, loadMode);
				scenes.Add(hashCode, sceneLoad);
				TrackSceneLoaded(sceneLoad);
			}
		}

		private void OnSceneUnloaded(SceneCompat scene)
		{
			int hashCode = scene.GetHashCode();
			SceneLoad value;
			if (scenes.TryGetValue(hashCode, out value))
			{
				scenes.Remove(hashCode);
				TrackSceneUnloaded(value);
				if (scenes.Count == 0)
				{
					TrackActEnded(currentAct);
					currentAct = null;
				}
			}
		}

		private void OnPlacementShowContent(string placement)
		{
			if (currentAct != null)
			{
				currentAct.AddPlacement(placement);
			}
			foreach (SceneLoad value in scenes.Values)
			{
				value.AddPlacement(placement);
			}
			if (currentSceneActive != null)
			{
				currentSceneActive.AddPlacement(placement);
			}
		}

		private void TrackActStarted(Act act)
		{
		}

		private void TrackActEnded(Act act)
		{
			TrackUsage("Unity.actEnded", JsonUtilityCompat.ToJson(new ActDimensions(act)), JsonUtilityCompat.ToJson(new UsageValues(act)));
		}

		private void TrackSceneLoaded(SceneLoad sceneLoad)
		{
		}

		private void TrackSceneUnloaded(SceneLoad sceneLoad)
		{
			TrackUsage("Unity.sceneUnloaded", JsonUtilityCompat.ToJson(new SceneLoadDimensions(sceneLoad)), JsonUtilityCompat.ToJson(new UsageValues(sceneLoad)));
		}

		private void TrackActiveSceneChanged(SceneActive previousSceneActive, SceneCompat newActiveScene)
		{
			TrackUsage("Unity.activeSceneChanged", JsonUtilityCompat.ToJson(new SceneActiveDimensions(previousSceneActive, newActiveScene)), JsonUtilityCompat.ToJson(new UsageValues(previousSceneActive)));
		}

		private void TrackUsage(string name, string dimensions, string values)
		{
			ApiBinding.Instance.TrackUsage(name, dimensions, values);
		}
	}
}
