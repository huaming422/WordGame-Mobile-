using System;
using UnityEngine;

namespace TapjoyUnity.Internal
{
	[Serializable]
	public class FyberSettings
	{
		[HideInInspector]
		[SerializeField]
		private string appId;

		[HideInInspector]
		[SerializeField]
		private string appToken;

		[HideInInspector]
		[SerializeField]
		private string userId;

		private bool dirty;

		public string AppId
		{
			get
			{
				return appId;
			}
			set
			{
				if (appId != value)
				{
					appId = value;
					dirty = true;
				}
			}
		}

		public string AppToken
		{
			get
			{
				return appToken;
			}
			set
			{
				if (appToken != value)
				{
					appToken = value;
					dirty = true;
				}
			}
		}

		public string UserId
		{
			get
			{
				return userId;
			}
			set
			{
				if (userId != value)
				{
					userId = value;
					dirty = true;
				}
			}
		}

		public bool Valid
		{
			get
			{
				return appId != null && !"".Equals(appId) && appToken != null && !"".Equals(appToken);
			}
		}

		public bool HasData
		{
			get
			{
				return (appId != null && !"".Equals(appId)) || (appToken != null && !"".Equals(appToken));
			}
		}

		public bool Dirty
		{
			get
			{
				return dirty;
			}
			set
			{
				dirty = value;
			}
		}
	}
}
