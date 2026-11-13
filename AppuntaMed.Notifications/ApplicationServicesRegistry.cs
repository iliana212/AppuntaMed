using AppuntaMed.Application.Interfaces.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace AppuntaMed.Notifications
{
	public static class ApplicationServicesRegistry
	{
		public static IServiceCollection AddNotificationServices(this IServiceCollection services)
		{
			services.AddScoped<INotificationService, EmailService>();

			return services;
		}
	}
}
