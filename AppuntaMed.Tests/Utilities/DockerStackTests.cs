using System.Text.Json;
using System.Text.RegularExpressions;

namespace AppuntaMed.Tests.Utilities
{
	public  class DockerStackTests
	{
		private static readonly string Compose = RepoFiles.Read("docker-compose.yml");

		[Fact]
		public void Compose_PublishesTheApiOnPort8081_TheRealmAllowsAsRedirectUri()
		{
			Assert.Contains("\"8081:8080\"", Compose);

			using var realm = JsonDocument.Parse(RepoFiles.Read(Path.Combine("keycloak", "realm-appuntamed.json")));
			var client = realm.RootElement.GetProperty("clients").EnumerateArray()
               .Single(c => c.GetProperty("clientId").GetString() == "appuntamed-api");
			var redirects = client.GetProperty("redirectUris").EnumerateArray().Select(x => x.GetString()).ToList();

			Assert.Contains("http://localhost:8081/*", redirects);
		}

		[Fact]
		public void EnvExample_DocumentsEveryVariableTheComposeFileRequires()
		{
			var envExample = RepoFiles.Read(".env.example");

			// ${NAME} and ${NAME:?message} are required; ${NAME:-default} is optional.
			var required = Regex.Matches(Compose, @"\$\{([A-Z][A-Z0-9_]*)(?::\?[^}]*)?\}")
               .Select(m => m.Groups[1].Value)
               .Distinct()
               .ToList();

			Assert.NotEmpty(required);
			Assert.All(required, name =>
				Assert.Matches($"(?m)^{name}=", envExample));
		}

		[Fact]
        public void Compose_ApiPointsToKeycloakAndSqlServerByServiceName()
        {
			Assert.Contains("Authentication__Authority: http://keycloak:8080/realms/AppuntaMed", Compose);
			Assert.Contains("Server=sqlserver,1433", Compose);
	    }

		[Fact]
        public void Compose_KeycloakIssuerIsFixedForBrowsersAndResolvedDynamicallyForTheBackchannel()
        {
			Assert.Contains("KC_HOSTNAME_URL: http://localhost:8080", Compose);
			Assert.Contains("KC_HOSTNAME_BACKCHANNEL_DYNAMIC: \"true\"", Compose);
	    }

		[Fact]
        public void Compose_DoesNotContainHardcodedPasswords()
        {
	        // Secrets come from .env (git-ignored); only references like ${MSSQL_SA_PASSWORD} are allowed.
			Assert.DoesNotMatch(@"(?m)^\s*MSSQL_SA_PASSWORD:\s*[^$\s]", Compose);
			Assert.DoesNotMatch(@"(?m)^\s*KEYCLOAK_ADMIN_PASSWORD:\s*[^$\s]", Compose);
	    }

		[Fact]
		public void Compose_KeycloakDataVolume_IsMountedOnTheDataFolder_SoKeycloakCanWriteItsDatabase()
		{
			Assert.Matches(@"(?m)keycloak_data:/opt/keycloak/data\s*$", Compose);
			Assert.DoesNotContain("/opt/keycloak/data/h2", Compose);
		}

		[Fact]
		public void Compose_KeycloakHealthcheck_OnlyUsesBashBuiltins()
		{
			// The Keycloak image is minimal: no curl, and possibly no grep/head.
			var keycloak = Compose[Compose.IndexOf("  keycloak:", StringComparison.Ordinal)..Compose.IndexOf("  mailpit:", StringComparison.Ordinal)];

			Assert.Contains("/dev/tcp/", keycloak);
			Assert.Contains("HTTP/1.1", keycloak);
			Assert.Contains("Host: localhost", keycloak);
			Assert.DoesNotContain("HTTP/1.0", keycloak);
			Assert.DoesNotContain("grep", keycloak.Replace("nor grep", ""));
			Assert.DoesNotContain("curl -", keycloak);
		}

		[Fact]
        public void Dockerfile_RunsAsNonRoot_AndExposesPort8080()
        {
			var dockerfile = RepoFiles.Read("Dockerfile");
	
			Assert.Contains("USER $APP_UID", dockerfile);
			Assert.Contains("ASPNETCORE_HTTP_PORTS=8080", dockerfile);
	    }
	}
}
