using UnityEngine;
using UnityEngine.Events;

internal struct ColorTween : ITweenValue
{
	private enum ColorTweenMode
	{
		All,
		RGB,
		Alpha
	}

	private UnityAction<Color> m_Target;

	private UnityAction m_FishCallBack;

	private Color m_StartColor;

	private Color m_TargetColor;

	private ColorTweenMode m_TweenMode;

	private float m_Duration;

	private bool m_IgnoreTimeScale;

	public Color startColor
	{
		get
		{
			return m_StartColor;
		}
		set
		{
			m_StartColor = value;
		}
	}

	public Color targetColor
	{
		get
		{
			return m_TargetColor;
		}
		set
		{
			m_TargetColor = value;
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

	public UnityAction<Color> tweenThing
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

	public void SetColorTweenMode(int value)
	{
		m_TweenMode = (ColorTweenMode)value;
	}

	public void TweenValue(float floatPercentage)
	{
		if (ValidTarget())
		{
			Color arg = Color.Lerp(m_StartColor, m_TargetColor, floatPercentage);
			if (m_TweenMode == ColorTweenMode.Alpha)
			{
				arg.r = m_StartColor.r;
				arg.g = m_StartColor.g;
				arg.b = m_StartColor.b;
			}
			else if (m_TweenMode == ColorTweenMode.RGB)
			{
				arg.a = m_StartColor.a;
			}
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
