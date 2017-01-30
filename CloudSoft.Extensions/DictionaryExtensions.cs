using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Extensions
{
	public static class DictionaryExtensions
	{
		public static Dictionary<string, string> ToDictionary(this System.Collections.Specialized.NameValueCollection nvc)
		{
			var result = new Dictionary<string, string>();
			foreach (var key in nvc.AllKeys)
			{
				result.Add(key, nvc[key]);
			}
			return result;
		}

		public static string GetValueOrDefault(this Dictionary<string, string> input, string key)
		{
			if (input.Keys.Contains(key))
			{
				return input[key];
			}
			return null;
		}

		public static T GetValueWithIgnoreCaseKey<T>(this Dictionary<string, object> dic, string key)
		{
			if (dic == null)
			{
				return default(T);
			}

			var dicKey = dic.Keys.FirstOrDefault(i => i.Equals(key, StringComparison.InvariantCultureIgnoreCase));
			if (dicKey == null)
			{
				return default(T);
			}

			var value = dic[dicKey];

			var result = (T)Convert.ChangeType(value, typeof(T));
			return result;
		}

	}
}
