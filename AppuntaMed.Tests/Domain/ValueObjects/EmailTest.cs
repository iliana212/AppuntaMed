using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Tests.Domain.ValueObjects
{
	public class EmailTest
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("doctor.com")]
		[InlineData("doctor@")]
		[InlineData("doctor@com")]
		public void Email_Invalid_ThrowsBusinessRuleException(string? invalidEmail)
		{
			Assert.Throws<BusinessRuleException>(() => new Email(invalidEmail!));
		}

		[Fact]
		public void Email_Valid_CreatesValueObject()
		{
			var email = new Email("doctor@example.com");
			Assert.Equal("doctor@example.com", email.Value);
		}
	}
}
