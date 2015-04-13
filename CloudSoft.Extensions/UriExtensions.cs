using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Extensions
{
	public static class UriExtensions
	{
		public static System.Collections.Specialized.NameValueCollection GetParameters(this Uri uri)
		{
			if (uri == null)
			{
				return null;
			}
			var result = new System.Collections.Specialized.NameValueCollection();
			var query = uri.Query.Trim('?').Trim().Split('&');
			foreach (var part in query)
			{
				var nv = part.Split('=');
				if (nv.Count() > 1)
				{
					result.Add(nv[0], nv[1]);
				}
			}
			return result;
		}

		public static int HostId(this Uri uri)
		{
			var host = uri.Host;
			host = host.ToLower(); // important
			host = host.Replace("www.", "");

			int h = 0, g = 0;
			for (int i = host.Length - 1; i >= 0; i--)
			{
				int c = (int)host[i];
				h = ((h << 6) & 0xfffffff) + c + (c << 14);
				if ((g = h & 0xfe00000) != 0)
				{
					h = (h ^ (g >> 21));
				}
			}
			return h;
		}

		public static int PathId(this Uri uri)
		{
			var localPath = uri.LocalPath;
			localPath = localPath.ToLower(); // important

			int h = 0, g = 0;
			for (int i = localPath.Length - 1; i >= 0; i--)
			{
				int c = (int)localPath[i];
				h = ((h << 6) & 0xfffffff) + c + (c << 14);
				if ((g = h & 0xfe00000) != 0)
				{
					h = (h ^ (g >> 21));
				}
			}
			return h;
		}

		public static int LinkId(this Uri uri)
		{
			var url = uri.ToString();
			url = url.ToLower(); // important

			int h = 0, g = 0;
			for (int i = url.Length - 1; i >= 0; i--)
			{
				int c = (int)url[i];
				h = ((h << 6) & 0xfffffff) + c + (c << 14);
				if ((g = h & 0xfe00000) != 0)
				{
					h = (h ^ (g >> 21));
				}
			}
			return h;
		}


	}
}
