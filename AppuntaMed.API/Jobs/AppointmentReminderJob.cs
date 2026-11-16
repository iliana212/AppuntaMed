using AppuntaMed.Application.UseCases.Appointments.Commands.SendReminder;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.API.Jobs
{
	public class AppointmentReminderJob(IServiceScopeFactory scopeFactory, ILogger<AppointmentReminderJob> logger) : BackgroundService
	{
		private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
                {
					var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

					if (now.Hour == 8)
					{
						using var scope = scopeFactory.CreateScope();
						var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
						await mediator.Send(new SendAppointmentReminderCommand());
					}
				}
				catch (Exception ex) when (ex is not OperationCanceledException)
				{
					logger.LogError(ex, "The appointment reminder run failed");
				}

				await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
			}
		}
	}
}