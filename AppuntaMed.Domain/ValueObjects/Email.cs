using AppuntaMed.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace AppuntaMed.Domain.ValueObjects
{
	public record Email
	{
		public string Value { get; } = null!;

		private Email() { }
		public Email(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				throw new BusinessRuleException($"{nameof(email)} is Required");

			if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
				throw new BusinessRuleException($"{nameof(email)} is not valid");
		
			Value = email;
		}
	}
}
