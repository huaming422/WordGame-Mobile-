using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EarnController : MonoBehaviour
{
	private Button _earnButton;

	private bool _lastAdLoadState;

	private bool pauseCheck;

	private Image mask_Image;

	private Tween watch_Tween;

	private Tween mask_Twen;

	private float timer;

	private void Awake()
	{
		_earnButton = base.transform.Find("Earn").GetComponent<Button>();
		mask_Image = base.transform.parent.Find("Mask Image").GetComponent<Image>();
		SetMaskStates(false, true);
		_earnButton.gameObject.SetActive(false);
		_earnButton.onClick.AddListener(OnClickEarnButton);
	}

	private void SetEarnStates(bool setGameObjStetes)
	{
		if (watch_Tween != null)
		{
			watch_Tween.Kill();
		}
		if (setGameObjStetes)
		{
			watch_Tween = _earnButton.transform.DOScale(Vector3.one, 0.5f);
			_earnButton.gameObject.SetActive(setGameObjStetes);
		}
		else
		{
			watch_Tween = _earnButton.transform.DOScale(Vector3.zero, 0.3f).OnComplete(delegate
			{
				_earnButton.gameObject.SetActive(setGameObjStetes);
			});
		}
	}

	private void SetMaskStates(bool states, bool justClose = false)
	{
		if (mask_Twen != null)
		{
			mask_Twen.Kill();
		}
		if (justClose)
		{
			mask_Image.gameObject.SetActive(false);
		}
	}

	private void OnClickEarnButton()
	{
		SetMaskStates(true);
		OnClikWatchVideo();
	}

	private void OnClikWatchVideo()
	{
		SetEarnStates(false);
		pauseCheck = true;
		_lastAdLoadState = false;
		_earnButton.interactable = false;
		SingleObject<AdsSuperManager>.instance.PlayVideoAD(PushReward);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<AdsSuperManager>.instance.LoadVideoAD();
			pauseCheck = false;
			_earnButton.interactable = true;
			SetMaskStates(false);
		}, 1.2f);
	}

	private void PushReward(bool result, string fromName)
	{
		Debug.Log("PushReward:  " + result + "  Name: " + fromName);
		if (result)
		{
			PlayerData.i.coinCount += WordConfig.watchVideoRewardCount;
			PlayerData.Save();
			Vector3 targetPos = (Vector3)SingleObject<CacheDataManager>.instance.GetCacheObj("datakey_12001");
			TipUIManagerCtrl.instance.ShowGetRewards(targetPos, WordConfig.watchVideoRewardCount, delegate
			{
				MessageManger.SendMessage(100);
			});
		}
	}

	private void Update()
	{
		if (_lastAdLoadState != SingleObject<AdsSuperManager>.instance.VideoAdIsLoadOk())
		{
			_lastAdLoadState = SingleObject<AdsSuperManager>.instance.VideoAdIsLoadOk();
			SetEarnStates(_lastAdLoadState);
		}
		timer += Time.deltaTime;
		if (timer > 5f)
		{
			if (!SingleObject<AdsSuperManager>.instance.VideoAdIsLoadOk())
			{
				SingleObject<AdsSuperManager>.instance.LoadVideoAD();
			}
			timer = 0f;
		}
	}
}
