using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace AdColony
{
	public class AdColonyJson
	{
		private enum JsonToken
		{
			NONE,
			CURLY_OPEN,
			CURLY_CLOSE,
			SQUARED_OPEN,
			SQUARED_CLOSE,
			COLON,
			COMMA,
			STRING,
			NUMBER,
			TRUE,
			FALSE,
			NULL
		}

		private const int BUILDER_CAPACITY = 2000;

		private static AdColonyJson instance = new AdColonyJson();

		private int lastErrorIndex = -1;

		private string lastDecode = string.Empty;

		public static object Decode(string json)
		{
			if (json == null)
			{
				return null;
			}
			instance.lastDecode = json;
			object result = null;
			try
			{
				char[] json2 = json.ToCharArray();
				int index = 0;
				bool success = true;
				result = instance.ParseValue(json2, ref index, ref success);
				if (success)
				{
					instance.lastErrorIndex = -1;
					return result;
				}
				instance.lastErrorIndex = index;
				return result;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return result;
			}
		}

		public static string Encode(object json)
		{
			if (json == null)
			{
				return null;
			}
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder(2000);
			try
			{
				flag = instance.SerializeValue(json, stringBuilder);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
			return (!flag) ? null : stringBuilder.ToString();
		}

		public static bool LastDecodeSuccessful()
		{
			return instance.lastErrorIndex == -1;
		}

		public static int GetLastErrorIndex()
		{
			return instance.lastErrorIndex;
		}

		public static string GetLastErrorSnippet()
		{
			if (instance.lastErrorIndex == -1)
			{
				return string.Empty;
			}
			int num = instance.lastErrorIndex - 5;
			int num2 = instance.lastErrorIndex + 15;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 >= instance.lastDecode.Length)
			{
				num2 = instance.lastDecode.Length - 1;
			}
			return instance.lastDecode.Substring(num, num2 - num + 1);
		}

		private Hashtable ParseObject(char[] json, ref int index)
		{
			Hashtable hashtable = new Hashtable();
			NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				switch (LookAhead(json, index))
				{
				case JsonToken.NONE:
					return null;
				case JsonToken.COMMA:
					NextToken(json, ref index);
					continue;
				case JsonToken.CURLY_CLOSE:
					NextToken(json, ref index);
					return hashtable;
				}
				string text = ParseString(json, ref index);
				if (text == null)
				{
					return null;
				}
				JsonToken jsonToken = NextToken(json, ref index);
				if (jsonToken != JsonToken.COLON)
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
			return hashtable;
		}

		private ArrayList ParseArray(char[] json, ref int index)
		{
			ArrayList arrayList = new ArrayList();
			NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				switch (LookAhead(json, index))
				{
				case JsonToken.NONE:
					return null;
				case JsonToken.COMMA:
					NextToken(json, ref index);
					continue;
				case JsonToken.SQUARED_CLOSE:
					break;
				default:
				{
					bool success = true;
					object value = ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}
					arrayList.Add(value);
					continue;
				}
				}
				NextToken(json, ref index);
				break;
			}
			return arrayList;
		}

		private object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (LookAhead(json, index))
			{
			case JsonToken.STRING:
				return ParseString(json, ref index);
			case JsonToken.NUMBER:
				return ParseNumber(json, ref index);
			case JsonToken.CURLY_OPEN:
				return ParseObject(json, ref index);
			case JsonToken.SQUARED_OPEN:
				return ParseArray(json, ref index);
			case JsonToken.TRUE:
				NextToken(json, ref index);
				return true;
			case JsonToken.FALSE:
				NextToken(json, ref index);
				return false;
			case JsonToken.NULL:
				NextToken(json, ref index);
				return null;
			default:
				success = false;
				return null;
			}
		}

		private string ParseString(char[] json, ref int index)
		{
			string text = string.Empty;
			EatWhitespace(json, ref index);
			char c = json[index++];
			bool flag = false;
			while (!flag && index != json.Length)
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
						default:
							continue;
						}
						int num = json.Length - index;
						if (num >= 4)
						{
							char[] array = new char[4];
							Array.Copy(json, index, array, 0, 4);
							text += char.ConvertFromUtf32(int.Parse(new string(array), NumberStyles.HexNumber));
							index += 4;
							continue;
						}
					}
					break;
				default:
					text += c;
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

		private double ParseNumber(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);
			int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
			int num = lastIndexOfNumber - index + 1;
			char[] array = new char[num];
			Array.Copy(json, index, array, 0, num);
			index = lastIndexOfNumber + 1;
			return double.Parse(new string(array));
		}

		private int GetLastIndexOfNumber(char[] json, int index)
		{
			int i;
			for (i = index; i < json.Length && "0123456789+-.eE".IndexOf(json[i]) != -1; i++)
			{
			}
			return i - 1;
		}

		private void EatWhitespace(char[] json, ref int index)
		{
			while (index < json.Length && " \t\n\r".IndexOf(json[index]) != -1)
			{
				index++;
			}
		}

		private JsonToken LookAhead(char[] json, int index)
		{
			int index2 = index;
			return NextToken(json, ref index2);
		}

		private JsonToken NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);
			if (index == json.Length)
			{
				return JsonToken.NONE;
			}
			char c = json[index];
			index++;
			switch (c)
			{
			case '{':
				return JsonToken.CURLY_OPEN;
			case '}':
				return JsonToken.CURLY_CLOSE;
			case '[':
				return JsonToken.SQUARED_OPEN;
			case ']':
				return JsonToken.SQUARED_CLOSE;
			case ',':
				return JsonToken.COMMA;
			case '"':
				return JsonToken.STRING;
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
				return JsonToken.NUMBER;
			case ':':
				return JsonToken.COLON;
			default:
			{
				index--;
				int num = json.Length - index;
				if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
				{
					index += 5;
					return JsonToken.FALSE;
				}
				if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
				{
					index += 4;
					return JsonToken.TRUE;
				}
				if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
				{
					index += 4;
					return JsonToken.NULL;
				}
				return JsonToken.NONE;
			}
			}
		}

		private bool SerializeObjectOrArray(object objectOrArray, StringBuilder builder)
		{
			if (objectOrArray is Hashtable)
			{
				return SerializeObject((Hashtable)objectOrArray, builder);
			}
			if (objectOrArray is ArrayList)
			{
				return SerializeArray((ArrayList)objectOrArray, builder);
			}
			return false;
		}

		private bool SerializeObject(Hashtable anObject, StringBuilder builder)
		{
			builder.Append("{");
			IDictionaryEnumerator enumerator = anObject.GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext())
			{
				string aString = enumerator.Key.ToString();
				object value = enumerator.Value;
				if (!flag)
				{
					builder.Append(", ");
				}
				SerializeString(aString, builder);
				builder.Append(":");
				if (!SerializeValue(value, builder))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("}");
			return true;
		}

		private bool SerializeArray(ArrayList anArray, StringBuilder builder)
		{
			builder.Append("[");
			bool flag = true;
			for (int i = 0; i < anArray.Count; i++)
			{
				object value = anArray[i];
				if (!flag)
				{
					builder.Append(", ");
				}
				if (!SerializeValue(value, builder))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("]");
			return true;
		}

		private bool SerializeValue(object value, StringBuilder builder)
		{
			if (value == null)
			{
				builder.Append("null");
			}
			else if (value.GetType().IsArray)
			{
				SerializeArray(new ArrayList((ICollection)value), builder);
			}
			else if (value is string)
			{
				SerializeString((string)value, builder);
			}
			else if (value is char)
			{
				SerializeString(Convert.ToString((char)value), builder);
			}
			else if (value is Hashtable)
			{
				SerializeObject((Hashtable)value, builder);
			}
			else if (value is ArrayList)
			{
				SerializeArray((ArrayList)value, builder);
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

		private void SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");
			char[] array = aString.ToCharArray();
			foreach (char c in array)
			{
				switch (c)
				{
				case '"':
					builder.Append("\\\"");
					continue;
				case '\\':
					builder.Append("\\\\");
					continue;
				case '\b':
					builder.Append("\\b");
					continue;
				case '\f':
					builder.Append("\\f");
					continue;
				case '\n':
					builder.Append("\\n");
					continue;
				case '\r':
					builder.Append("\\r");
					continue;
				case '\t':
					builder.Append("\\t");
					continue;
				}
				int num = Convert.ToInt32(c);
				if (num >= 32 && num <= 126)
				{
					builder.Append(c);
				}
				else
				{
					builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
				}
			}
			builder.Append("\"");
		}

		private void SerializeNumber(double number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number));
		}
	}
}
