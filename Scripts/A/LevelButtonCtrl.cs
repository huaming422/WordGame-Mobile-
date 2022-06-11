using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelButtonCtrl : MonoBehaviour
{
	private Button _button;

	private Transform _pass;

	private Transform _noPass;

	private Transform _starts;

	public UnityAction onClickButton;

	private int _level = 1;

	public int level
	{
		get
		{
			return _level;
		}
	}

	private void Awake()
	{
		_button = GetComponent<Button>();
		_pass = base.transform.Find("Pass");
		_noPass = base.transform.Find("NoPass");
		_starts = base.transform.Find("Starts");
		_button.onClick.AddListener(OnClickButton);
	}

	public void InitButton(int level, bool isOpen, int startCount)
	{
		_level = level;
		SetLevel(_pass, level);
		SetLevel(_noPass, level);
		_pass.gameObject.SetActive(isOpen);
		_noPass.gameObject.SetActive(!isOpen);
		InitStart(startCount);
	}

	public void SetIsPass(bool isPass = true)
	{
		_pass.gameObject.SetActive(isPass);
		_noPass.gameObject.SetActive(!isPass);
	}

	private void SetLevel(Transform item, int level)
	{
		item.Find("Text").GetComponent<Text>().text = level.ToString();
	}

	private void InitStart(int startCount)
	{
		for (int i = 0; i < _starts.childCount; i++)
		{
			Transform child = _starts.GetChild(i);
			ProjectCommonFunction.UpdateButton(child, i < startCount);
		}
	}

	private void OnClickButton()
	{
		onClickButton();
	}
}
