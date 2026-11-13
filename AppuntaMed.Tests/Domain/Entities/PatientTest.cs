using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class PatientTest
	{
		private readonly Email email = new("doctor@example.com");
		private readonly PhoneNumber phone = new("555-1234");
		private readonly DateTime dateOfBirth = new(1980, 02, 21);

		[Theory]
		[InlineData("John", "Doe", "1978-03-09")]
		[InlineData("Ana", "Martínez", "1999-07-20")]
		public void Constructor_ValidData_CreatesDoctor(string firstName, string lastName, DateTime dateBirth)
		{
			var patient = new Patient(firstName, lastName, dateBirth, phone, email, "xxxxx");

			Assert.NotNull(patient);
			Assert.Equal(firstName, patient.FirstName);
			Assert.Equal(lastName, patient.LastName);
			Assert.Equal(dateBirth, patient.DateOfBirth);
			Assert.Equal(email, patient.Email);
			Assert.Equal(phone, patient.PhoneNumber);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Constructor_InvalidFirstName_ThrowsBusinessRuleException(string? invalidFirstName)
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Patient(invalidFirstName!, "Doe", dateOfBirth, phone, email, "xxxxx"));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Constructor_InvalidLastName_ThrowsBusinessRuleException(string? invalidLastName)
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Patient("John", invalidLastName!, dateOfBirth, phone, email, "xxxxx"));
		}

		[Fact]
		public void Constructor_InvalidDateOfBirth_ThrowsBusinessRuleException()
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Patient("John", "Doe", DateTime.MinValue, phone, email, "xxxxx"));
		}
	}
}
