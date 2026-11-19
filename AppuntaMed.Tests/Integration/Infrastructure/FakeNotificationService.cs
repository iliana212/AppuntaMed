using AppuntaMed.Application.Interfaces.Notifications;
using System.Collections.Concurrent;

namespace AppuntaMed.Tests.Integration.Infrastructure
{
	/// <summary>Records the emails the API would send, instead of talking to an SMTP server.</summary>
	public sealed class FakeNotificationService : INotificationService
	{
		private readonly ConcurrentQueue<ConfirmedAppointmentDTO> _confirmations = new();
		private readonly ConcurrentQueue<AppointmentReminderDTO> _reminders = new();

		public IReadOnlyList<ConfirmedAppointmentDTO> Confirmations => _confirmations.ToList();

		public IReadOnlyList<AppointmentReminderDTO> Reminders => _reminders.ToList();

		public Task SendConfirmationAppointment(ConfirmedAppointmentDTO appointmentDTO)
		{
			_confirmations.Enqueue(appointmentDTO);
			return Task.CompletedTask;
		}

		public Task SendReminderAppointment(AppointmentReminderDTO appointmentDTO)
		{
			_reminders.Enqueue(appointmentDTO);
			return Task.CompletedTask;
		}
	}
}
