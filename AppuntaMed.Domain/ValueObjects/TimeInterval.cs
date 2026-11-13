using AppuntaMed.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Domain.ValueObjects
{
	public record TimeInterval
	{
		public DateTime Start { get; }
		public DateTime End { get; }

		private TimeInterval() { }
		public TimeInterval(DateTime start, DateTime end)
		{
			if (start > end)
				throw new BusinessRuleException("Start date cannot be greater than End date");

			Start = start;
			End = end;
		}
	}
}
