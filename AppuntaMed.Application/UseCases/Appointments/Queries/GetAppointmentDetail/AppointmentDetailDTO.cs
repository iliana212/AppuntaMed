using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentDetail
{
	public record AppointmentDetailDTO
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
