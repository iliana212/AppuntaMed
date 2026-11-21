using System.Text.RegularExpressions;

namespace AppuntaMed.Tests.Utilities
{
	public class CIConfigTests
	{
		private static readonly string Ci = RepoFiles.Read(Path.Combine(".github", "workflows", "ci.yml"));

		private static string TargetFrameworkMajor(string project)
		{
			var match = Regex.Match(RepoFiles.Read(Path.Combine(project, project + ".csproj")), @"<TargetFramework>net(\d+)\.0</TargetFramework>");

			Assert.True(match.Success, $"{project} has no <TargetFramework>netX.0</TargetFramework>");

			return match.Groups[1].Value;
		}

		[Theory]
		[InlineData("AppuntaMed.API")]
		[InlineData("AppuntaMed.Tests")]
		public void Pipeline_UsesTheSameDotNetVersionAsTheProjects(string project)
		{
			var major = TargetFrameworkMajor(project);

			Assert.Contains($"dotnet-version: {major}.0.x", Ci);
		}

		[Fact]
		public void Dockerfile_UsesTheSameDotNetVersionAsTheProjects()
		{
			var major = TargetFrameworkMajor("AppuntaMed.API");
			var dockerfile = RepoFiles.Read("Dockerfile");

			Assert.Contains($"dotnet/sdk:{major}.0", dockerfile);
			Assert.Contains($"dotnet/aspnet:{major}.0", dockerfile);
		}

		[Fact]
		public void Pipeline_RunsOnPushesToMain_AndOnPullRequests()
		{
			Assert.Matches(@"(?m)^on:\s*\r?\n\s+push:\s*\r?\n\s+branches:\s*\[main\]", Ci);
			Assert.Matches(@"(?m)^\s+pull_request:", Ci);
		}

		[Fact]
		public void Pipeline_RunsUnitAndIntegrationTestsAsSeparateSteps()
		{
			Assert.Contains("Category!=Integration", Ci);
			Assert.Contains("Category=Integration", Ci);
		}

		[Fact]
		public void Pipeline_CollectsCoverage_WithTheSettingsFileInTheRepository()
		{
			Assert.Contains("--settings coverlet.runsettings", Ci);
			Assert.Contains("XPlat Code Coverage", Ci);

			var settings = RepoFiles.Read("coverlet.runsettings");
			Assert.Contains("Migrations", settings);
			Assert.Contains("[AppuntaMed.Tests]*", settings);
		}

		[Fact]
		public void Pipeline_HasReadOnlyPermissions_AndNeedsNoSecrets()
		{
			Assert.Matches(@"(?m)^permissions:\s*\r?\n\s+contents:\s*read", Ci);
			Assert.DoesNotContain("secrets.", Ci);
		}

		[Fact]
		public void Pipeline_ActionsArePinnedToAMajorVersion_NeverToABranch()
		{
			var uses = Regex.Matches(Ci, @"uses:\s*(\S+)").Select(m => m.Groups[1].Value).ToList();

			Assert.NotEmpty(uses);
			Assert.All(uses, action => Assert.Matches(@"@v\d+$", action));
		}

		[Fact]
		public void Pipeline_SmokeTestsTheDockerStack_WithTheExampleEnvironmentFile()
		{
			Assert.Contains(".env.example", Ci);
			Assert.Contains("docker compose up", Ci);
			Assert.Contains("scripts/smoke-test.sh", Ci);
		}

		[Fact]
		public void SmokeScript_TargetsTheApiPortPublishedByCompose()
		{
			var script = RepoFiles.Read(Path.Combine("scripts", "smoke-test.sh"));

			Assert.Contains("http://localhost:8081", script);
			Assert.Contains("/health", script);
			Assert.Contains("/api/auth", script);
			Assert.Contains("\"8081:8080\"", RepoFiles.Read("docker-compose.yml"));
		}

		[Fact]
		public void SmokeScript_UsesTheDemoUserOfTheKeycloakRealm()
		{
			var script = RepoFiles.Read(Path.Combine("scripts", "smoke-test.sh"));
			var realm = RepoFiles.Read(Path.Combine("keycloak", "realm-appuntamed.json"));

			Assert.Contains("\"username\": \"ana\"", realm);
			Assert.Contains("\"user\":\"ana\"", script);
		}

		[Fact]
		public void Dependabot_WatchesNugetPackages_Actions_AndDockerImages()
		{
			var dependabot = RepoFiles.Read(Path.Combine(".github", "dependabot.yml"));

			foreach (var ecosystem in new[] { "nuget", "github-actions", "docker", "docker-compose" })
				Assert.Contains($"package-ecosystem: {ecosystem}", dependabot);
		}

		[Fact]
		public void Dependabot_DoesNotJumpToANewDotNetMajor_OrToSqlServer2025()
		{
			// A new .NET major version needs the target framework to change with it, and the SQL Server 2025 image
			// has no sqlcmd (the compose healthcheck needs it): both would only produce red pull requests.
			var dependabot = RepoFiles.Read(Path.Combine(".github", "dependabot.yml"));

			foreach (var dependency in new[] { "Microsoft.*", "System.*", "dotnet/sdk", "dotnet/aspnet", "mssql/server" })
				Assert.Contains($"dependency-name: \"{dependency}\"", dependabot);
		}

		[Fact]
		public void CodeQl_AnalyzesCSharp_OnPullRequestsAndOnASchedule()
		{
			var codeql = RepoFiles.Read(Path.Combine(".github", "workflows", "codeql.yml"));

			Assert.Contains("languages: csharp", codeql);
			Assert.Contains("pull_request:", codeql);
			Assert.Contains("schedule:", codeql);
			Assert.Matches(@"security-events:\s*write", codeql);
		}
	}
}
