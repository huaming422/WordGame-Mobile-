using System.Collections.Generic;
using ChartboostSDK;
using UnityEngine;

public class ChartboostExample : MonoBehaviour
{
	public Texture2D logo;

	public Vector2 scrollPosition = Vector2.zero;

	private List<string> delegateHistory;

	private bool hasInterstitial;

	private bool hasRewardedVideo;

	private int frameCount;

	private bool ageGate;

	private bool autocache = true;

	private bool activeAgeGate;

	private bool showInterstitial = true;

	private bool showRewardedVideo = true;

	private int BANNER_HEIGHT = 110;

	private int REQUIRED_HEIGHT = 650;

	private int ELEMENT_WIDTH = 190;

	private Rect scrollRect;

	private Rect scrollArea;

	private Vector3 guiScale;

	private float scale;

	private Vector2 beginFinger;

	private float deltaFingerY;

	private Vector2 beginPanel;

	private Vector2 latestPanel;

	private void OnEnable()
	{
		SetupDelegates();
	}

	private void Start()
	{
		delegateHistory = new List<string>();
		Chartboost.setAutoCacheAds(autocache);
		Chartboost.setPIDataUseConsent(CBPIDataUseConsent.YesBehavioral);
		AddLog("Is Initialized: " + Chartboost.isInitialized());
	}

	private void SetupDelegates()
	{
		Chartboost.didInitialize += didInitialize;
		Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial += didDismissInterstitial;
		Chartboost.didCloseInterstitial += didCloseInterstitial;
		Chartboost.didClickInterstitial += didClickInterstitial;
		Chartboost.didCacheInterstitial += didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial += didDisplayInterstitial;
		Chartboost.didFailToRecordClick += didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo += didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
		Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
		Chartboost.willDisplayVideo += willDisplayVideo;
	}

	private void Update()
	{
		UpdateScrolling();
		frameCount++;
		if (frameCount > 30)
		{
			hasInterstitial = Chartboost.hasInterstitial(CBLocation.Default);
			hasRewardedVideo = Chartboost.hasRewardedVideo(CBLocation.Default);
			frameCount = 0;
		}
	}

