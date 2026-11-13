using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Tests.Domain.ValueObjects
{
	public class PhoneNumberTest
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("abc-1234")]
		[InlineData("555!1234")]
		[InlineData("++44 123456")]
		public void PhoneNumber_Invalid_ThrowsBusinessRuleException(string? invalidPhone)
		{
			Assert.Throws<BusinessRuleException>(() => new PhoneNumber(invalidPhone!));
		}

		[Fact]
		public void PhoneNumber_Valid_CreatesValueObject()
		{
			var phone = new PhoneNumber("(477) 123-4567");
			Assert.Equal("(477) 123-4567", phone.Value);
		}
	}
}
