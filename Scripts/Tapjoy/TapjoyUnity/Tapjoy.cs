using System;
using System.Collections.Generic;
using TapjoyUnity.Internal;
using UnityEngine;

namespace TapjoyUnity
{
	public class Tapjoy
	{
		public delegate void OnConnectSuccessHandler();

		public delegate void OnConnectFailureHandler();

		public delegate void OnSetUserIDSuccessHandler();

		public delegate void OnSetUserIDFailureHandler(string errorMessage);

		public delegate void OnGetCurrencyBalanceResponseHandler(string currencyName, int balance);

		public delegate void OnGetCurrencyBalanceResponseFailureHandler(string errorMessage);

		public delegate void OnSpendCurrencyResponseHandler(string currencyName, int balance);

		public delegate void OnSpendCurrencyResponseFailureHandler(string errorMessage);

		public delegate void OnAwardCurrencyResponseHandler(string currencyName, int balance);

		public delegate void OnAwardCurrencyResponseFailureHandler(string errorMessage);

		public delegate void OnEarnedCurrencyHandler(string currencyName, int amount);

		public delegate void OnVideoStartHandler();

		public delegate void OnVideoErrorHandler(string errorMessage);

		public delegate void OnVideoCompleteHandler();

		internal const string VERSION_NAME = "12.1.0";

		private static bool _isConnected = false;

		private static OnConnectSuccessHandler OnConnectSuccessInvoker;

		private static OnConnectSuccessHandler OnConnectSuccessInternalInvoker;

		private static OnConnectFailureHandler OnConnectFailureInvoker;

		private static OnConnectFailureHandler OnConnectFailureInternalInvoker;

		private static OnSetUserIDSuccessHandler OnSetUserIDSuccessInvoker;

		private static OnSetUserIDFailureHandler OnSetUserIDFailureInvoker;

		private static OnGetCurrencyBalanceResponseHandler OnGetCurrencyBalanceResponseInvoker;

		private static OnGetCurrencyBalanceResponseFailureHandler OnGetCurrencyBalanceResponseFailureInvoker;

		private static OnSpendCurrencyResponseHandler OnSpendCurrencyResponseInvoker;

		private static OnSpendCurrencyResponseFailureHandler OnSpendCurrencyResponseFailureInvoker;

		private static OnAwardCurrencyResponseHandler OnAwardCurrencyResponseInvoker;

		private static OnAwardCurrencyResponseFailureHandler OnAwardCurrencyResponseFailureInvoker;

		private static OnEarnedCurrencyHandler OnEarnedCurrencyInvoker;

		private static OnVideoStartHandler OnVideoStartInvoker;

		private static OnVideoErrorHandler OnVideoErrorInvoker;

		private static OnVideoCompleteHandler OnVideoCompleteInvoker;

		public static string Version
		{
			get
			{
				return "12.1.0";
			}
		}

		public static bool IsConnected
		{
			get
			{
				return _isConnected;
			}
			set
			{
				_isConnected = value;
			}
		}

		public static event OnConnectSuccessHandler OnConnectSuccess
		{
			add
			{
				OnConnectSuccessInvoker = (OnConnectSuccessHandler)Delegate.Combine(OnConnectSuccessInvoker, value);
			}
			remove
			{
				OnConnectSuccessInvoker = (OnConnectSuccessHandler)Delegate.Remove(OnConnectSuccessInvoker, value);
			}
		}

		internal static event OnConnectSuccessHandler OnConnectSuccessInternal
		{
			add
			{
				OnConnectSuccessInternalInvoker = (OnConnectSuccessHandler)Delegate.Combine(OnConnectSuccessInternalInvoker, value);
			}
			remove
			{
				OnConnectSuccessInternalInvoker = (OnConnectSuccessHandler)Delegate.Remove(OnConnectSuccessInternalInvoker, value);
			}
		}

		public static event OnConnectFailureHandler OnConnectFailure
		{
			add
			{
				OnConnectFailureInvoker = (OnConnectFailureHandler)Delegate.Combine(OnConnectFailureInvoker, value);
			}
			remove
			{
				OnConnectFailureInvoker = (OnConnectFailureHandler)Delegate.Remove(OnConnectFailureInvoker, value);
			}
		}

