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
		private readonly Doctor _carlos = TestData.Doctor("user-carlos");

		public DoctorAuthorizationTests()
		{
			_repository.GetById(_carlos.Id).Returns(Task.FromResult<Doctor?>(_carlos));
		}

		private void LoginAs(string userId, string role)
		{
			_currentUser.UserId.Returns(userId);
			_currentUser.Roles.Returns(new[] { role });
		}

		private UpdateDoctorCommand NewUpdate() => new()
		{
			Id = _carlos.Id,
			FirstName = "Carlos",
			LastName = "Pérez",
			Profession = "Neurology",
			Speciality = "Epilepsy",
			Email = "carlos@example.com",
			PhoneNumber = "5555678"
		};

		[Fact]
        public async Task Update_DoctorUpdatesOwnProfile_ChangesProfessionAndSpecialityIndependently()
        {
			LoginAs("user-carlos", "Doctor");
			var handler = new UpdateDoctorHandler(_repository, _unitOfWork, _currentUser);
	
			await handler.Handle(NewUpdate());
	
			Assert.Equal("Neurology", _carlos.Profession);
			Assert.Equal("Epilepsy", _carlos.Speciality);
			await _unitOfWork.Received(1).Commit();
	    }

		[Fact]
        public async Task Update_DoctorUpdatesAnotherDoctor_ThrowsForbidden_AndKeepsData()
        {
			LoginAs("user-laura", "Doctor");
			var handler = new UpdateDoctorHandler(_repository, _unitOfWork, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(NewUpdate()));
	
			Assert.Equal("Cardiology", _carlos.Profession);
			await _unitOfWork.DidNotReceive().Commit();
	    }

		[Fact]
        public async Task Update_Admin_CanUpdateAnyDoctor()
        {
			LoginAs("admin-user", "Admin");
			var handler = new UpdateDoctorHandler(_repository, _unitOfWork, _currentUser);
	
			await handler.Handle(NewUpdate());
	
			Assert.Equal("Epilepsy", _carlos.Speciality);
	    }
	}
}
