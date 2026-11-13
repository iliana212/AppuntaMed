using AppuntaMed.Application.Interfaces.Notifications;
using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CreateAppointment
{
	public static class MapExtensions
	{
		public static ConfirmedAppointmentDTO ToDTO(this Appointment appointment)
		{
			return new ConfirmedAppointmentDTO(
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