		internal static event OnConnectFailureHandler OnConnectFailureInternal
		{
			add
			{
				OnConnectFailureInternalInvoker = (OnConnectFailureHandler)Delegate.Combine(OnConnectFailureInternalInvoker, value);
			}
			remove
			{
				OnConnectFailureInternalInvoker = (OnConnectFailureHandler)Delegate.Remove(OnConnectFailureInternalInvoker, value);
			}
		}

		public static event OnSetUserIDSuccessHandler OnSetUserIDSuccess
		{
			add
			{
				OnSetUserIDSuccessInvoker = (OnSetUserIDSuccessHandler)Delegate.Combine(OnSetUserIDSuccessInvoker, value);
			}
			remove
			{
				OnSetUserIDSuccessInvoker = (OnSetUserIDSuccessHandler)Delegate.Remove(OnSetUserIDSuccessInvoker, value);
			}
		}

		public static event OnSetUserIDFailureHandler OnSetUserIDFailure
		{
			add
			{
				OnSetUserIDFailureInvoker = (OnSetUserIDFailureHandler)Delegate.Combine(OnSetUserIDFailureInvoker, value);
			}
			remove
			{
				OnSetUserIDFailureInvoker = (OnSetUserIDFailureHandler)Delegate.Remove(OnSetUserIDFailureInvoker, value);
			}
		}

		public static event OnGetCurrencyBalanceResponseHandler OnGetCurrencyBalanceResponse
		{
			add
			{
				OnGetCurrencyBalanceResponseInvoker = (OnGetCurrencyBalanceResponseHandler)Delegate.Combine(OnGetCurrencyBalanceResponseInvoker, value);
			}
			remove
			{
				OnGetCurrencyBalanceResponseInvoker = (OnGetCurrencyBalanceResponseHandler)Delegate.Remove(OnGetCurrencyBalanceResponseInvoker, value);
			}
		}

		public static event OnGetCurrencyBalanceResponseFailureHandler OnGetCurrencyBalanceResponseFailure
		{
			add
			{
				OnGetCurrencyBalanceResponseFailureInvoker = (OnGetCurrencyBalanceResponseFailureHandler)Delegate.Combine(OnGetCurrencyBalanceResponseFailureInvoker, value);
			}
			remove
			{
				OnGetCurrencyBalanceResponseFailureInvoker = (OnGetCurrencyBalanceResponseFailureHandler)Delegate.Remove(OnGetCurrencyBalanceResponseFailureInvoker, value);
			}
		}

		public static event OnSpendCurrencyResponseHandler OnSpendCurrencyResponse
		{
			add
			{
				OnSpendCurrencyResponseInvoker = (OnSpendCurrencyResponseHandler)Delegate.Combine(OnSpendCurrencyResponseInvoker, value);
			}
			remove
			{
				OnSpendCurrencyResponseInvoker = (OnSpendCurrencyResponseHandler)Delegate.Remove(OnSpendCurrencyResponseInvoker, value);
			}
		}

		public static event OnSpendCurrencyResponseFailureHandler OnSpendCurrencyResponseFailure
		{
			add
			{
				OnSpendCurrencyResponseFailureInvoker = (OnSpendCurrencyResponseFailureHandler)Delegate.Combine(OnSpendCurrencyResponseFailureInvoker, value);
			}
			remove
			{
				OnSpendCurrencyResponseFailureInvoker = (OnSpendCurrencyResponseFailureHandler)Delegate.Remove(OnSpendCurrencyResponseFailureInvoker, value);
			}
		}

		public static event OnAwardCurrencyResponseHandler OnAwardCurrencyResponse
		{
			add
			{
				OnAwardCurrencyResponseInvoker = (OnAwardCurrencyResponseHandler)Delegate.Combine(OnAwardCurrencyResponseInvoker, value);
			}
			remove
			{
				OnAwardCurrencyResponseInvoker = (OnAwardCurrencyResponseHandler)Delegate.Remove(OnAwardCurrencyResponseInvoker, value);
			}
		}

		public static event OnAwardCurrencyResponseFailureHandler OnAwardCurrencyResponseFailure
		{
			add
			{
				OnAwardCurrencyResponseFailureInvoker = (OnAwardCurrencyResponseFailureHandler)Delegate.Combine(OnAwardCurrencyResponseFailureInvoker, value);
			}
			remove
			{
				OnAwardCurrencyResponseFailureInvoker = (OnAwardCurrencyResponseFailureHandler)Delegate.Remove(OnAwardCurrencyResponseFailureInvoker, value);
			}
		}

