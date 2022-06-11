using TapjoyUnity;
using UnityEngine;

public class PlacementExample : MonoBehaviour
{
	public TJPlacement directPlayPlacement;

	public TJPlacement offerwallPlacement;

	public TJPlacement samplePlacement;

	public string samplePlacementName = "video_unit";

	public string output = string.Empty;

	public bool shouldPreload;

	public bool contentIsReadyForPlacement;

	private GUIStyle inputStyle;

	private GUIStyle headerStyle;

	private GUIStyle outputStyle;

	private int fontSize = 20;

	private float startY;

	private float centerX;

	private float buttonWidth;

	private float buttonHeight;

	private float headerHeight;

	private float halfButtonWidth;

	private float thirdButtonWidth;

	private float yPadding = 50f;

	private void OnEnable()
	{
		Debug.Log("C# PlacementExample Enable -- Adding Tapjoy Placement delegates");
		TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
		TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
		TJPlacement.OnContentReady += HandlePlacementContentReady;
		TJPlacement.OnContentShow += HandlePlacementContentShow;
		TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
		TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
		TJPlacement.OnRewardRequest += HandleOnRewardRequest;
		TJPlacement.OnVideoStart += HandleVideoStart;
		TJPlacement.OnVideoError += HandleVideoError;
		TJPlacement.OnVideoComplete += HandleVideoComplete;
		Tapjoy.OnAwardCurrencyResponse += HandleAwardCurrencyResponse;
		Tapjoy.OnAwardCurrencyResponseFailure += HandleAwardCurrencyResponseFailure;
		Tapjoy.OnSpendCurrencyResponse += HandleSpendCurrencyResponse;
		Tapjoy.OnSpendCurrencyResponseFailure += HandleSpendCurrencyResponseFailure;
		Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
		Tapjoy.OnGetCurrencyBalanceResponseFailure += HandleGetCurrencyBalanceResponseFailure;
		Tapjoy.OnEarnedCurrency += HandleEarnedCurrency;
		if (directPlayPlacement == null)
		{
			directPlayPlacement = TJPlacement.CreatePlacement("video_unit");
			if (directPlayPlacement != null)
			{
				directPlayPlacement.RequestContent();
			}
		}
		if (offerwallPlacement == null)
		{
			offerwallPlacement = TJPlacement.CreatePlacement("offerwall_unit");
		}
		InitUI();
	}

	private void OnDisable()
	{
		Debug.Log("C#: Disabling and removing Tapjoy Delegates");
		TJPlacement.OnRequestSuccess -= HandlePlacementRequestSuccess;
		TJPlacement.OnRequestFailure -= HandlePlacementRequestFailure;
		TJPlacement.OnContentReady -= HandlePlacementContentReady;
		TJPlacement.OnContentShow -= HandlePlacementContentShow;
		TJPlacement.OnContentDismiss -= HandlePlacementContentDismiss;
		TJPlacement.OnPurchaseRequest -= HandleOnPurchaseRequest;
		TJPlacement.OnRewardRequest -= HandleOnRewardRequest;
		TJPlacement.OnVideoStart -= HandleVideoStart;
		TJPlacement.OnVideoError -= HandleVideoError;
		TJPlacement.OnVideoComplete -= HandleVideoComplete;
		Tapjoy.OnAwardCurrencyResponse -= HandleAwardCurrencyResponse;
		Tapjoy.OnAwardCurrencyResponseFailure -= HandleAwardCurrencyResponseFailure;
		Tapjoy.OnSpendCurrencyResponse -= HandleSpendCurrencyResponse;
		Tapjoy.OnSpendCurrencyResponseFailure -= HandleSpendCurrencyResponseFailure;
		Tapjoy.OnGetCurrencyBalanceResponse -= HandleGetCurrencyBalanceResponse;
		Tapjoy.OnGetCurrencyBalanceResponseFailure -= HandleGetCurrencyBalanceResponseFailure;
		Tapjoy.OnEarnedCurrency -= HandleEarnedCurrency;
	}

	public void HandlePlacementRequestSuccess(TJPlacement placement)
	{
		if (placement.IsContentAvailable())
		{
			Debug.Log("C#: Content available for " + placement.GetName());
			output = "Content available for " + placement.GetName();
			if (placement.GetName() == samplePlacementName && samplePlacement != null)
			{
				contentIsReadyForPlacement = true;
			}
			else if (placement.GetName() == "offerwall_unit")
			{
				placement.ShowContent();
			}
		}
		else
		{
			output = "No content available for " + placement.GetName();
			Debug.Log("C#: No content available for " + placement.GetName());
		}
	}

