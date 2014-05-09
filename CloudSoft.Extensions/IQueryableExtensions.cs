using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace CloudSoft.Extensions
{
	public static class IQueryableExtensions
	{
		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string memberName)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query does not be null");
			}

			if (memberName.IsNullOrTrimmedEmpty())
			{
				throw new ArgumentException("memberName does not be null or empty");
			}

			if (!query.IsColumnExists(memberName))
			{
				string message = string.Format("Sort Column {0} does not exists in query {1}", memberName, query.ToString());
				throw new System.Data.InvalidExpressionException(message);
			}

			var typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };

			var pi = typeof(T).GetProperty(memberName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			return (IOrderedQueryable<T>)query.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable),
					"OrderBy",
					new Type[] { typeof(T), pi.PropertyType },
					query.Expression,
					Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
			);
		}

		public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string memberName)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query does not be null");
			}

			if (memberName.IsNullOrTrimmedEmpty())
			{
				throw new ArgumentException("memberName does not be null or empty");
			}

			if (!query.IsColumnExists(memberName))
			{
				string message = string.Format("Sort Column {0} does not exists in query {1}", memberName, query.ToString());
				throw new System.Data.InvalidExpressionException(message);
			}

			var typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };

			var pi = typeof(T).GetProperty(memberName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			return (IOrderedQueryable<T>)query.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable),
					"OrderByDescending",
					new Type[] { typeof(T), pi.PropertyType },
					query.Expression,
					Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
			);
		}

		public static bool IsColumnExists<T>(this IQueryable<T> query, string columnName)
		{
			var elementType = query.ElementType;
			foreach (var pi in elementType.GetProperties())
			{
				if (pi.Name.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

	}
}
