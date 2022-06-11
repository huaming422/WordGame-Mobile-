using UnityEngine;

public class StartMain : MonoBehaviour
{
	public bool isLocal = true;

	public GameChannel channel = GameChannel.None;

	public bool isClearnData;

	public bool isStartGame = true;

	public bool isOpenLevel;

	public bool debugMode;

	public bool isCloseAds;

	public bool appinPurchDebugMode;

	public bool jumpGreenTourguide;

	public bool isOpenLog = true;

	public bool isBundleModel;

	public bool isUpdateModel;

	public bool openTotalLevel;

	private void Start()
	{
		AppConst.IsLocal = isLocal;
		if (isLocal)
		{
			AppConst.ServerIp = AppConst.localServerIp;
		}
		AppConst.Channel = channel;
		AppConst.IsOpenLevel = isOpenLevel;
		AppConst.DebugMode = debugMode;
		AppConst.IsCloseAds = isCloseAds;
		AppConst.IsIAPTestMode = appinPurchDebugMode;
		AppConst.IsOpenLog = isOpenLog;
		AppConst.BundleMode = isBundleModel;
		AppConst.UpdateMode = isUpdateModel;
		AppConst.openTotalLevel = openTotalLevel;
		if (isClearnData)
		{
			PlayerPrefs.DeleteAll();
			SingleObject<GameDataBaseManger>.instance.Clearn();
			AccountDataManager.Clearn();
		}
		if (Application.isMobilePlatform)
		{
			Application.targetFrameRate = AppConst.GameFrameRate;
			Screen.sleepTimeout = -1;
		}
		Application.runInBackground = true;
		LoadUICtrl.ShowLoading(5f, () => SingleObject<AdsSuperManager>.instance.InterAdIsLoad(), null, delegate
		{
			InitManager();
		});
	}

	private void InitManager()
	{
		TableDataMannager.instance.InitConfig();
		AccountDataManager.Init("guest");
		SingleObject<GameDataBaseManger>.instance.Init();
		SingleObject<SDKManager>.instance.Init();
		SingleObject<UnityInAppPurchManager>.instance.Init();
		SingleObject<AdsSuperManager>.instance.Init();
		SingleObject<FrameManager>.instance.Init();
		SingleObject<GamePlayPosManager>.instance.Init();
	}
}
