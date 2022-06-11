using UnityEngine;
using UnityEngine.UI;

public class DrawBoxLine : MonoBehaviour
{
	private Image image;

	public bool isDrawBox;

	private Material boxlineMatri;

	public void UpdateScaleRate(float rate)
	{
		if (image == null)
		{
			image = GetComponent<Image>();
		}
		if (!(image == null))
		{
			image.material.SetFloat("_ScaleRate", rate);
		}
	}

	public void ClearnBoxLine()
	{
		if (image == null)
		{
			image = GetComponent<Image>();
		}
		if (!(image == null))
		{
			image.material = null;
		}
	}

	public void SetBoxlineMatri(Material boxlineMatri, bool isShow = false)
	{
		this.boxlineMatri = boxlineMatri;
		if (isShow)
		{
			ShowBoxLine();
		}
	}

	public void ShowBoxLine()
	{
		if (image == null)
		{
			image = GetComponent<Image>();
		}
		if (!(image == null) && !(boxlineMatri == null))
		{
			image.material = boxlineMatri;
		}
	}
}
