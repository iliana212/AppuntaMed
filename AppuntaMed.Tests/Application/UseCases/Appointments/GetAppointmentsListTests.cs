using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Repositories.Models;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Tests.Application.UseCases.Appointments
{
	public class GetAppointmentsListTests
	{
		private readonly IAppointmentRepository _repository = Substitute.For<IAppointmentRepository>();
		private readonly IDoctorRepository _doctorRepo = Substitute.For<IDoctorRepository>();
		private readonly IPatientRepository _patientRepo = Substitute.For<IPatientRepository>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		[Fact]
		public async Task Handle_ReturnsListOfAppointments_WhenAppointmentsExist()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1), new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");
			var doctor = new Doctor("Carlos", "Pérez", "Cardiology", "Heart Surgery", new Email("carlos@example.com"), new PhoneNumber("5555678"), "xxxxx");
			var consultingRoom = new ConsultingRoom("Room A");
			var interval = new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

			var appointment = new Appointment(patient.Id, doctor.Id, consultingRoom.Id, interval);
			appointment.AssignReferences(patient, doctor, consultingRoom);
			
			var appointments = new List<Appointment>
			{
				appointment
			};		

			_repository.GetFilterData(Arg.Any<AppointmentsFilterDTO>())
				.Returns(Task.FromResult(appointments.AsEnumerable()));

			var handler = new GetAppointmentsListHandler(_repository, _doctorRepo, _patientRepo, _currentUser);
			var query = new GetAppointmentsListQuery();

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal($"{patient.FirstName} {patient.LastName}", result[0].Patient);
			Assert.Equal($"{doctor.FirstName} {doctor.LastName}", result[0].Doctor);
			Assert.Equal(consultingRoom.Name, result[0].ConsultingRoom);
			Assert.Equal(interval.Start, result[0].Start);
			Assert.Equal(interval.End, result[0].End);
		}

		[Fact]
		public async Task Handle_ReturnsEmptyList_WhenNoAppointmentsExist()
		{
			// Arrange
			_repository.GetFilterData(Arg.Any<AppointmentsFilterDTO>())
				.Returns(Task.FromResult(Enumerable.Empty<Appointment>()));

			var handler = new GetAppointmentsListHandler(_repository, _doctorRepo, _patientRepo, _currentUser);
			var query = new GetAppointmentsListQuery();

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task Handle_MapsAppointmentsToDTOsCorrectly()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1), new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");
			var doctor = new Doctor("Carlos", "Pérez", "Cardiology", "Heart Surgery", new Email("carlos@example.com"), new PhoneNumber("5555678"), "xxxxx");
			var consultingRoom = new ConsultingRoom("Room A");
			var interval = new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

			var appointment = new Appointment(patient.Id, doctor.Id, consultingRoom.Id, interval);
			appointment.AssignReferences(patient, doctor, consultingRoom);

			_repository.GetFilterData(Arg.Any<AppointmentsFilterDTO>())
				.Returns(Task.FromResult(new List<Appointment> { appointment }.AsEnumerable()));

			var handler = new GetAppointmentsListHandler(_repository, _doctorRepo, _patientRepo, _currentUser);
			var query = new GetAppointmentsListQuery();

			// Act
			var result = await handler.Handle(query);
			var dto = result.First();

			// Assert
			Assert.Equal($"{patient.FirstName} {patient.LastName}", dto.Patient);
			Assert.Equal($"{doctor.FirstName} {doctor.LastName}", dto.Doctor);
			Assert.Equal(consultingRoom.Name, dto.ConsultingRoom);
			Assert.Equal(interval.Start, dto.Start);
			Assert.Equal(interval.End, dto.End);
			Assert.Equal(appointment.Status.ToString(), dto.Status);
		}
	}
}
