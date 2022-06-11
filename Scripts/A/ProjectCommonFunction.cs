using System;
using UnityEngine;

public class ProjectCommonFunction
{
	public static bool IsLaterBuildTime(int day)
	{
		return (DateTime.Now - AppConst.BuildTime).TotalDays > (double)day;
	}

	public static void UpdateButton(Transform button, bool isOn)
	{
		button.Find("On").gameObject.SetActive(isOn);
		button.Find("Off").gameObject.SetActive(!isOn);
	}

	public static string GetMonoyString(long count, char splitChar = ',', int splitCharCount = 3)
	{
		if (splitCharCount == 0)
		{
			return count.ToString();
		}
		if ((float)count < Mathf.Pow(10f, splitCharCount))
		{
			return count.ToString();
		}
		string text = count.ToString();
		int num = text.Length % splitCharCount;
		string text2 = ((num != 0) ? (text.Substring(0, num) + splitChar) : string.Empty);
		text = text.Substring(num);
		for (int i = 0; i < text.Length; i++)
		{
			if (i % splitCharCount == 0 && i != 0)
			{
				text2 += splitChar;
			}
			text2 += text[i];
		}
		return text2;
	}

	public static string GetMoneyFloatUnit(float data, int count = 1)
	{
		if ((double)(data - (float)Mathf.FloorToInt(data)) < Math.Pow(0.10000000149011612, count))
		{
			return string.Empty;
		}
		string text = data.ToString("f" + count);
		return text.Substring(text.Length - (count + 1));
	}

	public static string GetMonoyIntString(int data, int rate = 100)
	{
		int num = data % rate;
		string text = ((num >= 10) ? num.ToString() : ("0" + num));
		return GetMonoyString(data / rate) + "." + text;
	}

	public static string GetBigNumberString(int number, string format = "f2")
	{
		string result = number.ToString();
		if (number > 1000)
		{
			result = ((float)number * 1f / 1000f).ToString(format) + "k";
		}
		if (number > 1000000)
		{
			result = ((float)number * 1f / 1000000f).ToString(format) + "M";
		}
		return result;
	}

	public static string GetShengYuTime(TimeSpan total, DateTime start)
	{
		TimeSpan timeSpan = total - (DateTime.Now - start);
		return ((!(timeSpan.TotalMinutes > 9.0)) ? ("0" + (int)timeSpan.TotalMinutes) : ((int)timeSpan.TotalMinutes).ToString()) + ":" + ((timeSpan.Seconds <= 9) ? ("0" + timeSpan.Seconds) : timeSpan.Seconds.ToString());
	}

	public static void SendFeedback()
	{
		string title = "Feedback From " + Application.productName + " [" + SystemInfo.deviceModel + "]  " + Application.version;
		SingleObject<SDKManager>.instance.SendEmail("support@sample.com", title, string.Empty);
	}
}
