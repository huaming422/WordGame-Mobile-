using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AlphabetTableItem : GridNode
{
	public string data = string.Empty;

	public Transform obj;

	public Vector3 targetLocalPos = Vector3.zero;

	public Text text;

	public Transform back;

	private Button button;

	private int _state = 1;

	public int state
	{
		get
		{
			return _state;
		}
	}

	public AlphabetTableItem()
	{
	}

	public AlphabetTableItem(Transform obj, string data)
	{
		this.obj = obj;
		this.data = data;
	}

	public void Init()
	{
		if (!(obj == null))
		{
			text = obj.Find("Alphabet").GetComponent<Text>();
			back = obj.Find("Background");
			text.text = data.ToUpper();
			button = obj.GetComponent<Button>();
			RectTransform rectTransform = obj as RectTransform;
			rectTransform.sizeDelta = new Vector2(width, height);
			obj.localPosition = pos;
			RectTransform rectTransform2 = back as RectTransform;
			rectTransform2.sizeDelta = rectTransform.sizeDelta * 0.9f;
			text.fontSize = (int)(width * 0.8f);
		}
	}

	public void Hidden()
	{
		obj.localScale = new Vector3(0f, 0f, 1f);
		CanvasGroup component = obj.GetComponent<CanvasGroup>();
		component.alpha = 0f;
	}

	public void DoSelect(bool isSelect)
	{
		ProjectCommonFunction.UpdateButton(back, isSelect);
		text.gameObject.SetActive(true);
		text.color = Color.white;
	}

	public void DoTip()
	{
		ProjectCommonFunction.UpdateButton(back, true);
		text.gameObject.SetActive(true);
		text.color = Color.white;
	}

	public void DoTipChangeBack(bool isSelect)
	{
		ProjectCommonFunction.UpdateButton(back, isSelect);
	}

	public void ChangeSate(int state)
	{
		_state = state;
	}

	public void AddClickEvent(UnityAction<AlphabetTableItem> callback)
	{
		if (!(button == null))
		{
			button.onClick.AddListener(delegate
			{
				callback.MyInvoke(this);
			});
		}
	}
}
