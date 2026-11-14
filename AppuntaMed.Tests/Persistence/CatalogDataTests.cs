using AppuntaMed.Persistence.Seeding;

namespace AppuntaMed.Tests.Persistence
{
	public class CatalogDataTests
	{
		[Fact]
		public void Professions_HaveUniqueIdsAndNames()
		{
			var professions = CatalogData.Professions;

			Assert.NotEmpty(professions);
			Assert.Equal(professions.Count, professions.Select(p => p.Id).Distinct().Count());
			Assert.Equal(professions.Count, professions.Select(p => p.Name.ToLowerInvariant()).Distinct().Count());
			Assert.DoesNotContain(professions, p => p.Id == Guid.Empty || string.IsNullOrWhiteSpace(p.Name));
		}

		[Fact]
		public void Specialities_HaveUniqueIds_AndEveryOneBelongsToAnExistingProfession()
		{
			var professionIds = CatalogData.Professions.Select(p => p.Id).ToHashSet();
			var specialities = CatalogData.Specialities;

			Assert.NotEmpty(specialities);
			Assert.Equal(specialities.Count, specialities.Select(s => s.Id).Distinct().Count());
			Assert.All(specialities, s => Assert.Contains(s.ProfessionId, professionIds));
			Assert.DoesNotContain(specialities, s => s.Id == Guid.Empty || string.IsNullOrWhiteSpace(s.Name));
		}

		[Fact]
        public void Specialities_AreUniqueByNameWithinTheirProfession()
        {
			var duplicates = CatalogData.Specialities
				.GroupBy(s => (s.ProfessionId, Name: s.Name.ToLowerInvariant()))
	            .Where(g => g.Count() > 1)
	            .Select(g => g.Key.Name)
	            .ToList();
	
			Assert.Empty(duplicates);
	    }

		[Fact]
        public void Dentist_HasOrthodontics()
        {
			var orthodontics = Assert.Single(CatalogData.Specialities, s => s.Name == "Orthodontics");
	
			Assert.Equal(CatalogData.DentistId, orthodontics.ProfessionId);
			Assert.Equal(CatalogData.OrthodonticsId, orthodontics.Id);
	    }

		[Fact]
        public void FixedIds_AreStable()
        {
			// Fixed ids must never change: they are referenced by migrations and by demo data.
			Assert.Equal(new Guid("67c0884c-ef4f-5457-8017-9eaffff23484"), CatalogData.DentistId);
	    }
	}
}
