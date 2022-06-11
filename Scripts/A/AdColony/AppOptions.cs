using System;
using System.Collections;

namespace AdColony
{
	public class AppOptions : Options
	{
		private bool _disableLogging;

		private string _userId;

		private AdOrientationType _adOrientation = AdOrientationType.AdColonyOrientationAll;

		private bool _multiWindowEnabled;

		private string _originStore;

		private bool _testModeEnabled;

		private bool _gdprRequired;

		private string _gdprConsentString;

		public bool DisableLogging
		{
			get
			{
				return _disableLogging;
			}
			set
			{
				_disableLogging = value;
				_data[Constants.AppOptionsDisableLoggingKey] = _disableLogging;
			}
		}

		public string UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				_userId = value;
				_data[Constants.AppOptionsUserIdKey] = _userId;
			}
		}

		public AdOrientationType AdOrientation
		{
			get
			{
				return _adOrientation;
			}
			set
			{
				_adOrientation = value;
				_data[Constants.AppOptionsOrientationKey] = Convert.ToInt32(_adOrientation);
			}
		}

		public bool MultiWindowEnabled
		{
			get
			{
				return _multiWindowEnabled;
			}
			set
			{
				_multiWindowEnabled = value;
				_data[Constants.AppOptionsMultiWindowEnabledKey] = _multiWindowEnabled;
			}
		}

		public string OriginStore
		{
			get
			{
				return _originStore;
			}
			set
			{
				_originStore = value;
				_data[Constants.AppOptionsOriginStoreKey] = _originStore;
			}
		}

		public bool TestModeEnabled
		{
			get
			{
				return _testModeEnabled;
			}
			set
			{
				_testModeEnabled = value;
				_data[Constants.AppOptionsTestModeKey] = _testModeEnabled;
			}
		}

		public bool GdprRequired
		{
			get
			{
				return _gdprRequired;
			}
			set
			{
				_gdprRequired = value;
				_data[Constants.AppOptionsGdprRequiredKey] = _gdprRequired;
			}
		}

		public string GdprConsentString
		{
			get
			{
				return _gdprConsentString;
			}
			set
			{
				_gdprConsentString = value;
				_data[Constants.AppOptionsGdprConsentStringKey] = _gdprConsentString;
			}
		}

		public AppOptions()
		{
		}

		public AppOptions(Hashtable values)
			: base(values)
		{
			if (values != null)
			{
				_data = new Hashtable(values);
				if (values.ContainsKey(Constants.AppOptionsDisableLoggingKey))
				{
					_disableLogging = Convert.ToBoolean(values[Constants.AppOptionsDisableLoggingKey]);
				}
				if (values.ContainsKey(Constants.AppOptionsUserIdKey))
				{
					_userId = values[Constants.AppOptionsUserIdKey] as string;
				}
				if (values.ContainsKey(Constants.AppOptionsOrientationKey))
				{
					_adOrientation = (AdOrientationType)Convert.ToInt32(values[Constants.AppOptionsOrientationKey]);
				}
				if (values.ContainsKey(Constants.AppOptionsMultiWindowEnabledKey))
				{
					_multiWindowEnabled = Convert.ToBoolean(values[Constants.AppOptionsMultiWindowEnabledKey]);
				}
				if (values.ContainsKey(Constants.AppOptionsOriginStoreKey))
				{
					_originStore = values[Constants.AppOptionsOriginStoreKey] as string;
				}
				if (values.ContainsKey(Constants.AppOptionsGdprRequiredKey))
				{
					_gdprRequired = Convert.ToBoolean(values[Constants.AppOptionsGdprRequiredKey]);
				}
				if (values.ContainsKey(Constants.AppOptionsGdprConsentStringKey))
				{
					_gdprConsentString = values[Constants.AppOptionsGdprConsentStringKey] as string;
				}
			}
		}
	}
}
