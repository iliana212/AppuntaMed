using AppuntaMed.Application.Interfaces.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AppuntaMed.Security
{
	public class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
	{
		public string? UserId => contextAccessor.HttpContext?.User?.FindFirstValue(KeycloakClaims.UserId);

		public string? Email => contextAccessor.HttpContext?.User?.FindFirstValue(KeycloakClaims.Email);

		public string? UserName => contextAccessor.HttpContext?.User?.FindFirstValue(KeycloakClaims.Username)
								   ?? contextAccessor.HttpContext?.User?.Identity?.Name;

		public IEnumerable<string> Roles => contextAccessor.HttpContext?.User?.FindAll(KeycloakClaims.Roles)
			.Select(c => c.Value) ?? Enumerable.Empty<string>();

		public IEnumerable<string> Permissions => contextAccessor.HttpContext?.User?.FindAll(KeycloakClaims.Permission)
			.Select(x => x.Value) ?? Enumerable.Empty<string>();

		public bool IsAuthenticated => contextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
	}
}
