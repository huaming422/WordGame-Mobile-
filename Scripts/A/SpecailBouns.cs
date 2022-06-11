using System;
using UnityEngine;
using UnityEngine.UI;

public class SpecailBouns : MonoBehaviour
{
	private Button _pecilBoundCollectButton;

	private Text spicilBounsTime;

	private float m_IntervarUpdate;

	private bool m_IsUpdateTime = true;

	private GameObject _light;

	private GameObject _timeBack;

	private GameObject _collectImage;

	private TimeSpan specilBounsInterval = new TimeSpan(1, 0, 0);

	private void Awake()
	{
		_pecilBoundCollectButton = GetComponent<Button>();
		spicilBounsTime = base.transform.Find("Time").GetComponent<Text>();
		spicilBounsTime.text = "00:00";
		_light = base.transform.Find("Light").gameObject;
		_timeBack = base.transform.Find("Back").gameObject;
		_collectImage = base.transform.Find("Collect").gameObject;
		_pecilBoundCollectButton.onClick.AddListener(OnClickCllocet);
	}

	private void Start()
	{
		InitSpecilBounds();
	}

	private void InitSpecilBounds()
	{
		string lastGetSpecilBounsTime = GetLastGetSpecilBounsTime();
		DateTime dateTime = new DateTime(long.Parse(lastGetSpecilBounsTime));
		bool flag = (DateTime.Now - dateTime).TotalSeconds > specilBounsInterval.TotalSeconds;
		_pecilBoundCollectButton.interactable = flag;
		m_IsUpdateTime = !flag;
		spicilBounsTime.gameObject.SetActive(!flag);
		_timeBack.SetActive(!flag);
		_collectImage.SetActive(flag);
		_light.SetActive(flag);
		if (!flag)
		{
			spicilBounsTime.text = ProjectCommonFunction.GetShengYuTime(specilBounsInterval, dateTime);
		}
	}

	private string GetLastGetSpecilBounsTime()
	{
		string text = PlayerPrefs.GetString("datakey_201", string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			text = DateTime.Now.Ticks.ToString();
			PlayerPrefs.SetString("datakey_201", text);
		}
		return text;
	}

	private void Update()
	{
		if (m_IsUpdateTime)
		{
			m_IntervarUpdate += Time.deltaTime;
			if (m_IntervarUpdate > 1f)
			{
				m_IntervarUpdate = 0f;
				InitSpecilBounds();
			}
		}
	}

	private void OnClickCllocet()
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.LotteryUI);
		PlayerPrefs.SetString("datakey_201", DateTime.Now.Ticks.ToString());
		InitSpecilBounds();
	}
}
