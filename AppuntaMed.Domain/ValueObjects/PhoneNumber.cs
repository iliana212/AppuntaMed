using AppuntaMed.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace AppuntaMed.Domain.ValueObjects
{
	public record PhoneNumber
	{
		public string Value { get; } = null!;

		private PhoneNumber() { }
		public PhoneNumber(string phoneNumber)
		{
			if (string.IsNullOrWhiteSpace(phoneNumber))
				throw new BusinessRuleException($"{nameof(phoneNumber)} is Required");
			
			if (!Regex.IsMatch(phoneNumber, @"^\+?[0-9\s\-\(\)]{7,20}$"))
				throw new BusinessRuleException($"{nameof(phoneNumber)} is not valid");

			Value = phoneNumber;
		}
	}
}
