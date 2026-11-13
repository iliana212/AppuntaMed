using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AppuntaMed.Persistence.Utilities
{
	public static class IQueryableExtensions
	{
		public static IQueryable<T> Pagination<T>(this IQueryable<T> queryable, int page, int pageSize)
		{
			return queryable
				.Skip((page - 1) * pageSize)
				.Take(pageSize);
		}

	}
}
