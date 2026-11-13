using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class CreateDoctorTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		[Fact]
		public async Task Handle_ValidCommand_CreatesDoctorAndCommits()
		{
			var command = new CreateDoctorCommand { FirstName = "Ana", LastName = "Martínez", 
				Profession = "Cardiology", Speciality = "Heart Surgery", Email = "ana@example.com", PhoneNumber = "5551234" };

			var doctor = new Doctor(command.FirstName, command.LastName, command.Profession, command.Speciality, 
				new Email(command.Email), new PhoneNumber(command.PhoneNumber), "xxxxx");

			_repository.Add(Arg.Any<Doctor>()).Returns(Task.FromResult(doctor));

			var handler = new CreateDoctorHandler(_repository, _unitOfWork, _currentUser);
			var result = await handler.Handle(command);

			Assert.Equal(doctor.Id, result);
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			var command = new CreateDoctorCommand
			{
				FirstName = "Ana",
				LastName = "Martínez",
				Profession = "Cardiology",
				Speciality = "Heart Surgery",
				Email = "ana@example.com",
				PhoneNumber = "5551234"
			};

			_repository.Add(Arg.Any<Doctor>()).Throws(new Exception("DB error"));

			var handler = new CreateDoctorHandler(_repository, _unitOfWork, _currentUser);

			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);
			await _unitOfWork.Received(1).Rollback();
		}
	}
}
