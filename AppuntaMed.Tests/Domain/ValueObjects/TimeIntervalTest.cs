using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Tests.Domain.ValueObjects
{
	public class TimeIntervalTest
	{
		[Fact]
		public void StartDateGreaterThanEndDate_ThrowsException()
		{
			Assert.Throws<BusinessRuleException>(() =>
			{
				new TimeInterval(DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));
			});
		}

		[Fact]
		public void StartDate_EndDate_Valid()
		{
			var start = DateTime.UtcNow;
			var end = DateTime.UtcNow.AddDays(1);

			var ex = Record.Exception(() => new TimeInterval(start, end));

			Assert.Null(ex);
		}
	}
}
