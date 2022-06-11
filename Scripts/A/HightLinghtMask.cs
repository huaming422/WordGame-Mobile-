using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class HightLinghtMask : Image
{
	public RectTransform mask;

	private Vector3 center = Vector3.zero;

	private Vector2 size = Vector2.zero;

	private bool maskIsAcitive = true;

	private Vector3 thisCenter = Vector3.zero;

	private Vector2 thisSize = Vector2.zero;

	private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

	public void DoUpdate()
	{
		if (thisCenter != base.rectTransform.localPosition || thisSize != base.rectTransform.rect.size)
		{
			thisCenter = base.rectTransform.localPosition;
			thisSize = base.rectTransform.rect.size;
			SetAllDirty();
		}
		if (!(mask == null) && (center != mask.localPosition || size != mask.rect.size || mask.gameObject.activeSelf != maskIsAcitive))
		{
			maskIsAcitive = mask.gameObject.activeSelf;
			center = mask.localPosition;
			size = mask.rect.size;
			SetAllDirty();
		}
	}

	private void Update()
	{
		DoUpdate();
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		if (mask == null || !mask.gameObject.activeSelf)
		{
			base.OnPopulateMesh(vh);
			return;
		}
		vh.Clear();
		Vector4 vector = new Vector4(thisCenter.x - thisSize.x / 2f, thisCenter.y - thisSize.y / 2f, thisCenter.x + thisSize.x / 2f, thisCenter.y + thisSize.y / 2f);
		Vector4 vector2 = new Vector4(center.x - size.x / 2f, center.y - size.y / 2f, center.x + size.x / 2f, center.y + size.y / 2f);
		vh.AddVert(new Vector3(vector.x, vector.y), color, uvCenter);
		vh.AddVert(new Vector3(vector.x, vector.w), color, uvCenter);
		vh.AddVert(new Vector3(vector.z, vector.w), color, uvCenter);
		vh.AddVert(new Vector3(vector.z, vector.y), color, uvCenter);
		vh.AddVert(new Vector3(vector2.x, vector2.y), color, uvCenter);
		vh.AddVert(new Vector3(vector2.x, vector2.w), color, uvCenter);
		vh.AddVert(new Vector3(vector2.z, vector2.w), color, uvCenter);
		vh.AddVert(new Vector3(vector2.z, vector2.y), color, uvCenter);
		for (int i = 0; i < 4; i++)
		{
			if (i < 3)
			{
				vh.AddTriangle(i, i + 1, i + 4);
				vh.AddTriangle(i + 4, i + 1, i + 4 + 1);
			}
			else
			{
				vh.AddTriangle(i, 0, i + 4);
				vh.AddTriangle(i + 4, 0, 4);
			}
		}
	}

	private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
	{
		Vector4 vector = ((!(base.overrideSprite == null)) ? DataUtility.GetPadding(base.overrideSprite) : Vector4.zero);
		Vector2 vector2 = ((!(base.overrideSprite == null)) ? new Vector2(base.overrideSprite.rect.width, base.overrideSprite.rect.height) : Vector2.zero);
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		int num = Mathf.RoundToInt(vector2.x);
		int num2 = Mathf.RoundToInt(vector2.y);
		Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
		if (shouldPreserveAspect && vector2.sqrMagnitude > 0f)
		{
			float num3 = vector2.x / vector2.y;
			float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
			if (num3 > num4)
			{
				float height = pixelAdjustedRect.height;
				pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
				pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
			}
			else
			{
				float width = pixelAdjustedRect.width;
				pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
				pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
			}
		}
		return new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
	}
}
