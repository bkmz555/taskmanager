﻿<%@ WebHandler Language="C#" Class="getUserImg" %>

using System;
using System.Web;
using System.Drawing;
using System.IO;

public class getUserImg : IHttpHandler
{
	void error(HttpContext context)
	{
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddHours(24));
		context.Response.Cache.SetMaxAge(new TimeSpan(24, 0, 0));
		context.Response.ContentType = "image/png";
		string file = context.Server.MapPath("images/img_avatar.png");
		context.Response.AddHeader("Content-Length", (new FileInfo(file)).Length.ToString());
		context.Response.WriteFile(file);
	}
	public void ProcessRequest(HttpContext context)
	{
		object sz = context.Request.QueryString["sz"];
		string sid = context.Request.QueryString["id"];
		if (string.IsNullOrEmpty(sid))
		{
			string sttid = context.Request.QueryString["ttid"];
			if (string.IsNullOrEmpty(sttid))
			{
				string eml = context.Request.QueryString["eml"];
				if (!string.IsNullOrEmpty(eml))
				{
					DefectUser du = DefectUser.FindByEmail(eml);
					if (du == null)
					{
						error(context);
						return;
					}
					sid = du.TRID.ToString();
				}
				if (string.IsNullOrEmpty(sid))
				{
					string pho = context.Request.QueryString["pho"];
					if (!string.IsNullOrEmpty(pho))
					{
						MPSUser mu = MPSUser.FindUserbyPhone(pho);
						if (mu != null)
						{
							sid = mu.ID.ToString();
						}
					}
				}

				if (string.IsNullOrEmpty(sid))
				{
					error(context);
					return;
				}
			}
			else
			{
				int ttid;
				if (!int.TryParse(sttid, out ttid) || ttid < 1)
				{
					error(context);
					return;
				}
				DefectUser du = new DefectUser(ttid);
				sid = du.TRID.ToString();
			}
		}
		int id;
		if (!int.TryParse(sid, out id) || id < 1)
		{
			error(context);
			return;
		}
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddDays(10));
		context.Response.Cache.SetMaxAge(new TimeSpan(10, 0, 0, 0));
		context.Response.ContentType = "image/jpg";
		int? isz = null;
		if (sz != null)
		{
			isz = Convert.ToInt32(sz);
		};
		LoadImageFile(context, id, isz);
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
	static object _Lock = new object();
	void DellOldFiles(string dir, string validprefix)
	{
		var files = Directory.EnumerateFiles(dir);
		foreach (var file in files)
		{
			string filename = Path.GetFileName(file);
			if (!filename.StartsWith(validprefix) && !filename.ToUpper().Contains("GITIGNORE"))
			{
				File.Delete(file);
			}
		}
	}
	void LoadImageFile(HttpContext context, int id, int? size)
	{
		string dir = context.Server.MapPath($"images/cache/users/");
		string prefix = ReferenceVersion.REFSVERSION();
		string newfilename = $"{dir}{prefix}scaled-id-{size}-sz-{id}.jpg";
		string origfilename = $"{dir}{prefix}orig-id-{id}.jpg";
		if (!File.Exists(origfilename) || !File.Exists(newfilename))
		{
			lock (_Lock)
			{
				if (!File.Exists(origfilename))
				{
					MPSUser u = new MPSUser(id);
					byte[] data = u.GetImage();
					if (data == null)
					{
						error(context);
						return;
					}
					File.WriteAllBytes(origfilename, data);
					DellOldFiles(dir, prefix);
				}
				if (size == null)
				{
					File.Copy(origfilename, newfilename);
				}
				else
				{
					using (Bitmap orig = new Bitmap(origfilename))
					{
						using (Bitmap newb = new Bitmap((Image)orig, new Size(size.Value, size.Value)))
						{
							newb.Save(newfilename);
							DellOldFiles(dir, prefix);
						}
					}
				}
			}
		}
		context.Response.AddHeader("Content-Length", (new FileInfo(newfilename)).Length.ToString());
		context.Response.TransmitFile(newfilename);
	}
}