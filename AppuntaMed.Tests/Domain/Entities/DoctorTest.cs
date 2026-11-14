using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class DoctorTest
	{
		private readonly Email email = new("doctor@example.com");
		private readonly PhoneNumber phone = new("555-1234");
		private readonly Guid professionId = Guid.NewGuid();
		private readonly Guid specialityId = Guid.NewGuid();

		[Theory]
		[InlineData("John", "Doe")]
		[InlineData("Ana", "Martínez")]
		[InlineData("Luis", "García")]
		public void Constructor_ValidData_CreatesDoctor(string firstName, string lastName)
		{
			var doctor = new Doctor(firstName, lastName, professionId, specialityId, email, phone, "xxxxx");

			Assert.NotEqual(Guid.Empty, doctor.Id);
			Assert.Equal(firstName, doctor.FirstName);
			Assert.Equal(lastName, doctor.LastName);
			Assert.Equal(email, doctor.Email);
			Assert.Equal(phone, doctor.PhoneNumber);
			Assert.Equal(professionId, doctor.ProfessionId);
			Assert.Equal(specialityId, doctor.SpecialityId);
		}

		[Fact]
		public void Constructor_WithoutSpeciality_CreatesDoctorWithNullSpeciality()
		{
			var doctor = new Doctor("John", "Doe", professionId, null, email, phone, "xxxxx");

			Assert.Equal(professionId, doctor.ProfessionId);
			Assert.Null(doctor.SpecialityId);
		}

		[Fact]
		public void Constructor_EmptyProfessionId_ThrowsBusinessRuleException()
		{
			Assert.Throws<BusinessRuleException>(() => new Doctor("John", "Doe", Guid.Empty, null, email, phone, "xxxxx"));
		}

		[Fact]
        public void Constructor_EmptySpecialityId_ThrowsBusinessRuleException()
        {
			Assert.Throws<BusinessRuleException>(() => new Doctor("John", "Doe", professionId, Guid.Empty, email, phone, "xxxxx"));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Constructor_InvalidFirstName_ThrowsBusinessRuleException(string? invalidFirstName)
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Doctor(invalidFirstName!, "Doe", professionId, specialityId, email, phone, "xxxxx"));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Constructor_InvalidLastName_ThrowsBusinessRuleException(string? invalidLastName)
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Doctor("John", invalidLastName!, professionId, specialityId, email, phone, "xxxxx"));
		}

		[Fact]
		public void Constructor_NullEmail_ThrowsBusinessRuleException()
		{
			Assert.Throws<BusinessRuleException>(() =>
				new Doctor("John", "Doe", professionId, specialityId, null!, phone, "xxxxx"));
		}	
	}
}
