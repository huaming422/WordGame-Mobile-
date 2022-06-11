using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public class MiniJSON
{
	private const string INDENT = "\t";

	private const int TOKEN_NONE = 0;

	private const int TOKEN_CURLY_OPEN = 1;

	private const int TOKEN_CURLY_CLOSE = 2;

	private const int TOKEN_SQUARED_OPEN = 3;

	private const int TOKEN_SQUARED_CLOSE = 4;

	private const int TOKEN_COLON = 5;

	private const int TOKEN_COMMA = 6;

	private const int TOKEN_STRING = 7;

	private const int TOKEN_NUMBER = 8;

	private const int TOKEN_TRUE = 9;

	private const int TOKEN_FALSE = 10;

	private const int TOKEN_NULL = 11;

	private const int BUILDER_CAPACITY = 2000;

	protected static int _lastErrorIndex = -1;

	protected static string _lastDecode = string.Empty;

	public static object JsonDecode(string json)
	{
		_lastDecode = json;
		char[] json2 = json.ToCharArray();
		int index = 0;
		bool success = true;
		object result = ParseValue(json2, ref index, ref success);
		if (success)
		{
			_lastErrorIndex = -1;
		}
		else
		{
			_lastErrorIndex = index;
		}
		return result;
	}

	public static string JsonEncode(object json, bool indent = false, bool sortKey = false)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		return (!SerializeValue(json, stringBuilder, 0, indent, sortKey)) ? null : stringBuilder.ToString();
	}

	public static bool LastDecodeSuccessful()
	{
		return _lastErrorIndex == -1;
	}

	public static int GetLastErrorIndex()
	{
		return _lastErrorIndex;
	}

	public static string GetLastErrorSnippet()
	{
		if (_lastErrorIndex == -1)
		{
			return string.Empty;
		}
		int num = _lastErrorIndex - 5;
		int num2 = _lastErrorIndex + 15;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 >= _lastDecode.Length)
		{
			num2 = _lastDecode.Length - 1;
		}
		return _lastDecode.Substring(num, num2 - num + 1);
	}

	protected static Hashtable ParseObject(char[] json, ref int index)
	{
		Hashtable hashtable = new Hashtable();
		NextToken(json, ref index);
		while (true)
		{
			switch (LookAhead(json, index))
			{
			case 0:
				return null;
			case 6:
				NextToken(json, ref index);
				continue;
			case 2:
				NextToken(json, ref index);
				return hashtable;
			}
			string text = ParseString(json, ref index);
			if (text == null)
			{
				return null;
			}
			int num = NextToken(json, ref index);
			if (num != 5)
			{
				return null;
			}
			bool success = true;
			object value = ParseValue(json, ref index, ref success);
			if (!success)
			{
				return null;
			}
			hashtable[text] = value;
		}
	}

	protected static ArrayList ParseArray(char[] json, ref int index)
	{
		ArrayList arrayList = new ArrayList();
		NextToken(json, ref index);
		while (true)
		{
			switch (LookAhead(json, index))
			{
			case 0:
				return null;
			case 6:
				NextToken(json, ref index);
				continue;
			case 4:
				NextToken(json, ref index);
				return arrayList;
			}
			bool success = true;
			object value = ParseValue(json, ref index, ref success);
			if (!success)
			{
				return null;
			}
			arrayList.Add(value);
		}
	}

	protected static object ParseValue(char[] json, ref int index, ref bool success)
	{
		switch (LookAhead(json, index))
		{
		case 7:
			return ParseString(json, ref index);
		case 8:
			return ParseNumber(json, ref index);
		case 1:
			return ParseObject(json, ref index);
		case 3:
			return ParseArray(json, ref index);
		case 9:
			NextToken(json, ref index);
			return bool.Parse("TRUE");
		case 10:
			NextToken(json, ref index);
			return bool.Parse("FALSE");
		case 11:
			NextToken(json, ref index);
			return null;
		default:
			success = false;
			return null;
		}
	}

	protected static string ParseString(char[] json, ref int index)
	{
		string text = string.Empty;
		EatWhitespace(json, ref index);
		char c = json[index++];
		bool flag = false;
		while (index != json.Length)
		{
			c = json[index++];
			switch (c)
			{
			case '"':
				flag = true;
				break;
			case '\\':
				if (index != json.Length)
				{
					switch (json[index++])
					{
					default:
						continue;
					case '"':
						text += '"';
						continue;
					case '\\':
						text += '\\';
						continue;
					case '/':
						text += '/';
						continue;
					case 'b':
						text += '\b';
						continue;
					case 'f':
						text += '\f';
						continue;
					case 'n':
						text += '\n';
						continue;
					case 'r':
						text += '\r';
						continue;
					case 't':
						text += '\t';
						continue;
					case 'u':
						break;
					}
					int num = json.Length - index;
					if (num >= 4)
					{
						char[] array = new char[4];
						Array.Copy(json, index, array, 0, 4);
						text = text + "&#x" + new string(array) + ";";
						index += 4;
						continue;
					}
				}
				break;
			default:
				text += c.ToString(CultureInfo.InvariantCulture);
				continue;
			}
			break;
		}
		if (!flag)
		{
			return null;
		}
		return text;
	}

	protected static object ParseNumber(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);
		int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
		int num = lastIndexOfNumber - index + 1;
		char[] array = new char[num];
		Array.Copy(json, index, array, 0, num);
		index = lastIndexOfNumber + 1;
		string text = new string(array);
		if (text.Contains("."))
		{
			return double.Parse(text, CultureInfo.InvariantCulture);
		}
		return long.Parse(text, CultureInfo.InvariantCulture);
	}

	protected static int GetLastIndexOfNumber(char[] json, int index)
	{
		int i;
		for (i = index; i < json.Length && "0123456789+-.eE".IndexOf(json[i]) != -1; i++)
		{
		}
		return i - 1;
	}

	protected static void EatWhitespace(char[] json, ref int index)
	{
		while (index < json.Length && " \t\n\r".IndexOf(json[index]) != -1)
		{
			index++;
		}
	}

	protected static int LookAhead(char[] json, int index)
	{
		int index2 = index;
		return NextToken(json, ref index2);
	}

	protected static int NextToken(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);
		if (index == json.Length)
		{
			return 0;
		}
		char c = json[index];
		index++;
		switch (c)
		{
		case '{':
			return 1;
		case '}':
			return 2;
		case '[':
			return 3;
		case ']':
			return 4;
		case ',':
			return 6;
		case '"':
			return 7;
		case '-':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return 8;
		case ':':
			return 5;
		default:
		{
			index--;
			int num = json.Length - index;
			if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
			{
				index += 5;
				return 10;
			}
			if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
			{
				index += 4;
				return 9;
			}
			if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
			{
				index += 4;
				return 11;
			}
			return 0;
		}
		}
	}

	protected static bool SerializeObject(IDictionary anObject, StringBuilder builder, int lv, bool isIndent, bool sortKey)
	{
		string text = Join("\t", lv);
		string value = text + "\t";
		builder.Append("{");
		if (anObject.Count > 0)
		{
			if (isIndent)
			{
				builder.AppendLine();
				builder.Append(value);
			}
			bool flag = true;
			ICollection collection;
			if (sortKey)
			{
				ArrayList arrayList = new ArrayList(anObject);
				arrayList.Sort(new DictionaryEntryComparer());
				collection = arrayList;
			}
			else
			{
				collection = anObject;
			}
			IEnumerator enumerator = collection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					string aString = dictionaryEntry.Key.ToString();
					object value2 = dictionaryEntry.Value;
					if (!flag)
					{
						builder.Append(",");
						if (isIndent)
						{
							builder.AppendLine();
							builder.Append(value);
						}
						else
						{
							builder.Append(" ");
						}
					}
					SerializeString(aString, builder);
					builder.Append(":");
					if (isIndent)
					{
						builder.Append(" ");
					}
					if (!SerializeValue(value2, builder, lv + 1, isIndent, sortKey))
					{
						return false;
					}
					flag = false;
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
			if (isIndent)
			{
				builder.AppendLine();
				builder.Append(text);
			}
		}
		builder.Append("}");
		return true;
	}

	protected static bool SerializeDictionary(Dictionary<string, string> dict, StringBuilder builder, int lv, bool isIndent, bool sortKey)
	{
		string text = Join("\t", lv);
		string value = text + "\t";
		builder.Append("{");
		if (dict.Count > 0)
		{
			if (isIndent)
			{
				builder.AppendLine();
				builder.Append(value);
			}
			bool flag = true;
			ICollection<KeyValuePair<string, string>> collection;
			if (sortKey)
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(dict);
				list.Sort(new KeyValuePairComparer());
				collection = list;
			}
			else
			{
				collection = dict;
			}
			foreach (KeyValuePair<string, string> item in collection)
			{
				if (!flag)
				{
					builder.Append(",");
					if (isIndent)
					{
						builder.AppendLine();
						builder.Append(value);
					}
					else
					{
						builder.Append(" ");
					}
				}
				SerializeString(item.Key, builder);
				builder.Append(":");
				if (isIndent)
				{
					builder.Append(" ");
				}
				SerializeString(item.Value, builder);
				flag = false;
			}
			if (isIndent)
			{
				builder.AppendLine();
				builder.Append(text);
			}
		}
		builder.Append("}");
		return true;
	}

	public static string Join(string s, int n)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < n; i++)
		{
			stringBuilder.Append(s);
		}
		return stringBuilder.ToString();
	}

	protected static bool SerializeArray(IList anArray, StringBuilder builder, int lv, bool isIndent, bool sortKey)
	{
		string text = Join("\t", lv);
		string value = text + "\t";
		builder.Append("[");
		if (anArray.Count > 0)
		{
			if (isIndent)
			{
				builder.AppendLine();
				builder.Append(value);
			}
			bool flag = true;
			IEnumerator enumerator = anArray.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (!flag)
					{
						builder.Append(",");
						if (isIndent)
						{
							builder.AppendLine();
							builder.Append(value);
						}
						else
						{
							builder.Append(" ");
						}
					}
					if (!SerializeValue(current, builder, lv + 1, isIndent, sortKey))
					{
						return false;
					}
					flag = false;
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
			if (isIndent)
			{
				builder.AppendLine();
				builder.Append(text);
			}
		}
		builder.Append("]");
		return true;
	}

	protected static bool SerializeValue(object value, StringBuilder builder, int lv, bool isIndent, bool sortKey)
	{
		if (value == null)
		{
			builder.Append("null");
		}
		else if (value.GetType().IsArray)
		{
			SerializeArray(new ArrayList((ICollection)value), builder, lv, isIndent, sortKey);
		}
		else if (value is string)
		{
			SerializeString((string)value, builder);
		}
		else if (value.GetType().IsEnum)
		{
			SerializeNumber(Convert.ToInt32(value), builder);
		}
		else if (value is char)
		{
			SerializeString(Convert.ToString((char)value), builder);
		}
		else if (value is Dictionary<string, string>)
		{
			SerializeDictionary((Dictionary<string, string>)value, builder, lv, isIndent, sortKey);
		}
		else if (value is IDictionary)
		{
			SerializeObject((IDictionary)value, builder, lv, isIndent, sortKey);
		}
		else if (value is IList)
		{
			SerializeArray((IList)value, builder, lv, isIndent, sortKey);
		}
		else if (value is bool && (bool)value)
		{
			builder.Append("true");
		}
		else if (value is bool && !(bool)value)
		{
			builder.Append("false");
		}
		else
		{
			if (!value.GetType().IsPrimitive)
			{
				return false;
			}
			SerializeNumber(Convert.ToDouble(value), builder);
		}
		return true;
	}

	protected static void SerializeString(string aString, StringBuilder builder)
	{
		builder.Append("\"");
		char[] array = aString.ToCharArray();
		char[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			char c = array2[i];
			switch (c)
			{
			case '"':
				builder.Append("\\\"");
				break;
			case '\\':
				builder.Append("\\\\");
				break;
			case '\b':
				builder.Append("\\b");
				break;
			case '\f':
				builder.Append("\\f");
				break;
			case '\n':
				builder.Append("\\n");
				break;
			case '\r':
				builder.Append("\\r");
				break;
			case '\t':
				builder.Append("\\t");
				break;
			default:
				builder.Append(c.ToString(CultureInfo.InvariantCulture));
				break;
			}
		}
		builder.Append("\"");
	}

	protected static void SerializeNumber(double number, StringBuilder builder)
	{
		builder.Append(number.ToString(CultureInfo.InvariantCulture));
	}

	public Hashtable LoadFormFile(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		FileStream fileStream = File.OpenRead(path);
		StreamReader streamReader = new StreamReader(fileStream);
		string json = streamReader.ReadToEnd();
		streamReader.Close();
		fileStream.Close();
		return json.DecodeJson();
	}
}
