namespace AdColony
{
	public interface IAds
	{
		void Configure(string appId, AppOptions options, params string[] zoneIds);

		string GetSDKVersion();

		void RequestInterstitialAd(string zoneId, AdOptions adOptions);

		Zone GetZone(string zoneId);

		string GetUserID();

		void SetAppOptions(AppOptions options);

		AppOptions GetAppOptions();

		void SendCustomMessage(string type, string content);

		void LogInAppPurchase(string transactionId, string productId, int purchasePriceMicro, string currencyCode);

		void ShowAd(InterstitialAd ad);

		void CancelAd(InterstitialAd ad);

		void DestroyAd(string id);
	}
}
