using UnityEngine;

namespace ChartboostSDK
{
	public class CBSettings : ScriptableObject
	{
		private const string cbSettingsAssetName = "ChartboostSettings";

		private const string cbSettingsPath = "Chartboost/Resources";

		private const string cbSettingsAssetExtension = ".asset";

		private const string iOSExampleAppIDLabel = "CB_IOS_APP_ID";

		private const string iOSExampleAppSignatureLabel = "CB_IOS_APP_SIGNATURE";

		private const string iOSExampleAppID = "4f21c409cd1cb2fb7000001b";

		private const string iOSExampleAppSignature = "92e2de2fd7070327bdeb54c15a5295309c6fcd2d";

		private const string androidExampleAppIDLabel = "CB_ANDROID_APP_ID";

		private const string androidExampleAppSignatureLabel = "CB_ANDROID_APP_SIGNATURE";

		private const string androidExampleAppID = "4f7b433509b6025804000002";

		private const string androidExampleAppSignature = "dd2d41b69ac01b80f443f5b6cf06096d457f82bd";

		private const string amazonExampleAppIDLabel = "CB_AMAZON_APP_ID";

		private const string amazonExampleAppSignatureLabel = "CB_AMAZON_APP_SIGNATURE";

		private const string amazonExampleAppID = "542ca35d1873da32dbc90488";

		private const string amazonExampleAppSignature = "90654a340386c9fb8de33315e4210d7c09989c43";

		private const string credentialsWarningDefaultFormat = "CHARTBOOST: You are using the Chartboost {0} example {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";

		private const string credentialsWarningEmptyFormat = "CHARTBOOST: You are using an empty string for the {0} {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";

		private const string credentialsWarningIOS = "IOS";

		private const string credentialsWarningAndroid = "Android";

		private const string credentialsWarningAmazon = "Amazon";

		private const string credentialsWarningAppID = "App ID";

		private const string credentialsWarningAppSignature = "App Signature";

		private static bool credentialsWarning;

		private static CBSettings instance;

		[SerializeField]
		public string iOSAppId = "CB_IOS_APP_ID";

		[SerializeField]
		public string iOSAppSecret = "CB_IOS_APP_SIGNATURE";

		[SerializeField]
		public string androidAppId = "CB_ANDROID_APP_ID";

		[SerializeField]
		public string androidAppSecret = "CB_ANDROID_APP_SIGNATURE";

		[SerializeField]
		public string amazonAppId = "CB_AMAZON_APP_ID";

		[SerializeField]
		public string amazonAppSecret = "CB_AMAZON_APP_SIGNATURE";

		[SerializeField]
		public bool isLoggingEnabled;

		[SerializeField]
		public string[] androidPlatformLabels = new string[2] { "Google Play", "Amazon" };

		[SerializeField]
		public int selectedAndroidPlatformIndex;

