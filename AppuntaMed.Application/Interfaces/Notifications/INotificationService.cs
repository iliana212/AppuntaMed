using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Interfaces.Notifications
{
	public interface INotificationService
	{
		Task SendConfirmationAppointment(ConfirmedAppointmentDTO appointmentDTO);
		Task SendReminderAppointment(AppointmentReminderDTO appointmentDTO);
	}
}
