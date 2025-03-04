﻿using System;
using System.Collections.Generic;
using System.Data;

public class BuildRequest
{
	public int ID { get; set; }
	public int TTID { get; set; }
	public string SUMMARY { get; set; }
	public string USER { get; set; }
	public string COMM { get; set; }
	public string BRANCH { get; set; }
}

public class DefectBuild : IdBasedObject
{
	protected static string _pid = "idRecord";
	protected static string _par = "ParentID";
	protected static string _stat = "Status";
	protected static string _date = "DateTimeCreate";
	protected static string _dateUp = "DateTimeUpdate";
	protected static string _dateB = "DateTimeBuild";
	protected static string _mach = "Machine";
	protected static string _not = "Notes";
	protected static string _gui = "UGuid";
	protected static string _stText = "StatusText";
	protected static string _TTID = "TTID";
	protected static string _User = "UserID";
	protected static string _tguid = "TestGUID";
	protected static string _type = "BuildType";
	protected static string _Tabl = "[TT_RES].[dbo].[DefectBuild]";
	protected static string[] _allBasecols = new string[] { _pid, _par, _date, _stat, _dateUp, _mach, _not, _stText, _TTID, _User, _dateB, _tguid, _type };

	public int ID
	{
		get { return GetAsInt(_pid); }
		set { this[_pid] = value; }
	}
	public string DATE
	{
		get
		{
			return GetAsDateTime(_date, "");
		}
		set { this[_date] = value; }
	}
	public string TESTGUID
	{
		get { return this[_tguid].ToString(); }
		set { this[_tguid] = value; }
	}
	public string NOTES
	{
		get { return this[_not].ToString(); }
		set { this[_not] = value; }
	}
	public string MACHINE
	{
		get { return this[_mach].ToString(); }
		set { this[_mach] = value; }
	}
	public int DURATION
	{
		get
		{
			if (this[_dateB] == DBNull.Value)
				return 0;
			if (this[_dateUp] == DBNull.Value)
				return 0;
			DateTime d1 = Convert.ToDateTime(this[_dateB]);
			DateTime d2 = Convert.ToDateTime(this[_dateUp]);
			return (int)(d2 - d1).TotalMinutes;
		}
		set { }
	}
	public enum BuildType
	{
		testbuild = 1,
		releasebuild = 2
	}
	public int TYPE
	{
		get
		{
			return GetAsInt(_type, (int)BuildType.testbuild);
		}
		set { this[_type] = value; }
	}
	public string DATEUP
	{
		get
		{
			return GetAsDateTime(_dateUp, "");
		}
		set { this[_dateUp] = value; }
	}
	public string DATEBUILD
	{
		get
		{
			return GetAsDateTime(_dateB, "");
		}
		set { }
	}
	public int DEFID
	{
		get { return GetAsInt(_par); }
		set { this[_par] = value; }
	}
	public int TTID
	{
		get { return GetAsInt(_TTID); }
		set { this[_TTID] = value; }
	}
	public int TTUSERID
	{
		get { return GetAsInt(_User); }
		set { this[_User] = value; }
	}
	public string STATUSTXT
	{
		get { return this[_stText].ToString(); }
		set { this[_stText] = value; }
	}
	public enum BuildStatus
	{
		progress = 1,
		finishedok = 2,
		cancelled = 3,
		failed = 4,
		notstarted = 5,
	}
	public BuildStatus GetBuildStatus()
	{
		var o = this[_stat];
		return o == DBNull.Value ? BuildStatus.notstarted : (BuildStatus)Convert.ToInt32(o);
	}
	public void SetBuildStatus(BuildStatus s)
	{
		this[_stat] = (int)s;
	}
	public bool CANCELLED
	{
		get
		{
			return GetBuildStatus() == BuildStatus.cancelled;
		}
		set { }
	}
	public bool STARTED
	{
		get
		{
			return GetBuildStatus() == BuildStatus.progress;
		}
		set { }
	}
	public string COLOR
	{
		set { }
		get
		{
			var o = this[_stat];
			if (o == DBNull.Value)
			{
				return "#0000ff3b";
			}
			else
			{
				switch ((BuildStatus)Convert.ToInt32(o))
				{
					case BuildStatus.progress:
						{
							return "#ffeeba";
						}
					case BuildStatus.finishedok:
						{
							return "#c3e6cb";
						}
					case BuildStatus.cancelled:
						{
							return "#d6d8db";
						}
					case BuildStatus.failed:
						{
							return "#f5c6cb";
						}
					default:
						{
							return "purple";
						}
				}
			}
		}
	}
	public string STATUS
	{
		get
		{
			var o = this[_stat];
			if (o == DBNull.Value)
			{
				return "Awaiting build machine...";
			}
			else
			{
				switch ((BuildStatus)Convert.ToInt32(o))
				{
					case BuildStatus.progress:
						{
							return "Building in progress...";
						}
					case BuildStatus.finishedok:
						{
							return "Finished. Status: OK!";
						}
					case BuildStatus.cancelled:
						{
							return "Cancelled";
						}
					case BuildStatus.failed:
						{
							return "Finished. Status: FAILED!";
						}
					default:
						{
							return "Unknown";
						}
				}
			}
		}
		set
		{
			int res;//for set from web interface and/or normal c# code
			if (int.TryParse(value, out res))
			{
				this[_stat] = res;
				return;
			}
			BuildStatus st = (BuildStatus)Enum.Parse(typeof(BuildStatus), value);
			this[_stat] = (int)st;
		}
	}

