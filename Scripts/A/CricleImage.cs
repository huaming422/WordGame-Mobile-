using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class CricleImage : Image
{
	[Range(10f, 100f)]
	public int segements = 30;

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		toFill.Clear();
		Rect rect = base.rectTransform.rect;
		Vector4 vector = ((!(base.overrideSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(base.overrideSprite));
		float num = (vector.x + vector.z) * 0.5f;
		float num2 = (vector.y + vector.w) * 0.5f;
		float num3 = (vector.z - vector.x) / rect.width;
		float num4 = (vector.w - vector.y) / rect.height;
		float num5 = (float)Math.PI * 2f / (float)segements;
		UIVertex v = default(UIVertex);
		v.position = Vector3.zero;
		v.color = color;
		v.uv0 = new Vector2(0.5f, 0.5f);
		toFill.AddVert(v);
		List<UIVertex> list = new List<UIVertex>();
		for (int i = 0; i < segements; i++)
		{
			UIVertex v2 = default(UIVertex);
			float num6 = rect.width * Mathf.Cos(num5 * (float)i) * 0.5f;
			float num7 = rect.height * Mathf.Sin(num5 * (float)i) * 0.5f;
			v2.position = new Vector3(num6, num7);
			v2.color = color;
			v2.uv0 = new Vector2(num6 * num3 + num, num7 * num4 + num2);
			toFill.AddVert(v2);
			if (i == segements - 1)
			{
				toFill.AddTriangle(segements, 0, 1);
			}
			else
			{
				toFill.AddTriangle(i + 1, 0, i + 2);
			}
		}
	}
}