	public void HandlePlacementRequestFailure(TJPlacement placement, string error)
	{
		Debug.Log("C#: HandlePlacementRequestFailure");
		Debug.Log("C#: Request for " + placement.GetName() + " has failed because: " + error);
		output = "Request for " + placement.GetName() + " has failed because: " + error;
	}

	public void HandlePlacementContentReady(TJPlacement placement)
	{
		Debug.Log("C#: HandlePlacementContentReady");
		output = "HandlePlacementContentReady";
		if (!placement.IsContentAvailable())
		{
			Debug.Log("C#: no content");
		}
	}

	public void HandlePlacementContentShow(TJPlacement placement)
	{
		Debug.Log("C#: HandlePlacementContentShow");
	}

	public void HandlePlacementContentDismiss(TJPlacement placement)
	{
		Debug.Log("C#: HandlePlacementContentDismiss");
		contentIsReadyForPlacement = false;
		output = "TJPlacement " + placement.GetName() + " has been dismissed";
	}

	private void HandleOnPurchaseRequest(TJPlacement placement, TJActionRequest request, string productId)
	{
		Debug.Log("C#: HandleOnPurchaseRequest");
		request.Completed();
	}

	private void HandleOnRewardRequest(TJPlacement placement, TJActionRequest request, string itemId, int quantity)
	{
		Debug.Log("C#: HandleOnRewardRequest");
		request.Completed();
	}

	public void HandleAwardCurrencyResponse(string currencyName, int balance)
	{
		Debug.Log("C#: HandleAwardCurrencySucceeded: currencyName: " + currencyName + ", balance: " + balance);
		output = "Awarded Currency -- " + currencyName + " Balance: " + balance;
	}

	public void HandleAwardCurrencyResponseFailure(string error)
	{
		Debug.Log("C#: HandleAwardCurrencyResponseFailure: " + error);
	}

	public void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
	{
		Debug.Log("C#: HandleGetCurrencyBalanceResponse: currencyName: " + currencyName + ", balance: " + balance);
		output = currencyName + " Balance: " + balance;
	}

	public void HandleGetCurrencyBalanceResponseFailure(string error)
	{
		Debug.Log("C#: HandleGetCurrencyBalanceResponseFailure: " + error);
	}

	public void HandleSpendCurrencyResponse(string currencyName, int balance)
	{
		Debug.Log("C#: HandleSpendCurrencyResponse: currencyName: " + currencyName + ", balance: " + balance);
		output = currencyName + " Balance: " + balance;
	}

	public void HandleSpendCurrencyResponseFailure(string error)
	{
		Debug.Log("C#: HandleSpendCurrencyResponseFailure: " + error);
	}

	public void HandleEarnedCurrency(string currencyName, int amount)
	{
		Debug.Log("C#: HandleEarnedCurrency: currencyName: " + currencyName + ", amount: " + amount);
		output = currencyName + " Earned: " + amount;
		Tapjoy.ShowDefaultEarnedCurrencyAlert();
	}

	public void HandleVideoStart(TJPlacement placement)
	{
		Debug.Log("C#: HandleVideoStarted for placement " + placement.GetName());
	}

	public void HandleVideoError(TJPlacement placement, string message)
	{
		Debug.Log("C#: HandleVideoError for placement " + placement.GetName() + "with message: " + message);
	}

	public void HandleVideoComplete(TJPlacement placement)
	{
		Debug.Log("C#: HandleVideoComplete for placement " + placement.GetName());
	}

	private void InitUI()
	{
		headerStyle = new GUIStyle();
		headerStyle.alignment = TextAnchor.MiddleLeft;
		headerStyle.normal.textColor = Color.white;
		headerStyle.wordWrap = true;
		headerStyle.fontSize = fontSize;
		outputStyle = new GUIStyle();
		outputStyle.alignment = TextAnchor.MiddleCenter;
		outputStyle.normal.textColor = Color.white;
		outputStyle.wordWrap = true;
		outputStyle.fontSize = fontSize;
		startY = Screen.height / 10;
		centerX = Screen.width / 2;
		buttonWidth = Screen.width - Screen.width / 6;
		buttonHeight = Screen.height / 15;
		halfButtonWidth = buttonWidth / 2f;
		thirdButtonWidth = buttonWidth / 3f;
		headerHeight = Screen.height / 20;
		yPadding = buttonHeight + 10f;
	}