		public static event OnEarnedCurrencyHandler OnEarnedCurrency
		{
			add
			{
				OnEarnedCurrencyInvoker = (OnEarnedCurrencyHandler)Delegate.Combine(OnEarnedCurrencyInvoker, value);
			}
			remove
			{
				OnEarnedCurrencyInvoker = (OnEarnedCurrencyHandler)Delegate.Remove(OnEarnedCurrencyInvoker, value);
			}
		}

		public static event OnVideoStartHandler OnVideoStart
		{
			add
			{
				OnVideoStartInvoker = (OnVideoStartHandler)Delegate.Combine(OnVideoStartInvoker, value);
			}
			remove
			{
				OnVideoStartInvoker = (OnVideoStartHandler)Delegate.Remove(OnVideoStartInvoker, value);
			}
		}

		public static event OnVideoErrorHandler OnVideoError
		{
			add
			{
				OnVideoErrorInvoker = (OnVideoErrorHandler)Delegate.Combine(OnVideoErrorInvoker, value);
			}
			remove
			{
				OnVideoErrorInvoker = (OnVideoErrorHandler)Delegate.Remove(OnVideoErrorInvoker, value);
			}
		}

		public static event OnVideoCompleteHandler OnVideoComplete
		{
			add
			{
				OnVideoCompleteInvoker = (OnVideoCompleteHandler)Delegate.Combine(OnVideoCompleteInvoker, value);
			}
			remove
			{
				OnVideoCompleteInvoker = (OnVideoCompleteHandler)Delegate.Remove(OnVideoCompleteInvoker, value);
			}
		}

		public static void Connect()
		{
			TapjoyComponent tapjoyComponent = TapjoyComponent.FindInstance();
			if (tapjoyComponent == null)
			{
				Debug.LogWarning("Can't connect. Tapjoy object is missing.");
			}
			else
			{
				tapjoyComponent.Reconnect();
			}
		}

		public static void Connect(string sdkKey)
		{
			TapjoyComponent tapjoyComponent = TapjoyComponent.FindInstance();
			if (tapjoyComponent == null)
			{
				Debug.LogWarning("Can't connect. Tapjoy object is missing.");
			}
			else
			{
				tapjoyComponent.ConnectManually(sdkKey);
			}
		}

		public static void Connect(string sdkKey, Dictionary<string, object> connectFlags)
		{
			TapjoyComponent tapjoyComponent = TapjoyComponent.FindInstance();
			if (tapjoyComponent == null)
			{
				Debug.LogWarning("Can't connect. Tapjoy object is missing.");
			}
			else
			{
				tapjoyComponent.ConnectManually(sdkKey, connectFlags);
			}
		}

		public static void SetDebugEnabled(bool enable)
		{
			ApiBinding.Instance.SetDebugEnabled(enable);
		}

		public static void SetGcmSender(string senderId)
		{
			ApiBinding.Instance.SetGcmSender(senderId);
		}

		public static void SubjectToGDPR(bool subject)
		{
			ApiBinding.Instance.SubjectToGDPR(subject);
		}

		public static void SetUserConsent(string consent)
		{
			ApiBinding.Instance.SetUserConsent(consent);
		}

		public static void ActionComplete(string actionID)
		{
			ApiBinding.Instance.ActionComplete(actionID);
		}

		public static void SetAppDataVersion(string dataVersion)
		{
			ApiBinding.Instance.SetAppDataVersion(dataVersion);
		}

		public static void SetUserID(string userId)
		{
			ApiBinding.Instance.SetUserID(userId);
		}

		public static void SetUserLevel(int userLevel)
		{
			ApiBinding.Instance.SetUserLevel(userLevel);
		}

		public static void SetUserFriendCount(int friendCount)
		{
			ApiBinding.Instance.SetUserFriendCount(friendCount);
		}

		public static void SetUserCohortVariable(int variableIndex, string value)
		{
			ApiBinding.Instance.SetUserCohortVariable(variableIndex, value);
		}

		public static void ClearUserTags()
		{
			ApiBinding.Instance.ClearUserTags();
		}

		public static List<string> GetUserTags()
		{
			return ApiBinding.Instance.GetUserTags();
		}

