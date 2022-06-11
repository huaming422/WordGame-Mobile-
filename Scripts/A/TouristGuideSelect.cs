using UnityEngine;

public class TouristGuideSelect : TouristGuideUICtrl
{
	private Transform _content;

	private Transform _step;

	private int _nowStep = 1;

	private Transform _nowShowStpe;

	private string[] _words = new string[2] { "GUM", "MUG" };

	private void Start()
	{
		_content = base.transform.Find("Content");
		_step = _content.Find("Step");
		InitUI();
		touristGuideUI.StartGuide(1, null);
		SingleObject<MessageManger>.instance.AddEvent(50, OnStartSelectAlphabet);
		SingleObject<MessageManger>.instance.AddEvent(51, OnEndSelectAlphabet);
	}

	private void OnDestroy()
	{
		SingleObject<MessageManger>.instance.RemoveEvent(50, OnStartSelectAlphabet);
		SingleObject<MessageManger>.instance.RemoveEvent(51, OnEndSelectAlphabet);
	}

	private void InitUI()
	{
		_nowShowStpe = _step.GetChild(_nowStep - 1);
		for (int i = 0; i < _step.childCount; i++)
		{
			Transform child = _step.GetChild(i);
			child.gameObject.SetActive(i == _nowStep - 1);
		}
	}

	private void OnStartSelectAlphabet(int id, object mes)
	{
		_nowShowStpe.gameObject.SetActive(false);
	}

	private void OnEndSelectAlphabet(int id, object mes)
	{
		string text = _words[_nowStep - 1];
		if (text.ToLower() == (mes as string).ToLower())
		{
			AplhabetTableCtrl.instance.DoWrodSelect(text);
			AplhabetSelectCtrl.instance.DoSelectRightThing(text);
			WordCuteData.playerLevelData.AddSelectWord(text);
			EffectObjCtrl.instance.ShowStimulatePic();
			if (AplhabetTableCtrl.instance.CheckIsSelectFinish())
			{
				MessageManger.SendMessage(101);
			}
			if (_nowStep == 2)
			{
				touristGuideUI.EndGuide(2, null);
				SingleObject<UIManager>.instance.Close(myToutistUIAsset);
				return;
			}
			_nowStep++;
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				InitUI();
			}, 0.5f);
		}
		else
		{
			AplhabetSelectCtrl.instance.DoSelectErrorThing(text);
			_nowShowStpe.gameObject.SetActive(true);
		}
	}
}
