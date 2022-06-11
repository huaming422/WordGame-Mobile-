using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameMainUICtrl : MonoBehaviour, ITouristGuide
{
	public static GameMainUICtrl instance;

	private Transform _content;

	private AplhabetTableCtrl _tableCtrl;

	private AplhabetSelectCtrl _selectCtrl;

	private GameMainButtonsCtrl _buttonsCtrl;

	private void Start()
	{
		instance = this;
		_content = base.transform.Find("Content");
		_tableCtrl = _content.Find("AlphabetTable").GetComponent<AplhabetTableCtrl>();
		_selectCtrl = _content.Find("AlphabetSelect").GetComponent<AplhabetSelectCtrl>();
		_buttonsCtrl = _content.Find("GameManButtons").GetComponent<GameMainButtonsCtrl>();
		AplhabetSelectCtrl selectCtrl = _selectCtrl;
		selectCtrl.onSelectFinish = (UnityAction<string>)Delegate.Combine(selectCtrl.onSelectFinish, new UnityAction<string>(OnSelectWord));
		SingleObject<MessageManger>.instance.AddEvent(100, OnCoinCountChange);
		SingleObject<MessageManger>.instance.AddEvent(101, OnSelectFinish);
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
	}

	private void OnDestroy()
	{
		SingleObject<MessageManger>.instance.RemoveEvent(100, OnCoinCountChange);
		SingleObject<MessageManger>.instance.RemoveEvent(101, OnSelectFinish);
	}

	public void StartGame(int level)
	{
		WordCuteData.nowLevel = level;
		LevelData levelData = WordCuteTools.ReadLevelData(level);
		PlayerLevelData byLevel = PlayerLevelData.GetByLevel(level);
		if (levelData != null)
		{
			if (byLevel.isFinish)
			{
				byLevel.ClearnSelect();
			}
			WordCuteData.nowLevelData = levelData;
			WordCuteData.playerLevelData = PlayerLevelData.GetByLevel(level);
			_selectCtrl.InitSelect(WordCuteTools.StringToStingArray(levelData.useAlphabets));
			_tableCtrl.InitGridTable(levelData);
			_buttonsCtrl.InitUI();
			ReadBounsWords(level);
			PlayOpenAnimation();
			WordCuteData.gameState = GameSate.Playing;
			TouristGuideManager.TouristGuide(1, this);
		}
	}

	public void DealyStartGame(float dealy, int level)
	{
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			StartGame(level);
		}, dealy);
	}

	private void OnSelectWord(string word)
	{
		if (WordCuteData.gameState == GameSate.TouristGuide)
		{
			return;
		}
		LevelData nowLevelData = WordCuteData.nowLevelData;
		PlayerLevelData playerLevelData = WordCuteData.playerLevelData;
		if (WordCuteTools.CheckIsSelectRight(word, nowLevelData))
		{
			if (WordCuteTools.CheckIsRepeatSelect(word, playerLevelData))
			{
				_tableCtrl.PlayRepeatSelctWord(word);
				_selectCtrl.DoSelectRepeatThing(word);
				SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.word_exist);
				return;
			}
			_tableCtrl.DoWrodSelect(word);
			_selectCtrl.DoSelectRightThing(word);
			playerLevelData.AddSelectWord(word);
			EffectObjCtrl.instance.ShowStimulatePic();
			if (_tableCtrl.CheckIsSelectFinish())
			{
				MessageManger.SendMessage(101);
			}
			SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.word_out);
		}
		else if (CheckIsBounsWord(word))
		{
			AddSelectBounsWord(word);
			_selectCtrl.HiddenShowText();
			_buttonsCtrl.FlyBounsWord(word, delegate
			{
				_buttonsCtrl.UpdateBounsCountByAnimaton();
				CheckFinshAndDoReward();
			});
			SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.word_out);
		}
		else
		{
			_selectCtrl.DoSelectErrorThing(word);
			SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.word_error);
		}
	}

	private void OnSelectFinish(int id, object mes)
	{
		WordCuteData.gameState = GameSate.Finish;
		WordCuteData.playerLevelData.SetFinish();
		DoOnSelectFinish();
	}

	private void DoOnSelectFinish()
	{
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.OpenUI(UITypeDefine.GameFinishUI);
		}, 1f);
	}

	private void PlayOpenAnimation()
	{
		_selectCtrl.PlayAnimation(true);
		_tableCtrl.PlayAnimation(true);
	}

	public void CloseGameMain(UnityAction callback)
	{
		_selectCtrl.PlayAnimation(false);
		_tableCtrl.PlayAnimation(false);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.GameMain);
			callback.MyInvoke();
		}, 1f);
	}

	public void ToDating(UnityAction callback)
	{
		_selectCtrl.PlayAnimation(false);
		_tableCtrl.PlayAnimation(false);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			callback.MyInvoke();
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				SingleObject<UIManager>.instance.Close(UITypeDefine.GameMain);
			}, 0.5f);
		}, 1f);
	}

	private void OnCoinCountChange(int id, object mes)
	{
		_buttonsCtrl.UpdateCoinText();
	}

	private void CheckFinshAndDoReward()
	{
		if (CheckIsSelectFinish())
		{
			PlayerPrefs.SetString("datakey_203", string.Empty);
			Vector3 coinPos = _buttonsCtrl.GetCoinPos();
			RewardMessageBox.ShowAndSendReward(WordConfig.bounsRewardCount, coinPos, delegate
			{
				_buttonsCtrl.UpdateBounsCount();
			});
		}
	}

	private void ReadBounsWords(int level)
	{
		WordsList tableOneRowData = TableDataMannager.instance.GetTableOneRowData((WordsList line) => line.level == level);
		if (tableOneRowData != null)
		{
			WordCuteData.nowLevelBounsWords = tableOneRowData;
		}
	}

	private bool CheckIsBounsWord(string word)
	{
		if (string.IsNullOrEmpty(word))
		{
			return false;
		}
		if (WordCuteData.nowLevelBounsWords == null)
		{
			return false;
		}
		word = word.ToLower();
		string[] bouns = WordCuteData.nowLevelBounsWords.bouns;
		if (bouns.IsEmpty())
		{
			return false;
		}
		if (Array.IndexOf(bouns, word) == -1)
		{
			return false;
		}
		string[] selectBounsWord = WordCuteTools.GetSelectBounsWord();
		if (Array.IndexOf(selectBounsWord, word) == -1)
		{
			return true;
		}
		CheckFinshAndDoReward();
		return false;
	}

	private bool CheckIsSelectFinish()
	{
		string[] selectBounsWord = WordCuteTools.GetSelectBounsWord();
		return selectBounsWord.Length == WordConfig.bounsWordsCount;
	}

	private void AddSelectBounsWord(string word)
	{
		word = word.ToLower();
		string @string = PlayerPrefs.GetString("datakey_203", string.Empty);
		Hashtable hashtable = null;
		hashtable = ((!string.IsNullOrEmpty(@string)) ? @string.DecodeJson() : new Hashtable());
		ArrayList arrayList = null;
		arrayList = (hashtable.ContainsKey("words") ? hashtable.GetArrayList("words") : new ArrayList());
		arrayList.Add(word);
		hashtable["words"] = arrayList;
		string value = hashtable.ToJson();
		PlayerPrefs.SetString("datakey_203", value);
	}

	public void StartGuide(int index, object mes)
	{
		WordCuteData.gameState = GameSate.TouristGuide;
	}

	public void EndGuide(int index, object mes)
	{
		TouristGuideManager.SetTouristGuideState(1, 1);
		WordCuteData.gameState = GameSate.Finish;
	}
}
