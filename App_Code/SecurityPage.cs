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
	public static string GetPageOgImage()
	{
		return
			HttpContext.Current.Request.Url.Scheme +
			"://" +
			HttpContext.Current.Request.Url.Host +
			HttpContext.Current.Request.ApplicationPath +
			"/images/task.png";
	}
	public static string GetPageOgTitle()
	{
		if (HttpContext.Current == null || HttpContext.Current.Request == null)
		{
			return "";
		}
		object o = HttpContext.Current.Request.QueryString["ttid"];
		if (o == null)
		{
			o = HttpContext.Current.Request.QueryString[SecurityPage.returl];
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
			o = s.Substring(ind + findstr.Length);
		}
		int id = int.Parse(o.ToString());
		return DefectBase.GetTaskDispName(id);
	}
}