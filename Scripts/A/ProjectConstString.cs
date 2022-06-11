public class ProjectConstString
{
	public const string GameTempTextureName = "temp.jpg";

	public const string GameSpritePath = "Sprite/PixTextrue/";

	public const string appMarkHttpUrl = "https://www.amazon.com/gp/mas/dl/";

	public const string appMarkAppUrl = "market://details?id=com.s.mes&q=word%20games";

	public const string recommonAppMarkUrl = "amzn://apps/android?p=com.s.e&amp;s=bingo%20games";

	public const string termHttpUrl = "http://www./terms.html";

	public const string policyHttpUrl = "http://www/policy.html";

	public const string PlayerDIYLevelType = "PlayerDIY";

	public static string GameTempTexturePath
	{
		get
		{
			return AppConst.TempDir + "GameTempTexturePath/";
		}
	}

	public static string PlayerDIYPicPath
	{
		get
		{
			return AppConst.DataDir + "PlayerDIYPic/";
		}
	}
}
