using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace DG.Tweening.Core
{
	[AddComponentMenu("")]
	public class DOTweenComponent : MonoBehaviour, IDOTweenInit
	{
		public int inspectorUpdater;

		private float _unscaledTime;

		private float _unscaledDeltaTime;

		private bool _duplicateToDestroy;

		private void Awake()
		{
			inspectorUpdater = 0;
			_unscaledTime = Time.realtimeSinceStartup;
			Type looseScriptType = Utils.GetLooseScriptType("DG.Tweening.DOTweenModuleUtils");
			if (looseScriptType == null)
			{
				Debug.LogError("DOTween ► Couldn't load Modules system");
			}
			else
			{
				looseScriptType.GetMethod("Init", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
			}
		}

		private void Start()
		{
			if (DOTween.instance != this)
			{
				_duplicateToDestroy = true;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Update()
		{
			_unscaledDeltaTime = Time.realtimeSinceStartup - _unscaledTime;
			if (DOTween.useSmoothDeltaTime && _unscaledDeltaTime > DOTween.maxSmoothUnscaledTime)
			{
				_unscaledDeltaTime = DOTween.maxSmoothUnscaledTime;
			}
			if (TweenManager.hasActiveDefaultTweens)
			{
				TweenManager.Update(UpdateType.Normal, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) * DOTween.timeScale, _unscaledDeltaTime * DOTween.timeScale);
			}
			_unscaledTime = Time.realtimeSinceStartup;
			if (!TweenManager.isUnityEditor)
			{
				return;
			}
			inspectorUpdater++;
			if (DOTween.showUnityEditorReport && TweenManager.hasActiveTweens)
			{
				if (TweenManager.totActiveTweeners > DOTween.maxActiveTweenersReached)
				{
					DOTween.maxActiveTweenersReached = TweenManager.totActiveTweeners;
				}
				if (TweenManager.totActiveSequences > DOTween.maxActiveSequencesReached)
				{
					DOTween.maxActiveSequencesReached = TweenManager.totActiveSequences;
				}
			}
		}

		private void LateUpdate()
		{
			if (TweenManager.hasActiveLateTweens)
			{
				TweenManager.Update(UpdateType.Late, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) * DOTween.timeScale, _unscaledDeltaTime * DOTween.timeScale);
			}
		}

		private void FixedUpdate()
		{
			if (TweenManager.hasActiveFixedTweens && Time.timeScale > 0f)
			{
				TweenManager.Update(UpdateType.Fixed, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) * DOTween.timeScale, (DOTween.useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime) / Time.timeScale * DOTween.timeScale);
			}
		}

		internal void ManualUpdate(float deltaTime, float unscaledDeltaTime)
		{
			if (TweenManager.hasActiveManualTweens)
			{
				TweenManager.Update(UpdateType.Manual, deltaTime * DOTween.timeScale, unscaledDeltaTime * DOTween.timeScale);
			}
		}

		private void OnDrawGizmos()
		{
			if (!DOTween.drawGizmos || !TweenManager.isUnityEditor)
			{
				return;
			}
			int count = DOTween.GizmosDelegates.Count;
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					DOTween.GizmosDelegates[i]();
				}
			}
		}

		private void OnDestroy()
		{
			if (!_duplicateToDestroy)
			{
				if (DOTween.showUnityEditorReport)
				{
					Debugger.LogReport("REPORT > Max overall simultaneous active Tweeners/Sequences: " + DOTween.maxActiveTweenersReached + "/" + DOTween.maxActiveSequencesReached);
				}
				if (DOTween.instance == this)
				{
					DOTween.instance = null;
				}
			}
		}

		private void OnApplicationQuit()
		{
			DOTween.isQuitting = true;
		}

		public IDOTweenInit SetCapacity(int tweenersCapacity, int sequencesCapacity)
		{
			TweenManager.SetCapacities(tweenersCapacity, sequencesCapacity);
			return this;
		}

		internal IEnumerator WaitForCompletion(Tween t)
		{
			while (t.active && !t.isComplete)
			{
				yield return null;
			}
		}

		internal IEnumerator WaitForRewind(Tween t)
		{
			while (t.active && (!t.playedOnce || t.position * (float)(t.completedLoops + 1) > 0f))
			{
				yield return null;
			}
		}

		internal IEnumerator WaitForKill(Tween t)
		{
			while (t.active)
			{
				yield return null;
			}
		}

		internal IEnumerator WaitForElapsedLoops(Tween t, int elapsedLoops)
		{
			while (t.active && t.completedLoops < elapsedLoops)
			{
				yield return null;
			}
		}

		internal IEnumerator WaitForPosition(Tween t, float position)
		{
			while (t.active && t.position * (float)(t.completedLoops + 1) < position)
			{
				yield return null;
			}
		}

		internal IEnumerator WaitForStart(Tween t)
		{
			while (t.active && !t.playedOnce)
			{
				yield return null;
			}
		}

		internal static void Create()
		{
			if (!(DOTween.instance != null))
			{
				GameObject obj = new GameObject("[DOTween]");
				UnityEngine.Object.DontDestroyOnLoad(obj);
				DOTween.instance = obj.AddComponent<DOTweenComponent>();
			}
		}

		internal static void DestroyInstance()
		{
			if (DOTween.instance != null)
			{
				UnityEngine.Object.Destroy(DOTween.instance.gameObject);
			}
			DOTween.instance = null;
		}
	}
}
