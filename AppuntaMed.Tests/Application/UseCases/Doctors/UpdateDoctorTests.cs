using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Doctors.Commands.UpdateDoctor;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class UpdateDoctorTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		[Fact]
		public async Task Handle_ValidId_UpdatesDoctorAndCommits()
		{
			var doctor = new Doctor("Ana", "Martínez", "Cardiology", "Heart Surgery", new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");
			_repository.GetById(doctor.Id)!.Returns(Task.FromResult(doctor));

			var command = new UpdateDoctorCommand
			{
				Id = doctor.Id,
				FirstName = "Ana",
				LastName = "Martínez",
				Profession = "Cardiology",
				Speciality = "Heart Surgery",
				Email = "ana@example.com",
				PhoneNumber = "5551234"
			};

			var handler = new UpdateDoctorHandler(_repository, _unitOfWork, _currentUser);

			await handler.Handle(command);
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			var command = new UpdateDoctorCommand
			{
				Id = Guid.NewGuid(),
				FirstName = "Ana",
				LastName = "Martínez",
				Profession = "Cardiology",
				Speciality = "Heart Surgery",
				Email = "ana@example.com",
				PhoneNumber = "5551234"
			}; ;
			
			_repository.GetById(command.Id).Returns(Task.FromResult<Doctor?>(null));

			var handler = new UpdateDoctorHandler(_repository, _unitOfWork, _currentUser);

			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			var doctor = new Doctor("Ana", "Martínez", "Cardiology", "Heart Surgery", new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");
			_repository.GetById(doctor.Id)!.Returns(Task.FromResult(doctor));
			_repository.Update(doctor).Throws(new Exception("DB error"));

			var command = new UpdateDoctorCommand
			{
				Id = doctor.Id,
				FirstName = "Ana",
				LastName = "Martínez",
				Profession = "Cardiology",
				Speciality = "Heart Surgery",
				Email = "ana@example.com",
				PhoneNumber = "5551234"
			};

			var handler = new UpdateDoctorHandler(_repository, _unitOfWork, _currentUser);

			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);
			await _unitOfWork.Received(1).Rollback();
		}
	}
}
