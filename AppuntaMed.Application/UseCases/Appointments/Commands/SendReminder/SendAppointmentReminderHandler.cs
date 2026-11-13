using AppuntaMed.Application.Interfaces.Notifications;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Repositories.Models;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Enums;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.SendReminder
{
	public class SendAppointmentReminderHandler(IAppointmentRepository repository, INotificationService notification) : IRequestHandler<SendAppointmentReminderCommand>
	{
		public async Task Handle(SendAppointmentReminderCommand request)
		{
			var tomorrow = DateTime.UtcNow.Date.AddDays(1);
			var start = tomorrow;
			var end = tomorrow.AddDays(1);
			var filter = new AppointmentsFilterDTO
			{
				Start = start,
				End = end,
				Status = AppointmentStatus.Scheduled
			};

			var appointments = await repository.GetFilterData(filter);
			foreach(var appointment in appointments)
			{
				var dto = appointment.ToDTO();
				await notification.SendReminderAppointment(dto);
			}
		}
	}
}