	private void OnGUI()
	{
		float num = startY;
		if (inputStyle == null)
		{
			inputStyle = GUI.skin.textField;
			inputStyle.fontSize = fontSize;
		}
		Rect position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Show Offerwall") && offerwallPlacement != null)
		{
			offerwallPlacement.RequestContent();
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Show Direct Play Video Ad"))
		{
			if (directPlayPlacement.IsContentAvailable())
			{
				if (directPlayPlacement.IsContentReady())
				{
					directPlayPlacement.ShowContent();
				}
				else
				{
					output = "Direct play video not ready to show.";
				}
			}
			else
			{
				output = "No direct play video to show.";
			}
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, headerHeight);
		GUI.Label(position, "Managed Currency:", headerStyle);
		num += yPadding - (yPadding - headerHeight);
		position = new Rect(centerX - (thirdButtonWidth + thirdButtonWidth / 2f), num, thirdButtonWidth, buttonHeight);
		if (GUI.Button(position, "Get"))
		{
			ResetCurrencyLabel();
			Tapjoy.GetCurrencyBalance();
		}
		position = new Rect(centerX - thirdButtonWidth / 2f, num, thirdButtonWidth, buttonHeight);
		if (GUI.Button(position, "Spend"))
		{
			ResetCurrencyLabel();
			Tapjoy.SpendCurrency(10);
		}
		position = new Rect(centerX + thirdButtonWidth / 2f, num, thirdButtonWidth, buttonHeight);
		if (GUI.Button(position, "Award"))
		{
			ResetCurrencyLabel();
			Tapjoy.AwardCurrency(10);
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, headerHeight);
		GUI.Label(position, "Content Placement:", headerStyle);
		num += yPadding - (yPadding - headerHeight);
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, headerHeight);
		samplePlacementName = GUI.TextField(position, samplePlacementName, 30, inputStyle);
		num += headerHeight + 10f;
		position = new Rect(centerX - halfButtonWidth, num, halfButtonWidth, buttonHeight);
		if (GUI.Button(position, "Request"))
		{
			samplePlacement = TJPlacement.CreatePlacement(samplePlacementName);
			if (samplePlacement != null)
			{
				samplePlacement.RequestContent();
				output = "Requesting content for placement: " + samplePlacementName;
			}
		}
		if (!contentIsReadyForPlacement)
		{
			GUI.enabled = false;
		}
		position = new Rect(centerX, num, halfButtonWidth, buttonHeight);
		if (GUI.Button(position, "Show") && samplePlacement != null)
		{
			samplePlacement.ShowContent();
		}
		if (!contentIsReadyForPlacement)
		{
			GUI.enabled = true;
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, headerHeight);
		GUI.Label(position, "Purchase:", headerStyle);
		num += yPadding - (yPadding - headerHeight);
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Purchase"))
		{
			Tapjoy.TrackPurchase("product1", "USD", 0.99);
			output = "Sent track purchase";
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Purchase (Campaign)"))
		{
			Tapjoy.TrackPurchase("product1", "USD", 1.99, "TestCampaignID");
			output = "Sent track purchase 2";
		}
		num += yPadding;
		position = new Rect(centerX - halfButtonWidth, num, halfButtonWidth, buttonHeight);
		if (GUI.Button(position, "Purchase (GooglePlayStore)"))
		{
			Tapjoy.TrackPurchaseInGooglePlayStore(getDummySkuDetails(), getDummyPurchaseData(), getDummyDataSignature(), "TestCampaignID");
			output = "Sent TrackPurchaseInGooglePlayStore";
		}
		position = new Rect(centerX, num, halfButtonWidth, buttonHeight);
		if (GUI.Button(position, "Purchase (AppleAppStore)"))
		{
			Tapjoy.TrackPurchaseInAppleAppStore("product1", "USD", 1.99, "transactionId", "TestCampaignID");
			output = "Sent TrackPurchaseInAppleAppStore";
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth / 2f, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Show Tapjoy Support Page"))
		{
			Application.OpenURL(Tapjoy.GetSupportURL());
		}
		num += yPadding;
		GUI.Label(new Rect(centerX - 200f, num, 400f, 150f), output, outputStyle);
	}

	private void ResetCurrencyLabel()
	{
		output = "Updating Currency...";
	}

	private string getDummySkuDetails()
	{
		return "{\"title\":\"TITLE\",\"price\":\"$3.33\",\"type\":\"inapp\",\"description\":\"DESC\",\"price_amount_micros\":3330000,\"price_currency_code\":\"USD\",\"productId\":\"3\"}";
	}

	private string getDummyPurchaseData()
	{
		return null;
	}

	private string getDummyDataSignature()
	{
		return null;
	}
}
