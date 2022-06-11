using System;
using UnityEngine;
using UnityEngine.UI;

public class UISoundCtrl : MonoBehaviour
{
	public Transform[] exceptObjs;

	public string backgroundSound = SoundType.bg;

	public bool isAutoPlayBackGroundSound;

	public bool isAutoStopBackGroundSound;

	public string buttonSound = SoundType.click;

	public string toggleSound = SoundType.click;

	public bool toggleIsOnlyOn = true;

	public bool isAutoInit = true;

	public float delayInit = 0.3f;

	private void Start()
	{
		if (isAutoInit)
		{
			if (delayInit <= 0f)
			{
				Init();
			}
			else
			{
				SingleObject<SchudleManger>.instance.Schudle(Init, delayInit);
			}
		}
		AutoPlayerBackground();
	}

	private void OnDestroy()
	{
		AutoStopBackGround();
	}

	public void Init()
	{
		Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
		Toggle[] componentsInChildren2 = GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!CheckIsExceptObj(componentsInChildren[i].transform))
			{
				componentsInChildren[i].onClick.AddListener(OnClickButton);
			}
		}
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (!CheckIsExceptObj(componentsInChildren2[j].transform))
			{
				componentsInChildren2[j].onValueChanged.AddListener(OnToggleValueChange);
			}
		}
	}

	private bool CheckIsExceptObj(Transform chil)
	{
		if (exceptObjs == null || exceptObjs.Length == 0)
		{
			return false;
		}
		int num = Array.IndexOf(exceptObjs, chil);
		if (num == -1)
		{
			return false;
		}
		return true;
	}

	private void OnToggleValueChange(bool isOn)
	{
		if (toggleIsOnlyOn)
		{
			if (isOn)
			{
				SingleObject<SoundManager>.instance.PlayWordCuteSound(toggleSound);
			}
		}
		else
		{
			SingleObject<SoundManager>.instance.PlayWordCuteSound(toggleSound);
		}
	}

	private void OnClickButton()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(buttonSound);
	}

	private void AutoPlayerBackground()
	{
		if (string.IsNullOrEmpty(backgroundSound) && isAutoPlayBackGroundSound)
		{
			SingleObject<SoundManager>.instance.PlayWordCuteSound(backgroundSound, true);
		}
	}

	private void AutoStopBackGround()
	{
		if (string.IsNullOrEmpty(backgroundSound) && isAutoStopBackGroundSound)
		{
			SingleObject<SoundManager>.instance.PlayWordCuteSound(backgroundSound, true, true);
		}
	}
}
