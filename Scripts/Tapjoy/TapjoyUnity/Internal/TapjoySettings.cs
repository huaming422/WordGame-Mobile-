using UnityEngine;

namespace TapjoyUnity.Internal
{
	public class TapjoySettings : ScriptableObject
	{
		[HideInInspector]
		[SerializeField]
		private PlatformSettings androidSettings = new PlatformSettings();

		[HideInInspector]
		[SerializeField]
		private PlatformSettings iosSettings = new PlatformSettings();

		[HideInInspector]
		[SerializeField]
		private bool autoConnectEnabled = true;

		[HideInInspector]
		[SerializeField]
		private bool debugEnabled = false;

		[HideInInspector]
		[SerializeField]
		private TapjoyRuntimeCallbacks tjCallbacks;

		[HideInInspector]
		[SerializeField]
		private string hostURL;

		[HideInInspector]
		[SerializeField]
		private string eventURL;

		private bool dirty = false;

		public PlatformSettings AndroidSettings
		{
			get
			{
				return androidSettings;
			}
		}

		public PlatformSettings IosSettings
		{
			get
			{
				return iosSettings;
			}
		}

		public bool AutoConnectEnabled
		{
			get
			{
				return autoConnectEnabled;
			}
			set
			{
				if (autoConnectEnabled != value)
				{
					autoConnectEnabled = value;
					dirty = true;
				}
			}
		}

		public bool DebugEnabled
		{
			get
			{
				return debugEnabled;
			}
			set
			{
				if (debugEnabled != value)
				{
					debugEnabled = value;
					dirty = true;
				}
			}
		}

		public string HostURL
		{
			get
			{
				return hostURL;
			}
			set
			{
				if (hostURL != value)
				{
					hostURL = value;
					dirty = true;
				}
			}
		}

		public string EventURL
		{
			get
			{
				return eventURL;
			}
			set
			{
				if (eventURL != value)
				{
					eventURL = value;
					dirty = true;
				}
			}
		}

		public bool Dirty
		{
			get
			{
				return dirty || androidSettings.Dirty || iosSettings.Dirty;
			}
			set
			{
				dirty = value;
				androidSettings.Dirty = value;
				iosSettings.Dirty = value;
			}
		}
	}
}
