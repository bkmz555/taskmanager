﻿using System;
using System.Collections.Generic;
using System.Linq;

public class DefectSeverity : Reference
{
	private const string _plan = "Planable";

	private const string _Tabl = "[TT_RES].[DBO].[FLDSEVER]";
	static string[] _allCols = _allBaseCols.Concat(new string[] { _plan }).ToArray();

	public bool PLAN
	{
		get
		{
			if (this[_plan] == DBNull.Value)
			{
				return false;
			}
			return Convert.ToBoolean(this[_plan]);
		}
		set { this[_plan] = value; }
	}
	public DefectSeverity()
		: base(_Tabl, _allCols, 0.ToString(), _ID, false)
	{
	}
	public DefectSeverity(object id)
		: base(_Tabl, _allCols, id.ToString(), _ID)
	{
	}
	public static List<DefectSeverity> Enum()
	{
		List<DefectSeverity> res = new List<DefectSeverity>();
		foreach (int i in EnumRecords(_Tabl, _ID))
		{
			res.Add(new DefectSeverity(i));
		}
		return res;
	}
	public static List<int> EnumPlanable()
	{
		List<int> res = new List<int>();
		foreach (int i in EnumRecords(_Tabl, _ID, new string[] { string.Format(" UPPER(LEFT({0}, 1)) ", _Desc) }, new object[] { "A" }))
		{
			res.Add(i);
		}
		return res;
	}
}