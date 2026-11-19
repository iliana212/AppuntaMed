using AppuntaMed.API.Jobs;
using AppuntaMed.Application.Interfaces.Notifications;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace AppuntaMed.Tests.Integration.Infrastructure
{
	/// <summary>
	/// The real API in memory, on top of a REAL SQL Server (Testcontainers): migrations, seeding and every query
	/// run exactly as in production. Only two things are replaced: authentication (headers instead of a Keycloak JWT)
	/// and email (recorded instead of sent). One instance is shared by all the integration tests.
	/// Docker must be running.
	/// </summary>
	public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
	{
		private readonly MsSqlContainer _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();	

		public FakeNotificationService Notifications { get; } = new();

		public async Task InitializeAsync()
		{
			await _sqlServer.StartAsync();
		}

		async Task IAsyncLifetime.DisposeAsync()
		{
			await base.DisposeAsync();
			await _sqlServer.DisposeAsync();
		}

		public HttpClient CreateClientFor(TestUser user)
		{
			var client = CreateClient();

			client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, user.UserId);
			client.DefaultRequestHeaders.Add(TestAuthHandler.UserNameHeader, user.UserName);
			client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, user.Email);
			client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, user.Role);

			return client;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			var connectionString = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
			{
				InitialCatalog = "AppuntaMed_IntegrationTests"
			}.ConnectionString;

			builder.UseEnvironment("Testing");

			// UseSetting (not ConfigureAppConfiguration): Program.cs reads Seeding:DemoData while it builds the services.
			builder.UseSetting("ConnectionStrings:AppuntaMedSqlServer", connectionString);
			builder.UseSetting("Database:MigrateOnStartup", "true");
			builder.UseSetting("Seeding:DemoData", "true");

			builder.ConfigureTestServices(services =>
			{
				services.AddAuthentication(options =>
				{
					options.DefaultScheme = TestAuthHandler.SchemeName;
					options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
					options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
					options.DefaultForbidScheme = TestAuthHandler.SchemeName;
				}).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

				services.RemoveAll<INotificationService>();
				services.AddSingleton<INotificationService>(Notifications);

				// The 8:00 am reminder job must not run (nor send emails) while the tests are running.
				var reminderJob = services.SingleOrDefault(d =>
					d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(AppointmentReminderJob));

				if (reminderJob is not null)
					services.Remove(reminderJob);
			});
		}
	}

	[CollectionDefinition(Name)]
	public sealed class ApiCollection : ICollectionFixture<ApiFactory>
	{
		public const string Name = "API integration (SQL Server in Docker)";
	}
}