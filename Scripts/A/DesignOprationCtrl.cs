using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DesignOprationCtrl : MonoBehaviour
{
	private InputField _level;

	private Button _saveLevelButton;

	private Button _openLevelButton;

	private InputField _searchContent;

	private Button _searchButton;

	private Button _clearnButton;

	private Button _deleteButton;

	private Button _backButton;

	private Button _copyUseButton;

	private Button _updateButton;

	private Button _savePos;

	private InputField[] _allInputField;

	private void Start()
	{
		_level = base.transform.Find("Level").GetComponent<InputField>();
		_saveLevelButton = base.transform.Find("Save").GetComponent<Button>();
		_openLevelButton = base.transform.Find("Open").GetComponent<Button>();
		_searchContent = base.transform.Find("SearchContent").GetComponent<InputField>();
		_searchButton = base.transform.Find("Search").GetComponent<Button>();
		_clearnButton = base.transform.Find("Clearn").GetComponent<Button>();
		_deleteButton = base.transform.Find("Delet").GetComponent<Button>();
		_backButton = base.transform.Find("Back").GetComponent<Button>();
		_copyUseButton = base.transform.Find("Copy").GetComponent<Button>();
		_updateButton = base.transform.Find("Update").GetComponent<Button>();
		_savePos = base.transform.Find("SavePos").GetComponent<Button>();
		_saveLevelButton.onClick.AddListener(OnClickSaveButton);
		_openLevelButton.onClick.AddListener(OnClickOpenLevelButton);
		_searchButton.onClick.AddListener(OnClickSearch);
		_clearnButton.onClick.AddListener(OnClickClearnButton);
		_deleteButton.onClick.AddListener(OnClickDeleteButton);
		_backButton.onClick.AddListener(OnClickBackButton);
		_copyUseButton.onClick.AddListener(OnClickCopyUse);
		_updateButton.onClick.AddListener(OnClickUpdate);
		_savePos.onClick.AddListener(OnClickSavePos);
		_allInputField = GetComponentsInChildren<InputField>();
	}

	private void OnClickCopyUse()
	{
		if (WorldDesignUICtrl.nowLevelData != null)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = WorldDesignUICtrl.nowLevelData.useAlphabets;
			textEditor.OnFocus();
			textEditor.Copy();
		}
	}

	private bool CanInputEvent()
	{
		if (_allInputField.IsEmpty())
		{
			return true;
		}
		for (int i = 0; i < _allInputField.Length; i++)
		{
			if (_allInputField[i].isFocused)
			{
				return false;
			}
		}
		return true;
	}

	private void Update()
	{
		CheckAlphabetKey();
		CheckDeleteKey();
	}

	private void CheckAlphabetKey()
	{
		if (!CanInputEvent())
		{
			return;
		}
		for (int i = 97; i < 123; i++)
		{
			KeyCode key = (KeyCode)i;
			if (Input.GetKey(key))
			{
				DoClickAlphabet(key);
			}
		}
	}

	private void CheckDeleteKey()
	{
		if (CanInputEvent() && (Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace)))
		{
			OnClickDeleteButton();
		}
	}

	private void DoClickAlphabet(KeyCode key)
	{
		DisignGridCtrl.instance.SetNowText(key.ToString());
	}

	private void OnClickSaveButton()
	{
		string text = _level.text;
		int result = 0;
		if (!int.TryParse(text, out result))
		{
			DisignInfoCtrl.instance.LogError("关卡输入错误");
		}
		else if (result < 1)
		{
			DisignInfoCtrl.instance.LogError("关卡输入错误");
		}
		else
		{
			WorldDesignUICtrl.SaveLevelData(result);
		}
	}

	private void OnClickOpenLevelButton()
	{
		string text = _level.text;
		int result = 0;
		if (!int.TryParse(text, out result))
		{
			DisignInfoCtrl.instance.LogError("关卡输入错误");
		}
		else if (result < 1)
		{
			DisignInfoCtrl.instance.LogError("关卡输入错误");
		}
		else
		{
			WorldDesignUICtrl.OpenLevel(result);
		}
	}

	private void OnClickClearnButton()
	{
		DisignGridCtrl.instance.Clearn();
	}

	private void OnClickDeleteButton()
	{
		DisignGridCtrl.instance.SetNowText(string.Empty);
	}

	private void OnClickBackButton()
	{
		DisignGridCtrl.instance.ShowState();
	}

	private void OnClickUpdate()
	{
		DisignGridCtrl.instance.UpdateLevelInfo();
	}

	private void OnClickSavePos()
	{
		string text = _level.text;
		int result = 0;
		if (!int.TryParse(text, out result))
		{
			DisignInfoCtrl.instance.LogError("关卡输入错误");
		}
		else if (result < 1)
		{
			DisignInfoCtrl.instance.LogError("关卡输入错误");
		}
		else
		{
			WorldDesignUICtrl.ForceSave(result);
		}
	}

	private void OnClickSearch()
	{
		string text = _searchContent.text;
		if (string.IsNullOrEmpty(text))
		{
			DisignInfoCtrl.instance.LogError("输入为空!!!!");
			return;
		}
		text = text.ToLower();
		text = Regex.Replace(text, "[^a-z0-9:\\(\\)&\\|\\*\\-]", string.Empty);
		DecodeSearchContent(text);
	}

	private void DecodeSearchContent(string searchContent)
	{
		if (!DesignCommonTools.CheckHaveOtherSymbol(searchContent))
		{
			DoNoFilterSearch(searchContent);
		}
		else
		{
			DoFilterSearch(searchContent);
		}
	}

	private void DoNoFilterSearch(string searchContent)
	{
		//List<WordLine> likeWorldLine = SingleObject<DisignDBManager>.instance.GetLikeWorldLine(searchContent);
		//WordListCtrl.instance.InitList(likeWorldLine);
	}

	private void DoFilterSearch(string searchContent)
	{
		SingleSysObj<SerchFilter>.instance.BuildFilter(searchContent);
		List<WordLine> filterSearchResult = GetFilterSearchResult(searchContent);
		List<WordLine> list = new List<WordLine>();
		for (int i = 0; i < filterSearchResult.Count; i++)
		{
			if (SingleSysObj<SerchFilter>.instance.CheckSrc(filterSearchResult[i].word))
			{
				list.Add(filterSearchResult[i]);
			}
		}
		WordListCtrl.instance.InitList(list);
	}

	private List<WordLine> GetFilterSearchResult(string searchContent)
	{
		MatchCollection matchCollection = Regex.Matches(searchContent, "[a-z0-9]+:[a-z0-9\\*\\-]+");
		string contans = string.Empty;
		string text = string.Empty;
		string lengthMax = string.Empty;
		for (int i = 0; i < matchCollection.Count; i++)
		{
			string text2 = matchCollection[i].ToString();
			string[] array = text2.Split(':');
			if (array[0] == "l")
			{
				if (array[1].IndexOf("-") != -1)
				{
					string[] array2 = array[1].Split('-');
					text = array2[0];
					lengthMax = array2[1];
				}
				else
				{
					text = array[1];
					lengthMax = text;
				}
			}
			if (array[0] == "c")
			{
				contans = array[1];
			}
		}
		return SingleObject<DisignDBManager>.instance.GetFilerSearchReult(contans, text, lengthMax);
	}
}
