using UnityEngine;
using UnityEngine.Events;

public class GamePlayPosManager : SingleObject<GamePlayPosManager>
{
	private float m_LastPressEscape;

	private float m_DoublePressInterval = 0.2f;

	private bool m_IsWait;

	public UnityEvent onAnglePress = new UnityEvent();

	public UnityEvent onDoublePrss = new UnityEvent();

	public bool canUseEscapeBack = true;

	public void Init()
	{
	}

	private void Update()
	{
		if (!canUseEscapeBack)
		{
			return;
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (m_IsWait && Time.time - m_LastPressEscape < m_DoublePressInterval)
			{
				onDoublePrss.Invoke();
				m_IsWait = false;
				return;
			}
			m_IsWait = true;
			m_LastPressEscape = Time.time;
		}
		if (m_IsWait && Time.time - m_LastPressEscape > m_DoublePressInterval)
		{
			onAnglePress.Invoke();
			m_IsWait = false;
		}
	}
}
