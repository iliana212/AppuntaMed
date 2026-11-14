using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Doctors.Commands.UpdateDoctor;
using AppuntaMed.Domain.Entities;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class DoctorAuthorizationTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
		private readonly IProfessionRepository _professions = Substitute.For<IProfessionRepository>();

		private readonly Doctor _carlos = TestData.Doctor("user-carlos");
		private readonly Profession _dentist = TestData.Profession("Dentist", "Orthodontics");

		public DoctorAuthorizationTests()
		{
			_repository.GetById(_carlos.Id).Returns(Task.FromResult<Doctor?>(_carlos));
			_professions.GetByIdWithSpecialities(_dentist.Id).Returns(Task.FromResult<Profession?>(_dentist));
		}

		private void LoginAs(string userId, string role)
		{
			_currentUser.UserId.Returns(userId);
			_currentUser.Roles.Returns(new[] { role });
		}

		private UpdateDoctorHandler CreateHandler() => new(_repository, _unitOfWork, _currentUser, _professions);

		private UpdateDoctorCommand NewUpdate() => new()
		{
			Id = _carlos.Id,
			FirstName = "Carlos",
			LastName = "Pérez",
			ProfessionId = _dentist.Id,
			SpecialityId = _dentist.Specialities.Single().Id,
			Email = "carlos@example.com",
			PhoneNumber = "5555678"
		};

		[Fact]
		public async Task Update_DoctorUpdatesOwnProfile_ChangesProfessionAndSpeciality()
		{
			LoginAs("user-carlos", "Doctor");
			await CreateHandler().Handle(NewUpdate());

			Assert.Equal(_dentist.Id, _carlos.ProfessionId);
			Assert.Equal(_dentist.Specialities.Single().Id, _carlos.SpecialityId);

			await _unitOfWork.Received(1).Commit();
	    }

		[Fact]
		public async Task Update_DoctorUpdatesAnotherDoctor_ThrowsForbidden_AndKeepsData()
		{
			var originalProfessionId = _carlos.ProfessionId;
			LoginAs("user-laura", "Doctor");

			await Assert.ThrowsAsync<ForbiddenException>(() => CreateHandler().Handle(NewUpdate()));
			Assert.Equal(originalProfessionId, _carlos.ProfessionId);

			await _unitOfWork.DidNotReceive().Commit();
	    }

		[Fact]
        public async Task Update_Admin_CanUpdateAnyDoctor()
        {
			LoginAs("admin-user", "Admin");
			await CreateHandler().Handle(NewUpdate());
			
			Assert.Equal(_dentist.Id, _carlos.ProfessionId);
		}
	}
}
