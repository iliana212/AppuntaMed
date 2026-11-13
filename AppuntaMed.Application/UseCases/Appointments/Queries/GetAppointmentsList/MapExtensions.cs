using AppuntaMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList
{
	public static class MapExtensions
	{
		public static AppointmentsListDTO ToDTO( this Appointment appointment)
		{
			var dto = new AppointmentsListDTO
			(
				appointment.Id,
				$"{appointment.Patient!.FirstName} {appointment.Patient.LastName}",
				$"{appointment.Doctor!.FirstName} {appointment.Doctor.LastName}",
				appointment.ConsultingRoom!.Name,
				appointment.TimeInterval.Start,
				appointment.TimeInterval.End,
				appointment.Status.ToString()
			);

			return dto;
		}
	}
}