		private static CBSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load("ChartboostSettings") as CBSettings;
					if (instance == null)
					{
						instance = ScriptableObject.CreateInstance<CBSettings>();
					}
				}
				return instance;
			}
		}

		public int SelectedAndroidPlatformIndex
		{
			get
			{
				return selectedAndroidPlatformIndex;
			}
		}

		public string[] AndroidPlatformLabels
		{
			get
			{
				return androidPlatformLabels;
			}
			set
			{
				if (!androidPlatformLabels.Equals(value))
				{
					androidPlatformLabels = value;
					DirtyEditor();
				}
			}
		}

		public static void setAppId(string appId, string appSignature)
		{
			if (Instance.selectedAndroidPlatformIndex == 0)
			{
				Debug.Log("Overriding Google AppId: " + appId);
				Instance.SetAndroidAppId(appId);
				Instance.SetAndroidAppSecret(appSignature);
			}
			else
			{
				Debug.Log("Overriding Amazon AppId: " + appId);
				Instance.SetAmazonAppId(appId);
				Instance.SetAmazonAppSecret(appSignature);
			}
		}

		public void SetAndroidPlatformIndex(int index)
		{
			if (selectedAndroidPlatformIndex != index)
			{
				selectedAndroidPlatformIndex = index;
				DirtyEditor();
			}
		}

		public void SetIOSAppId(string id)
		{
			if (!Instance.iOSAppId.Equals(id))
			{
				Instance.iOSAppId = id;
				DirtyEditor();
			}
		}

		public static string getIOSAppId()
		{
			if (Instance.iOSAppId.Equals("CB_IOS_APP_ID"))
			{
				CredentialsWarning("CHARTBOOST: You are using the Chartboost {0} example {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "IOS", "App ID");
				return "4f21c409cd1cb2fb7000001b";
			}
			if (Instance.iOSAppId.Equals(string.Empty))
			{
				CredentialsWarning("CHARTBOOST: You are using an empty string for the {0} {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "IOS", "App ID");
			}
			return Instance.iOSAppId;
		}

		public void SetIOSAppSecret(string secret)
		{
			if (!Instance.iOSAppSecret.Equals(secret))
			{
				Instance.iOSAppSecret = secret;
				DirtyEditor();
			}
		}

		public static string getIOSAppSecret()
		{
			if (Instance.iOSAppSecret.Equals("CB_IOS_APP_SIGNATURE"))
			{
				CredentialsWarning("CHARTBOOST: You are using the Chartboost {0} example {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "IOS", "App Signature");
				return "92e2de2fd7070327bdeb54c15a5295309c6fcd2d";
			}
			if (Instance.iOSAppSecret.Equals(string.Empty))
			{
				CredentialsWarning("CHARTBOOST: You are using an empty string for the {0} {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "IOS", "App Signature");
			}
			return Instance.iOSAppSecret;
		}

		public void SetAndroidAppId(string id)
		{
			if (!Instance.androidAppId.Equals(id))
			{
				Instance.androidAppId = id;
				DirtyEditor();
			}
		}

		public static string getAndroidAppId()
		{
			if (Instance.androidAppId.Equals("CB_ANDROID_APP_ID"))
			{
				CredentialsWarning("CHARTBOOST: You are using the Chartboost {0} example {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Android", "App ID");
				return "4f7b433509b6025804000002";
			}
			if (Instance.androidAppId.Equals(string.Empty))
			{
				CredentialsWarning("CHARTBOOST: You are using an empty string for the {0} {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Android", "App ID");
			}
			return Instance.androidAppId;
		}

		public void SetAndroidAppSecret(string secret)
		{
			if (!Instance.androidAppSecret.Equals(secret))
			{
				Instance.androidAppSecret = secret;
				DirtyEditor();
			}
		}

		public static string getAndroidAppSecret()
		{
			if (Instance.androidAppSecret.Equals("CB_ANDROID_APP_SIGNATURE"))
			{
				CredentialsWarning("CHARTBOOST: You are using the Chartboost {0} example {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Android", "App Signature");
				return "dd2d41b69ac01b80f443f5b6cf06096d457f82bd";
			}
			if (Instance.androidAppSecret.Equals(string.Empty))
			{
				CredentialsWarning("CHARTBOOST: You are using an empty string for the {0} {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Android", "App Signature");
			}
			return Instance.androidAppSecret;
		}

		public void SetAmazonAppId(string id)
		{
			if (!Instance.amazonAppId.Equals(id))
			{
				Instance.amazonAppId = id;
				DirtyEditor();
			}
		}

		public static string getAmazonAppId()
		{
			if (Instance.amazonAppId.Equals("CB_AMAZON_APP_ID"))
			{
				CredentialsWarning("CHARTBOOST: You are using the Chartboost {0} example {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Amazon", "App ID");
				return "542ca35d1873da32dbc90488";
			}
			if (Instance.amazonAppId.Equals(string.Empty))
			{
				CredentialsWarning("CHARTBOOST: You are using an empty string for the {0} {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Amazon", "App ID");
			}
			return Instance.amazonAppId;
		}

		public void SetAmazonAppSecret(string secret)
		{
			if (!Instance.amazonAppSecret.Equals(secret))
			{
				Instance.amazonAppSecret = secret;
				DirtyEditor();
			}
		}

		public static string getAmazonAppSecret()
		{
			if (Instance.amazonAppSecret.Equals("CB_AMAZON_APP_SIGNATURE"))
			{
				CredentialsWarning("CHARTBOOST: You are using the Chartboost {0} example {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Amazon", "App Signature");
				return "90654a340386c9fb8de33315e4210d7c09989c43";
			}
			if (Instance.amazonAppSecret.Equals(string.Empty))
			{
				CredentialsWarning("CHARTBOOST: You are using an empty string for the {0} {1}! Go to the Chartboost dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com", "Amazon", "App Signature");
			}
			return Instance.amazonAppSecret;
		}

		public static string getSelectAndroidAppId()
		{
			if (Instance.selectedAndroidPlatformIndex == 0)
			{
				return getAndroidAppId();
			}
			return getAmazonAppId();
		}

		public static string getSelectAndroidAppSecret()
		{
			if (Instance.selectedAndroidPlatformIndex == 0)
			{
				return getAndroidAppSecret();
			}
			return getAmazonAppSecret();
		}

		public static void enableLogging(bool enabled)
		{
			Instance.isLoggingEnabled = enabled;
			DirtyEditor();
		}

		public static bool isLogging()
		{
			return Instance.isLoggingEnabled;
		}

		private static void DirtyEditor()
		{
		}

		private static void CredentialsWarning(string warning, string platform, string field)
		{
			if (!credentialsWarning)
			{
				credentialsWarning = true;
				Debug.LogWarning(string.Format(warning, platform, field));
			}
		}

		public static void resetSettings()
		{
			if (Instance.iOSAppId.Equals("4f21c409cd1cb2fb7000001b"))
			{
				Instance.SetIOSAppId("CB_IOS_APP_ID");
			}
			if (Instance.iOSAppSecret.Equals("92e2de2fd7070327bdeb54c15a5295309c6fcd2d"))
			{
				Instance.SetIOSAppSecret("CB_IOS_APP_SIGNATURE");
			}
			if (Instance.androidAppId.Equals("4f7b433509b6025804000002"))
			{
				Instance.SetAndroidAppId("CB_ANDROID_APP_ID");
			}
			if (Instance.androidAppSecret.Equals("dd2d41b69ac01b80f443f5b6cf06096d457f82bd"))
			{
				Instance.SetAndroidAppSecret("CB_ANDROID_APP_SIGNATURE");
			}
			if (Instance.amazonAppId.Equals("542ca35d1873da32dbc90488"))
			{
				Instance.SetAmazonAppId("CB_AMAZON_APP_ID");
			}
			if (Instance.amazonAppSecret.Equals("90654a340386c9fb8de33315e4210d7c09989c43"))
			{
				Instance.SetAmazonAppSecret("CB_AMAZON_APP_SIGNATURE");
			}
		}
	}
}
