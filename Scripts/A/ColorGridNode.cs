using UnityEngine;
using UnityEngine.UI;

public class ColorGridNode : GridNode
{
	public Transform obj;

	public Text text;

	public Image image;

	protected Material _boxMaterial;

	protected int _textStartSize;

	public void Init(Transform touchObj)
	{
		obj = touchObj;
		obj.transform.localPosition = pos;
		text = obj.GetChild(0).GetComponent<Text>();
		text.text = dataIndex.ToString();
		image = obj.GetComponent<Image>();
		_textStartSize = text.fontSize;
		_boxMaterial = image.material;
	}

	public void UpdateTouchObjPos()
	{
		if (!(obj == null))
		{
			obj.transform.localPosition = pos;
		}
	}

	public void UpdateText()
	{
		if (!(text == null))
		{
			text.text = dataIndex.ToString();
		}
	}

	public void ScaleText(float value)
	{
		if (!(text == null))
		{
			text.fontSize = Mathf.RoundToInt(width * 0.5f);
		}
	}

	public void SetText(string value)
	{
		if (!(text == null))
		{
			text.text = value;
		}
	}

	public void SetColor(Color color)
	{
		if (!(image == null))
		{
			image.color = color;
		}
	}

	public void ShowHiddenText(bool isShow)
	{
		if (!(text == null))
		{
			text.gameObject.SetActive(isShow);
		}
	}

	public void ShowHiddenBox(bool isShow)
	{
		if (!(image == null))
		{
			if (isShow)
			{
				image.material = _boxMaterial;
			}
			else
			{
				image.material = null;
			}
		}
	}
}
