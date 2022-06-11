using System.Collections.Generic;

namespace TapjoyUnity.Internal
{
	internal class ApiBindingNone : ApiBinding
	{
		public ApiBindingNone()
			: base("None")
		{
		}

		public override void Connect(string sdkKey, Dictionary<string, object> flag)
		{
		}

		public override void ActionComplete(string actionID)
		{
		}

		public override string GetSDKVersion()
		{
			return "none";
		}

		public override void SetDebugEnabled(bool enabled)
		{
		}

		public override void SetGcmSender(string senderId)
		{
		}

		public override void SetAppDataVersion(string dataVersion)
		{
		}

		public override void ActivateUnitySupport()
		{
		}

		public override void SubjectToGDPR(bool subject)
		{
		}

		public override void SetUserConsent(string consent)
		{
		}

		public override void GetCurrencyBalance()
		{
		}

		public override void SpendCurrency(int amount)
		{
		}

		public override void AwardCurrency(int amount)
		{
		}

		public override float GetCurrencyMultiplier()
		{
			return 0f;
		}

		public override string GetSupportURL()
		{
			return "";
		}

		public override string GetSupportURL(string currencyID)
		{
			return "";
		}

		public override void SetCurrencyMultiplier(float multiplier)
		{
		}

		public override void ShowDefaultEarnedCurrencyAlert()
		{
		}

		public override void CreatePlacement(string placementGuid, string eventName)
		{
		}

		public override void DismissPlacementContent()
		{
		}

		public override void RequestPlacementContent(string placementGuid)
		{
		}

		public override void ShowPlacementContent(string placementGuid)
		{
		}

		public override bool IsPlacementContentReady(string placementGuid)
		{
			return false;
		}

		public override bool IsPlacementContentAvailable(string placementGuid)
		{
			return false;
		}

		public override void ActionRequestCompleted(string requestId)
		{
		}

		public override void ActionRequestCancelled(string requestId)
		{
		}

		public override void RemovePlacement(string placementGuid)
		{
		}

		public override void RemoveActionRequest(string requestID)
		{
		}

		public override void StartSession()
		{
		}

		public override void EndSession()
		{
		}

		public override void SetUserID(string userId)
		{
		}

		public override void SetUserLevel(int userLevel)
		{
		}

		public override void SetUserFriendCount(int friendCount)
		{
		}

		public override void SetUserCohortVariable(int variableIndex, string value)
		{
		}

		public override void ClearUserTags()
		{
		}

		public override List<string> GetUserTags()
		{
			return null;
		}

		public override void AddUserTag(string tag)
		{
		}

		public override void RemoveUserTag(string tag)
		{
		}

		public override void TrackEvent(string name, long value)
		{
		}

		public override void TrackEvent(string category, string name, long value)
		{
		}

		public override void TrackEvent(string category, string name, string parameter1, string parameter2, long value)
		{
		}

		public override void TrackEvent(string category, string name, string parameter1, string parameter2, string value1Name, long value1, string value2Name, long value2, string value3Name, long value3)
		{
		}

		public override void TrackPurchase(string productId, string currencyCode, double price, string campaignId)
		{
		}

		public override void TrackPurchaseInGooglePlayStore(string skuDetails, string purchaseData, string dataSignature, string campaignId)
		{
		}

		public override void TrackPurchaseInAppleAppStore(string productId, string currencyCode, double productPrice, string transactionId, string campaignId)
		{
		}

		public override bool IsPushNotificationDisabled()
		{
			return false;
		}

		public override void SetPushNotificationDisabled(bool disabled)
		{
		}

		public override void TrackUsage(string name, string dimensions, string values)
		{
		}

		public override void SetDeviceToken(string deviceToken)
		{
		}

		public override void SetReceiveRemoteNotification(Dictionary<string, string> remoteMessage)
		{
		}
	}
}
