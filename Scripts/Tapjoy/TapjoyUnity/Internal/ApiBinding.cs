using System.Collections.Generic;

namespace TapjoyUnity.Internal
{
	public abstract class ApiBinding
	{
		internal delegate void OnInstanceSetHandler();

		protected const string DEFAULT_EVENT_VALUE_NAME = "value";

		protected const string VERSION_NAME = "12.1.0";

		protected const string CONNECT_FLAG_DICTIONARY_NAME = "connectFlags";

		protected const string REMOTE_MESSAGE_DICTIONARY_NAME = "fcmRemoteMessage";

		private static ApiBinding instance = null;

		private static OnInstanceSetHandler onInstanceSetHandler;

		private string name;

		public static ApiBinding Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ApiBindingNone();
				}
				return instance;
			}
		}

		internal static OnInstanceSetHandler OnInstanceSet
		{
			set
			{
				onInstanceSetHandler = value;
				if (value != null && !(instance is ApiBindingNone))
				{
					value();
				}
			}
		}

		internal string Name
		{
			get
			{
				return name;
			}
		}

		protected ApiBinding(string name)
		{
			this.name = name;
		}

		protected static void SetInstance(ApiBinding value)
		{
			if (instance == null || instance is ApiBindingNone)
			{
				instance = value;
				if (onInstanceSetHandler != null)
				{
					onInstanceSetHandler();
				}
			}
		}

		public abstract void Connect(string sdkKey, Dictionary<string, object> flag);

		public abstract string GetSDKVersion();

		public abstract void SetDebugEnabled(bool enabled);

		public abstract void SetGcmSender(string senderId);

		public abstract void SetAppDataVersion(string dataVersion);

		public abstract void ActivateUnitySupport();

		public abstract void SubjectToGDPR(bool subject);

		public abstract void SetUserConsent(string consent);

		public abstract void GetCurrencyBalance();

		public abstract void SpendCurrency(int points);

		public abstract void AwardCurrency(int points);

		public abstract float GetCurrencyMultiplier();

		public abstract string GetSupportURL();

		public abstract string GetSupportURL(string currencyID);

		public abstract void SetCurrencyMultiplier(float multiplier);

		public abstract void ShowDefaultEarnedCurrencyAlert();

		public abstract void ActionComplete(string actionID);

		public abstract void CreatePlacement(string placementGuid, string placementName);

		public abstract void DismissPlacementContent();

		public abstract void RequestPlacementContent(string placementGuid);

		public abstract void ShowPlacementContent(string placementGuid);

		public abstract void ActionRequestCompleted(string requestId);

		public abstract void ActionRequestCancelled(string requestId);

		public abstract bool IsPlacementContentReady(string placementGuid);

		public abstract bool IsPlacementContentAvailable(string placementGuid);

		public abstract void RemovePlacement(string placementGuid);

		public abstract void RemoveActionRequest(string requestId);

		public abstract void StartSession();

		public abstract void EndSession();

		public abstract void SetUserID(string userId);

		public abstract void SetUserLevel(int userLevel);

		public abstract void SetUserFriendCount(int friendCount);

		public abstract void SetUserCohortVariable(int variableIndex, string value);

		public abstract void ClearUserTags();

		public abstract List<string> GetUserTags();

		public abstract void AddUserTag(string tag);

		public abstract void RemoveUserTag(string tag);

		public abstract bool IsPushNotificationDisabled();

		public abstract void SetPushNotificationDisabled(bool disabled);

		public abstract void SetDeviceToken(string deviceToken);

		public abstract void SetReceiveRemoteNotification(Dictionary<string, string> remoteMessage);

		public abstract void TrackEvent(string name, long value);

		public abstract void TrackEvent(string category, string name, long value);

		public abstract void TrackEvent(string category, string name, string parameter1, string parameter2, long value);

		public abstract void TrackEvent(string category, string name, string parameter1, string parameter2, string value1Name, long value1, string value2Name, long value2, string value3Name, long value3);

		public abstract void TrackPurchase(string productId, string currencyCode, double price, string campaignId);

		public abstract void TrackPurchaseInGooglePlayStore(string skuDetails, string purchaseData, string dataSignature, string campaignId);

		public abstract void TrackPurchaseInAppleAppStore(string productId, string currencyCode, double productPrice, string transactionId, string campaignId);

		public abstract void TrackUsage(string name, string dimensions, string values);
	}
}
