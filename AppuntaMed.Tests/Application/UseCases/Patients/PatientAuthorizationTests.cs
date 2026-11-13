using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Patients.Commands.UpdatePatient;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientDetail;
using AppuntaMed.Domain.Entities;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Patients
{
	public class PatientAuthorizationTests
	{
		private readonly IPatientRepository _repository = Substitute.For<IPatientRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
		private readonly Patient _ana = TestData.Patient("user-ana");

		public PatientAuthorizationTests()
		{
			_repository.GetById(_ana.Id).Returns(Task.FromResult<Patient?>(_ana));
		}

		private void LoginAs(string userId, string role)
		{
			_currentUser.UserId.Returns(userId);
			_currentUser.Roles.Returns(new[] { role });
		}

        private UpdatePatientCommand NewUpdate() => new()
        {
			Id = _ana.Id,
			FirstName = "Ana María",
			LastName = _ana.LastName,
			DateOfBirth = _ana.DateOfBirth,
			PhoneNumber = "5559999",
			Email = "ana@example.com"
        };

		[Fact]
        public async Task GetDetail_PatientReadsOwnProfile_ReturnsIt()
        {
			LoginAs("user-ana", "Patient");
			var handler = new GetPatientDetailHandler(_repository, _currentUser);
	
			var result = await handler.Handle(new GetPatientDetailQuery { Id = _ana.Id });
	
			Assert.Equal(_ana.FirstName, result.FirstName);
	    }

		[Fact]
        public async Task GetDetail_PatientReadsAnotherPatient_ThrowsForbidden()
        {
			LoginAs("user-beto", "Patient");
			var handler = new GetPatientDetailHandler(_repository, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(new GetPatientDetailQuery { Id = _ana.Id }));
	    }

	    [Theory]
		[InlineData("Admin")]
		[InlineData("Doctor")]
        public async Task GetDetail_StaffRoles_CanReadAnyPatient(string role)
        {
			LoginAs("staff-user", role);
			var handler = new GetPatientDetailHandler(_repository, _currentUser);
	
			var result = await handler.Handle(new GetPatientDetailQuery { Id = _ana.Id });
	
			Assert.Equal(_ana.LastName, result.LastName);
	    }

		[Fact]
        public async Task Update_PatientUpdatesOwnProfile_Succeeds()
        {
			LoginAs("user-ana", "Patient");
			var handler = new UpdatePatientHandler(_repository, _unitOfWork, _currentUser);
	
			await handler.Handle(NewUpdate());
	
			Assert.Equal("Ana María", _ana.FirstName);
			await _unitOfWork.Received(1).Commit();
	    }

		[Fact]
        public async Task Update_PatientUpdatesAnotherPatient_ThrowsForbidden_AndKeepsData()
        {
			LoginAs("user-beto", "Patient");
			var handler = new UpdatePatientHandler(_repository, _unitOfWork, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(NewUpdate()));
	
			Assert.Equal("Ana", _ana.FirstName);
			await _unitOfWork.DidNotReceive().Commit();
	    }

		[Fact]
        public async Task Update_Admin_CanUpdateAnyPatient()
        {
			LoginAs("admin-user", "Admin");
			var handler = new UpdatePatientHandler(_repository, _unitOfWork, _currentUser);
	
			await handler.Handle(NewUpdate());
	
			Assert.Equal("Ana María", _ana.FirstName);
	    }
	}
}
