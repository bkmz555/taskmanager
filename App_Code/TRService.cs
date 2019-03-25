﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Net.Mail;
using System.Globalization;
using System.DirectoryServices;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Telegram.Bot;
using GitHelper;
using System.Linq;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class TRService : System.Web.Services.WebService
{
	static public string defDateFormat = "MM-dd-yyyy";
	public TRService()
	{
		//Uncomment the following line if using designed components 
		//InitializeComponent(); 
	}
	//========================references==============================================================
	//================================================================================================
	void StoreObjects(IdBasedObject[] data)
	{
		foreach (var d in data)
		{
			var dstore = Activator.CreateInstance(d.GetType(), d.GetID()) as Reference;
			dstore.FromAnotherObject(d);
			if (dstore.IsModified())
			{
				dstore.Store();
			}
		}
	}
	[WebMethod(EnableSession = true)]
	public List<DefectComp> gettaskcomps()
	{
		return DefectComp.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settaskcomps(List<DefectComp> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectDispo> gettaskdispos()
	{
		return DefectDispo.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void setdispos(List<DefectDispo> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectSeverity> gettasksevers()
	{
		return DefectSeverity.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void setsevers(List<DefectSeverity> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectProduct> gettaskproducts()
	{
		return DefectProduct.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settaskproducts(List<DefectProduct> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectType> gettasktypes()
	{
		return DefectType.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settasktypes(List<DefectType> data)
	{
		StoreObjects(data.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<DefectPriority> gettaskpriorities()
	{
		return DefectPriority.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void settaskpriorities(List<DefectPriority> data)
	{
		StoreObjects(data.ToArray());
	}
	//================================================================================================
	//================================================================================================
	[WebMethod(EnableSession = true)]
	public List<DefectBase> getplanned(string userid)
	{
		if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
			return null;

		DefectBase d = new DefectBase();
		return d.EnumPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> getplannedShort(string userid)
	{
		if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
			return null;

		DefectBase d = new DefectBase();
		return d.EnumPlanShort(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> getunplanned(string userid)
	{
		if (!CurrentContext.Valid && string.IsNullOrEmpty(userid))
			return null;

		DefectBase d = new DefectBase();
		return d.EnumUnPlan(string.IsNullOrEmpty(userid) ? CurrentContext.User.TTUSERID : Convert.ToInt32(userid));
	}
	[WebMethod(EnableSession = true)]
	public int newTask4MeNow(string summary)
	{
		if (string.IsNullOrEmpty(summary))
			return -1;
		DefectBase d = new DefectBase(Defect.New(summary));
		d.AUSER = CurrentContext.TTUSERID.ToString();
		d.ESTIM = 1;
		d.DISPO = DefectDispo.GetWorkingRec().ToString();
		d.ORDER = 1;
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public int planTask(string summary, int ttuserid)
	{
		if (string.IsNullOrEmpty(summary))
			return -1;
		DefectBase d = new DefectBase(Defect.New(summary));
		d.AUSER = ttuserid == -1 ? CurrentContext.TTUSERID.ToString() : ttuserid.ToString();
		d.ESTIM = 1;
		List<int> disp = DefectDispo.EnumCanStart();
		if (disp.Count > 0)
		{
			d.DISPO = disp[0].ToString();
		}
		d.ORDER = 1;
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public void addVacation(string summary, int ttuserid, int num)
	{
		if (string.IsNullOrEmpty(summary) || ttuserid < 1 || num < 1 || num > 100)
			return;
		for (int i = 0; i < num; i++)
		{
			DefectBase d = new DefectBase(Defect.New(summary + " #" + (i + 1).ToString()));
			d.AUSER = ttuserid.ToString();
			d.ESTIM = 8;
			d.COMP = DefectComp.GetVacationRec()[0].ToString();
			List<int> disp = DefectDispo.EnumCannotStart();
			if (disp.Count > 0)
			{
				d.DISPO = disp[0].ToString();
			}
			d.DATE = new DateTime(DateTime.Now.Year, 12, 31).ToString(defDateFormat);
			d.Store();
		}
	}
	[WebMethod(EnableSession = true)]
	public int newTask(string summary)
	{
		if (string.IsNullOrEmpty(summary))
			return -1;
		DefectBase d = new DefectBase(Defect.New(summary));
		d.ESTIM = 1;
		List<int> disp = DefectDispo.EnumCanStart();
		if (disp.Count > 0)
		{
			d.DISPO = disp[0].ToString();
		}
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public int copyTask(int ttid)
	{
		Defect old = new Defect(ttid);
		Defect d = new Defect(Defect.New(old.SUMMARY));
		d.From(old);
		d.AUSER = "";
		d.ESTIM = 0;
		d.ORDER = -1;
		d.Store();
		return d.ID;
	}
	[WebMethod(EnableSession = true)]
	public void copyTasks(string ttids)
	{
		string[] ids = ttids.Split(',');
		foreach (string id in ids)
		{
			Defect old = new Defect(int.Parse(id));
			Defect d = new Defect(Defect.New(old.SUMMARY));
			d.From(old);
			d.DISPO = old.DISPO;
			d.Store();
		}
	}
	[WebMethod(EnableSession = true)]
	public StoredDefectsFilter saveFilter(string name, DefectsFilter filter)
	{
		return StoredDefectsFilter.NewFilter(name, filter, CurrentContext.TTUSERID);
	}
	[WebMethod(EnableSession = true)]
	public void deleteFilter(int id)
	{
		StoredDefectsFilter.Delete(id);
	}
	[WebMethod(EnableSession = true)]
	public DefectsFilter savedFilterData(int id)
	{
		return (new StoredDefectsFilter(id)).GetFilter();
	}
	[WebMethod(EnableSession = true)]
	public List<StoredDefectsFilter> getFilters()
	{
		return StoredDefectsFilter.Enum(CurrentContext.TTUSERID);
	}
	[WebMethod(EnableSession = true)]
	public Defect gettask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		Defect d = new Defect(Convert.ToInt32(ttid));
		if (!d.IsLoaded())
			return null;
		return d;
	}
	[WebMethod(EnableSession = true)]
	public string settask(Defect d)
	{
		Defect dstore = new Defect(d.ID);
		if (d.ORDER != dstore.ORDER)
		{
			//copy object specifics for multiple savings from same page: only order change should be processed.
			d.BACKORDER = dstore.BACKORDER;
		}
		dstore.FromAnotherObject(d);
		dstore.REQUESTRESET = d.REQUESTRESET;
		if (dstore.IsModified())
		{
			dstore.Store();
		}
		return "OK";
	}
	[WebMethod(EnableSession = true)]
	public void settaskBase(List<DefectBase> defects)
	{
		foreach (DefectBase d in defects)
		{
			Defect dstore = new Defect(d.ID);
			dstore.FromAnotherObject(d);
			if (dstore.IsModified())
			{
				dstore.Store();
			}
		}
	}
	[WebMethod(EnableSession = true)]
	public List<DefectHistory> gettaskhistory(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectHistory.GetHisotoryByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectAttach> getattachsbytask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectAttach.GetAttachsByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBuild> getBuildsByTask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return new List<DefectBuild>();
		return DefectBuild.GetEventsByTask(int.Parse(ttid));
	}
	[WebMethod(EnableSession = true)]
	public void addBuildByTask(string ttid, string notes)
	{
		if (string.IsNullOrEmpty(ttid))
			return;
		DefectBuild.AddRequestByTask(Convert.ToInt32(ttid), notes == null ? "" : notes);
	}
	[WebMethod(EnableSession = true)]
	public void cancelBuildByTask(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return;
		DefectBuild.CancelRequestByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectEvent> gettaskevents(string ttid)
	{
		if (string.IsNullOrEmpty(ttid))
			return null;
		return DefectEvent.GetEventsByTask(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<DefectUser> gettaskusers()
	{
		return DefectUser.Enum();
	}
	[WebMethod(EnableSession = true)]
	public void newfileupload(string ttid, string filename, string data)
	{
		DefectAttach.AddAttachByTask(Convert.ToInt32(ttid), filename, data.Remove(0, data.IndexOf("base64,") + 7));
	}
	[WebMethod(EnableSession = true)]
	public void delfileupload(string ttid, string id)
	{
		int iid = Convert.ToInt32(id);
		if (iid < 1)
		{
			return;
		}
		DefectAttach.DeleteAttach(ttid, iid);
	}
	[WebMethod(EnableSession = true)]
	public LockInfo locktask(string ttid, string lockid)
	{
		if (!CurrentContext.Valid || string.IsNullOrEmpty(ttid))
		{
			return null;
		}
		return Defect.Locktask(ttid, lockid, CurrentContext.UserID.ToString());
	}
	[WebMethod(EnableSession = true)]
	public void unlocktask(string ttid, string lockid)
	{
		Defect.UnLocktask(ttid, lockid);
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> gettasks(DefectsFilter f)
	{
		DefectBase enm = new DefectBase();
		return enm.Enum(f);
	}
	[WebMethod(EnableSession = true)]
	public TRRec gettrrec(string date)
	{
		if (!CurrentContext.Valid)
		{
			return null;
		}
		DateTime d = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		return r;
	}
	[WebMethod(EnableSession = true)]
	public void settrrec(TRRec rec)
	{
		TRRec store = new TRRec(rec.ID);
		store.FromAnotherObject(rec);
		if (store.IsModified())
		{
			store.Store();
		}
	}
	[WebMethod(EnableSession = true)]
	public void deltrrec(int id)
	{
		TRRec.DelRec(id);
	}
	[WebMethod(EnableSession = true)]
	public void todayrrec(string lastday)
	{
		if (!CurrentContext.Valid)
		{
			return;
		}
		DateTime d = DateTime.Today;
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		if (r == null)
		{
			TRRec.NewRec(d, CurrentContext.User.ID, lastday == "True");
		}
	}
	[WebMethod(EnableSession = true)]
	public void addrec(string date, string lastday)
	{
		DateTime d = DateTime.ParseExact(date, defDateFormat, CultureInfo.InvariantCulture);
		TRRec r = TRRec.GetRec(d, CurrentContext.User.ID);
		if (r == null)
		{
			TRRec.NewRec(d, CurrentContext.User.ID, lastday == "True");
		}
	}
	[WebMethod(EnableSession = true)]
	public MPSUser getcurrentuser()
	{
		return CurrentContext.User;
	}
	[WebMethod(EnableSession = true)]
	public DefectBase settaskdispo(string ttid, string disp)
	{
		if (Defect.Locked(ttid))
			return null;
		Defect d = new Defect(Convert.ToInt32(ttid));
		d.DISPO = disp;
		if (Convert.ToInt32(d.DISPO) == DefectDispo.GetWorkingRec())
		{
			if (d.ORDER < 1)
			{
				d.ORDER = 1;
			}
		}
		d.Store();
		return new DefectBase(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public DefectBase scheduletask(string ttid, string date)
	{
		if (Defect.Locked(ttid))
			return null;
		Defect d = new Defect(Convert.ToInt32(ttid));
		d.DISPO = DefectDispo.GetWorkingRec().ToString();
		d.DATE = date;
		d.Store();
		return new DefectBase(Convert.ToInt32(ttid));
	}
	[WebMethod(EnableSession = true)]
	public List<MPSUser> getActiveMPSusers()
	{
		return getMPSUsers(true);
	}
	[WebMethod(EnableSession = true)]
	public List<MPSUser> getMPSUsers(bool active)
	{
		return MPSUser.EnumAllUsers(active);
	}
	[WebMethod(EnableSession = true)]
	public string setusers(List<MPSUser> users)
	{
		foreach (MPSUser u in users)
		{
			MPSUser ustore = new MPSUser(u.ID);
			ustore.FromAnotherObject(u);
			if (ustore.IsModified())
			{
				ustore.Store();
			}
		}
		return "OK";
	}
	public class TTBackOrder
	{
		public int ttid { get; set; }
		public int backorder { get; set; }
		public bool moved { get; set; }
	}
	[WebMethod(EnableSession = true)]
	public void setschedule(List<TTBackOrder> ttids, List<string> unschedule)
	{
		if (!CurrentContext.Valid)
		{
			return;
		}

		foreach (var ttid in unschedule)
		{
			Defect d = new Defect(Convert.ToInt32(ttid));
			d.ORDER = -1;
			d.Store();
		}

		foreach (var ttid in ttids)
		{
			DefectBase d;
			if (ttid.moved)
			{
				d = new Defect(ttid.ttid); //will add history record about moving
			}
			else
			{
				d = new DefectBase(ttid.ttid);
			}
			d.BACKORDER = Convert.ToInt32(ttid.backorder);
			d.Store();
		}
	}
	[WebMethod(EnableSession = true)]
	public List<TRRec> getreports(List<string> dates)
	{
		if (!CurrentContext.Valid || dates.Count < 1)
		{
			return new List<TRRec>();
		}
		List<DateTime> datetimes = new List<DateTime>();
		foreach (var d in dates)
		{
			datetimes.Add(DateTime.ParseExact(d, defDateFormat, CultureInfo.InvariantCulture));
		}
		return TRRec.Enum(datetimes.ToArray());
	}
	[WebMethod(EnableSession = true)]
	public List<Machine> getMachines()
	{
		if (!CurrentContext.Valid)
		{
			return new List<Machine>();
		}
		return Machine.Enum();
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> enumCloseVacations(string start, int days)
	{
		return DefectBase.EnumCloseVacations(start, days);
	}
	[WebMethod(EnableSession = true)]
	public List<DefectBase> enumUnusedVacations()
	{
		return DefectBase.EnumUnusedVacations();
	}
	[WebMethod(EnableSession = true)]
	public void remMachine(string m)
	{
		Machine.Delete(m);
	}
	[WebMethod(EnableSession = true, CacheDuration = 600)]
	public List<string> getDomainComputers()
	{
		List<string> ComputerNames = new List<string>();

		DirectoryEntry entry = new DirectoryEntry("LDAP://mps");
		DirectorySearcher mySearcher = new DirectorySearcher(entry);
		mySearcher.Filter = ("(objectClass=computer)");
		mySearcher.SizeLimit = int.MaxValue;
		mySearcher.PageSize = int.MaxValue;
		foreach (SearchResult resEnt in mySearcher.FindAll())
		{
			string ComputerName = resEnt.GetDirectoryEntry().Name;
			if (ComputerName.StartsWith("CN="))
				ComputerName = ComputerName.Remove(0, "CN=".Length);
			ComputerNames.Add(ComputerName);
		}
		mySearcher.Dispose();
		entry.Dispose();
		return ComputerNames;
	}
	[WebMethod(EnableSession = true)]
	public void wakeMachine(string m)
	{
		Machine mach = Machine.FindOrCreate(m);
		string[] macs = mach.MAC.Split(' ');
		foreach (var mac in macs)
		{
			Process process = new Process();
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = HttpRuntime.AppDomainAppPath + "bin\\WolCmd.exe";
			process.StartInfo.Arguments = mac + " 192.168.0.1 255.255.255.0 3";
			process.Start();
		}
	}
	[WebMethod(EnableSession = true)]
	public void shutMachine(string m)
	{
		Machine ma = Machine.FindOrCreate(m);
		Process process = new Process();
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.FileName = "shutdown";
		process.StartInfo.Arguments = string.Format(@"/s /m \\{0} /t 0", ma.NAME);
		process.Start();
	}
	[WebMethod(EnableSession = true)]
	public void scanMachine(string m)
	{
		try
		{
			Machine ma = Machine.FindOrCreate(m);
			string scope = string.Format("\\\\{0}\\root\\CIMV2", ma.NAME);
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, "SELECT * FROM Win32_NetworkAdapterConfiguration");
			string newmac = "";
			foreach (ManagementObject queryObj in searcher.Get())
			{
				object o = queryObj["MACAddress"];
				if (o == null)
				{
					continue;
				}
				if (string.IsNullOrEmpty(newmac))
					newmac = o.ToString().Replace(":", "");
				else
					newmac += " " + o.ToString().Replace(":", "");
			}
			var cpu = new ManagementObjectSearcher(scope, "select * from Win32_Processor").Get().Cast<ManagementObject>().First();
			var wmi = new ManagementObjectSearcher(scope, "select * from Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
			if (!string.IsNullOrEmpty(newmac))
			{
				int memory = Convert.ToInt32(wmi["TotalVisibleMemorySize"]) / 1024;
				ma.DETAILS = wmi["Caption"].ToString() + "<br/>" + cpu["Name"].ToString() + "<br/>" + memory.ToString() + "Mb";
				ma.MAC = newmac;
				ma.Store();
			}
		}
		catch (Exception /*e*/)
		{
		}
	}
	[WebMethod(EnableSession = true)]
	public void pageLoadedComplete(int id)
	{
		PageLoadNofify.RemoveLoad(id);
	}
	[WebMethod(EnableSession = true)]
	public List<TRRecSignal> enumTRSignal(string from, string to)
	{
		if (!CurrentContext.Valid)
		{
			return new List<TRRecSignal>();
		}
		return TRRecSignal.Enum(DateTime.ParseExact(from, defDateFormat, CultureInfo.InvariantCulture), DateTime.ParseExact(to, defDateFormat, CultureInfo.InvariantCulture));
	}
	[WebMethod(EnableSession = true)]
	public List<string> getVersionLog()
	{
		if (!CurrentContext.Valid)
		{
			return new List<string>();
		}
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.RunCommand(@"show HEAD:""Projects.32/ChangeLog.txt""");
	}
	[WebMethod(EnableSession = true)]
	public List<Statistic> getStatistics(string start, string days)
	{
		return Defect.EnumStatistics(DateTime.ParseExact(start, defDateFormat, CultureInfo.InvariantCulture), Convert.ToInt32(days));
	}
	[WebMethod(EnableSession = true)]
	public List<TRStatistic> getTRStatistic(string start, string days)
	{
		return TRRec.EnumTRStatistics(DateTime.ParseExact(start, defDateFormat, CultureInfo.InvariantCulture), Convert.ToInt32(days));
	}
	[WebMethod(EnableSession = true)]
	public List<TRRec> getreports4Person(int personid, string start, int days, string text)
	{
		if (!CurrentContext.Valid)
		{
			return new List<TRRec>();
		}
		return TRRec.EnumPersonal(personid, DateTime.ParseExact(start, defDateFormat, CultureInfo.InvariantCulture), days, text);
	}
	[WebMethod(EnableSession = true)]
	public RawSettings getSettings()
	{
		return RawSettings.CurrentRawSettings;
	}
	[WebMethod(EnableSession = true)]
	public void setSettings(RawSettings s)
	{
		if (!CurrentContext.Valid || !CurrentContext.Admin)
		{
			return;
		}
		s.Store();
	}
	[WebMethod(EnableSession = true)]
	public DefectDefaults getDefaults()
	{
		return DefectDefaults.CurrentDefaults;
	}
	[WebMethod(EnableSession = true)]
	public void setDefaults(DefectDefaults d)
	{
		if (!CurrentContext.Valid || !CurrentContext.Admin)
		{
			return;
		}
		DefectDefaults.CurrentDefaults = d;
	}
	public class BuildRequest
	{
		public int ID { get; set; }
		public int TTID { get; set; }
		public string SUMMARY { get; set; }
		public string USER { get; set; }
		public string COMM { get; set; }
		public string BRANCH { get; set; }
	}
	[WebMethod]
	public BuildRequest getBuildRequest(string machine)
	{
		DefectBuild b = DefectBuild.GetTask2Build(machine);
		BuildRequest r = new BuildRequest();
		if (b != null)
		{
			DefectBase def = new DefectBase(Defect.GetTTbyID(b.DEFID));
			DefectUser user = new DefectUser(int.Parse(def.AUSER));
			r.ID = b.ID;
			r.TTID = def.ID;
			r.COMM = b.NOTES;
			string em = user.EMAIL.Trim();
			if (string.IsNullOrEmpty(em))
			{
				r.USER = "ADMIN";
			}
			else
			{
				r.USER = em.Substring(0, em.IndexOf("@")).ToUpper();
			}
			r.SUMMARY = def.SUMMARY;
			r.BRANCH = def.BRANCH;
		}
		return r;
	}
	[WebMethod]
	public List<DefectBuild> getBuildRequests(int from, int to)
	{
		return DefectBuild.EnumData(from, to);
	}
	[WebMethod]
	public bool hasBuildRequest()
	{
		return DefectBuild.hasBuildRequest();
	}
	[WebMethod]
	public List<Branch> enumbranches(int from, int to, string user)
	{
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.EnumBranches(from, to, user);
	}
	[WebMethod(EnableSession = true)]
	public void deleteBranch(string branch)
	{
		if (string.IsNullOrEmpty(branch) || branch.ToUpper() == "MASTER" || branch.ToUpper() == "RELEASE")
			return;
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		git.DeleteBranch(branch);
	}
	[WebMethod]
	public List<Commit> EnumCommits(string branch, int from, int to)
	{
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.GetBranch(branch).EnumCommits(from, to);
	}
	[WebMethod]
	public string BranchHash(string branch)
	{
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		return git.GetBranch(branch).TopCommit();
	}
	[WebMethod]
	public List<string> getCommitDiff(string commit)
	{
		Git git = new Git(Settings.CurrentSettings.WORKGITLOCATION);
		Commit c = new Commit(git);
		c.COMMIT = commit;
		return Git.DiffFriendOutput(c.Diff());
	}
	[WebMethod]
	public void CommentBuild(int id, string comment)
	{
		DefectBuild b = new DefectBuild(id)
		{
			STATUSTXT = comment
		};
		b.Store();
	}
	[WebMethod]
	public void FinishBuild(int id, string requestguid)
	{
		DefectBuild b = new DefectBuild(id)
		{
			STATUS = DefectBuild.BuildStatus.finishedok.ToString()
			,
			TESTGUID = requestguid
		};
		b.Store();
		if (Settings.CurrentSettings.RELEASETTID == b.TTID.ToString())
		{
			VersionBuilder.SendAlarm("New local release build has been finished. Testing is starting...");
		}
		else
		{
			TelegramBotClient client = new TelegramBotClient(Settings.CurrentSettings.TELEGRAMTESTTOKEN);
			client.GetMeAsync().Wait();
			DefectUser u = new DefectUser(b.TTUSERID);
			string mess = $"New task from {u.FULLNAME} is ready for tests!<a href='{Settings.CurrentSettings.GetTTURL(b.TTID)}'>&#8205;</a>";
			client.SendTextMessageAsync(Settings.CurrentSettings.TELEGRAMTESTCHANNEL, mess, Telegram.Bot.Types.Enums.ParseMode.Html).Wait();
		}

		Defect d = new Defect(b.TTID);
		string bst_b = d.BSTBATCHES.Trim();
		string bst_c = d.BSTCOMMANDS.Trim();
		if (!string.IsNullOrEmpty(bst_b) || !string.IsNullOrEmpty(bst_c))
		{
			string batches = string.Join(",", bst_b.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
			string commands = string.Join(",", bst_c.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
			using (var wcClient = new WebClient())
			{
				var reqparm = new NameValueCollection();
				reqparm.Add("guid", requestguid);
				reqparm.Add("commaseparatedbatches", batches);
				reqparm.Add("commaseparatedcommands", commands);
				reqparm.Add("priority", d.TESTPRIORITY);
				wcClient.UploadValues(Settings.CurrentSettings.BSTSITESERVICE + "/StartTest", reqparm);
			}
		}
	}
	[WebMethod]
	public bool IsBuildCancelled(int id)
	{
		DefectBuild b = new DefectBuild(id);
		return b.CANCELLED;
	}
	[WebMethod]
	public string geBuildLogDir()
	{
		return Settings.CurrentSettings.BUILDLOGSDIR;
	}
	[WebMethod]
	public void FailBuild(int id)
	{
		DefectBuild b = new DefectBuild(id)
		{
			STATUS = DefectBuild.BuildStatus.failed.ToString()
		};
		b.Store();
		if (Settings.CurrentSettings.RELEASETTID == b.TTID.ToString())
		{
			VersionBuilder.SendAlarm("Failed to build version. Please check the logs!!!");
		}
	}
	[WebMethod(EnableSession = true)]
	public string alarmEmail(int ttid, string addresses)
	{
		if (!CurrentContext.Valid)
			return "Please login";

		Defect d = new Defect(ttid);
		MailMessage mail = new MailMessage();

		foreach (string addr in addresses.Split(','))
		{
			mail.To.Add(new MailAddress(addr.Trim()));
		}
		mail.From = new MailAddress(CurrentContext.User.EMAIL.Trim());
		mail.Subject = string.Format("TT{0} {1}", d.ID, d.SUMMARY);
		mail.IsBodyHtml = true;

		string descr = d.DESCR.Replace(Environment.NewLine, "<br/>");
		descr = descr.Replace("\n", "<br/>");

		descr = BodyProcessor.ResolveLinks(descr);
		descr = Regex.Replace(descr, "----+", "<hr>");
		descr = Regex.Replace(descr, "====+", "<hr>");

		string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
		body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
		body += "</HEAD><BODY'>" + descr + " </BODY></HTML>";

		System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
		AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
		mail.AlternateViews.Add(alternate);

		SmtpClient smtp = new SmtpClient();
		Settings sett = Settings.CurrentSettings;
		smtp.Host = sett.SMTPHOST;
		smtp.Port = Convert.ToInt32(sett.SMTPPORT);
		smtp.EnableSsl = Convert.ToBoolean(sett.SMTPENABLESSL); ;
		smtp.Timeout = Convert.ToInt32(sett.SMTPTIMEOUT); ;
		smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
		smtp.UseDefaultCredentials = false;
		smtp.Credentials = new NetworkCredential(sett.CREDENTIALS1, sett.CREDENTIALS2);

		string strError = "The email was sent!";
		long counter = 0;
		while (true)
		{
			try
			{
				counter++;
				smtp.Send(mail);
				break;
			}
			catch (Exception e)
			{
				strError = e.Message;
				if (!strError.Contains("The operation has timed out.") || counter > 10)
				{
					break;
				}
			}
		}
		return strError;
	}
	[WebMethod(EnableSession = true)]
	public List<string> getBSTBatches()
	{
		Uri _uri = new Uri(Settings.CurrentSettings.BSTSITESERVICE + "/EnumBatches");
		using (var wcClient = new WebClient())
		{
			string res = wcClient.UploadString(_uri, "POST", "");
			XmlSerializer ser = new XmlSerializer(typeof(string[]), new XmlRootAttribute("ArrayOfString") { Namespace = "http://tempuri.org/" });
			string[] arrres = (string[])ser.Deserialize(new StringReader(res));
			return new List<string>(arrres);
		}
	}
	[WebMethod(EnableSession = true)]
	public List<string> getBSTBatchData(string batch)
	{
		using (var wcClient = new WebClient())
		{
			var reqparm = new NameValueCollection();
			reqparm.Add("name", batch);
			byte[] result = wcClient.UploadValues(Settings.CurrentSettings.BSTSITESERVICE + "/getBatchData", reqparm);
			string res = Encoding.ASCII.GetString(result);
			XmlSerializer ser = new XmlSerializer(typeof(string[]), new XmlRootAttribute("ArrayOfString") { Namespace = "http://tempuri.org/" });
			string[] arrres = (string[])ser.Deserialize(new StringReader(res));
			return new List<string>(arrres);
		}
	}
	[WebMethod(EnableSession = true)]
	public int getTestID(string requestGUID)
	{
		using (var wcClient = new WebClient())
		{
			var reqparm = new NameValueCollection();
			reqparm.Add("guid", requestGUID);
			byte[] result = wcClient.UploadValues(Settings.CurrentSettings.BSTSITESERVICE + "/GetTestID", reqparm);
			string sres = Encoding.ASCII.GetString(result);
			XmlSerializer ser = new XmlSerializer(typeof(int), new XmlRootAttribute("int") { Namespace = "http://tempuri.org/" });
			return (int)ser.Deserialize(new StringReader(sres));
		}
	}
	[WebMethod(EnableSession = true)]
	public string getUpdateWorkGit()
	{
		if (!CurrentContext.Valid)
		{
			return "FAILED";
		}
		if (!CurrentContext.Admin)
		{
			return "FAILED - not admin.";
		}
		return VersionBuilder.PrepareGit();
	}
	[WebMethod(EnableSession = true)]
	public string versionIncrement()
	{
		if (!CurrentContext.Valid)
		{
			return "FAILED";
		}
		if (!CurrentContext.Admin)
		{
			return "FAILED - not admin.";
		}
		return VersionBuilder.VersionIncrement();
	}
	[WebMethod(EnableSession = true)]
	public string push2Master()
	{
		if (!CurrentContext.Valid)
		{
			return "FAILED";
		}
		if (!CurrentContext.Admin)
		{
			return "FAILED - not admin.";
		}
		string res = VersionBuilder.PushRelease();
		VersionBuilder.SendVersionAlarm();
		return res + "<br/>Finished!";
	}
}