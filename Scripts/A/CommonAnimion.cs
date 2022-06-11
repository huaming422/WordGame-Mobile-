using System;
using UnityEngine;
using UnityEngine.UI;

public static class CommonAnimion
{
	public static void UpdateIntText(Text text, int value, bool isAnimation = false, float time = 0.5f, Func<int, string> format = null, int startValue = -1)
	{
		if (text == null)
		{
			return;
		}
		if (!isAnimation)
		{
			if (format != null)
			{
				text.text = format(value);
			}
			else
			{
				text.text = value.ToString();
			}
			return;
		}
		if (startValue == -1)
		{
			startValue = int.Parse(text.text);
		}
		SampleTweenVaule.IntTween(text, startValue, value, time, delegate(int n)
		{
			if (format != null)
			{
				text.text = format(n);
			}
			else
			{
				text.text = n.ToString();
			}
		});
		text.GetComponent<Animation>().OrderPlay(string.Empty);
	}

	public static void UpdateFloatText(Text text, float value, bool isAnimation = false, float time = 0.5f, Func<float, string> format = null)
	{
		if (text == null)
		{
			return;
		}
		if (!isAnimation)
		{
			if (format != null)
			{
				text.text = format(value);
			}
			else
			{
				text.text = value.ToString();
			}
			return;
		}
		float start = float.Parse(text.text);
		SampleTweenVaule.FloatTween(text, start, value, time, delegate(float n)
		{
			if (format != null)
			{
				text.text = format(n);
			}
			else
			{
				text.text = n.ToString();
			}
		});
		text.GetComponent<Animation>().OrderPlay(string.Empty);
	}
}
