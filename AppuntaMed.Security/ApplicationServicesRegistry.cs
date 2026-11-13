using AppuntaMed.Application.Interfaces.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AppuntaMed.Security
{
	public static class ApplicationServicesRegistry
	{
		public static IServiceCollection AddSecurityServices(this IServiceCollection services)
		{
			services.AddScoped<ICurrentUserService, CurrentUserService>();

			return services;
		}
	}
}
