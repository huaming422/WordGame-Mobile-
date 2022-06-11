using UnityEngine;

public class FrameManager : SingleObject<FrameManager>
{
	private int _nowFrameRate;

	private int _lastFrameCount;

	private int _cacultInterval = 1;

	private float _nowTime;

	public int nowFrameRate
	{
		get
		{
			return _nowFrameRate;
		}
	}

	public void Init()
	{
	}

	private void Update()
	{
		_nowTime += Time.deltaTime;
		if (_nowTime >= (float)_cacultInterval)
		{
			_nowTime = 0f;
			_nowFrameRate = (Time.frameCount - _lastFrameCount) / _cacultInterval;
			_lastFrameCount = Time.frameCount;
		}
	}
}
