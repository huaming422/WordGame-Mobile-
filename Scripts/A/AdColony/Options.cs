using System;
using System.Collections;
using UnityEngine;

namespace AdColony
{
	public class Options
	{
		public UserMetadata Metadata;

		protected Hashtable _data = new Hashtable();

		public Options()
		{
		}

		public Options(Hashtable values)
		{
			_data = new Hashtable(values);
			if (values.ContainsKey(Constants.OptionsMetadataKey))
			{
				Hashtable values2 = values[Constants.OptionsMetadataKey] as Hashtable;
				Metadata = new UserMetadata(values2);
				_data.Remove(Constants.OptionsMetadataKey);
			}
		}

		public void SetOption(string key, string value)
		{
			if (key == null)
			{
				Debug.Log("Invalid option key.");
			}
			else if (value == null)
			{
				Debug.Log("Invalid option value.");
			}
			else
			{
				_data[key] = value;
			}
		}

		public void SetOption(string key, int value)
		{
			if (key == null)
			{
				Debug.Log("Invalid option key.");
			}
			else
			{
				_data[key] = value;
			}
		}

		public void SetOption(string key, double value)
		{
			if (key == null)
			{
				Debug.Log("Invalid option key.");
			}
			else
			{
				_data[key] = value;
			}
		}

		public void SetOption(string key, bool value)
		{
			if (key == null)
			{
				Debug.Log("Invalid option key.");
			}
			else
			{
				_data[key] = value;
			}
		}

		public string GetStringOption(string key)
		{
			return (!_data.ContainsKey(key)) ? null : (_data[key] as string);
		}

		public int GetIntOption(string key)
		{
			return _data.ContainsKey(key) ? Convert.ToInt32(_data[key]) : 0;
		}

		public double GetDoubleOption(string key)
		{
			return (!_data.ContainsKey(key)) ? 0.0 : Convert.ToDouble(_data[key]);
		}

		public bool GetBoolOption(string key)
		{
			return _data.ContainsKey(key) && Convert.ToBoolean(_data[key]);
		}

		public Hashtable ToHashtable()
		{
			Hashtable hashtable = new Hashtable(_data);
			if (Metadata != null)
			{
				Hashtable value = Metadata.ToHashtable();
				hashtable[Constants.OptionsMetadataKey] = value;
			}
			return hashtable;
		}

		public string ToJsonString()
		{
			Hashtable json = ToHashtable();
			return AdColonyJson.Encode(json);
		}
	}
}
