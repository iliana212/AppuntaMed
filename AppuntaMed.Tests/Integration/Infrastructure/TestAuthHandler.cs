using AppuntaMed.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AppuntaMed.Tests.Integration.Infrastructure
{
	/// <summary>
	/// Replaces JWT validation in the tests: the "token" is a set of headers. It emits the same claims Keycloak does
	/// (sub, roles, realm_access), so the REAL claims transformation, policies and ownership rules run.
	/// No header means anonymous (401).
	/// </summary>
	public sealed class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
		: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
	{
		public const string SchemeName = "Test";
		public const string UserIdHeader = "X-Test-Sub";
		public const string UserNameHeader = "X-Test-Username";
		public const string EmailHeader = "X-Test-Email";
		public const string RolesHeader = "X-Test-Roles";

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var userId = Request.Headers[UserIdHeader].ToString();

			if (string.IsNullOrWhiteSpace(userId))
				return Task.FromResult(AuthenticateResult.NoResult());

			var roles = Request.Headers[RolesHeader].ToString()
               .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			var claims = new List<Claim>
            {
				new (KeycloakClaims.UserId, userId),
				new (KeycloakClaims.Username, Request.Headers[UserNameHeader].ToString()),
				new (KeycloakClaims.Email, Request.Headers[EmailHeader].ToString()),
				new ("realm_access", JsonSerializer.Serialize(new { roles })),
			};

			claims.AddRange(roles.Select(role => new Claim(KeycloakClaims.Roles, role)));

			var identity = new ClaimsIdentity(claims, SchemeName, KeycloakClaims.Username, KeycloakClaims.Roles);
			var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
