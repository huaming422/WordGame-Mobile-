using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EffectObjCtrl : MonoBehaviour
{
	public static EffectObjCtrl instance;

	private Transform _stimulatePic;

	private int _nowUsePicIndex;

	private void Start()
	{
		instance = this;
		_stimulatePic = base.transform.Find("StimulatePic");
	}

	public void ShowStimulatePic()
	{
		int index = _nowUsePicIndex % 3;
		_nowUsePicIndex++;
		GameObject gameObject = _stimulatePic.GetChild(index).gameObject;
		Transform tipPic = Object.Instantiate(gameObject).transform;
		tipPic.gameObject.SetActive(true);
		tipPic.SetParent(FlyObjMananger.instance.transform, false);
		Vector3 endValue = gameObject.transform.localPosition + new Vector3(0f, 100f, 0f);
		Image component = tipPic.GetComponent<Image>();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(component.DOFade(1f, 0.3f));
		sequence.Join(tipPic.DOScale(1f, 0.3f));
		sequence.AppendInterval(0.5f);
		sequence.Append(tipPic.DOLocalMove(endValue, 0.5f));
		sequence.Join(component.DOFade(0f, 0.5f));
		sequence.AppendCallback(delegate
		{
			if (tipPic != null)
			{
				Object.Destroy(tipPic.gameObject);
			}
		});
		sequence.Play();
	}
}