	private void UpdateScrolling()
	{
		if (Input.touchCount == 1)
		{
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Began)
			{
				beginFinger = touch.position;
				beginPanel = scrollPosition;
			}
			if (touch.phase == TouchPhase.Moved)
			{
				deltaFingerY = touch.position.y - beginFinger.y;
				float y = beginPanel.y + deltaFingerY / scale;
				latestPanel = beginPanel;
				latestPanel.y = y;
				scrollPosition = latestPanel;
			}
		}
	}

	private void AddLog(string text)
	{
		Debug.Log(text);
		delegateHistory.Insert(0, text + "\n");
		int count = delegateHistory.Count;
		if (count > 20)
		{
			delegateHistory.RemoveRange(20, count - 20);
		}
	}

	private void OnGUI()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		float a = num / 240f;
		float b = num2 / 210f;
		float num3 = Mathf.Min(6f, Mathf.Min(a, b));
		if (scale != num3)
		{
			scale = num3;
			guiScale = new Vector3(scale, scale, 1f);
		}
		GUI.matrix = Matrix4x4.Scale(guiScale);
		ELEMENT_WIDTH = (int)(num / scale) - 30;
		float height = REQUIRED_HEIGHT;
		scrollRect = new Rect(0f, BANNER_HEIGHT, ELEMENT_WIDTH + 30, num2 / scale - (float)BANNER_HEIGHT);
		scrollArea = new Rect(-10f, BANNER_HEIGHT, ELEMENT_WIDTH, height);
		LayoutHeader();
		if (activeAgeGate)
		{
			GUI.ModalWindow(1, new Rect(0f, 0f, Screen.width, Screen.height), LayoutAgeGate, "Age Gate");
			return;
		}
		scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, scrollArea);
		LayoutButtons();
		LayoutToggles();
		GUI.EndScrollView();
	}

	private void LayoutHeader()
	{
		GUILayout.Label(logo, GUILayout.Height(30f), GUILayout.Width(ELEMENT_WIDTH + 20));
		string text = string.Empty;
		foreach (string item in delegateHistory)
		{
			text += item;
		}
		GUILayout.TextArea(text, GUILayout.Height(70f), GUILayout.Width(ELEMENT_WIDTH + 20));
	}

	private void LayoutToggles()
	{
		GUILayout.Space(5f);
		GUILayout.Label("Options:");
		showInterstitial = GUILayout.Toggle(showInterstitial, "Should Display Interstitial");
		showRewardedVideo = GUILayout.Toggle(showRewardedVideo, "Should Display Rewarded Video");
		if (GUILayout.Toggle(ageGate, "Should Pause for AgeGate") != ageGate)
		{
			ageGate = !ageGate;
			Chartboost.setShouldPauseClickForConfirmation(ageGate);
		}
		if (GUILayout.Toggle(autocache, "Auto cache ads") != autocache)
		{
			autocache = !autocache;
			Chartboost.setAutoCacheAds(autocache);
		}
	}

	private void LayoutButtons()
	{
		GUILayout.Space(5f);
		GUILayout.Label("Has Interstitial: " + hasInterstitial);
		if (GUILayout.Button("Cache Interstitial", GUILayout.Width(ELEMENT_WIDTH)))
		{
			Chartboost.cacheInterstitial(CBLocation.Default);
		}
		if (GUILayout.Button("Show Interstitial", GUILayout.Width(ELEMENT_WIDTH)))
		{
			Chartboost.showInterstitial(CBLocation.Default);
		}
		GUILayout.Space(5f);
		GUILayout.Label("Has Rewarded Video: " + hasRewardedVideo);
		if (GUILayout.Button("Cache Rewarded Video", GUILayout.Width(ELEMENT_WIDTH)))
		{
			Chartboost.cacheRewardedVideo(CBLocation.Default);
		}
		if (GUILayout.Button("Show Rewarded Video", GUILayout.Width(ELEMENT_WIDTH)))
		{
			Chartboost.showRewardedVideo(CBLocation.Default);
		}
		GUILayout.Space(5f);
		GUILayout.Label("Post install events:");
		if (GUILayout.Button("Send PIA Main Level Event", GUILayout.Width(ELEMENT_WIDTH)))
		{
			Chartboost.trackLevelInfo("Test Data", CBLevelType.HIGHEST_LEVEL_REACHED, 1, "Test Send mail level Information");
		}
		if (GUILayout.Button("Send PIA Sub Level Event", GUILayout.Width(ELEMENT_WIDTH)))
		{
			Chartboost.trackLevelInfo("Test Data", CBLevelType.HIGHEST_LEVEL_REACHED, 1, 2, "Test Send sub level Information");
		}
		if (GUILayout.Button("Track IAP", GUILayout.Width(ELEMENT_WIDTH)))
		{
			TrackIAP();
		}
	}

	private void LayoutAgeGate(int windowID)
	{
		GUILayout.Space(BANNER_HEIGHT);
		GUILayout.Label("Want to pass the age gate?");
		GUILayout.BeginHorizontal(GUILayout.Width(ELEMENT_WIDTH));
		if (GUILayout.Button("YES"))
		{
			Chartboost.didPassAgeGate(true);
			activeAgeGate = false;
		}
		if (GUILayout.Button("NO"))
		{
			Chartboost.didPassAgeGate(false);
			activeAgeGate = false;
		}
		GUILayout.EndHorizontal();
	}

	private void OnDisable()
	{
		Chartboost.didInitialize -= didInitialize;
		Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial -= didDismissInterstitial;
		Chartboost.didCloseInterstitial -= didCloseInterstitial;
		Chartboost.didClickInterstitial -= didClickInterstitial;
		Chartboost.didCacheInterstitial -= didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
		Chartboost.didFailToRecordClick -= didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
		Chartboost.didPauseClickForConfirmation -= didPauseClickForConfirmation;
		Chartboost.willDisplayVideo -= willDisplayVideo;
	}

	private void didInitialize(bool status)
	{
		AddLog(string.Format("didInitialize: {0}", status));
	}

	private void didFailToLoadInterstitial(CBLocation location, CBImpressionError error)
	{
		AddLog(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
	}

	private void didDismissInterstitial(CBLocation location)
	{
		AddLog("didDismissInterstitial: " + location);
	}

	private void didCloseInterstitial(CBLocation location)
	{
		AddLog("didCloseInterstitial: " + location);
	}

	private void didClickInterstitial(CBLocation location)
	{
		AddLog("didClickInterstitial: " + location);
	}

	private void didCacheInterstitial(CBLocation location)
	{
		AddLog("didCacheInterstitial: " + location);
	}

	private bool shouldDisplayInterstitial(CBLocation location)
	{
		AddLog(string.Concat("shouldDisplayInterstitial @", location, " : ", showInterstitial));
		return showInterstitial;
	}

	private void didDisplayInterstitial(CBLocation location)
	{
		AddLog("didDisplayInterstitial: " + location);
	}

	private void didFailToRecordClick(CBLocation location, CBClickError error)
	{
		AddLog(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
	}

	private void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
	{
		AddLog(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
	}

	private void didDismissRewardedVideo(CBLocation location)
	{
		AddLog("didDismissRewardedVideo: " + location);
	}

	private void didCloseRewardedVideo(CBLocation location)
	{
		AddLog("didCloseRewardedVideo: " + location);
	}

	private void didClickRewardedVideo(CBLocation location)
	{
		AddLog("didClickRewardedVideo: " + location);
	}

	private void didCacheRewardedVideo(CBLocation location)
	{
		AddLog("didCacheRewardedVideo: " + location);
	}

	private bool shouldDisplayRewardedVideo(CBLocation location)
	{
		AddLog(string.Concat("shouldDisplayRewardedVideo @", location, " : ", showRewardedVideo));
		return showRewardedVideo;
	}

	private void didCompleteRewardedVideo(CBLocation location, int reward)
	{
		AddLog(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
	}

	private void didDisplayRewardedVideo(CBLocation location)
	{
		AddLog("didDisplayRewardedVideo: " + location);
	}

	private void didPauseClickForConfirmation()
	{
	}

	private void willDisplayVideo(CBLocation location)
	{
		AddLog("willDisplayVideo: " + location);
	}

	private void TrackIAP()
	{
		Debug.Log("TrackIAP");
		Chartboost.trackInAppGooglePlayPurchaseEvent("SampleItem", "TestPurchase", "0.99", "USD", "ProductID", "PurchaseData", "PurchaseSignature");
	}
}
