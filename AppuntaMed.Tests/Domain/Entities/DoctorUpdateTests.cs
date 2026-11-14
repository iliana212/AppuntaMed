using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Tests.Application.UseCases;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class DoctorUpdateTests
	{
		[Fact]
		public void UpdateSpecialization_ChangesProfessionAndSpecialityTogether()
		{
			var doctor = TestData.Doctor();
			var dentist = TestData.Profession("Dentist", "Orthodontics");
			var orthodontics = dentist.Specialities.Single();

			doctor.UpdateSpecialization(dentist.Id, orthodontics.Id);

			Assert.Equal(dentist.Id, doctor.ProfessionId);
			Assert.Equal(orthodontics.Id, doctor.SpecialityId);
		}

		[Fact]
		public void UpdateSpecialization_NullSpeciality_ClearsSpeciality()
		{
			var doctor = TestData.Doctor();
			var professionId = doctor.ProfessionId;

			doctor.UpdateSpecialization(professionId, null);

			Assert.Equal(professionId, doctor.ProfessionId);
			Assert.Null(doctor.SpecialityId);
		}

		[Fact]
		public void UpdateSpecialization_EmptyProfessionId_ThrowsAndKeepsValues()
		{
			var doctor = TestData.Doctor();
			var professionId = doctor.ProfessionId;
			var specialityId = doctor.SpecialityId;

			Assert.Throws<BusinessRuleException>(() => doctor.UpdateSpecialization(Guid.Empty, null));

			Assert.Equal(professionId, doctor.ProfessionId);
			Assert.Equal(specialityId, doctor.SpecialityId);
		}

		[Fact]
		public void UpdateSpecialization_EmptySpecialityId_Throws()
		{
			var doctor = TestData.Doctor();
			Assert.Throws<BusinessRuleException>(() => doctor.UpdateSpecialization(doctor.ProfessionId, Guid.Empty));
		}

		[Fact]
		public void AssignCatalog_MatchingIds_ExposesNames()
		{
			var doctor = TestData.Doctor();

			Assert.Equal("Physician", doctor.Profession.Name);
			Assert.Equal("Cardiology", doctor.Speciality!.Name);
		}

		[Fact]
		public void AssignCatalog_ProfessionOfAnotherDoctor_ThrowsBusinessRuleException()
		{
			var doctor = TestData.Doctor();
			var other = TestData.Profession("Dentist", "Orthodontics");

			Assert.Throws<BusinessRuleException>(() => doctor.AssignCatalog(other, other.Specialities.Single()));
		}
	}
}
