﻿using System;
using System.Collections.Generic;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]

public class TrackerService : WebService
{
	public TrackerService() { }
	[WebMethod(EnableSession = true)]
	public List<Tracker> getTrackers(int user)
	{
		CurrentContext.Validate();
		return Tracker.Enum(user);
	}
	[WebMethod(EnableSession = true)]
	public string getTrackerModified(int id)
	{
		CurrentContext.Validate();
		DefectsFilter flt = new StoredDefectsFilter(id).GetFilter();
		return (new DefectBase()).ModTime(flt).ToString();
	}
	[WebMethod(EnableSession = true)]
	public List<Tracker> assignTracker(int id, int userid)
	{
		CurrentContext.Validate();
		Tracker t = new Tracker(id);
		t.IDCLIENT = userid;
		t.Store();
		return Tracker.Enum(CurrentContext.TTUSERID);
	}
	[WebMethod(EnableSession = true)]
	public void delTracker(int id)
	{
		CurrentContext.Validate();
		Tracker.Delete(id);
	}
	[WebMethod(EnableSession = true)]
	public List<DefectPlan> getItems(int filterid)
	{
		CurrentContext.Validate();
		DefectsFilter flt = new StoredDefectsFilter(filterid).GetFilter();
		List<DefectBase> defs = (new DefectBase()).Enum(flt);
		return DefectPlan.Convert2Plan(defs);
	}
	[WebMethod(EnableSession = true)]
	public Tracker newTracker(string name, int user, int filter)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		CurrentContext.Validate();
		return Tracker.New(name, user, filter);
	}
	public class PublicFilter
	{
		public PublicFilter() { }
		public string NAME { get; set; }
		public int ID { get; set; }
		public bool SHARED { get; set; }
	}
	[WebMethod(EnableSession = true)]
	public List<PublicFilter> getFilters(int user)
	{
		CurrentContext.Validate();
		List<StoredDefectsFilter> sdf = StoredDefectsFilter.Enum(user);
		List<PublicFilter> lsout = new List<PublicFilter>();
		foreach (var f in sdf)
		{
			lsout.Add(new PublicFilter() { NAME = f.NAME, ID = f.ID, SHARED = f.SHARED });
		}
		return lsout;
	}
}