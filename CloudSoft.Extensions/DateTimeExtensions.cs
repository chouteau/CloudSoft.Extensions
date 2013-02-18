using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Extensions
{
	public static class DateTimeExtensions
	{
		public static int ToDayId(this DateTime date)
		{
			var diff = (new DateTime(date.Year, date.Month, date.Day) - new DateTime(2000, 1, 1)).TotalDays;
			return Convert.ToInt32(diff);
		}

	}
}
