using System.Collections;

namespace AdColony
{
	public interface IEventTracker
	{
		void LogTransactionWithID(string itemID, int quantity, double price, string currencyCode, string receipt, string store, string description);

		void LogCreditsSpentWithName(string name, int quantity, double value, string currencyCode);

		void LogPaymentInfoAdded();

		void LogAchievementUnlocked(string description);

		void LogLevelAchieved(int level);

		void LogAppRated();

		void LogActivated();

		void LogTutorialCompleted();

		void LogSocialSharingEventWithNetwork(string network, string description);

		void LogRegistrationCompletedWithMethod(string method, string description);

		void LogCustomEvent(string eventName, string description);

		void LogAddToCartWithID(string itemID);

		void LogAddToWishlistWithID(string itemID);

		void LogCheckoutInitiated();

		void LogContentViewWithID(string contentID, string contentType);

		void LogInvite();

		void LogLoginWithMethod(string method);

		void LogReservation();

		void LogSearchWithQuery(string queryString);

		void LogEvent(string name, Hashtable data);
	}
}
