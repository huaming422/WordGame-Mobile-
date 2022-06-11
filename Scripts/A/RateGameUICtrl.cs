using UnityEngine;
using UnityEngine.UI;

public class RateGameUICtrl : MonoBehaviour
{
	private Transform _content;

	private Transform _steps;

	private Transform _startList;

	private int _nowStep;

	private int _nowStartCount;

	private void Start()
	{
		_content = base.transform.Find("Content");
		_steps = _content.Find("Step");
		ShowUIByStep();
		InitSteps();
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
	}

	private void ShowUIByStep()
	{
		for (int i = 0; i < _steps.childCount; i++)
		{
			_steps.GetChild(i).gameObject.SetActive(i == _nowStep);
		}
	}

	private void InitSteps()
	{
		for (int i = 0; i < _steps.childCount; i++)
		{
			int temp = i;
			Transform child = _steps.GetChild(i);
			Transform transform = child.Find("Yes");
			Transform transform2 = child.Find("No");
			if (transform != null)
			{
				transform.GetComponent<Button>().onClick.AddListener(delegate
				{
					OnClickYesButton(temp);
				});
			}
			if (transform2 != null)
			{
				transform2.GetComponent<Button>().onClick.AddListener(delegate
				{
					OnClickNoButton(temp);
				});
			}
			if (i == 1)
			{
				InitStartList(child);
			}
		}
	}

	private void InitStartList(Transform stepItem)
	{
		_startList = stepItem.Find("Starts");
		for (int i = 0; i < _startList.childCount; i++)
		{
			Transform child = _startList.GetChild(i);
			UpdateButton(child, false);
			int temp = i;
			child.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnClickStart(temp);
			});
		}
	}

	private void InitStarts()
	{
		for (int i = 0; i < _startList.childCount; i++)
		{
			Transform child = _startList.GetChild(i);
			UpdateButton(child, i < _nowStartCount);
		}
	}

	private void OnClickStart(int index)
	{
		_nowStartCount = index + 1;
		InitStarts();
	}

	private void UpdateButton(Transform button, bool isOn)
	{
		button.Find("On").gameObject.SetActive(isOn);
		button.Find("Off").gameObject.SetActive(true);
	}

	private void OnClickYesButton(int nowStep)
	{
		_nowStep++;
		if (nowStep == 0)
		{
			ShowUIByStep();
		}
		if (nowStep == 1)
		{
			if (_nowStartCount < 5)
			{
				CloseUI();
			}
			else
			{
				ShowUIByStep();
			}
		}
		if (nowStep == 2)
		{
			CloseUI();
			Application.OpenURL("market://details?id=com.saun.wogames&q=word%20games");
		}
	}

	private void OnClickNoButton(int nowStep)
	{
		CloseUI();
	}

	private void CloseUI()
	{
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.GameRateUI);
		}, 0.5f);
	}
}
