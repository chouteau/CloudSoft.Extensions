using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using CloudSoft.Extensions;

using NFluent;

namespace CloudSofts.Extensions.Tests
{
	[TestClass]
	public class IQueryableExtensionsTests
	{
		public class Test
		{
			public int Property1 { get; set; }
			public string Property2 { get; set; }
		}

		[TestMethod]
		public void Column_Exists()
		{
			var list = new List<Test>();

			var query = from item in list.AsQueryable()
						select item;


			var result = query.IsColumnExists("property1");

			Check.That(result).IsTrue();
		}

		[TestMethod]
		public void Column_Not_Exists()
		{
			var list = new List<Test>();

			var query = from item in list.AsQueryable()
						select item;


			var result = query.IsColumnExists("property");

			Check.That(result).IsFalse();
		}

	}
}
