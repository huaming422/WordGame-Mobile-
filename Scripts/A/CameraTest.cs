using UnityEngine;

public class CameraTest : MonoBehaviour
{
	private float m_RefenceWidth = 720f;

	private float m_RefenceHeight = 1280f;

	private float m_OldScreenWidth;

	private float m_OldScreenHeight;

	private Camera m_Camera;

	private float m_CameraOldorthographicSize;

	private void Start()
	{
		m_Camera = GetComponent<Camera>();
		if (m_Camera != null)
		{
			m_CameraOldorthographicSize = m_Camera.orthographicSize;
		}
	}

	private void Update()
	{
		if ((float)Screen.height != m_OldScreenHeight || (float)Screen.width != m_OldScreenWidth)
		{
			UpdateOrthographcSize();
		}
	}

	private void UpdateOrthographcSize()
	{
		if (!(m_Camera == null))
		{
			m_OldScreenWidth = Screen.width;
			m_OldScreenHeight = Screen.height;
			float num = Screen.height;
			float num2 = Screen.width;
			float num3 = num2 / num;
			float num4 = m_RefenceWidth / m_RefenceHeight;
			float num5 = 0f;
			num5 = ((!(num3 > num4)) ? (num4 / num3) : 1f);
			m_Camera.orthographicSize = m_CameraOldorthographicSize * num5;
		}
	}
}
