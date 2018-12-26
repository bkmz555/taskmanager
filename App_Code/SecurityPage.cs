﻿using System;
using System.Linq;
using System.Web;

public class SecurityPage : System.Web.UI.Page
{
	public static string returl = "ReturnUrl";
	public static string loginpage = "login.aspx";

	void CheckRetired()
	{
		if (CurrentContext.Valid && CurrentContext.User.RETIRED)
		{
			Response.Redirect(string.Format("{0}?{1}=1", loginpage, CurrentContext.retiredURL), false);
			Context.ApplicationInstance.CompleteRequest();
		}
	}
	protected void Page_PreInit(object sender, EventArgs e)
	{
		if (Request.Url.Segments.Last().ToUpper() == loginpage.ToUpper())
		{
			return;
		}
		if (CurrentContext.Valid)
		{
			CheckRetired();
			return;
		}
		else
		{
			Response.Redirect(loginpage + "?" + returl + "=" + Request.Url.PathAndQuery, false);
			Context.ApplicationInstance.CompleteRequest();
		}
		CheckRetired();
		return;
	}

	public static string GetPageOgName()
	{
		string ttid = GetPageTTID();
		if (!string.IsNullOrEmpty(ttid))
		{
			return "TT" + ttid;
		}
		return "";
	}
	public static string GetPageOgImage()
	{
		return
			HttpContext.Current.Request.Url.Scheme +
			"://" +
			HttpContext.Current.Request.Url.Host +
			HttpContext.Current.Request.ApplicationPath +
			"/images/taskicon.png";
	}
	static string GetPageTTID()
	{
		if (HttpContext.Current == null || HttpContext.Current.Request == null)
		{
			return "";
		}
		object o = HttpContext.Current.Request.QueryString[SecurityPage.returl];
		if (o == null)
		{
			return "";
		}
		string findstr = "ttid=";
		string s = o.ToString();
		int ind = s.IndexOf(findstr);
		if (ind < 0)
		{
			return "";
		}
		return s.Substring(ind + findstr.Length);
	}
	public static string GetPageOgTitle()
	{
		string ttid = GetPageTTID();
		if (!string.IsNullOrEmpty(ttid))
		{
			return DefectBase.GetTaskDispName(int.Parse(ttid));
		}
		return "";
	}
	public static string GetPageOgDesc()
	{
		string ttid = GetPageTTID();
		if (!string.IsNullOrEmpty(ttid))
		{
			int id = int.Parse(ttid);
			return "Estimated: " + DefectBase.GetTaskEstim(id) + ", Assigned: " + DefectBase.GetTaskUserName(id);
		}
		return "Click to see details.";
	}
}