using UnityEngine;

public class SingleObject<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T m_Instance;

	public static T instance
	{
		get
		{
			if ((Object)m_Instance == (Object)null)
			{
				m_Instance = Object.FindObjectOfType<T>();
				if ((Object)m_Instance == (Object)null)
				{
					GameObject gameObject = GameObject.Find("[GameManager]");
					if (gameObject == null)
					{
						gameObject = new GameObject("[GameManager]");
						Object.DontDestroyOnLoad(gameObject);
					}
					m_Instance = gameObject.AddComponent<T>();
				}
			}
			return m_Instance;
		}
	}
}