		public static void AddUserTag(string tag)
		{
			ApiBinding.Instance.AddUserTag(tag);
		}

		public static void RemoveUserTag(string tag)
		{
			ApiBinding.Instance.RemoveUserTag(tag);
		}

		public static void TrackEvent(string name, long value = 0L)
		{
			ApiBinding.Instance.TrackEvent(name, value);
		}

		public static void TrackEvent(string category, string name, long value)
		{
			ApiBinding.Instance.TrackEvent(category, name, value);
		}

		public static void TrackEvent(string category, string name, string parameter1, string parameter2 = null, long value = 0L)
		{
			ApiBinding.Instance.TrackEvent(category, name, parameter1, parameter2, value);
		}

		public static void TrackEvent(string category, string name, string parameter1, string parameter2, string value1Name, long value1, string value2Name = null, long value2 = 0L, string value3Name = null, long value3 = 0L)
		{
			ApiBinding.Instance.TrackEvent(category, name, parameter1, parameter2, value1Name, value1, value2Name, value2, value3Name, value3);
		}

		public static void TrackPurchase(string productId, string currencyCode, double productPrice, string campaignId = null)
		{
			ApiBinding.Instance.TrackPurchase(productId, currencyCode, productPrice, campaignId);
		}

		public static void TrackPurchaseInGooglePlayStore(string skuDetails, string purchaseData, string dataSignature, string campaignId = null)
		{
			ApiBinding.Instance.TrackPurchaseInGooglePlayStore(skuDetails, purchaseData, dataSignature, campaignId);
		}

		public static void TrackPurchaseInAppleAppStore(string productId, string currencyCode, double productPrice, string transactionId, string campaignId = null)
		{
			ApiBinding.Instance.TrackPurchaseInAppleAppStore(productId, currencyCode, productPrice, transactionId, campaignId);
		}

		public static bool IsPushNotificationDisabled()
		{
			return ApiBinding.Instance.IsPushNotificationDisabled();
		}

		public static void SetPushNotificationDisabled(bool disabled)
		{
			ApiBinding.Instance.SetPushNotificationDisabled(disabled);
		}

		public static void SetDeviceToken(string deviceToken)
		{
			ApiBinding.Instance.SetDeviceToken(deviceToken);
		}

		public static void SetReceiveRemoteNotification(Dictionary<string, string> remoteMessage)
		{
			ApiBinding.Instance.SetReceiveRemoteNotification(remoteMessage);
		}

		public static void AwardCurrency(int amount)
		{
			ApiBinding.Instance.AwardCurrency(amount);
		}

		public static void GetCurrencyBalance()
		{
			ApiBinding.Instance.GetCurrencyBalance();
		}

		public static void SpendCurrency(int amount)
		{
			ApiBinding.Instance.SpendCurrency(amount);
		}

		public static float GetCurrencyMultiplier()
		{
			return ApiBinding.Instance.GetCurrencyMultiplier();
		}

		public static void SetCurrencyMultiplier(float multiplier)
		{
			ApiBinding.Instance.SetCurrencyMultiplier(multiplier);
		}

		public static string GetSupportURL()
		{
			return ApiBinding.Instance.GetSupportURL();
		}

		public static string GetSupportURL(string currencyID)
		{
			return ApiBinding.Instance.GetSupportURL(currencyID);
		}

		public static void ShowDefaultEarnedCurrencyAlert()
		{
			ApiBinding.Instance.ShowDefaultEarnedCurrencyAlert();
		}

		internal static void DispatchConnectEvent(string connectCallbackMethod)
		{
			switch (connectCallbackMethod)
			{
			case "OnConnectSuccess":
				_isConnected = true;
				if (OnConnectSuccessInternalInvoker != null)
				{
					OnConnectSuccessInternalInvoker();
				}
				if (OnConnectSuccessInvoker != null)
				{
					OnConnectSuccessInvoker();
				}
				break;
			case "OnConnectFailure":
				if (OnConnectFailureInternalInvoker != null)
				{
					OnConnectFailureInternalInvoker();
				}
				if (OnConnectFailureInvoker != null)
				{
					OnConnectFailureInvoker();
				}
				break;
			}
		}

