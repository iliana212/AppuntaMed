using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentDetail
{
	public static class MapExtensions
	{
		public static AppointmentDetailDTO ToDTO(this Appointment appointment)
		{
			var dto = new AppointmentDetailDTO
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
