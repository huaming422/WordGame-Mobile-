using UnityEngine;
using UnityEngine.Events;

internal struct IntTween : ITweenValue
{
	private UnityAction<int> m_Target;

	private UnityAction m_FishCallBack;

	private int m_StartValue;

	private int m_TargetValue;

	private float m_Duration;

	private bool m_IgnoreTimeScale;

	public int startValue
	{
		get
		{
			return m_StartValue;
		}
		set
		{
			m_StartValue = value;
		}
	}

	public int targetValue
	{
		get
		{
			return m_TargetValue;
		}
		set
		{
			m_TargetValue = value;
		}
	}

	public float duration
	{
		get
		{
			return m_Duration;
		}
		set
		{
			m_Duration = value;
		}
	}

	public bool ignoreTimeScale
	{
		get
		{
			return m_IgnoreTimeScale;
		}
		set
		{
			m_IgnoreTimeScale = value;
		}
	}

	public UnityAction<int> tweenThing
	{
		get
		{
			return m_Target;
		}
		set
		{
			m_Target = value;
		}
	}

	public UnityAction finshCallback
	{
		get
		{
			return m_FishCallBack;
		}
		set
		{
			m_FishCallBack = value;
		}
	}

	public void TweenValue(float floatPercentage)
	{
		if (ValidTarget())
		{
			int arg = Mathf.RoundToInt((float)m_StartValue + (float)(m_TargetValue - m_StartValue) * floatPercentage);
			if (m_Target != null)
			{
				m_Target(arg);
			}
		}
	}

	public bool ValidTarget()
	{
		return m_Target != null;
	}

	public void Finish()
	{
		if (finshCallback != null)
		{
			finshCallback();
		}
	}
}
