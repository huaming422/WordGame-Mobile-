using UnityEngine;
using UnityEngine.UI;

public class RateGameUI2Ctrl : MonoBehaviour
{
	private Transform m_Content;

	private void Start()
	{
		m_Content = base.transform.Find("Content");
		m_Content.Find("No").GetComponent<Button>().onClick.AddListener(OnClikNo);
		m_Content.Find("Yes").GetComponent<Button>().onClick.AddListener(OnClikYes);
		m_Content.localScale = Vector3.zero;
		iTween.ScaleTo(m_Content.gameObject, Vector3.one, 0.5f);
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && PlayerData.i.rateGameSate == 1)
		{
			Vector3 targetPos = (Vector3)SingleObject<CacheDataManager>.instance.GetCacheObj("datakey_12001");
			RewardMessageBox.ShowAndSendReward(WordConfig.rateGameRewardCount, targetPos);
			CloseUI();
		}
	}

	private void OnClikNo()
	{
		PlayerData.i.rateGameSate = 2;
		CloseUI();
	}

	private void OnClikYes()
	{
		PlayerData.i.rateGameSate = 1;
		Application.OpenURL("market://details?id=com.san.womes&q=word%20games");
	}

	private void CloseUI()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		iTween.ScaleTo(m_Content.gameObject, Vector3.zero, 0.5f);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.GameRateUI);
		}, 0.5f);
	}
}
