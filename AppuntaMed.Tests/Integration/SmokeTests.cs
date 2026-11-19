using AppuntaMed.Persistence.Seeding;
using AppuntaMed.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppuntaMed.Tests.Integration
{
	[Collection(ApiCollection.Name)]
	[Trait("Category", "Integration")]
	public sealed class SmokeTests(ApiFactory factory) : IntegrationTestBase(factory)
	{
		[Fact]
		public async Task Health_IsOk_WithoutAuthentication()
		{
			var response = await Anonymous().GetAsync("/health");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
		}

		[Theory]
		[InlineData("/api/doctors")]
		[InlineData("/api/patients")]
		[InlineData("/api/professions")]
		[InlineData("/api/appointments")]
		public async Task ProtectedEndpoints_WithoutCredentials_Return401(string url)
		{
			var response = await Anonymous().GetAsync(url);

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
        public async Task EveryMigration_IsApplied()
        {
			var pending = await InDb(db => db.Database.GetPendingMigrationsAsync());
	
			Assert.Empty(pending);
	    }

		[Fact]
        public async Task CatalogShippedWithTheMigrations_IsInTheDatabase()
        {
			var professions = await InDb(db => db.Professions.CountAsync());
			var specialities = await InDb(db => db.Specialities.CountAsync());
	
			Assert.Equal(CatalogData.Professions.Count, professions);
			Assert.Equal(CatalogData.Specialities.Count, specialities);
	    }

		[Fact]
        public async Task DemoData_WasSeededOnStartup()
        {
			var plan = DemoData.Build(DateTime.UtcNow);
			var doctorIds = plan.Doctors.Select(x => x.Id).ToArray();
			var patientIds = plan.Patients.Select(x => x.Id).ToArray();
			var roomIds = plan.ConsultingRooms.Select(x => x.Id).ToArray();
			var appointmentIds = plan.Appointments.Select(x => x.Id).ToArray();

			Assert.Equal(doctorIds.Length, await InDb(db => db.Doctors.CountAsync(d => doctorIds.Contains(d.Id))));
			Assert.Equal(patientIds.Length, await InDb(db => db.Patients.CountAsync(p => patientIds.Contains(p.Id))));
			Assert.Equal(roomIds.Length, await InDb(db => db.ConsultingRooms.CountAsync(r => roomIds.Contains(r.Id))));
			Assert.Equal(appointmentIds.Length, await InDb(db => db.Appointments.CountAsync(a => appointmentIds.Contains(a.Id))));
		}
    }
}
