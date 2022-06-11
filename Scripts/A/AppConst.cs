using System;
using UnityEngine;

public class AppConst
{
	public static string AppVersion = "1.0.1";

	public static DateTime BuildTime = new DateTime(2018, 9, 27);

	public static GameChannel Channel = GameChannel.None;

	public static bool IsLocal = false;

	public static bool DebugMode = false;

	public static bool IsCloseAds = false;

	public static bool IsIAPTestMode = false;

	public static bool IsOpenLevel = false;

	public static bool IsOpenLog = true;

	public static bool openTotalLevel = false;

	public static string localServerIp = "192.168.1.223";

	public static string ServerIp = "66.228.50.210";

	public static int ServerPort = 9968;

	public static int LocalUdpPort = 1313;

	public static int WebrServerPort = 9989;

	public static string WebHost = "http://www.bigwinlab.com";

	public static bool UpdateMode = true;

	public static bool BundleMode = false;

	public static int TimerInterval = 1;

	public static int GameFrameRate = 30;

	public static string AppName = Application.productName;

	public static string ExtName = ".unity3d";

	public static string AssetDir = "StreamingAssets";

	public static string ResWebPath = "BingoLove/GameRes/";

	public static string ResUpdateUrl
	{
		get
		{
			return string.Format("http://{0}/{1}", ServerIp, ResWebPath);
		}
	}

	public static string ResDir
	{
		get
		{
			return Util.persistentDataPath + "GameRes/";
		}
	}

	public static string DataDir
	{
		get
		{
			return Util.persistentDataPath + "GameData/";
		}
	}

	public static string DownloadDir
	{
		get
		{
			return Util.persistentDataPath + "GameDownload/";
		}
	}

	public static string CacheDir
	{
		get
		{
			return Util.persistentDataPath + "GameCache/";
		}
	}

	public static string TempDir
	{
		get
		{
			return Util.persistentDataPath + "GameTemp/";
		}
	}
}
