using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Doctors.Commands.DeleteDoctor;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class DeleteDoctorTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly IAppointmentRepository _appointments = Substitute.For<IAppointmentRepository>();

		[Fact]
		public async Task Handle_ValidId_DeletesDoctorAndCommits()
		{
			var doctor = new Doctor("Carlos", "Pérez", Guid.NewGuid(), null, new Email("carlos@example.com"), new PhoneNumber("5555678"), "xxxxx"); _repository.GetById(doctor.Id)!.Returns(Task.FromResult(doctor));

			var command = new DeleteDoctorCommand { Id = doctor.Id };
			var handler = new DeleteDoctorHandler(_repository, _unitOfWork, _appointments);

			await handler.Handle(command);

			await _repository.Received(1).Delete(doctor);
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			var command = new DeleteDoctorCommand { Id = Guid.NewGuid() };
			_repository.GetById(command.Id).Returns(Task.FromResult<Doctor?>(null));

			var handler = new DeleteDoctorHandler(_repository, _unitOfWork, _appointments);

			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			var doctor = new Doctor("Carlos", "Pérez", Guid.NewGuid(), null, new Email("carlos@example.com"), new PhoneNumber("5555678"), "xxxxx"); _repository.GetById(doctor.Id)!.Returns(Task.FromResult(doctor));
			_repository.Delete(doctor).Throws(new Exception("DB error"));

			var command = new DeleteDoctorCommand { Id = doctor.Id };
			var handler = new DeleteDoctorHandler(_repository, _unitOfWork, _appointments);

			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);
			await _unitOfWork.Received(1).Rollback();
		}
	}
}
