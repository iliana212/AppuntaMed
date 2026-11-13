using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList
{
	public record AppointmentsListDTO
	(
		Guid Id,
		string Patient,
		string Doctor,
		string ConsultingRoom,
		DateTime Start,
		DateTime End,
		string Status
	);
}
