using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipUIManagerCtrl : MonoBehaviour
{
	private static TipUIManagerCtrl _instance;

	private int waiteTipid;

	private GameObject _waitTip;

	private GameObject _getTools;

	private bool _errorTipIsShow;

	public static TipUIManagerCtrl instance
	{
		get
		{
			if (_instance == null)
			{
				CreateUI();
			}
			return _instance;
		}
	}

	private static void CreateUI()
	{
		GameObject gameObject = SingleObject<ResourceManager>.instance.LoadAsset<GameObject>(UITypeDefine.TipManagerUI);
		gameObject.transform.SetParent(UIManager.UIRoot, false);
		_instance = gameObject.GetComponent<TipUIManagerCtrl>();
		if (_instance == null)
		{
			_instance = gameObject.AddComponent<TipUIManagerCtrl>();
		}
	}

	private void Awake()
	{
		_waitTip = base.transform.Find("Wait").gameObject;
		_getTools = base.transform.Find("GetRewards").gameObject;
	}

	public void ShowWait(bool isShow, string tipText = "", UnityAction outTimeCallback = null, float witeTime = 15f)
	{
		_waitTip.SetActive(isShow);
		if (!isShow)
		{
			waiteTipid++;
			return;
		}
		int nowIndex = waiteTipid;
		_waitTip.transform.Find("Text").GetComponent<Text>().text = tipText;
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			if (nowIndex == waiteTipid)
			{
				_waitTip.SetActive(false);
				outTimeCallback.MyInvoke();
			}
		}, witeTime);
	}

	public void ShowGetRewards(Vector3 targetPos, int count, UnityAction callback = null)
	{
		Transform objTemp = Object.Instantiate(_getTools).transform;
		objTemp.SetParent(base.transform, false);
		objTemp.gameObject.SetActive(true);
		Transform coins = objTemp.Find("Coins");
		Transform addText = objTemp.Find("AddText");
		addText.position = targetPos;
		Text component = addText.Find("Text").GetComponent<Text>();
		component.text = ((count <= 0) ? count.ToString() : ("+" + count));
		Vector3 vector = new Vector3(0f, 50f, 0f);
		coins.localPosition += vector;
		for (int i = 0; i < coins.childCount; i++)
		{
			Transform item = coins.GetChild(i);
			Vector3 endValue = item.localPosition - vector;
			item.gameObject.SetActive(false);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval((float)i * 0.05f);
			sequence.AppendCallback(delegate
			{
				item.gameObject.SetActive(true);
				Image image = item.GetComponent<Image>();
				image.color = new Color(1f, 1f, 1f, 0f);
				SampleTweenVaule.FloatTween(this, 0f, 1f, 0.5f, delegate(float n)
				{
					image.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, n));
				});
			});
			sequence.Append(item.DOLocalMove(endValue, 0.5f));
			sequence.Play();
		}
		Sequence sequence2 = DOTween.Sequence();
		sequence2.AppendInterval(0.05f * (float)coins.childCount + 1f);
		sequence2.AppendCallback(delegate
		{
			for (int j = 0; j < coins.childCount; j++)
			{
				Transform item2 = coins.GetChild(coins.childCount - j - 1);
				Sequence sequence3 = DOTween.Sequence();
				sequence3.AppendInterval((float)j * 0.1f);
				sequence3.Append(item2.DOMove(targetPos, 1f));
				sequence3.AppendCallback(delegate
				{
					item2.gameObject.SetActive(false);
				});
				sequence3.Play();
			}
		});
		sequence2.AppendCallback(delegate
		{
			SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.coin_increse);
		});
		sequence2.AppendInterval(0.1f * (float)coins.childCount + 1f);
		sequence2.AppendCallback(delegate
		{
			coins.gameObject.SetActive(false);
			addText.gameObject.SetActive(true);
			addText.GetComponent<Animation>().OrderPlay(string.Empty);
			callback.MyInvoke();
		});
		sequence2.AppendInterval(1f);
		sequence2.AppendCallback(delegate
		{
			if (objTemp != null)
			{
				Object.Destroy(objTemp.gameObject);
			}
		});
		sequence2.Play();
	}
}