	public DefectBuild()
	  : base(_Tabl, _allBasecols, "", _pid, false)
	{
	}
	public DefectBuild(int id)
	  : base(_Tabl, _allBasecols, id.ToString(), _pid, true)
	{
	}
	protected override string OnTransformCol(string col)
	{
		if (col == _TTID)
		{
			return string.Format("(SELECT D.{0} FROM {1} D WHERE D.{2} = {3}) {4}", DefectBase._ID, DefectBase._Tabl, DefectBase._idRec, _par, _TTID);
		}
		return base.OnTransformCol(col);
	}
	protected override void OnProcessComplexColumn(string col, object val)
	{
		if (col == _TTID)
		{
			return;//nothing to do: readonly data
		}
		base.OnProcessComplexColumn(col, val);
	}
	public static List<DefectBuild> EnumData(int from, int to, int ttid = -1)
	{
		List<DefectBuild> res = new List<DefectBuild>();
		string ttfilter = ttid < 0 ? "" : $" WHERE {_par} = {Defect.GetIDbyTT(ttid)} ";
		string sql = $"SELECT * FROM (SELECT {_pid}, ROW_NUMBER() OVER (ORDER BY {_pid} DESC) ROWN FROM {_Tabl} {ttfilter}) TABL WHERE ROWN >= {from} AND ROWN <= {to} ORDER BY {_pid} desc";
		foreach (DataRow r in DBHelper.GetRows(sql))
		{
			res.Add(new DefectBuild(Convert.ToInt32(r[_pid])));
		}
		return res;
	}
	public static void AddRequestByTask(int ttid, string notes, BuildType type)
	{
		AddObject(_Tabl, new string[] { _par, _not, _User, _dateB, _type }, new object[] { Defect.GetIDbyTT(ttid), notes, CurrentContext.User.TTUSERID, DateTime.Now, (int)type }, _pid);
	}
	public static void CancelRequestByTask(int ttid)
	{
		string sql = $"UPDATE {_Tabl} SET {_stat} = {(int)BuildStatus.cancelled} WHERE {_par} = {Defect.GetIDbyTT(ttid)} AND ({_stat} = {(int)BuildStatus.progress} OR {_stat} = {(int)BuildStatus.notstarted} OR {_stat} IS NULL)";
		SQLExecute(sql);
	}
	public static DefectBuild GetTask2Build(string machine, BuildType type)
	{
		string g = Guid.NewGuid().ToString();

		SQLExecute(string.Format("UPDATE TOP (1) {0} SET [{1}] = {2}, [{3}] = '{4}', [{5}] = '{6}' WHERE [{1}] is NULL AND {7} = {8}", _Tabl, _stat, (int)BuildStatus.progress, _mach, machine, _gui, g, _type, (int)type));

		var o = DBHelper.GetValue(string.Format("SELECT [{0}] FROM {1} WHERE [{2}] = '{3}'", _pid, _Tabl, _gui, g));
		if (o == DBNull.Value || o == null)
		{
			return null;
		}
		return new DefectBuild(Convert.ToInt32(o));
	}
	public static bool hasBuildRequest()
	{
		DefectBuild worker = new DefectBuild();
		foreach (DataRow r in worker.GetRecords($"WHERE {_stat} = {(int)BuildStatus.progress} and DATEDIFF(MINUTE, {_dateUp}, GETDATE()) > {Settings.CurrentSettings.BUILDTIMEOUT}"))
		{
			DefectBuild d = new DefectBuild();
			d.Load(r);
			d.STATUSTXT = "Time out - cancelled!";
			d.SetBuildStatus(BuildStatus.cancelled);
			d.Store();
		}
		return Convert.ToInt32(GetValue(string.Format("SELECT COUNT(*) FROM {0} WHERE {1} IS NULL OR {1} = {2}", _Tabl, _stat, (int)BuildStatus.progress))) > 0;
	}
	protected override void PostStore()
	{
		DefectBase db = new DefectBase(TTID);
		NotifyHub.NotifyBuildChange(ID, TTID, int.Parse(db.AUSER), STATUSTXT);
	}
	protected override void OnChangeColumn(string col, string val)
	{
		if (col == _stat)
		{
			DefectBase db = new DefectBase(TTID);
			string ttimg = "";
			var st = GetBuildStatus();
			if (st == BuildStatus.failed || st == BuildStatus.cancelled)
			{
				ttimg = "taskfail.png";
			}
			NotifyHub.NotifyBuildStatusChange(ID, TTID, int.Parse(db.AUSER), STATUSTXT, ttimg);
		}
	}
}