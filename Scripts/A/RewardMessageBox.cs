using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardMessageBox : MonoBehaviour
{
	private Transform _content;

	private Text _rewardCount;

	private Image _rewardIcon;

	private Button _sureButton;

	private Text _tipText;

	private UnityAction callback;

	private int count;

	private string tipString = string.Empty;

	private bool isGetReward = true;

	private Vector3 _targetPos = Vector3.zero;

	private void Awake()
	{
		_content = base.transform.Find("Content");
		_rewardCount = _content.Find("RewardItem/count").GetComponent<Text>();
		_rewardIcon = _content.Find("RewardItem/icon").GetComponent<Image>();
		_tipText = _content.Find("TipText").GetComponent<Text>();
		_sureButton = _content.Find("Sure").GetComponent<Button>();
		_sureButton.onClick.AddListener(CloseUI);
		_sureButton.gameObject.SetActive(false);
	}

	private void Start()
	{
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
		IntiUI();
	}

	private void IntiUI()
	{
		_rewardCount.text = count.ToString();
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			PlayerData.i.coinCount += count;
			PlayerData.Save();
			TipUIManagerCtrl.instance.ShowGetRewards(_targetPos, count, delegate
			{
				MessageManger.SendMessage(100);
				ShowSureButton();
			});
		}, 0.5f);
	}

	private void ShowSureButton()
	{
		_sureButton.gameObject.SetActive(true);
		_sureButton.GetComponent<Animation>().OrderPlay(string.Empty);
	}

	private void CloseUI()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			callback.MyInvoke();
			SingleObject<UIManager>.instance.Close(UITypeDefine.RewardMessageBox);
		}, 0.5f);
	}

	public static void ShowAndSendReward(int count, Vector3 targetPos, UnityAction callbck = null, string tipString = "Thank you very much!", bool isGetReward = true)
	{
		if (count > 0)
		{
			SingleObject<UIManager>.instance.OpenUI(UITypeDefine.RewardMessageBox, null, delegate(GameObject obj)
			{
				RewardMessageBox component = obj.GetComponent<RewardMessageBox>();
				component.count = count;
				component.callback = callbck;
				component.tipString = tipString;
				component.isGetReward = isGetReward;
				component._targetPos = targetPos;
			});
		}
	}
}
