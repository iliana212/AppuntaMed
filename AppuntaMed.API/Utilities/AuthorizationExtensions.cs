using AppuntaMed.API.Auth;
using AppuntaMed.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AppuntaMed.API.Utilities
{
	public static class AuthorizationExtensions
	{
		public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
		{
			var authorization = services.AddAuthorizationBuilder();

			foreach (var permission in Permissions.All)
			{
				authorization.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
			}

			services.AddScoped<IAuthorizationHandler, PermissionHandler>();
			services.AddTransient<IClaimsTransformation, PermissionClaimsTransformation>();

			return services;
		}
	}
}