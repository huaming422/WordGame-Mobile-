using UnityEngine;
using UnityEngine.UI;

public class ColorAnimation : MonoBehaviour
{
	public enum LoopType
	{
		PingPong,
		Reset,
		None
	}

	public Color startColor = Color.white;

	public Color endColor = Color.white;

	public float time = 1f;

	public LoopType loopType;

	private Image m_Image;

	private float m_StartTime;

	private Color m_ImageColor = Color.white;

	private void Start()
	{
		m_Image = GetComponent<Image>();
		if (m_Image == null)
		{
			Object.Destroy(this);
			return;
		}
		m_StartTime = Time.time;
		m_ImageColor = m_Image.color;
	}

	private void Update()
	{
		m_Image.color = Color.Lerp(startColor, endColor, (Time.time - m_StartTime) / time);
		if (m_Image.color == endColor)
		{
			DoReset();
		}
	}

	private void DoReset()
	{
		if (loopType == LoopType.None)
		{
			Object.Destroy(this);
		}
		else if (loopType == LoopType.PingPong)
		{
			Color color = startColor;
			startColor = endColor;
			endColor = color;
			m_StartTime = Time.time;
		}
		else if (loopType == LoopType.Reset)
		{
			m_Image.color = startColor;
			m_StartTime = Time.time;
		}
	}

	public void SetInfor(Color color, float time, LoopType loopType = LoopType.None)
	{
		endColor = color;
		this.time = time;
		this.loopType = loopType;
	}

	public void Cover()
	{
		if (m_Image != null)
		{
			m_Image.color = m_ImageColor;
		}
	}

	public static void Destroy(GameObject target, bool isCover = false)
	{
		ColorAnimation component = target.GetComponent<ColorAnimation>();
		if (!(component == null))
		{
			if (isCover)
			{
				component.Cover();
			}
			Object.Destroy(component);
		}
	}

	public static void ColorTo(GameObject target, Color color, float time, LoopType loopType = LoopType.None)
	{
		if (!(time <= 0f))
		{
			ColorAnimation colorAnimation = target.GetComponent<ColorAnimation>();
			if (colorAnimation == null)
			{
				colorAnimation = target.AddComponent<ColorAnimation>();
			}
			colorAnimation.SetInfor(color, time, loopType);
		}
	}
}
