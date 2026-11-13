using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Appointments.Commands.NoShowAppointment;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Enums;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Appointments
{
	public class NoShowAppointmentTests
	{
		private readonly IAppointmentRepository _repository = Substitute.For<IAppointmentRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly IDoctorRepository _doctorRepo = Substitute.For<IDoctorRepository>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		[Fact]
		public async Task Handle_ValidAppointment_MarksNoShowAndCommits()
		{
			var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
				new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));

			_repository.GetById(appointment.Id)!.Returns(Task.FromResult(appointment));

			var command = new NoShowAppointmentCommand { Id = appointment.Id };
			var handler = new NoShowAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);

			await handler.Handle(command);

			Assert.Equal(AppointmentStatus.NoShow, appointment.Status);
			await _repository.Received(1).Update(appointment);
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			var command = new NoShowAppointmentCommand { Id = Guid.NewGuid() };
			_repository.GetById(command.Id).Returns(Task.FromResult<Appointment?>(null));

			var handler = new NoShowAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);

			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
				new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));

			_repository.GetById(appointment.Id)!.Returns(Task.FromResult(appointment));
			_repository.Update(appointment).Throws(new Exception("DB error"));

			var command = new NoShowAppointmentCommand { Id = appointment.Id };
			var handler = new NoShowAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);

			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
		}
	}
}
