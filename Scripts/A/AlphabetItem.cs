using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AlphabetItem
{
	public string data = string.Empty;

	public Transform obj;

	public Vector3 targetLocalPos = Vector3.zero;

	public Text text;

	public GameObject back;

	public AlphabetItem(Transform obj, string data)
	{
		this.obj = obj;
		this.data = data;
	}

	public void Init()
	{
		if (!(obj == null))
		{
			text = obj.Find("Alphabet").GetComponent<Text>();
			back = obj.Find("Background").gameObject;
			text.text = data.ToUpper();
		}
	}

	public void ToTargetPos(bool isAniamtion = false)
	{
		if (obj == null)
		{
			return;
		}
		if (!isAniamtion)
		{
			obj.localPosition = targetLocalPos;
			return;
		}
		Vector3 localPosition = obj.localPosition;
		if (!((targetLocalPos - localPosition).sqrMagnitude < 1f))
		{
			obj.DOLocalMove(targetLocalPos, 0.5f);
		}
	}

	public void DoSelect(bool isSelect)
	{
		back.gameObject.SetActive(isSelect);
		text.color = ((!isSelect) ? Color.black : Color.white);
	}
}
