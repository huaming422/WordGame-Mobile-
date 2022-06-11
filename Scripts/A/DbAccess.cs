//using System;
//using System.Collections.Generic;
//using Mono.Data.Sqlite;
//using UnityEngine;

//public class DbAccess : SingleSysObj<DbAccess>
//{
//	private SqliteConnection dbConnection;

//	private SqliteCommand dbCommand;

//	private SqliteDataReader reader;

//	public void OpenDB(string connectionString)
//	{
//		try
//		{
//			dbConnection = new SqliteConnection(connectionString);
//			dbConnection.Open();
//			Debug.Log("Connected to db");
//		}
//		catch (Exception ex)
//		{
//			string message = ex.ToString();
//			Debug.Log(message);
//		}
//	}

//	public void CloseSqlConnection()
//	{
//		if (dbCommand != null)
//		{
//			dbCommand.Dispose();
//		}
//		dbCommand = null;
//		if (reader != null)
//		{
//			reader.Dispose();
//		}
//		reader = null;
//		if (dbConnection != null)
//		{
//			dbConnection.Close();
//		}
//		dbConnection = null;
//		Debug.Log("Disconnected from db.");
//	}

//	public SqliteDataReader ExecuteQuery(string sqlQuery)
//	{
//		dbCommand = dbConnection.CreateCommand();
//		dbCommand.CommandText = sqlQuery;
//		reader = dbCommand.ExecuteReader();
//		return reader;
//	}

//	public SqliteDataReader ReadFullTable(string tableName)
//	{
//		string sqlQuery = "SELECT * FROM " + tableName;
//		return ExecuteQuery(sqlQuery);
//	}

//	public SqliteDataReader InsertInto(string tableName, string[] values)
//	{
//		string text = "INSERT INTO " + tableName + " VALUES ('" + values[0];
//		for (int i = 1; i < values.Length; i++)
//		{
//			text = text + "', '" + values[i];
//		}
//		text += "')";
//		return ExecuteQuery(text);
//	}

//	public void InsertIntoTask(string tableName, List<string[]> values)
//	{
//		dbCommand = dbConnection.CreateCommand();
//		SqliteTransaction sqliteTransaction = dbConnection.BeginTransaction();
//		try
//		{
//			for (int i = 0; i < values.Count; i++)
//			{
//				string insetInfoQuery = GetInsetInfoQuery(tableName, values[i]);
//				dbCommand.CommandText = insetInfoQuery;
//				dbCommand.ExecuteNonQuery();
//			}
//			sqliteTransaction.Commit();
//		}
//		catch (Exception ex)
//		{
//			sqliteTransaction.Rollback();
//			Debug.LogWarning("InsertIntoTask error:" + ex.ToString());
//		}
//	}

//	public string GetInsetInfoQuery(string tableName, string[] values)
//	{
//		string text = "INSERT INTO " + tableName + " VALUES ('" + values[0];
//		for (int i = 1; i < values.Length; i++)
//		{
//			text = text + "', '" + values[i];
//		}
//		return text + "')";
//	}

//	public SqliteDataReader UpdateInto(string tableName, string[] cols, string[] colsvalues, string selectkey, string selectvalue)
//	{
//		string text = "UPDATE " + tableName + " SET " + cols[0] + " = " + colsvalues[0];
//		string text2;
//		for (int i = 1; i < colsvalues.Length; i++)
//		{
//			text2 = text;
//			text = text2 + ", " + cols[i] + " =" + colsvalues[i];
//		}
//		text2 = text;
//		text = text2 + " WHERE " + selectkey + " = " + selectvalue + " ";
//		return ExecuteQuery(text);
//	}

//	public SqliteDataReader Delete(string tableName, string[] cols, string[] colsvalues)
//	{
//		string text = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + colsvalues[0];
//		for (int i = 1; i < colsvalues.Length; i++)
//		{
//			string text2 = text;
//			text = text2 + " or " + cols[i] + " = " + colsvalues[i];
//		}
//		Debug.Log(text);
//		return ExecuteQuery(text);
//	}

//	public SqliteDataReader InsertIntoSpecific(string tableName, string[] cols, string[] values)
//	{
//		if (cols.Length != values.Length)
//		{
//			throw new SqliteException("columns.Length != values.Length");
//		}
//		string text = "INSERT INTO " + tableName + "(" + cols[0];
//		for (int i = 1; i < cols.Length; i++)
//		{
//			text = text + ", " + cols[i];
//		}
//		text = text + ") VALUES (" + values[0];
//		for (int j = 1; j < values.Length; j++)
//		{
//			text = text + ", " + values[j];
//		}
//		text += ")";
//		return ExecuteQuery(text);
//	}

//	public SqliteDataReader DeleteContents(string tableName)
//	{
//		string sqlQuery = "DELETE FROM " + tableName;
//		return ExecuteQuery(sqlQuery);
//	}

//	public void DeleteTable(string tableName)
//	{
//		string sqlQuery = "DROP TABLE IF EXISTS " + tableName;
//		ExecuteQuery(sqlQuery);
//	}

//	public SqliteDataReader CreateTable(string name, string[] col, string[] colType)
//	{
//		if (col.Length != colType.Length)
//		{
//			throw new SqliteException("columns.Length != colType.Length");
//		}
//		string text = "CREATE TABLE  IF NOT EXISTS " + name + " (" + col[0] + " " + colType[0];
//		for (int i = 1; i < col.Length; i++)
//		{
//			string text2 = text;
//			text = text2 + ", " + col[i] + " " + colType[i];
//		}
//		text += ")";
//		return ExecuteQuery(text);
//	}

//	public SqliteDataReader SelectWhere(string tableName, string[] items, string[] col, string[] operation, string[] values)
//	{
//		if (col.Length != operation.Length || operation.Length != values.Length)
//		{
//			throw new SqliteException("col.Length != operation.Length != values.Length");
//		}
//		string text = "SELECT " + items[0];
//		for (int i = 1; i < items.Length; i++)
//		{
//			text = text + ", " + items[i];
//		}
//		string text2 = text;
//		text = text2 + " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";
//		for (int j = 1; j < col.Length; j++)
//		{
//			text2 = text;
//			text = text2 + " AND " + col[j] + operation[j] + "'" + values[0] + "' ";
//		}
//		return ExecuteQuery(text);
//	}

//	public SqliteDataReader SelectAllWhere(string tableName, string[] col, string[] operation, string[] values)
//	{
//		if (col.Length != operation.Length || operation.Length != values.Length)
//		{
//			throw new SqliteException("col.Length != operation.Length != values.Length");
//		}
//		string text = "SELECT *";
//		string text2 = text;
//		text = text2 + " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";
//		for (int i = 1; i < col.Length; i++)
//		{
//			text2 = text;
//			text = text2 + " AND " + col[i] + operation[i] + "'" + values[0] + "' ";
//		}
//		return ExecuteQuery(text);
//	}
//}
