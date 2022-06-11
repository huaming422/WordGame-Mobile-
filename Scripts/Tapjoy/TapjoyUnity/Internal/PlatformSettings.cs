using System;
using UnityEngine;

namespace TapjoyUnity.Internal
{
	[Serializable]
	public class PlatformSettings
	{
		[SerializeField]
		private string sdkKey = string.Empty;

		[SerializeField]
		private string pushKey = string.Empty;

		[SerializeField]
		private bool disableAdvertisingId = false;

		[SerializeField]
		private bool disablePersistentIds = false;

		private bool dirty = false;

		[HideInInspector]
		[SerializeField]
		private FyberSettings fyberSettings = new FyberSettings();

		public FyberSettings FyberMediationSettings
		{
			get
			{
				return fyberSettings;
			}
		}

		public string SdkKey
		{
			get
			{
				return sdkKey;
			}
			set
			{
				if (sdkKey != value)
				{
					sdkKey = value;
					dirty = true;
				}
			}
		}

		public string PushKey
		{
			get
			{
				return pushKey;
			}
			set
			{
				if (pushKey != value)
				{
					pushKey = value;
					dirty = true;
				}
			}
		}

		public bool DisableAdvertisingId
		{
			get
			{
				return disableAdvertisingId;
			}
			set
			{
				if (disableAdvertisingId != value)
				{
					disableAdvertisingId = value;
					dirty = true;
				}
			}
		}

		public bool DisablePersistentIds
		{
			get
			{
				return disablePersistentIds;
			}
			set
			{
				if (disablePersistentIds != value)
				{
					disablePersistentIds = value;
					dirty = true;
				}
			}
		}

		public bool Valid
		{
			get
			{
				return sdkKey != "";
			}
		}

		public bool Dirty
		{
			get
			{
				return dirty || (fyberSettings != null && fyberSettings.Dirty);
			}
			set
			{
				dirty = value;
				if (fyberSettings != null)
				{
					fyberSettings.Dirty = value;
				}
			}
		}
	}
}
