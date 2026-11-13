using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Enums;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class AppointmentTest
	{
		private Guid patientId = Guid.NewGuid();
		private Guid doctorId = Guid.NewGuid();
		private Guid roomId = Guid.NewGuid();
		private TimeInterval interval = new(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));


		[Fact]
		public void Constructor_ValidData_CreatesScheduledAppointment()
		{
			var appointment = new Appointment(patientId, doctorId, roomId, interval);

			Assert.NotNull(appointment);
			Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
			Assert.Equal(interval, appointment.TimeInterval);
			Assert.Equal(patientId, appointment.PatientId);
			Assert.Equal(doctorId, appointment.DoctorId);
			Assert.Equal(roomId, appointment.ConsultingRoomId);
		}

		[Fact]
		public void Constructor_StartDateBeforeToday_ThrowsBusinessRuleException()
		{
			var error_interval = new TimeInterval(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

			Assert.Throws<BusinessRuleException>(() =>
				new Appointment(patientId, doctorId, roomId, error_interval));
		}

		[Theory]
		[InlineData("CancelAppointment", AppointmentStatus.Cancelled)]
		[InlineData("CompleteAppointment", AppointmentStatus.Completed)]
		[InlineData("NoShowAppointment", AppointmentStatus.NoShow)]
		public void AppointmentActions_ValidStatus_UpdatesCorrectly(string methodName, AppointmentStatus expectedStatus)
		{
			var appointment = CreateValidAppointment();

			var method = appointment.GetType().GetMethod(methodName)!;
			method.Invoke(appointment, null);

			Assert.Equal(expectedStatus, appointment.Status);
		}
				
		[Theory]
		[InlineData("CancelAppointment", AppointmentStatus.Completed)]
		[InlineData("CancelAppointment", AppointmentStatus.Cancelled)]
		[InlineData("CancelAppointment", AppointmentStatus.NoShow)]
		[InlineData("CompleteAppointment", AppointmentStatus.Completed)]
		[InlineData("CompleteAppointment", AppointmentStatus.Cancelled)]
		[InlineData("CompleteAppointment", AppointmentStatus.NoShow)]
		[InlineData("NoShowAppointment", AppointmentStatus.Completed)]
		[InlineData("NoShowAppointment", AppointmentStatus.Cancelled)]
		[InlineData("NoShowAppointment", AppointmentStatus.NoShow)]
		public void AppointmentActions_InvalidStatus_ThrowsBusinessRuleException(string methodName, AppointmentStatus invalidStatus)
		{
			var appointment = CreateValidAppointment();
			appointment.GetType().GetProperty("Status")!.SetValue(appointment, invalidStatus);

			var method = appointment.GetType().GetMethod(methodName)!;

			var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(appointment, null));
			Assert.IsType<BusinessRuleException>(ex.InnerException);
		}

		private Appointment CreateValidAppointment()
		{
			var patientId = Guid.NewGuid();
			var doctorId = Guid.NewGuid();
			var roomId = Guid.NewGuid();
			var interval = new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

			return new Appointment(patientId, doctorId, roomId, interval);
		}
	}
}
