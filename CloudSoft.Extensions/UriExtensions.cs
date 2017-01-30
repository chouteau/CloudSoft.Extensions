using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
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

		/// <summary>
		/// Ajoute un parametre et sa valeur dans une URL en tenant compte de sa presence eventuelle
		/// </summary>
		/// <param name="pathAndQuery">The path and query.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <example>
		/// Exemple d'utilisation
		/// <code>
		/// 		<![CDATA[
		/// une url existante est de la forme
		/// http://domain.com/test?param1=valeur1
		/// <a href='<%=Url.AddParameter("param2", "valeur2")%>'>Mon lien</a>
		/// Donnera le resultat suivant :
		/// http://domain.com/test?param1=valeur1&param2=valeur2
		/// <a href='<%=Url.AddParameter("param1", "valeur2")%>'>Mon lien</a>
		/// Donnera le resultat suivant :
		/// http://domain.com/test?param1=valeur2
		/// ]]>
		/// 	</code>
		/// </example>
		/// <remarks>
		/// "value" est urlencodé par la methode, ne pas passer la valeur deja encodée
		/// </remarks>
		public static string AddUrlParameter(this string pathAndQuery, string key, int value)
		{
			return AddUrlParameter(pathAndQuery, key, value.ToString());
		}

		/// <summary>
		/// Ajoute des paramètres supplémentaires sur une url
		/// </summary>
		/// <param name="pathAndQuery"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static string AddUrlParameters(this string pathAndQuery, string parameters)
		{
			if (parameters.IsNullOrTrimmedEmpty()
				|| pathAndQuery.IsNullOrTrimmedEmpty())
			{
				return pathAndQuery;
			}

			var dic = parameters.Split('&');
			foreach (var p in dic)
			{
				var nv = p.Split('=');
				if (nv.Length == 2)
				{
					pathAndQuery = pathAndQuery.AddUrlParameter(nv[0], nv[1]);
				}
			}

			return pathAndQuery;
		}

		/// <summary>
		/// Ajoute un parametre et sa valeur dans une URL en tenant compte de sa presence eventuelle
		/// </summary>
		/// <param name="pathAndQuery">The path and query.</param>
		/// <param name="encodedKey">The encoded key.</param>
		/// <param name="encodedValue">The HtmlEncoded value.</param>
		/// <returns></returns>
		/// <example>
		/// Exemple d'utilisation
		/// <code>
		/// 		<![CDATA[
		/// une url existante est de la forme
		/// http://domain.com/test?param1=valeur1
		/// <a href='<%=Url.AddParameter("param2", "valeur2")%>'>Mon lien</a>
		/// Donnera le resultat suivant :
		/// http://domain.com/test?param1=valeur1&param2=valeur2
		/// <a href='<%=Url.AddParameter("param1", "valeur2")%>'>Mon lien</a>
		/// Donnera le resultat suivant :
		/// http://domain.com/test?param1=valeur2
		/// ]]>
		/// 	</code>
		/// </example>
		/// <remarks>
		/// "value" est urlencodé par la methode, ne pas passer la valeur deja encodée
		/// </remarks>
		public static string AddUrlParameter(this string pathAndQuery, string encodedKey, string encodedValue)
		{
			if (pathAndQuery.IsNullOrEmpty()
				|| encodedKey.IsNullOrEmpty()
				|| encodedValue.IsNullOrEmpty())
			{
				return pathAndQuery;
			}
			string anchor = null;
			string separator = "?";
			pathAndQuery = ExcludeUrlParameter(pathAndQuery, encodedKey, out separator, out anchor);
			var blackList = new[] { "gclid" };
			foreach (var key in blackList)
			{
				string dummy = null;
				pathAndQuery = ExcludeUrlParameter(pathAndQuery, key, out separator, out dummy);
			}
			pathAndQuery += string.Format("{0}{1}={2}", separator, encodedKey, encodedValue);
			if (!anchor.IsNullOrTrimmedEmpty())
			{
				pathAndQuery += anchor;
			}

			return pathAndQuery;
		}

		public static string AddUrlParameterWithHtmlEncoding(this string pathAndQuery, string key, string value)
		{
			return AddUrlParameter(pathAndQuery, key, value);
		}

		public static string RemoveUrlParameter(this string pathAndQuery, string key)
		{
			string anchor = null;
			string separator = "?";
			pathAndQuery = ExcludeUrlParameter(pathAndQuery, key, out separator, out anchor);
			if (!anchor.IsNullOrTrimmedEmpty())
			{
				pathAndQuery += anchor;
			}
			return pathAndQuery;
		}

		private static string ExcludeUrlParameter(this string pathAndQuery, string key, out string separator, out string anchor)
		{
			if (pathAndQuery == null)
			{
				anchor = string.Empty;
				separator = string.Empty;
				return pathAndQuery;
			}
			var parts = pathAndQuery.Split('?');
			var anchorParts = pathAndQuery.Split('#');
			anchor = string.Empty;
			var path = parts[0];
			separator = "?";

			if (anchorParts.Length > 1)
			{
				anchor = string.Format("#{0}", anchorParts[1]);
				path = path.Replace(anchor, string.Empty);
			}

			string parameters = null;
			var nvc = new NameValueCollection();
			if (parts.Length > 1)
			{
				parameters = parts[1];
				if (anchorParts.Length > 1)
				{
					parameters = parameters.Replace(anchor, string.Empty);
				}
				var list = parameters.Split('&');
				foreach (var token in list)
				{
					var keyvalue = token.Split('=');
					if (keyvalue.Length == 2)
					{
						var k = keyvalue[0];
						var value = keyvalue[1];
						nvc.Add(k, value);
					}
				}
			}

			pathAndQuery = path;
			foreach (var item in nvc.AllKeys)
			{
				if (item.Equals(key, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				var value = nvc[item];
				pathAndQuery += string.Format("{0}{1}={2}", separator, item, value);
				separator = "&";
			}
			return pathAndQuery;
		}

		public static string AddUrlParameters<T>(this string pathAndQuery, T input) where T : new()
		{
			if (input == null)
			{
				return pathAndQuery;
			}

			var list = input.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
			var ci = new System.Globalization.CultureInfo("en-US");

			foreach (var pi in list)
			{
				object value = pi.GetValue(input, null);
				if (value == null)
				{
					continue;
				}
				var s = string.Format(ci, "{0}", value);
				if (s.Length == 0)
				{
					continue;
				}

				pathAndQuery = pathAndQuery.AddUrlParameter(pi.Name, s);
			}
			return pathAndQuery;
		}

		public static int ToHostId(this Uri uri)
		{
			if (uri == null)
			{
				return 1;
			}
			var host = uri.Host;
			host = host.ToLower();
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

		public static string GetZoneAndTld(this System.Uri uri)
		{
			var parts = uri.Host.Split('.');
			string result = null;
			if (parts.Length >= 2)
			{
				result = parts[parts.Length - 2] + '.' + parts.Last();
			}
			return result;
		}


	}
}
