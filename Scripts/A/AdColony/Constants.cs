namespace AdColony
{
	public class Constants
	{
		public static string OptionsMetadataKey = "metadata";

		public static string AppOptionsDisableLoggingKey = "logging";

		public static string AppOptionsUserIdKey = "user_id";

		public static string AppOptionsOrientationKey = "orientation";

		public static string AppOptionsTestModeKey = "test_mode";

		public static string AppOptionsGdprRequiredKey = "gdpr_required";

		public static string AppOptionsGdprConsentStringKey = "consent_string";

		public static string AppOptionsMultiWindowEnabledKey = "multi_window_enabled";

		public static string AppOptionsOriginStoreKey = "origin_store";

		public static string AdOptionsPrePopupKey = "pre_popup";

		public static string AdOptionsPostPopupKey = "post_popup";

		public static string UserMetadataAgeKey = "age";

		public static string UserMetadataInterestsKey = "interests";

		public static string UserMetadataGenderKey = "gender";

		public static string UserMetadataLatitudeKey = "latitude";

		public static string UserMetadataLongitudeKey = "longitude";

		public static string UserMetadataZipCodeKey = "zipcode";

		public static string UserMetadataHouseholdIncomeKey = "income";

		public static string UserMetadataMaritalStatusKey = "marital_status";

		public static string UserMetadataEducationLevelKey = "edu_level";

		public static string ZoneIdentifierKey = "zone_id";

		public static string ZoneTypeKey = "type";

		public static string ZoneEnabledKey = "enabled";

		public static string ZoneRewardedKey = "rewarded";

		public static string ZoneViewsPerRewardKey = "views_per_reward";

		public static string ZoneViewsUntilRewardKey = "views_until_reward";

		public static string ZoneRewardAmountKey = "reward_amount";

		public static string ZoneRewardNameKey = "reward_name";

		public static string OnIAPOpportunityAdKey = "ad";

		public static string OnIAPOpportunityEngagementKey = "engagement";

		public static string OnIAPOpportunityIapProductIdKey = "iap_product_id";

		public static string OnRewardGrantedZoneIdKey = "zone_id";

		public static string OnRewardGrantedSuccessKey = "success";

		public static string OnRewardGrantedNameKey = "name";

		public static string OnRewardGrantedAmountKey = "amount";

		public static string OnCustomMessageReceivedTypeKey = "type";

		public static string OnCustomMessageReceivedMessageKey = "message";

		public static string AdsManagerName = "AdColony";

		public static string AdsMessageNotInitialized = "AdColony SDK not initialized, use Configure()";

		public static string AdsMessageAlreadyInitialized = "AdColony SDK already initialized";

		public static string AdsMessageSDKUnavailable = "AdColony SDK unavailable on current platform";

		public static string AdsMessageErrorNullAd = "Error, ad is null";

		public static string AdsMessageErrorUnableToRebuildAd = "Error, unable to rebuild ad";

		public static string AdsMessageErrorInvalidImplementation = "Error, platform-specific implementation not set";

		public const string AdapterVersion = "3.3.5";

		public const string AndroidSDKVersion = "3.3.5";

		public const string iOSSDKVersion = "3.3.5";
	}
}
