using AppuntaMed.Application.Security;
using AppuntaMed.Security;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace AppuntaMed.API.Auth
{
	public sealed class PermissionClaimsTransformation : IClaimsTransformation
	{
		public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
		{
			if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
				return Task.FromResult(principal);

			var realmAccessClaim = principal.FindFirst("realm_access");

			if (realmAccessClaim is null)
				return Task.FromResult(principal);

			try
			{
				using var doc = JsonDocument.Parse(realmAccessClaim.Value);

				if (!doc.RootElement.TryGetProperty("roles", out var rolesElement) || rolesElement.ValueKind != JsonValueKind.Array)
					return Task.FromResult(principal);

				foreach (var role in rolesElement.EnumerateArray())
				{
					var roleName = role.GetString();

					if (string.IsNullOrEmpty(roleName) || identity.HasClaim(ClaimTypes.Role, roleName))
						continue;

					// Standard role claim so [Authorize(Roles = "...")] keeps working.
					identity.AddClaim(new Claim(ClaimTypes.Role, roleName));

					if (!RolePermissions.Map.TryGetValue(roleName, out var permissions))
						continue;

					foreach (var permission in permissions)
					{
						if (!identity.HasClaim(KeycloakClaims.Permission, permission))
							identity.AddClaim(new Claim(KeycloakClaims.Permission, permission));
					}
				}
			}
			catch (JsonException)
			{
			}

			return Task.FromResult(principal);
		}
	}
}
