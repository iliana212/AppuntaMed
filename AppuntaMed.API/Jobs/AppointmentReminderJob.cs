using AppuntaMed.Application.UseCases.Appointments.Commands.SendReminder;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.API.Jobs
{
	public class AppointmentReminderJob(IServiceScopeFactory scopeFactory) : BackgroundService
	{
		private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

				if (now.Hour == 8)
				{
					using var scope = scopeFactory.CreateScope();
					var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
					await mediator.Send(new SendAppointmentReminderCommand());
				}

				await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
			}
		}
	}
}