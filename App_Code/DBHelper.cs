﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

public class DBHelper
{
	public const string SQLDateFormat = "yyyy-MM-dd HH:mm:ss";
	private const string g_connString = "Provider=SQLOLEDB;Data Source=192.168.0.1;Initial Catalog=TASKS;Persist Security Info=True;User ID=sa;Password=prosuite";
	private static OleDbConnection NewConnection
	{
		get { return new OleDbConnection(g_connString); }
	}

	public static object GetValue(string strSQL, OleDbParameter[] pars = null)
	{
		using (DataSet ds = GetDataSet(strSQL, pars))
		{
			if (ds.Tables.Count == 0)
				return null;
			if (ds.Tables[0].Rows.Count == 0)
				return null;
			if (ds.Tables[0].Columns.Count == 0)
				return null;
			return ds.Tables[0].Rows[0][0];
		}
	}
	public static DataSet GetDataSet(string strSQL, OleDbParameter[] pars = null)
	{
		DataSet ds = new DataSet();
		using (OleDbConnection conn = NewConnection)
		{
			conn.Open();
			using (OleDbDataAdapter adapter = new OleDbDataAdapter())
			{
				OleDbCommand comm = new OleDbCommand("RetrieveMyData", conn);
				adapter.SelectCommand = comm;
				if (pars != null)
				{
					foreach (OleDbParameter p in pars)
					{
						comm.Parameters.Add(p);
					}
				}
				adapter.SelectCommand.CommandText = strSQL;
				adapter.Fill(ds);
			}
		}
		return ds;
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
	public static void SQLExecute(string strSQL, OleDbParameter[] pars = null)
	{
		using (OleDbConnection conn = NewConnection)
		{
			conn.Open();
			using (OleDbCommand cmd = new OleDbCommand(strSQL, conn))
			{
				if (pars != null)
				{
					foreach (OleDbParameter p in pars)
					{
						cmd.Parameters.Add(p);
					}
				}
				cmd.ExecuteNonQuery();
			}
		}
	}
	public static DataRow GetValues(string strSQL)
	{
		using (DataSet ds = GetDataSet(strSQL))
		{
			if (ds.Tables.Count == 0)
				return null;
			if (ds.Tables[0].Rows.Count == 0)
				return null;
			if (ds.Tables[0].Columns.Count == 0)
				return null;
			return ds.Tables[0].Rows[0];
		}
	}
	public static DataRowCollection GetRows(string strSQL)
	{
		return GetDataSet(strSQL).Tables[0].Rows;
	}
}