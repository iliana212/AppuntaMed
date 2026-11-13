using AppuntaMed.Security;
using Microsoft.AspNetCore.Authorization;

namespace AppuntaMed.API.Auth
{
	public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
		{
			var hasPermission = context.User.Claims.Any(c => c.Type == KeycloakClaims.Permission && c.Value == requirement.Permission);

			if (hasPermission)
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}
