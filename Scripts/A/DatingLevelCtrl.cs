using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DatingLevelCtrl : MonoBehaviour, IFixCountScrollDataCtrl
{
	public UnityAction<LevelButtonCtrl> onButtonClick;

	public UnityAction<LevelButtonCtrl> onButtonChange;

	private FixCountScrollRect _fixCountScrollRect;

	private Transform _levelItemPrefabs;

	private Transform _levelButtons;

	private GameObject _levelButonPrefab;

	private Transform _headPic;

	private Transform _headPicFallowButton;

	private Transform _heaPicFollowLevelItem;

	private Button _jumpToNowButton;

	private Transform _background;

	private int _levelItemPrefabCount;

	private Dictionary<int, Transform> _levelItemInstanceCahe = new Dictionary<int, Transform>();

	private List<int> _levelItemsLevelsCount = new List<int>();

	private int _nowLevelItemIndex;

	private int _backgroundImageCount = 20;

	private bool _changeImageIsFinish = true;

	private void Awake()
	{
		_fixCountScrollRect = base.transform.Find("LevelContaner").GetComponent<FixCountScrollRect>();
		_levelItemPrefabs = base.transform.Find("LevelItemPrefabs");
		_levelButtons = base.transform.Find("LevelButtons");
		_levelButonPrefab = _levelButtons.Find("LevelButton").gameObject;
		_headPic = base.transform.Find("HeadPic");
		_jumpToNowButton = base.transform.Find("JumpNowPos").GetComponent<Button>();
		_jumpToNowButton.onClick.AddListener(OnClickJumpNowPos);
		_background = base.transform.Find("Background");
		_headPic.gameObject.SetActive(false);
		_fixCountScrollRect.RegistDataCtrl(this);
		InitData();
	}

	private void Start()
	{
		_nowLevelItemIndex = GetNowLevelItemIndex(PlayerData.i.openLevel);
		_fixCountScrollRect.InitFixCountScorll();
	}

	private void Update()
	{
		if (!(_headPicFallowButton == null) && !(_heaPicFollowLevelItem == null))
		{
			if (AttachData<int>.GetData(_headPicFallowButton) == PlayerData.i.openLevel && AttachData<int>.GetData(_heaPicFollowLevelItem) == _nowLevelItemIndex)
			{
				_headPic.gameObject.SetActive(true);
				_headPic.position = _headPicFallowButton.position;
			}
			else
			{
				_headPic.gameObject.SetActive(false);
			}
		}
	}

	private void OnDestroy()
	{
		AttachData<int>.Destroy();
	}

	private void InitData()
	{
		_levelItemPrefabCount = _levelItemPrefabs.childCount;
		for (int i = 0; i < _levelItemPrefabCount; i++)
		{
			_levelItemsLevelsCount.Add(_levelItemPrefabs.GetChild(i).Find("levels").childCount);
		}
	}

	private int GetNowItemStartLevel(int itemIndex)
	{
		int num = 0;
		int num2 = 1;
		while (num < itemIndex)
		{
			for (int i = 0; i < _levelItemsLevelsCount.Count; i++)
			{
				num2 += _levelItemsLevelsCount[i];
				num++;
				if (num >= itemIndex)
				{
					break;
				}
			}
		}
		return num2;
	}

	private int GetNowLevelItemIndex(int nowLevel)
	{
		int num = 0;
		int num2 = 1;
		while (num2 < nowLevel)
		{
			for (int i = 0; i < _levelItemsLevelsCount.Count; i++)
			{
				num2 += _levelItemsLevelsCount[i];
				if (num2 >= nowLevel)
				{
					break;
				}
				num++;
			}
		}
		return num;
	}

	private void InitLevelItem(Transform levelItem, int itemIndex)
	{
		int nowItemStartLevel = GetNowItemStartLevel(itemIndex);
		Transform transform = levelItem.Find("levels");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			Transform transform2 = null;
			if (child.childCount == 0)
			{
				transform2 = UnityEngine.Object.Instantiate(_levelButonPrefab, child).transform;
				transform2.gameObject.SetActive(true);
				transform2.localPosition = Vector3.zero;
			}
			else
			{
				transform2 = child.GetChild(0);
			}
			InitButtonByLevel(transform2, nowItemStartLevel + i, itemIndex);
		}
		if (itemIndex % _levelItemPrefabCount == 0)
		{
			Transform transform3 = levelItem.Find("SpecilLine");
			transform3.gameObject.SetActive(itemIndex != 0);
		}
		InitCloudAndLine(levelItem, nowItemStartLevel, transform);
	}

	private void InitCloudAndLine(Transform levelItem, int nowItemStartLevel, Transform levels)
	{
		Image component = levelItem.Find("Line").GetComponent<Image>();
		Transform transform = levelItem.Find("Cloud");
		if (nowItemStartLevel + levels.childCount <= WordConfig.showGameMaxLevel)
		{
			component.fillAmount = 1f;
			transform.gameObject.SetActive(false);
			return;
		}
		transform.gameObject.SetActive(true);
		int num = WordConfig.showGameMaxLevel - nowItemStartLevel;
		if (num >= 0 && num < levels.childCount)
		{
			Vector3 vector2 = (transform.localPosition = new Vector3(0f, levels.GetChild(num).localPosition.y + 150f, 0f));
			Vector2 size = (levelItem as RectTransform).rect.size;
			component.fillAmount = (vector2.y + size.y / 2f) / size.y;
		}
	}

	private void InitButtonByLevel(Transform levelButton, int level, int itemIndex)
	{
		levelButton.gameObject.SetActive(level <= WordConfig.showGameMaxLevel);
		LevelButtonCtrl buttonCtrl = levelButton.GetComponent<LevelButtonCtrl>();
		PlayerLevelData byLevel = PlayerLevelData.GetByLevel(level);
		buttonCtrl.InitButton(level, byLevel.isPass, byLevel.starCount);
		buttonCtrl.onClickButton = delegate
		{
			onButtonClick.MyInvoke(buttonCtrl);
		};
		onButtonChange.MyInvoke(buttonCtrl);
		AttachData<int>.Attach(levelButton, level);
		if (PlayerData.i.openLevel == level)
		{
			_headPicFallowButton = levelButton;
		}
	}

	private Transform GetLevelItemBuyIndex(int index)
	{
		Transform value = null;
		if (!_levelItemInstanceCahe.TryGetValue(index, out value))
		{
			value = _levelItemPrefabs.Find("LevelItem" + (index + 1));
			_levelItemInstanceCahe.Add(index, value);
		}
		value.gameObject.SetActive(true);
		return value;
	}

	private void FixScrollUpdateLevelItem(int index, GameObject obj)
	{
		index = Mathf.Abs(index);
		Transform transform = obj.transform;
		if (transform.childCount > 0)
		{
			transform.ActiveChils(false);
		}
		Transform levelItemBuyIndex = GetLevelItemBuyIndex(index % _levelItemPrefabCount);
		levelItemBuyIndex.SetParent(transform);
		levelItemBuyIndex.localPosition = Vector3.zero;
		InitLevelItem(levelItemBuyIndex, index);
		AttachData<int>.Attach(obj.transform, index);
		if (index == _nowLevelItemIndex)
		{
			_heaPicFollowLevelItem = obj.transform;
		}
	}

	public bool CanCreateChild(int index)
	{
		return true;
	}

	public void CreateChildItem(int index, GameObject obj)
	{
		FixScrollUpdateLevelItem(index, obj);
	}

	public bool CanUpdateChildItemDataIndex(int index)
	{
		if (index > 0)
		{
			return false;
		}
		index = Mathf.Abs(index);
		int nowItemStartLevel = GetNowItemStartLevel(Math.Abs(index));
		return nowItemStartLevel <= WordConfig.showGameMaxLevel;
	}

	public void UpdateChildItemDataIndex(int index, GameObject obj)
	{
		ShowBackgroundImage(Math.Abs(index), true);
		FixScrollUpdateLevelItem(index, obj);
	}

	public void UpdateChildItemPosIndex(int posIndex, GameObject obj)
	{
	}

	public void ShowBackgroundImage(int levelItemIndex, bool isAnimation)
	{
		int num = levelItemIndex % _backgroundImageCount + 1;
		Image component = _background.Find("Show").GetComponent<Image>();
		Sprite sprite = SingleObject<ResourceManager>.instance.LoadMulitSprite(WordConstString.datingBackgroundImage, "Background", "bg" + num, null);
		if (!isAnimation)
		{
			component.sprite = sprite;
		}
		else if (_changeImageIsFinish)
		{
			_changeImageIsFinish = false;
			Image component2 = _background.Find("Temp").GetComponent<Image>();
			component2.sprite = component.sprite;
			component2.color = Color.white;
			component.sprite = sprite;
			component.color = new Color(1f, 1f, 1f, 0f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(component.DOFade(1f, 3f));
			sequence.Join(component2.DOFade(0f, 3f));
			sequence.AppendCallback(delegate
			{
				_changeImageIsFinish = true;
			});
			sequence.Play();
		}
	}

	public Transform GetButtonByLevel(int level)
	{
		return null;
	}

	public void MoveHeadPicToLevel(int level, bool isAnimation)
	{
	}

	private void OnClickJumpNowPos()
	{
		int nowLevelItemIndex = GetNowLevelItemIndex(PlayerData.i.openLevel);
		JumpToNowPos(nowLevelItemIndex);
	}

	private void JumpToNowPos(int levelItemIndex)
	{
		ShowBackgroundImage(levelItemIndex, false);
		_fixCountScrollRect.JumpToPos(-levelItemIndex);
	}

	public void JumpToNowPos(int level, bool isAnimation)
	{
		JumpToNowPos(_nowLevelItemIndex);
	}
}
