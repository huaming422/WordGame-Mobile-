using System.Collections;
using UnityEngine;

namespace AdColony
{
	public class EventTrackerAndroid : IEventTracker
	{
		private AndroidJavaClass _plugin;

		private AndroidJavaClass _pluginWrapper;

		public EventTrackerAndroid()
		{
			_plugin = new AndroidJavaClass("com.adcolony.sdk.AdColonyEventTracker");
			_pluginWrapper = new AndroidJavaClass("com.adcolony.unityplugin.UnityADCAds");
		}

		public void LogTransactionWithID(string itemID, int quantity, double price, string currencyCode, string receipt, string store, string description)
		{
			_pluginWrapper.CallStatic("logTransactionWithID", itemID, quantity, price, currencyCode, receipt, store, description);
		}

		public void LogCreditsSpentWithName(string name, int quantity, double val, string currencyCode)
		{
			_pluginWrapper.CallStatic("logCreditsSpentWithName", name, quantity, val, currencyCode);
		}

		public void LogPaymentInfoAdded()
		{
			_plugin.CallStatic("logPaymentInfoAdded");
		}

		public void LogAchievementUnlocked(string description)
		{
			_plugin.CallStatic("logAchievementUnlocked", description);
		}

		public void LogLevelAchieved(int level)
		{
			_pluginWrapper.CallStatic("logLevelAchieved", level);
		}

		public void LogAppRated()
		{
			_plugin.CallStatic("logAppRated");
		}

		public void LogActivated()
		{
			_plugin.CallStatic("logActivated");
		}

		public void LogTutorialCompleted()
		{
			_plugin.CallStatic("logTutorialCompleted");
		}

		public void LogSocialSharingEventWithNetwork(string network, string description)
		{
			_plugin.CallStatic("logSocialSharingEvent", network, description);
		}

		public void LogRegistrationCompletedWithMethod(string method, string description)
		{
			_plugin.CallStatic("logRegistrationCompleted", method, description);
		}

		public void LogCustomEvent(string eventName, string description)
		{
			_plugin.CallStatic("logCustomEvent", eventName, description);
		}

		public void LogAddToCartWithID(string itemID)
		{
			_plugin.CallStatic("logAddToCart", itemID);
		}

		public void LogAddToWishlistWithID(string itemID)
		{
			_plugin.CallStatic("logAddToWishlist", itemID);
		}

		public void LogCheckoutInitiated()
		{
			_plugin.CallStatic("logCheckoutInitiated");
		}

		public void LogContentViewWithID(string contentID, string contentType)
		{
			_plugin.CallStatic("logContentView", contentID, contentType);
		}

		public void LogInvite()
		{
			_plugin.CallStatic("logInvite");
		}

		public void LogLoginWithMethod(string method)
		{
			_plugin.CallStatic("logLogin", method);
		}

		public void LogReservation()
		{
			_plugin.CallStatic("logReservation");
		}

		public void LogSearchWithQuery(string queryString)
		{
			_plugin.CallStatic("logSearch", queryString);
		}

		public void LogEvent(string name, Hashtable data)
		{
			if (data != null)
			{
				string text = AdColonyJson.Encode(data);
				_pluginWrapper.CallStatic("logEvent", name, text);
			}
			else
			{
				_plugin.CallStatic("logEvent", name);
			}
		}
	}
}
