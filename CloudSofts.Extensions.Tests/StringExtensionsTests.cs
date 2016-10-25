using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CloudSoft.Extensions;

using NFluent;

namespace CloudSofts.Extensions.Tests
{
	[TestClass]
	public class StringExtensionsTests
	{

		[TestMethod]
		public void Left()
		{
			var s = "123456789";
			var result = s.Left(5);

			Check.That(result.Length).IsEqualTo(5);
			Check.That(result).IsEqualTo("12345");

			result = s.Left(50);

			Check.That(result).IsEqualTo(s);

			result = s.Left(0);

			Check.That(result).IsEqualTo(s);

			result = s.Left(-1);

			Check.That(result).IsEqualTo(s);

			s = null;

			result = s.Left(10);

			Check.That(result).IsNull();


		}
	}
}