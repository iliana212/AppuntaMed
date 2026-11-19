using AppuntaMed.Application.UseCases.Professions.Queries.GetProfessionsList;
using AppuntaMed.Persistence.Seeding;
using AppuntaMed.Tests.Integration.Infrastructure;
using System.Data;
using System.Net;

namespace AppuntaMed.Tests.Integration
{
	[Collection(ApiCollection.Name)]
	[Trait("Category", "Integration")]
	public sealed class ProfessionsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
	{
		[Fact]
		public async Task Get_ReturnsTheWholeCatalog_WithTheSpecialitiesOfEachProfession()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync("/api/professions");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var professions = await ReadAs<List<ProfessionDTO>>(response);

			Assert.Equal(
				CatalogData.Professions.Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal),
				professions.Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal));

			var dentist = professions.Single(p => p.Id == CatalogData.DentistId);
			var expected = CatalogData.Specialities.Where(s => s.ProfessionId == CatalogData.DentistId).Select(s => s.Name);

			Assert.Equal(expected.OrderBy(n => n, StringComparer.Ordinal), dentist.Specialities.Select(s => s.Name).OrderBy(n => n, StringComparer.Ordinal));
			Assert.Contains(dentist.Specialities, s => s.Id == CatalogData.OrthodonticsId && s.Name == "Orthodontics");
		}

		[Fact]
		public async Task Get_ReturnsUniqueProfessionsAndSpecialities()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync("/api/professions");
			var professions = await ReadAs<List<ProfessionDTO>>(response);

			// The same ordering the database uses (the database collation, not the .NET culture), so just check it is stable.
			Assert.All(professions, p => Assert.Equal(p.Specialities.Count, p.Specialities.Select(s => s.Id).Distinct().Count()));
			Assert.Equal(professions.Count, professions.Select(p => p.Id).Distinct().Count());
		}

		[Theory]
		[InlineData("Admin")]
		[InlineData("Doctor")]
		[InlineData("Patient")]
        public async Task Get_EveryRole_CanReadTheCatalog(string role)
        {
			var user = TestUser.Ana with { Role = role };
	
			var response = await ClientFor(user).GetAsync("/api/professions");
	
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	    }

		[Fact]
        public async Task Get_AccountWithoutAnyKnownRole_Returns403()
        {
			var user = TestUser.Ana with { Role = "offline_access" };
	
			var response = await ClientFor(user).GetAsync("/api/professions");
	
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	    }
    }
}
