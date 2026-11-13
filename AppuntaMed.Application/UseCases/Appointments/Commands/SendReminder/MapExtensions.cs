using AppuntaMed.Application.Interfaces.Notifications;
using AppuntaMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.SendReminder
{
	public static class MapExtensions
	{
		public static AppointmentReminderDTO ToDTO(this Appointment appointment)
		{
			return new AppointmentReminderDTO
			(
				appointment.Id,
				$"{appointment.Patient!.FirstName} {appointment.Patient.LastName}",
				appointment.Patient.Email.Value,
				$"{appointment.Doctor!.FirstName} {appointment.Doctor.LastName}",
				appointment.ConsultingRoom!.Name,
				appointment.TimeInterval.Start
			);
		}
	}
}
