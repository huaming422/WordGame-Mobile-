using System;
using System.Collections;

namespace AdColony
{
	public class InterstitialAd
	{
		public string ZoneId;

		public bool Expired;

		public string Id;

		public InterstitialAd(Hashtable values)
		{
			UpdateValues(values);
		}

		public void UpdateValues(Hashtable values)
		{
			if (values != null)
			{
				if (values.ContainsKey("zone_id"))
				{
					ZoneId = values["zone_id"] as string;
				}
				if (values.ContainsKey("expired"))
				{
					Expired = Convert.ToBoolean(values["expired"]);
				}
				if (values.ContainsKey("id"))
				{
					Id = values["id"] as string;
				}
			}
		}

		~InterstitialAd()
		{
			if (IsValid())
			{
				Ads.SharedGameObject.EnqueueAction(delegate
				{
					Ads.DestroyAd(Id);
				});
			}
		}

		private bool IsValid()
		{
			return !string.IsNullOrEmpty(Id);
		}
	}
}
