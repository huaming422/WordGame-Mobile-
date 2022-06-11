using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SchudleManger : SingleObject<SchudleManger>
{
	private class SchudelItem
	{
		public UnityAction action;

		public float delayTime;

		public bool isLoop;

		public int loopCount;

		public bool isStop;

		public int aleryLoopCount;

		public UnityAction loopFinishCallback;

		public float startTime;

		private bool _isDestroy;

		public bool destroyed
		{
			get
			{
				return _isDestroy;
			}
		}

		public SchudelItem(UnityAction action, float time, bool isLoop, int loopCount, UnityAction loopFinishCallback)
		{
			this.action = action;
			this.isLoop = isLoop;
			this.loopCount = loopCount;
			delayTime = time;
			this.loopFinishCallback = loopFinishCallback;
		}

		public void Destroy()
		{
			_isDestroy = true;
		}
	}

	public delegate bool LoopCondition();

	private class LoopSchudleItem
	{
		public LoopCondition action;

		public float delayTime;

		public float lastExecteTime;

		public bool isStop;

		public UnityAction finshCallback;

		private bool _isDestroy;

		public bool destroyed
		{
			get
			{
				return _isDestroy;
			}
		}

		public LoopSchudleItem(LoopCondition action, float delayTime, float lastExecteTime, UnityAction finshCallback)
		{
			this.action = action;
			this.delayTime = delayTime;
			this.finshCallback = finshCallback;
			this.lastExecteTime = lastExecteTime;
		}

		public void Destroy()
		{
			_isDestroy = true;
		}
	}

	private List<SchudelItem> _allSchuldeItems = new List<SchudelItem>();

	private List<LoopSchudleItem> _allLoopSchudleItems = new List<LoopSchudleItem>();

	private void Start()
	{
		StartCoroutine(StartExecuteSchudle());
		StartCoroutine(StarExecuteLoopSchudle());
	}

	public void Schudle(UnityAction action, float time, bool isLoop = false, int loopCount = -1, UnityAction loopFinishCallback = null)
	{
		if (!isLoop || loopCount > 0)
		{
			if (time <= 0f)
			{
				action();
				return;
			}
			SchudelItem schudelItem = new SchudelItem(action, time, isLoop, loopCount, loopFinishCallback);
			schudelItem.startTime = Time.time;
			_allSchuldeItems.Add(schudelItem);
		}
	}

	private IEnumerator StartExecuteSchudle()
	{
		float nowTime2 = 0f;
		SchudelItem item2 = null;
		while (true)
		{
			yield return null;
			if (_allSchuldeItems.Count == 0)
			{
				continue;
			}
			nowTime2 = Time.time;
			for (int i = 0; i < _allSchuldeItems.Count; i++)
			{
				item2 = _allSchuldeItems[i];
				if (item2 == null || item2.destroyed || item2.action == null)
				{
					_allSchuldeItems.RemoveAt(i);
					i--;
				}
				else
				{
					if (nowTime2 - item2.startTime < item2.delayTime)
					{
						continue;
					}
					SafeExecute(item2.action);
					if (item2.isLoop)
					{
						item2.aleryLoopCount++;
						item2.startTime = nowTime2;
						if (item2.aleryLoopCount < item2.loopCount)
						{
							continue;
						}
						SafeExecute(item2.loopFinishCallback);
					}
					_allSchuldeItems.RemoveAt(i);
					i--;
				}
			}
		}
	}

	private void SafeExecute(UnityAction action)
	{
		if (action == null)
		{
			return;
		}
		try
		{
			action();
		}
		catch (Exception ex)
		{
			if (AppConst.IsOpenLog)
			{
				Util.LogWarning("SchudleManager exceute error ---->>>>" + ex.ToString());
			}
		}
	}

	public void UnSchudle(UnityAction action)
	{
		if (action == null)
		{
			return;
		}
		for (int i = 0; i < _allSchuldeItems.Count; i++)
		{
			if (action == _allSchuldeItems[i].action)
			{
				_allSchuldeItems[i].Destroy();
				_allSchuldeItems.RemoveAt(i);
				break;
			}
		}
	}

	public void ClearAll()
	{
		for (int i = 0; i < _allSchuldeItems.Count; i++)
		{
			_allSchuldeItems[i].Destroy();
		}
		_allSchuldeItems.Clear();
	}

	public void MustExecuteSchudle(UnityAction action, float time)
	{
		StartCoroutine(ExecuteMustSchdle(action, time));
	}

	private IEnumerator ExecuteMustSchdle(UnityAction action, float time)
	{
		yield return new WaitForSeconds(time);
		if (action != null)
		{
			action();
		}
	}

	public void NextFrame(UnityAction action)
	{
		StartCoroutine(ExecuteMustNextFrameSchdle(action));
	}

	private IEnumerator ExecuteMustNextFrameSchdle(UnityAction action)
	{
		yield return null;
		if (action != null)
		{
			action();
		}
	}

	public void LoopSchudle(LoopCondition action, float time, UnityAction finshCallback = null)
	{
		if (action != null && !IsRepeatAdd(action))
		{
			LoopSchudleItem item = new LoopSchudleItem(action, time, Time.time, finshCallback);
			_allLoopSchudleItems.Add(item);
		}
	}

	private bool IsRepeatAdd(LoopCondition action)
	{
		for (int i = 0; i < _allLoopSchudleItems.Count; i++)
		{
			if (_allLoopSchudleItems[i].action == action)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator StarExecuteLoopSchudle()
	{
		float nowTime2 = 0f;
		LoopSchudleItem item2 = null;
		while (true)
		{
			yield return null;
			if (_allLoopSchudleItems.Count == 0)
			{
				continue;
			}
			nowTime2 = Time.time;
			for (int i = 0; i < _allLoopSchudleItems.Count; i++)
			{
				item2 = _allLoopSchudleItems[i];
				if (item2 == null || item2.destroyed || item2.action == null)
				{
					_allLoopSchudleItems.RemoveAt(i);
					i--;
				}
				else if (!item2.isStop && !(nowTime2 - item2.lastExecteTime < item2.delayTime))
				{
					if (SafeExecuteLoopSchulde(item2.action))
					{
						item2.lastExecteTime = nowTime2;
						continue;
					}
					SafeExecute(item2.finshCallback);
					_allLoopSchudleItems.RemoveAt(i);
					i--;
				}
			}
		}
	}

	private bool SafeExecuteLoopSchulde(LoopCondition action)
	{
		try
		{
			return action();
		}
		catch (Exception ex)
		{
			Util.LogWarning("Execute loop Schulde error ---------------->>>>>>:" + ex.ToString());
		}
		return false;
	}

	public void RemoveLoopSchudle(LoopCondition action)
	{
		if (action == null)
		{
			return;
		}
		for (int i = 0; i < _allLoopSchudleItems.Count; i++)
		{
			if (_allLoopSchudleItems[i].action == action)
			{
				_allLoopSchudleItems[i].Destroy();
				_allLoopSchudleItems.RemoveAt(i);
				break;
			}
		}
	}

	public void RemoveLoopSchudleAll()
	{
		for (int i = 0; i < _allLoopSchudleItems.Count; i++)
		{
			_allLoopSchudleItems[i].Destroy();
		}
		_allLoopSchudleItems.Clear();
	}

	public void StopLoopSchudle(LoopCondition action)
	{
		if (action == null)
		{
			return;
		}
		for (int i = 0; i < _allLoopSchudleItems.Count; i++)
		{
			if (_allLoopSchudleItems[i].action == action)
			{
				_allLoopSchudleItems[i].isStop = true;
				break;
			}
		}
	}

	public void StartLoopchudle(LoopCondition action)
	{
		if (action == null)
		{
			return;
		}
		for (int i = 0; i < _allLoopSchudleItems.Count; i++)
		{
			if (_allLoopSchudleItems[i].action == action)
			{
				_allLoopSchudleItems[i].isStop = false;
				break;
			}
		}
	}
}
