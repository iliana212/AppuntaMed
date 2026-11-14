using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentDetail;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Appointments
{
	public class GetAppointmentDetailTests
	{
		private readonly IAppointmentRepository _repository = Substitute.For<IAppointmentRepository>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
		private readonly IDoctorRepository _doctorRepo = Substitute.For<IDoctorRepository>();
		private readonly IPatientRepository _patientRepo = Substitute.For<IPatientRepository>();


		[Fact]
		public async Task Handle_ValidId_ReturnsAppointmentDetailDTO()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1), new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");
			var doctor = new Doctor("Carlos", "Pérez", Guid.NewGuid(), null, new Email("carlos@example.com"), new PhoneNumber("5555678"), "xxxxx");
			var consultingRoom = new ConsultingRoom("Room A");
			var interval = new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

			var appointment = new Appointment(patient.Id, doctor.Id, consultingRoom.Id, interval);
			appointment.AssignReferences(patient, doctor, consultingRoom);

			_repository.GetById(appointment.Id)!.Returns(Task.FromResult(appointment));

			var handler = new GetAppointmentDetailHandler(_repository, _doctorRepo, _patientRepo, _currentUser);
			var query = new GetAppointmentDetailQuery { Id = appointment.Id };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(appointment.Id, result.Id);
			Assert.Equal($"{patient.FirstName} {patient.LastName}", result.Patient);
			Assert.Equal($"{doctor.FirstName} {doctor.LastName}", result.Doctor);
			Assert.Equal(consultingRoom.Name, result.ConsultingRoom);
			Assert.Equal(interval.Start, result.Start);
			Assert.Equal(interval.End, result.End);
			Assert.Equal(appointment.Status.ToString(), result.Status);
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			// Arrange
			var query = new GetAppointmentDetailQuery { Id = Guid.NewGuid() };
			_repository.GetById(query.Id).Returns(Task.FromResult<Appointment?>(null));

			var handler = new GetAppointmentDetailHandler(_repository, _doctorRepo, _patientRepo, _currentUser);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query));
		}
	}
}
