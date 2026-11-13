using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Appointments.Commands.CompleteAppointment;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Enums;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Appointments
{
	public class CompleteAppointmentTests
	{
		private readonly IAppointmentRepository _repository = Substitute.For<IAppointmentRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
		private readonly IDoctorRepository _doctorRepo = Substitute.For<IDoctorRepository>();

		[Fact]
		public async Task Handle_ValidAppointment_CompletesAndCommits()
		{
			var doctor = new Doctor("Ana", "Martínez", "Cardiology", "Heart Surgery", new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");

			var appointment = new Appointment(Guid.NewGuid(), doctor.Id, Guid.NewGuid(),
				new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));

			string userId = "test-user-id";
			_currentUser.UserId.Returns(userId);
			_doctorRepo.GetByUserId(userId)!.Returns(Task.FromResult(doctor));

			_repository.GetById(appointment.Id)!.Returns(Task.FromResult(appointment));

			var command = new CompleteAppointmentCommand { Id = appointment.Id };
			var handler = new CompleteAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);

			await handler.Handle(command);

			Assert.Equal(AppointmentStatus.Completed, appointment.Status);
			await _repository.Received(1).Update(appointment);
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			var command = new CompleteAppointmentCommand { Id = Guid.NewGuid() };
			_repository.GetById(command.Id).Returns(Task.FromResult<Appointment?>(null));

			var handler = new CompleteAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);

			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			var doctor = new Doctor("Ana", "Martínez", "Cardiology", "Heart Surgery", new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");
		
			var appointment = new Appointment(Guid.NewGuid(), doctor.Id, Guid.NewGuid(),
				new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));

			string userId = "test-user-id";
			_currentUser.UserId.Returns(userId);
			
			_doctorRepo.GetByUserId(userId)!.Returns(Task.FromResult(doctor));

			_repository.GetById(appointment.Id)!.Returns(Task.FromResult(appointment));
			_repository.Update(appointment).Throws(new Exception("DB error"));

			var command = new CompleteAppointmentCommand { Id = appointment.Id };
			var handler = new CompleteAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);

			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
		}
	}
}
