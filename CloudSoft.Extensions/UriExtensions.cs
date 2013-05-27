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

		public static int HostId(this Uri url)
		{
			var host = url.Host;
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

	}
}