		internal static void DispatchSetUserIDEvent(string commaDelimitedMessage)
		{
			string[] array = commaDelimitedMessage.Split(',');
			switch (array[0])
			{
			case "OnSetUserIDSuccess":
				if (OnSetUserIDSuccessInvoker != null)
				{
					OnSetUserIDSuccessInvoker();
				}
				break;
			case "OnSetUserIDFailure":
			{
				if (array.Length < 2)
				{
					break;
				}
				string text = array[1];
				if (array.Length > 2)
				{
					for (int i = 2; i < array.Length; i++)
					{
						text += array[i];
					}
				}
				if (OnSetUserIDFailureInvoker != null)
				{
					OnSetUserIDFailureInvoker(text);
				}
				break;
			}
			}
		}

		internal static void DispatchCurrencyEvent(string commaDelimitedMessage)
		{
			string[] array = commaDelimitedMessage.Split(',');
			switch (array[0])
			{
			case "OnGetCurrencyBalanceResponse":
				if (array.Length == 3)
				{
					string currencyName2 = array[1];
					int result2;
					if (int.TryParse(array[2], out result2) && OnGetCurrencyBalanceResponseInvoker != null)
					{
						OnGetCurrencyBalanceResponseInvoker(currencyName2, result2);
					}
				}
				break;
			case "OnGetCurrencyBalanceResponseFailure":
			{
				if (array.Length < 2)
				{
					break;
				}
				string text = array[1];
				if (array.Length > 2)
				{
					for (int i = 2; i < array.Length; i++)
					{
						text += array[i];
					}
				}
				if (OnGetCurrencyBalanceResponseFailureInvoker != null)
				{
					OnGetCurrencyBalanceResponseFailureInvoker(text);
				}
				break;
			}
			case "OnSpendCurrencyResponse":
				if (array.Length == 3)
				{
					string currencyName4 = array[1];
					int result4;
					if (int.TryParse(array[2], out result4) && OnSpendCurrencyResponseInvoker != null)
					{
						OnSpendCurrencyResponseInvoker(currencyName4, result4);
					}
				}
				break;
			case "OnSpendCurrencyResponseFailure":
			{
				if (array.Length < 2)
				{
					break;
				}
				string text2 = array[1];
				if (array.Length > 2)
				{
					for (int j = 2; j < array.Length; j++)
					{
						text2 += array[j];
					}
				}
				if (OnSpendCurrencyResponseFailureInvoker != null)
				{
					OnSpendCurrencyResponseFailureInvoker(text2);
				}
				break;
			}
			case "OnAwardCurrencyResponse":
				if (array.Length == 3)
				{
					string currencyName3 = array[1];
					int result3;
					if (int.TryParse(array[2], out result3) && OnAwardCurrencyResponseInvoker != null)
					{
						OnAwardCurrencyResponseInvoker(currencyName3, result3);
					}
				}
				break;
			case "OnAwardCurrencyResponseFailure":
			{
				if (array.Length != 2)
				{
					break;
				}
				string text3 = array[1];
				if (array.Length > 2)
				{
					for (int k = 2; k < array.Length; k++)
					{
						text3 += array[k];
					}
				}
				if (OnAwardCurrencyResponseFailureInvoker != null)
				{
					OnAwardCurrencyResponseFailureInvoker(text3);
				}
				break;
			}
			case "OnEarnedCurrency":
				if (array.Length == 3)
				{
					string currencyName = array[1];
					int result;
					if (int.TryParse(array[2], out result) && OnEarnedCurrencyInvoker != null)
					{
						OnEarnedCurrencyInvoker(currencyName, result);
					}
				}
				break;
			}
		}

		internal static void DispatchVideoEvent(string commaDelimitedMessage)
		{
			string[] array = commaDelimitedMessage.Split(new char[1] { ',' }, 2);
			switch (array[0])
			{
			case "OnVideoStart":
				if (OnVideoStartInvoker != null)
				{
					OnVideoStartInvoker();
				}
				break;
			case "OnVideoError":
				if (array.Length == 2)
				{
					string errorMessage = array[1];
					if (OnVideoErrorInvoker != null)
					{
						OnVideoErrorInvoker(errorMessage);
					}
				}
				break;
			case "OnVideoComplete":
				if (OnVideoCompleteInvoker != null)
				{
					OnVideoCompleteInvoker();
				}
				break;
			}
		}
	}
}
