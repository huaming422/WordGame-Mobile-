using System;
using System.Collections;

namespace AdColony
{
	public class AdOptions : Options
	{
		private bool _showPrePopup;

		private bool _showPostPopup;

		public bool ShowPrePopup
		{
			get
			{
				return _showPrePopup;
			}
			set
			{
				_showPrePopup = value;
				_data[Constants.AdOptionsPrePopupKey] = _showPrePopup;
			}
		}

		public bool ShowPostPopup
		{
			get
			{
				return _showPostPopup;
			}
			set
			{
				_showPostPopup = value;
				_data[Constants.AdOptionsPostPopupKey] = _showPostPopup;
			}
		}

		public AdOptions()
		{
		}

		public AdOptions(Hashtable values)
			: base(values)
		{
			if (values != null)
			{
				_data = new Hashtable(values);
				if (values.ContainsKey(Constants.AdOptionsPrePopupKey))
				{
					_showPrePopup = Convert.ToBoolean(values[Constants.AdOptionsPrePopupKey]);
				}
				if (values.ContainsKey(Constants.AdOptionsPostPopupKey))
				{
					_showPostPopup = Convert.ToBoolean(values[Constants.AdOptionsPostPopupKey]);
				}
			}
		}
	}
}
