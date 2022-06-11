using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdColony
{
	public class UserMetadata
	{
		private int _age;

		private List<string> _interests;

		private string _gender;

		private double _latitude;

		private double _longitude;

		private string _zipCode;

		private int _householdIncome;

		private string _maritalStatus;

		private string _educationLevel;

		private Hashtable _data = new Hashtable();

		public int Age
		{
			get
			{
				return _age;
			}
			set
			{
				if (value <= 0)
				{
					Debug.Log("Tried to set user metadata age with an invalid value. Value will not be included.");
					return;
				}
				_age = value;
				_data[Constants.UserMetadataAgeKey] = _age;
			}
		}

		public List<string> Interests
		{
			get
			{
				return _interests;
			}
			set
			{
				_interests = value;
				_data[Constants.UserMetadataInterestsKey] = new ArrayList(_interests);
			}
		}

		public string Gender
		{
			get
			{
				return _gender;
			}
			set
			{
				if (value == null)
				{
					Debug.Log("Tried to set user metadata gender with an invalid string. Value will not be included.");
					return;
				}
				string text = (_gender = value);
				_data[Constants.UserMetadataGenderKey] = _gender;
			}
		}

		public double Latitude
		{
			get
			{
				return _latitude;
			}
			set
			{
				_latitude = value;
				_data[Constants.UserMetadataLatitudeKey] = _latitude;
			}
		}

		public double Longitude
		{
			get
			{
				return _longitude;
			}
			set
			{
				_longitude = value;
				_data[Constants.UserMetadataLongitudeKey] = _longitude;
			}
		}

		public string ZipCode
		{
			get
			{
				return _zipCode;
			}
			set
			{
				if (value == null)
				{
					Debug.Log("Tried to set user metadata zip code with an invalid string. Value will not be included.");
					return;
				}
				string text = (_zipCode = value);
				_data[Constants.UserMetadataZipCodeKey] = _zipCode;
			}
		}

		public int HouseholdIncome
		{
			get
			{
				return _householdIncome;
			}
			set
			{
				_householdIncome = value;
				_data[Constants.UserMetadataHouseholdIncomeKey] = _householdIncome;
			}
		}

		public string MaritalStatus
		{
			get
			{
				return _maritalStatus;
			}
			set
			{
				if (value == null)
				{
					Debug.Log("Tried to set user metadata marital status with an invalid string. Value will not be included.");
					return;
				}
				string text = (_maritalStatus = value);
				_data[Constants.UserMetadataMaritalStatusKey] = _maritalStatus;
			}
		}

		public string EducationLevel
		{
			get
			{
				return _educationLevel;
			}
			set
			{
				if (value == null)
				{
					Debug.Log("Tried to set user metadata education level with an invalid string. Value will not be included.");
					return;
				}
				string text = (_educationLevel = value);
				_data[Constants.UserMetadataEducationLevelKey] = _educationLevel;
			}
		}

		public UserMetadata()
		{
		}

		public UserMetadata(Hashtable values)
		{
			_data = new Hashtable(values);
			if (values == null)
			{
				return;
			}
			if (values.ContainsKey(Constants.UserMetadataAgeKey))
			{
				_age = Convert.ToInt32(values[Constants.UserMetadataAgeKey]);
			}
			if (values.ContainsKey(Constants.UserMetadataInterestsKey))
			{
				ArrayList arrayList = values[Constants.UserMetadataInterestsKey] as ArrayList;
				Interests = new List<string>();
				IEnumerator enumerator = arrayList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string item = (string)enumerator.Current;
						Interests.Add(item);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = enumerator as IDisposable) != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (values.ContainsKey(Constants.UserMetadataGenderKey))
			{
				_gender = values[Constants.UserMetadataGenderKey] as string;
			}
			if (values.ContainsKey(Constants.UserMetadataLatitudeKey))
			{
				_latitude = Convert.ToDouble(values[Constants.UserMetadataLatitudeKey]);
			}
			if (values.ContainsKey(Constants.UserMetadataLongitudeKey))
			{
				_longitude = Convert.ToDouble(values[Constants.UserMetadataLongitudeKey]);
			}
			if (values.ContainsKey(Constants.UserMetadataZipCodeKey))
			{
				_zipCode = values[Constants.UserMetadataZipCodeKey] as string;
			}
			if (values.ContainsKey(Constants.UserMetadataHouseholdIncomeKey))
			{
				_householdIncome = Convert.ToInt32(values[Constants.UserMetadataHouseholdIncomeKey]);
			}
			if (values.ContainsKey(Constants.UserMetadataMaritalStatusKey))
			{
				_maritalStatus = values[Constants.UserMetadataMaritalStatusKey] as string;
			}
			if (values.ContainsKey(Constants.UserMetadataEducationLevelKey))
			{
				_educationLevel = values[Constants.UserMetadataEducationLevelKey] as string;
			}
		}

		public void SetMetadata(string key, string value)
		{
			if (key != null)
			{
				_data[key] = value;
			}
		}

		public void SetMetadata(string key, int value)
		{
			if (key != null)
			{
				_data[key] = value;
			}
		}

		public void SetMetadata(string key, double value)
		{
			if (key != null)
			{
				_data[key] = value;
			}
		}

		public void SetMetadata(string key, bool value)
		{
			if (key != null)
			{
				_data[key] = value;
			}
		}

		public string GetStringMetadata(string key)
		{
			return (!_data.ContainsKey(key)) ? null : (_data[key] as string);
		}

		public int GetIntMetadata(string key)
		{
			return _data.ContainsKey(key) ? Convert.ToInt32(_data[key]) : 0;
		}

		public double GetDoubleMetadata(string key)
		{
			return (!_data.ContainsKey(key)) ? 0.0 : Convert.ToDouble(_data[key]);
		}

		public bool GetBoolMetadata(string key)
		{
			return _data.ContainsKey(key) && Convert.ToBoolean(Convert.ToInt32(_data[key]));
		}

		public Hashtable ToHashtable()
		{
			return new Hashtable(_data);
		}

		public string ToJsonString()
		{
			return AdColonyJson.Encode(_data);
		}
	}
}
