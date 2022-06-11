using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishUICtrl : MonoBehaviour
{
	private Transform _content;

	private Button _nextButton;

	private Button _rePlayButton;

	private Text _brilliance;

	private Text _levelText;

	private Transform _starts;

	private Transform _hueStarts;

	private ParticleSystem _effect;

	private void Awake()
	{
		_content = base.transform.Find("Content");
		_nextButton = _content.Find("NextButton").GetComponent<Button>();
		_rePlayButton = _content.Find("RePlay").GetComponent<Button>();
		_brilliance = _content.Find("Brilliance").GetComponent<Text>();
		_levelText = _content.Find("Level").GetComponent<Text>();
		_effect = _content.Find("Fireworks").GetComponent<ParticleSystem>();
		_starts = _content.Find("Starts");
		_hueStarts = _starts.Find("HueStart");
		_nextButton.onClick.AddListener(OnClickNextButton);
		_rePlayButton.onClick.AddListener(OnClickReplay);
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
	}

	private void Start()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
		InitUI();
		UpdateOpenLevel();
	}

	private void InitUI()
	{
		_levelText.text = WordCuteData.nowLevel.ToString();
		_brilliance.text = PlayerData.i.brilliance.ToString();
		int startCount = WordCuteTools.GetStarCountByPlayerLevelData(WordCuteData.playerLevelData);
		int starCount = WordCuteData.playerLevelData.starCount;
		WordCuteData.playerLevelData.starCount = ((startCount <= starCount) ? starCount : startCount);
		PlayerLevelData.Save(WordCuteData.nowLevel);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			ShowStart(startCount, delegate
			{
				Sequence sequence = DOTween.Sequence();
				float inervalTime = GetInervalTime(WordCuteData.nowLevel);
				int start = PlayerData.i.brilliance;
				int end = PlayerData.i.brilliance + WordCuteData.nowLevel;
				PlayerData.i.brilliance = end;
				PlayerData.Save();
				SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.wit_increase);
				SampleTweenVaule.Float0To1Tween(this, inervalTime, delegate(float n)
				{
					_brilliance.text = ((int)Mathf.Lerp(start, end, n)).ToString();
				});
				sequence.AppendInterval(inervalTime);
				sequence.Append(_nextButton.transform.DOScale(Vector3.one, 0.5f));
				sequence.Join(_nextButton.image.DOFade(1f, 0.5f));
				sequence.Join(_rePlayButton.image.DOFade(1f, 0.5f));
				sequence.Join(_rePlayButton.transform.DOScale(Vector3.one, 0.5f));
				sequence.Play();
			});
		}, 0.5f);
	}

	private float GetInervalTime(int level)
	{
		if (level < 5)
		{
			return 0.2f;
		}
		if (level < 10)
		{
			return 0.3f;
		}
		return 0.5f;
	}

	private void OnClickNextButton()
	{
		ShowAd();
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.GameFinishUI);
			OpenNextLevel();
		}, WordConfig.openUIDuration);
	}

	private void ShowStart(int startCount, TweenCallback callback)
	{
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < startCount; i++)
		{
			int temp = i;
			RectTransform hueStart = _hueStarts.GetChild(i) as RectTransform;
			RectTransform colorStart = hueStart.Find("Mark") as RectTransform;
			colorStart.localScale = Vector3.zero;
			sequence.AppendCallback(delegate
			{
				colorStart.gameObject.SetActive(true);
			});
			sequence.AppendCallback(delegate
			{
				SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.star + (temp + 1));
				hueStart.Find("Circle").GetComponent<ParticleSystem>().Play();
			});
			sequence.Append(colorStart.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
		}
		sequence.AppendInterval(0.3f);
		if (startCount == 3)
		{
			sequence.AppendCallback(delegate
			{
				_effect.Play();
				SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.firework);
			});
			sequence.AppendInterval(1f);
		}
		sequence.AppendCallback(callback);
		sequence.Play();
	}

	private void OpenNextLevel()
	{
		int nextLevel = WordCuteData.nowLevel + 1;
		if (nextLevel > WordConfig.gameMaxLevel)
		{
			GameMainUICtrl.instance.ToDating(delegate
			{
				SingleObject<UIManager>.instance.OpenUI(UITypeDefine.DatingUI, null, delegate(GameObject obj)
				{
					DatingUICtrl component = obj.GetComponent<DatingUICtrl>();
					component.ShowAnimation();
				});
			});
		}
		else
		{
			GameMainUICtrl.instance.CloseGameMain(delegate
			{
				WordCuteTools.OpenGameMainLevel(nextLevel);
			});
		}
	}

	private void OnClickReplay()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.GameFinishUI);
			GameMainUICtrl.instance.CloseGameMain(delegate
			{
				WordCuteTools.OpenGameMainLevel(WordCuteData.nowLevel);
			});
		}, WordConfig.openUIDuration);
	}

	private void UpdateOpenLevel()
	{
		int num = WordCuteData.nowLevel + 1;
		if (num > WordConfig.gameMaxLevel)
		{
			num = WordConfig.gameMaxLevel;
		}
		if (num > PlayerData.i.openLevel)
		{
			PlayerData.i.openLevel = num;
			PlayerData.Save();
		}
	}

	private void ShowAd()
	{
		WordCuteData.nowFinishTime++;
		bool flag = false;
		if ((WordCuteData.nowLevel >= 31) ? (WordCuteData.nowFinishTime % 3 == 0) : (WordCuteData.nowFinishTime % 5 == 0))
		{
			SingleObject<AdsSuperManager>.instance.ShowInterAd();
			SingleObject<AdsSuperManager>.instance.LoadInterAd();
		}
	}
}
