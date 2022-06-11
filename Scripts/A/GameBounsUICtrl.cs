using UnityEngine;
using UnityEngine.UI;

public class GameBounsUICtrl : MonoBehaviour
{
	private Transform _content;

	private Button _closeButton;

	private Text _allGetText;

	private Text _RewardText;

	private Transform _wordList;

	private GameObject _wordItemPrefab;

	private string[] _bounsWords;

	private void Awake()
	{
		_content = base.transform.Find("Content");
		_closeButton = _content.Find("Close").GetComponent<Button>();
		_allGetText = _content.Find("AleryGetCount").GetComponent<Text>();
		_RewardText = _content.Find("Reward").GetComponent<Text>();
		_wordList = _content.Find("WordsList/Viewport/Content");
		_wordItemPrefab = _wordList.GetChild(0).gameObject;
		_closeButton.onClick.AddListener(CloseUI);
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
	}

	private void Start()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
		_RewardText.text = WordConfig.bounsRewardCount.ToString();
		_bounsWords = WordCuteTools.GetSelectBounsWord();
		_allGetText.text = _bounsWords.Length + "/" + WordConfig.bounsWordsCount;
		IntiUI();
	}

	private void IntiUI()
	{
		for (int i = 0; i < _bounsWords.Length; i++)
		{
			Transform transform = Object.Instantiate(_wordItemPrefab).transform;
			transform.SetParent(_wordList, false);
			transform.gameObject.SetActive(true);
			Text component = transform.Find("Text").GetComponent<Text>();
			component.text = _bounsWords[i];
		}
	}

	private void CloseUI()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<UIManager>.instance.DelayColse(UITypeDefine.GameBounsUI, WordConfig.duration);
	}
}
