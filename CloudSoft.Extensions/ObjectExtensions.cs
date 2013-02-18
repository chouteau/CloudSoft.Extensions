using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Configuration;

namespace CloudSoft.Extensions
{
	/// <summary>
	/// Extensions de methodes
	/// </summary>
	public static class ObjectExtensions
	{
		public static Dictionary<string, string> GetProperties(this object input)
		{
			var properties = input.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			var result = new Dictionary<string, string>();
			foreach (var property in properties)
			{
				var value = property.GetValue(input, null);
				if (value == null)
				{
					value = string.Empty;
				}
				result.Add(property.Name, value.ToString());
			}
			return result;
		}

		public static object GetPropertyValue(this object value, string propertyName)
		{
			var propertyInfo = value.GetType().GetProperty(propertyName);
			return propertyInfo.GetValue(value, null);
		}

		/// <summary>
		/// Retourne l'objet serialisé au format xml
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string SerializeToXml(this object value)
		{
			StringBuilder sb = new StringBuilder();
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				try
				{
					var serializer = new System.Xml.Serialization.XmlSerializer(value.GetType());
					serializer.Serialize(ms, value);
					sb.Append(System.Text.Encoding.UTF8.GetString(ms.GetBuffer()));
				}
				catch
				{
				}
				finally
				{
					ms.Close();
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Return the unmanaged size of an object, in bytes.
		/// </summary>
		/// <param name="value">the object to measure</param>
		/// <returns>The unmanaged size of an object in bytes.</returns>
		public static long SizeOf(this object value)
		{
			long size = 0;
			using (var m = new System.IO.MemoryStream())
			{
				var b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				b.Serialize(m, value);
				size = m.Length;
				m.Close();
			}
			return size;
		}

	}
}
