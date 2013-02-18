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
			var typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };

			var pi = typeof(T).GetProperty(memberName);

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
			var typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };

			var pi = typeof(T).GetProperty(memberName);

			return (IOrderedQueryable<T>)query.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable),
					"OrderByDescending",
					new Type[] { typeof(T), pi.PropertyType },
					query.Expression,
					Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
			);
		}

	}
}
