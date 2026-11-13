using AppuntaMed.Application.Interfaces.Notifications;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AppuntaMed.Notifications
{
	public class EmailService(IConfiguration configuration) : INotificationService
	{
		public async Task SendConfirmationAppointment(ConfirmedAppointmentDTO appointmentDTO)
		{
			var subject = "AppuntaMed - Appointment confirmation";

			var body = $"""
				Dear {appointmentDTO.Patient},
				Your appointment with Dr {appointmentDTO.Doctor} has beed confirmed 
				for the date {appointmentDTO.AppoitmentDate.ToString("f", new CultureInfo("es-MX"))} 
				 in the consulting room: {appointmentDTO.ConsultingRoom}.

				 Thank you.
				 Have a nice day.

				 AppuntaMed - Team.

				""";

			await SendMessage(appointmentDTO.Patient_Email, subject, body);

		}

		public async Task SendReminderAppointment(AppointmentReminderDTO appointmentDTO)
		{
			var subject = "AppuntaMed - Appointment confirmation";

			var body = $"""
				Dear {appointmentDTO.Patient},
				We remind you that your appointment with Dr {appointmentDTO.Doctor} 
				for the date {appointmentDTO.AppoitmentDate.ToString("f", new CultureInfo("es-MX"))} 
				in the consulting room: {appointmentDTO.ConsultingRoom}.

				 Thank you.
				 Have a nice day.

				 AppuntaMed - Team.

				""";

			await SendMessage(appointmentDTO.Patient_Email, subject, body);
		}

		private async Task SendMessage(string email, string subject, string body)
		{
			var baseMail = configuration.GetValue<string>("CONFIG_EMAIL:EMAIL");
			var passwd = configuration.GetValue<string>("CONFIG_EMAIL:PASSWD");
			var host = configuration.GetValue<string>("CONFIG_EMAIL:HOST");
			var port = configuration.GetValue<int>("CONFIG_EMAIL:PORT");

			var smtp = new SmtpClient(host, port);
			smtp.EnableSsl = true;
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = new NetworkCredential(baseMail, passwd);

			var message = new MailMessage(baseMail!, email, subject, body);
			await smtp.SendMailAsync(message);
		}


	}
}
