using System;
using System.Collections.Generic;

public class DisignDBManager : SingleObject<DisignDBManager>
{
	public void Init()
	{
		//SingleSysObj<DbAccess>.instance.OpenDB(SqliteDefine.connectString);
	}

	private void OnDestroy()
	{
		//SingleSysObj<DbAccess>.instance.CloseSqlConnection();
	}

	//private List<WordLine> DoQuery(string query)
	//{
		//SqliteDataReader sqliteDataReader = SingleSysObj<DbAccess>.instance.ExecuteQuery(query);
		//if (sqliteDataReader == null)
		//{
		//	return null;
		//}
		//List<WordLine> list = new List<WordLine>();
		//while (sqliteDataReader.Read())
		//{
		//	WordLine item = new WordLine(sqliteDataReader.GetString(1), sqliteDataReader.GetInt32(2), sqliteDataReader.GetString(3));
		//	list.Add(item);
		//}
		//return list;
	//}

	//public List<WordLine> GetLikeWorldLine(string likeContent)
	//{
	//	string query = "select * from " + SqliteDefine.tableName + " where word like '%" + likeContent + "%' limit 1000";
	//	//return DoQuery(query);
	//}

	//public List<WordLine> GetLikeWorldLineAndLength(string likeContent, int length)
	//{
	//	string query = "select * from " + SqliteDefine.tableName + " where word like '%" + likeContent + "%' and length = " + length + " limit 1000";
	//	//return DoQuery(query);
	//}

	public List<WordLine> GetWorldLineByLength(int length)
	{
		string query = "select * from " + SqliteDefine.tableName + " where length = " + length + " limit 1000";
		return DoQuery(query);
	}

    private List<WordLine> DoQuery(string query)
    {
        throw new NotImplementedException();
    }

    public List<WordLine> GetFilerSearchReult(string contans, string lengthMin, string lengthMax)
	{
		string text = "select * from " + SqliteDefine.tableName;
		string query = string.Empty;
		if (!string.IsNullOrEmpty(contans) && !string.IsNullOrEmpty(lengthMin))
		{
			query = ((!(lengthMin == lengthMax)) ? (text + " where word like '%" + contans + "%' and length >= " + lengthMin + " and length <= " + lengthMax) : (text + " where word like '%" + contans + "%' and length = " + lengthMin));
		}
		else if (string.IsNullOrEmpty(contans) && string.IsNullOrEmpty(lengthMin))
		{
			query = text;
		}
		else
		{
			if (!string.IsNullOrEmpty(contans))
			{
				query = text + " where word like '%" + contans + "%'";
			}
			if (!string.IsNullOrEmpty(lengthMin))
			{
				query = ((!(lengthMin == lengthMax)) ? (text + " where length >= " + lengthMin + " and length <= " + lengthMax) : (text + " where length = " + lengthMin));
			}
		}
		return DoQuery(query);
	}
}
