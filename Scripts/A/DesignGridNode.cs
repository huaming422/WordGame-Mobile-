using UnityEngine;
using UnityEngine.UI;

public class DesignGridNode : GridNode
{
	public string data = string.Empty;

	public Transform obj;

	public Vector3 targetLocalPos = Vector3.zero;

	public Text contentText;

	public Image backImage;

	public RectTransform _rectTran;

	public DesignGridNode()
	{
	}

	public DesignGridNode(Transform obj, string data)
	{
		this.obj = obj;
		this.data = data;
	}

	public void Init()
	{
		if (!(obj == null))
		{
			contentText = obj.Find("Text").GetComponent<Text>();
			_rectTran = obj as RectTransform;
			_rectTran.sizeDelta = new Vector2(width, height);
			backImage = obj.Find("Image").GetComponent<Image>();
		}
	}

	public void ClearnText()
	{
		contentText.text = string.Empty;
		data = string.Empty;
	}

	public void SetText(string text)
	{
		contentText.text = text;
		data = text;
	}

	public void SetImageColor(Color color)
	{
		backImage.color = color;
	}
}
