using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Doctors;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class DoctorSpecializationRulesTests
	{
		private readonly IProfessionRepository _professions = Substitute.For<IProfessionRepository>();
		private readonly Profession _dentist = TestData.Profession("Dentist", "Orthodontics");

		public DoctorSpecializationRulesTests()
		{
			_professions.GetByIdWithSpecialities(_dentist.Id).Returns(Task.FromResult<Profession?>(_dentist));
		}

		[Fact]
		public async Task EnsureValidSpecialization_ProfessionWithItsOwnSpeciality_DoesNotThrow()
		{
			await _professions.EnsureValidSpecialization(_dentist.Id, _dentist.Specialities.Single().Id);
		}

		[Fact]
        public async Task EnsureValidSpecialization_ProfessionWithoutSpeciality_DoesNotThrow()
        {
			await _professions.EnsureValidSpecialization(_dentist.Id, null);
	    }

		[Fact]
        public async Task EnsureValidSpecialization_UnknownProfession_ThrowsBusinessRule()
        {
			var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
			_professions.EnsureValidSpecialization(Guid.NewGuid(), null));
	
			Assert.Contains("profession", ex.Message);
	    }

		[Fact]
        public async Task EnsureValidSpecialization_UnknownSpeciality_ThrowsBusinessRule()
        {
			await Assert.ThrowsAsync<BusinessRuleException>(() =>
			_professions.EnsureValidSpecialization(_dentist.Id, Guid.NewGuid()));
	    }

		[Fact]
        public async Task EnsureValidSpecialization_SpecialityOfAnotherProfession_ThrowsBusinessRule()
        {
			var cardiology = TestData.Profession("Physician", "Cardiology").Specialities.Single();
	
			var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
			_professions.EnsureValidSpecialization(_dentist.Id, cardiology.Id));
	
			Assert.Contains("speciality", ex.Message);
	    }
    }
}
