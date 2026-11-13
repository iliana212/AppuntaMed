using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Interfaces.Notifications
{
	public record AppointmentReminderDTO
	(
		Guid Id,
		string Patient,
		string Patient_Email,
		string Doctor,
		string ConsultingRoom,
		DateTime AppoitmentDate
	);
}
