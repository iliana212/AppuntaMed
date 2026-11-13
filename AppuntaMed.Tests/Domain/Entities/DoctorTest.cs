using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class DoctorTest
	{
		private readonly Email email = new("doctor@example.com");
		private readonly PhoneNumber phone = new("555-1234");

		[Theory]
		[InlineData("John", "Doe", "Cardiology", "Heart Surgery")]
		[InlineData("Ana", "Martínez", "Pediatrics", "Neonatology")]
		[InlineData("Luis", "García", "Neurology", "Epilepsy")]
		public void Constructor_ValidData_CreatesDoctor(string firstName,string lastName,string profession,string speciality)
		{
			var doctor = new Doctor(firstName, lastName, profession, speciality, email, phone, "xxxxx");

			Assert.NotNull(doctor);
			Assert.Equal(firstName, doctor.FirstName);
			Assert.Equal(lastName, doctor.LastName);
			Assert.Equal(email, doctor.Email);
			Assert.Equal(phone, doctor.PhoneNumber);
			Assert.Equal(profession, doctor.Profession);
			Assert.Equal(speciality, doctor.Speciality);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Constructor_InvalidFirstName_ThrowsBusinessRuleException(string? invalidFirstName)
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Doctor(invalidFirstName!, "Doe", "Cardiology", "Heart Surgery", email, phone, "xxxxx"));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Constructor_InvalidLastName_ThrowsBusinessRuleException(string? invalidLastName)
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Doctor("John", invalidLastName!, "Cardiology", "Heart Surgery", email, phone, "xxxxx"));
		}

		[Fact]
		public void Constructor_NullEmail_ThrowsBusinessRuleException()
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Doctor("John", "Doe", "Cardiology", "Heart Surgery", null!, phone, "xxxxx"));
		}	
	}
}
