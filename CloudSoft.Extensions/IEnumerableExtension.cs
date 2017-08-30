using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Extensions
{
	/// <summary>
	/// Methodes d'extension pour les listes utilisants IEnumerable et qui ne sont pas couverte par System.Linq
	/// </summary>
	public static class IEnumerableExtension
	{
		/// <summary>
		/// Determines whether [is null or empty] [the specified list].
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <returns>
		/// 	<c>true</c> if [is null or empty] [the specified list]; otherwise, <c>false</c>.
		/// </returns>
		public static bool ListIsNullOrEmpty<T>(IEnumerable<T> list)
		{
			return (list == null || list.Count() == 0);
		}

		/// <summary>
		/// Determines whether [is null or empty] [the specified list].
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <returns>
		/// 	<c>true</c> if [is null or empty] [the specified list]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
		{
			return (list == null || list.Count() == 0);
		}

		/// <summary>
		/// Ins the not null or empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <returns></returns>
		[Obsolete("Use !IsNullOrEmpty instead", true)]
		public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> list)
		{
			return (list != null && list.Count() > 0);
		}

		/// <summary>
		/// Firsts the or new default.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <returns></returns>
		public static T FirstOrNewDefault<T>(this IEnumerable<T> list)
			 where T : new()
		{
			T result = list.FirstOrDefault();
			if (result == null)
			{
				return new T();
			}
			return result;
		}

		/// <summary>
		/// Firsts the or new default.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public static T FirstOrNewDefault<T>(this IEnumerable<T> list, Func<T, bool> predicate)
			where T : new()
		{
			T result = list.FirstOrDefault(predicate);
			if (result == null)
			{
				return new T();
			}
			return result;
		}

		/// <summary>
		/// Joins the string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public static string JoinString<T>(this IEnumerable<T> list, string separator, Func<T, string> predicate)
		{
			return string.Join(separator, list.Select(predicate).ToArray());
		}

		/// <summary>
		/// Joins the string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <param name="separator">The separator.</param>
		/// <returns></returns>
		public static string JoinString<T>(this IEnumerable<T> list, string separator)
		{
			return string.Join(separator, list.Select(i => Convert.ToString(i)).ToArray());
		}

		/// <summary>
		/// Removes all item match predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public static int RemoveAll<T>(this IList<T> list, Func<T, bool> predicate)
		{
			if (list.IsNullOrEmpty())
			{
				return 0;
			}
			int count = 0;
			while (true)
			{
				T item = list.FirstOrDefault(predicate);
				if (item == null)
				{
					break;
				}
				list.Remove(item);
				count++;
			}
			return count;
		}

		/// <summary>
		/// Removes all item match predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public static int RemoveAll<T>(this ICollection<T> list, Func<T, bool> predicate)
		{
			if (list.IsNullOrEmpty())
			{
				return 0;
			}
			int count = 0;
			while (true)
			{
				T item = list.FirstOrDefault(predicate);
				if (item == null)
				{
					break;
				}
				list.Remove(item);
				count++;
			}
			return count;
		}

		/// <summary>
		/// Permet de traverser une liste reccursive
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="recursiveFunction">The recursive function.</param>
		/// <returns></returns>
		public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursiveFunction)
		{
			foreach (T item in source)
			{
				yield return item;

				IEnumerable<T> seqRecurse = recursiveFunction(item);

				if (seqRecurse != null)
				{
					foreach (T itemRecurse in Traverse(seqRecurse, recursiveFunction))
					{
						yield return itemRecurse;
					}
				}
			}
		}

		public static bool IsIndexPair<T>(this IList<T> source, T item)
		{
			var index = source.IndexOf(item);
			return index / 2.0 != index / 2;
		}

		public static int ColumnIndex<T>(this IList<T> source, T item, int columnCount)
		{
			var index = source.IndexOf(item);
			return (index % columnCount) + 1;
		}

		public static string ColumnIndexName<T>(this IList<T> source, T item, int columnCount, string prefix)
		{
			var index = source.IndexOf(item);
			return string.Format("{0}{1}", prefix, (index % columnCount) + 1);
		}

		/// <summary>
		/// Extend distinct by property selector to compare
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="items"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
		{
			var keys = new HashSet<TKey>();
			foreach (T item in items)
			{
				var selected = selector(item);
				if (keys.Add(selected))
				{
					yield return item;
				}
			}
		}


	}
}
