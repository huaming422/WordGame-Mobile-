using UnityEngine;
using UnityEngine.Events;

internal struct FloatTween : ITweenValue
{
	private UnityAction<float> m_Target;

	private UnityAction m_FishCallBack;

	private float m_StartValue;

	private float m_TargetValue;

	private float m_Duration;

	private bool m_IgnoreTimeScale;

	public float startValue
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

	public float targetValue
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

	public UnityAction<float> tweenThing
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
			float arg = Mathf.Lerp(m_StartValue, m_TargetValue, floatPercentage);
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
