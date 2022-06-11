using UnityEngine;

public class WordConfig
{
	public static int showGameMaxLevel = 611;

	public static int tipCoinCount = 30;

	public static int diyTipCoinCount = 60;

	public static int bounsWordsCount = 15;

	public static int bounsRewardCount = 40;

	public static int watchVideoRewardCount = 20;

	public static int fillSurveyWallRewardCount = 100;

	public static int rateGameRewardCount = 100;

	public static float openUIDuration = 0.5f;

	public static float duration = 0.5f;

	public static Vector3 shakeRotaion = new Vector3(0f, 0f, 20f);

	public static int gameMaxLevel
	{
		get
		{
			if (AppConst.openTotalLevel)
			{
				return 610;
			}
			return 610;
		}
	}
}
