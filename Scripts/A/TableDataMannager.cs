using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;

public class TableDataMannager
{
	private static TableDataMannager s_Instance = null;

	private static GameAsset configFile = new GameAsset("GameData/ExcleTable/", "Config");

	private Dictionary<string, object> readTableData = new Dictionary<string, object>();

	private XmlNodeList m_XmlTables;

	public static TableDataMannager instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new TableDataMannager();
			}
			return s_Instance;
		}
	}

	public void InitConfig()
	{
		TextAsset textAsset = SingleObject<ResourceManager>.instance.LoadAsset<TextAsset>(configFile);
		if (!(textAsset == null))
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			m_XmlTables = xmlDocument.GetElementsByTagName("table");
		}
	}

	public T GetTableOneRowData<T>(string tableName, Func<T, bool> find) where T : GameDataParent, new()
	{
		List<T> tableRowDatas = GetTableRowDatas(tableName, find);
		if (tableRowDatas.IsEmpty())
		{
			return (T)null;
		}
		return tableRowDatas[0];
	}

	public T GetTableOneRowData<T>(Func<T, bool> find) where T : GameDataParent, new()
	{
		string name = typeof(T).Name;
		List<T> tableRowDatas = GetTableRowDatas(name, find);
		if (tableRowDatas.IsEmpty())
		{
			return (T)null;
		}
		return tableRowDatas[0];
	}

	public List<T> GetTableRowDatas<T>(string tableName, Func<T, bool> find) where T : GameDataParent, new()
	{
		List<T> tableData = GetTableData<T>(tableName);
		if (tableData == null || find == null)
		{
			return null;
		}
		List<T> list = new List<T>();
		for (int i = 0; i < tableData.Count; i++)
		{
			if (find(tableData[i]))
			{
				list.Add(tableData[i]);
			}
		}
		return list;
	}

	public List<T> GetTableRowDatas<T>(Func<T, bool> find) where T : GameDataParent, new()
	{
		string name = typeof(T).Name;
		return GetTableRowDatas(name, find);
	}

	public List<T> GetTableRowDatas<T>() where T : GameDataParent, new()
	{
		string name = typeof(T).Name;
		return GetTableData<T>(name);
	}

	private XmlNodeList GetTableFieldList(string tableName)
	{
		if (m_XmlTables == null)
		{
			InitConfig();
			if (m_XmlTables == null)
			{
				Util.LogError("dont load tableConfig!!!!!!!");
				return null;
			}
		}
		XmlNodeList xmlTables = m_XmlTables;
		if (xmlTables == null)
		{
			return null;
		}
		XmlNode xmlNode = null;
		for (int i = 0; i < xmlTables.Count; i++)
		{
			if (xmlTables[i].Attributes["name"].Value == tableName)
			{
				xmlNode = xmlTables[i];
				break;
			}
		}
		if (xmlNode == null)
		{
			Util.LogWarning("cant load " + tableName + " xml config !!!!!!!");
			return null;
		}
		return xmlNode.ChildNodes;
	}

	private List<T> GetTableData<T>(string tableName) where T : GameDataParent, new()
	{
		object value;
		if (readTableData.TryGetValue(tableName, out value))
		{
			return value as List<T>;
		}
		XmlNodeList tableFieldList = GetTableFieldList(tableName);
		if (tableFieldList == null)
		{
			return null;
		}
		List<T> list = new List<T>();
		byte[] array = LoadTableData(tableName);
		if (array == null)
		{
			return null;
		}
		ByteBuffer byteBuffer = new ByteBuffer(array);
		int num = 1;
		bool flag = true;
		while (byteBuffer.readPos < byteBuffer.dataLength)
		{
			object obj = ReadRowData(byteBuffer, tableName, typeof(T), num);
			if (obj == null)
			{
				Util.LogWarning("Read table:" + tableName + " data error");
				flag = false;
				break;
			}
			T item = obj as T;
			list.Add(item);
			num++;
		}
		byteBuffer.Close();
		if (!flag)
		{
			return null;
		}
		readTableData.Add(tableName, list);
		return list;
	}

	private object ReadRowData(ByteBuffer reader, string tableName, Type tableClass, int row)
	{
		if (reader == null || tableClass == null)
		{
			return null;
		}
		XmlNodeList tableFieldList = GetTableFieldList(tableName);
		if (tableFieldList == null)
		{
			return null;
		}
		object obj = Activator.CreateInstance(tableClass);
		if (SetRowInstanceFieldVale(obj, tableClass, reader, tableFieldList, row))
		{
			return obj;
		}
		return null;
	}

	private bool SetRowInstanceFieldVale(object rowdata, Type tableClass, ByteBuffer reader, XmlNodeList tableFields, int row)
	{
		for (int i = 0; i < tableFields.Count; i++)
		{
			bool isArray = false;
			XmlNode xmlNode = tableFields[i];
			string value = xmlNode.Attributes["name"].Value;
			string value2 = xmlNode.Attributes["type"].Value;
			int num = ((xmlNode.Attributes["count"] == null) ? 1 : int.Parse(xmlNode.Attributes["count"].Value));
			FieldInfo field = tableClass.GetField(value, BindingFlags.Instance | BindingFlags.Public);
			if (field == null)
			{
				Util.LogWarning("dont find field:" + value + " whit class:" + tableClass.Name);
				return false;
			}
			if (num != 1)
			{
				isArray = true;
				num = reader.ReadShort();
			}
			object obj = ReadFieldData(value2, reader, num, row, isArray);
			if (obj == null)
			{
				Util.LogWarning("read field date error  field:" + value + " whit class:" + tableClass.Name + " at row:" + row);
				return false;
			}
			try
			{
				field.SetValue(rowdata, obj);
			}
			catch (Exception ex)
			{
				Util.LogWarning(ex.ToString());
				string text = obj.GetType().ToString();
				Util.LogWarning(string.Format("read field date type error  [field:{0}] with [class:{1}] at [row:{2}] [column:{3}] [value:{4}] [valueType:{5}] [fieldType:{6}]", value, tableClass.Name, row, i + 1, obj, text, value2));
				return false;
			}
		}
		return true;
	}

	private object ReadFieldData(string type, ByteBuffer reader, int count, int row, bool isArray)
	{
		if (SampleCodeTools.IsBaseType(type))
		{
			return ReadBaseFieldData(type, reader, count, isArray);
		}
		if (isArray)
		{
			return readArrayFiled(type, reader, count, row);
		}
		return ReadRowData(reader, type, Type.GetType(type), row);
	}

	private Array readArrayFiled(string type, ByteBuffer reader, int count, int row)
	{
		Type type2 = Type.GetType(type);
		if (type2 == null)
		{
			return null;
		}
		Array array = Array.CreateInstance(type2, count);
		for (int i = 0; i < count; i++)
		{
			object obj = ReadRowData(reader, type, type2, row);
			if (obj == null)
			{
				return null;
			}
			array.SetValue(obj, i);
		}
		return array;
	}

	private object ReadBaseFieldData(string type, ByteBuffer reader, int count, bool isArray)
	{
		try
		{
			switch (type)
			{
			case "float":
				if (isArray)
				{
					return ReadFloatArray(reader, count);
				}
				return reader.ReadFloat();
			case "int":
				if (isArray)
				{
					return ReadIntArray(reader, count);
				}
				return reader.ReadInt();
			case "string":
				if (isArray)
				{
					return ReadStringArray(reader, count);
				}
				return reader.ReadString();
			}
		}
		catch (Exception ex)
		{
			Util.LogWarning(ex.ToString());
		}
		return null;
	}

	private float[] ReadFloatArray(ByteBuffer bufferReader, int count)
	{
		float[] array = new float[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = bufferReader.ReadFloat();
		}
		return array;
	}

	private int[] ReadIntArray(ByteBuffer bufferReader, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = bufferReader.ReadInt();
		}
		return array;
	}

	private string[] ReadStringArray(ByteBuffer bufferReader, int count)
	{
		string[] array = new string[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = bufferReader.ReadString();
		}
		return array;
	}

	private static byte[] LoadTableData(string tableName)
	{
		TextAsset textAsset = SingleObject<ResourceManager>.instance.LoadAsset<TextAsset>(configFile.abName, tableName);
		if (textAsset == null)
		{
			return null;
		}
		return textAsset.bytes;
	}
}
