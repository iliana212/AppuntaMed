using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Tests.Application.UseCases;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class DoctorUpdateTests
	{
		[Fact]
		public void UpdateSpeciality_ValidValue_ChangesSpecialityAndKeepsProfession()
		{
			var doctor = TestData.Doctor();
			var originalProfession = doctor.Profession;

			doctor.UpdateSpeciality("Neonatology");

			Assert.Equal("Neonatology", doctor.Speciality);
			Assert.Equal(originalProfession, doctor.Profession);
		}

		[Fact]
		public void UpdateProfession_ValidValue_ChangesProfessionAndKeepsSpeciality()
		{
			var doctor = TestData.Doctor();
			var originalSpeciality = doctor.Speciality;

			doctor.UpdateProfession("Pediatrics");

			Assert.Equal("Pediatrics", doctor.Profession);
			Assert.Equal(originalSpeciality, doctor.Speciality);
		}

		[Theory]
		[InlineData("")]
		[InlineData("   ")]
        public void UpdateSpeciality_BlankValue_ThrowsBusinessRuleException_AndKeepsSpeciality(string blank)
        {
			var doctor = TestData.Doctor();
			var original = doctor.Speciality;
	
			Assert.Throws<BusinessRuleException>(() => doctor.UpdateSpeciality(blank));
			Assert.Equal(original, doctor.Speciality);
	     }
	}
}
