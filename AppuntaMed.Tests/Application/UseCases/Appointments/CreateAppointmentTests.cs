using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Notifications;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Appointments.Commands.CreateAppointment;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Appointments
{
	public class CreateAppointmentTests
	{
		private readonly IAppointmentRepository _repository = Substitute.For<IAppointmentRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly INotificationService _notification = Substitute.For<INotificationService>();
		private readonly IPatientRepository _patientRepo = Substitute.For<IPatientRepository>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		[Fact]
		public async Task Handle_DuplicateAppointment_ThrowsValidationException()
		{
			// Arrange
			var command = new CreateAppointmentCommand
			{
				DoctorId = Guid.NewGuid(),
				PatientId = Guid.NewGuid(),
				ConsultingRoomId = Guid.NewGuid(),
				Start = DateTime.UtcNow.AddHours(1),
				End = DateTime.UtcNow.AddHours(2)
			};

			_repository.DuplicateAppointmentTime(command.DoctorId, command.Start, command.End)
				.Returns(Task.FromResult(true));

			var handler = new CreateAppointmentHandler(_repository, _unitOfWork, _notification, _patientRepo, _currentUser);

			// Act & Assert
			await Assert.ThrowsAsync<ValidationExceptions>(() => handler.Handle(command));			
			await _unitOfWork.DidNotReceive().Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		[Fact]
		public async Task Handle_ValidAppointment_CreatesAndCommits()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1), new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");
			var doctor = new Doctor("Carlos", "Pérez", Guid.NewGuid(), null, new Email("carlos@example.com"), new PhoneNumber("5555678"), "xxxxx");
			var consultingRoom = new ConsultingRoom("Room A");

			var command = new CreateAppointmentCommand
			{
				DoctorId = doctor.Id,
				PatientId = patient.Id,
				ConsultingRoomId = consultingRoom.Id,
				Start = DateTime.UtcNow.AddHours(1),
				End = DateTime.UtcNow.AddHours(2)
			};

			var appointment = new Appointment(command.PatientId, command.DoctorId, command.ConsultingRoomId,
				new TimeInterval(command.Start, command.End));

			appointment.AssignReferences(patient, doctor, consultingRoom);

			_repository.DuplicateAppointmentTime(command.DoctorId, command.Start, command.End)
				.Returns(Task.FromResult(false));
			_repository.Add(Arg.Any<Appointment>()).Returns(Task.FromResult(appointment));

			_repository.GetById(Arg.Any<Guid>())!.Returns(Task.FromResult(appointment));

			var handler = new CreateAppointmentHandler(_repository, _unitOfWork, _notification, _patientRepo, _currentUser);

			// Act
			var result = await handler.Handle(command);

			// Assert
			Assert.Equal(appointment.Id, result);
			await _unitOfWork.Received(1).Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			// Arrange
			var command = new CreateAppointmentCommand
			{
				DoctorId = Guid.NewGuid(),
				PatientId = Guid.NewGuid(),
				ConsultingRoomId = Guid.NewGuid(),
				Start = DateTime.UtcNow.AddHours(1),
				End = DateTime.UtcNow.AddHours(2)
			};

			_repository.DuplicateAppointmentTime(command.DoctorId, command.Start, command.End)
				.Returns(Task.FromResult(false));
			_repository.Add(Arg.Any<Appointment>()).Throws(new Exception("DB error"));

			var handler = new CreateAppointmentHandler(_repository, _unitOfWork, _notification, _patientRepo, _currentUser);

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public void Handle_InvalidTimeInterval_ThrowsBusinessRuleException()
		{
			// Arrange
			var start = DateTime.UtcNow.AddHours(3);
			var end = DateTime.UtcNow.AddHours(1); // End antes que Start

			// Act & Assert
			Assert.Throws<BusinessRuleException>(() =>
				new TimeInterval(start, end));
		}
	}
}
