using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppuntaMed.Persistence.Seeding
{
	public static class DemoDataSeeder
	{
		public static void Seed(DbContext context, DateTime utcNow)
		{
			var plan = DemoData.Build(utcNow);

			AddMissing(context, plan.ConsultingRooms, ExistingIds<ConsultingRoom>(context, plan.ConsultingRooms));
			AddMissing(context, plan.Patients, ExistingIds<Patient>(context, plan.Patients));
			AddMissing(context, plan.Doctors, ExistingIds<Doctor>(context, plan.Doctors));
			AddMissing(context, plan.Appointments, ExistingIds<Appointment>(context, plan.Appointments));

			context.SaveChanges();
		}

        public static async Task SeedAsync(DbContext context, DateTime utcNow, CancellationToken cancellationToken)
        {
			var plan = DemoData.Build(utcNow);
	
			AddMissing(context, plan.ConsultingRooms, await ExistingIdsAsync<ConsultingRoom>(context, plan.ConsultingRooms, cancellationToken));
			AddMissing(context, plan.Patients, await ExistingIdsAsync<Patient>(context, plan.Patients, cancellationToken));
			AddMissing(context, plan.Doctors, await ExistingIdsAsync<Doctor>(context, plan.Doctors, cancellationToken));
			AddMissing(context, plan.Appointments, await ExistingIdsAsync<Appointment>(context, plan.Appointments, cancellationToken));
	
			await context.SaveChangesAsync(cancellationToken);
	    }

        private static HashSet<Guid> ExistingIds<T>(DbContext context, IEnumerable<Seed<T>> seeds) where T : class
		{
			var ids = seeds.Select(s => s.Id).ToArray();
	
	        return context.Set<T>()
				.Where(e => ids.Contains(EF.Property<Guid>(e, "Id")))
	            .Select(e => EF.Property<Guid>(e, "Id"))
	            .ToHashSet();
	    }

        private static async Task<HashSet<Guid>> ExistingIdsAsync<T>(DbContext context, IEnumerable<Seed<T>> seeds, CancellationToken cancellationToken) where T : class
		{
			var ids = seeds.Select(s => s.Id).ToArray();
	
			var existing = await context.Set<T>()
				.Where(e => ids.Contains(EF.Property<Guid>(e, "Id")))
	            .Select(e => EF.Property<Guid>(e, "Id"))
	            .ToListAsync(cancellationToken);
	
			return existing.ToHashSet();
	    }

        private static void AddMissing<T>(DbContext context, IEnumerable<Seed<T>> seeds, HashSet<Guid> existingIds) where T : class
		{
			foreach (var seed in seeds.Where(s => !existingIds.Contains(s.Id)))
		    {
				context.Add(seed.Entity);
		
		        // The domain generates its own ids; the seed needs STABLE ones (the entity is still "Added").
				context.Entry(seed.Entity).Property("Id").CurrentValue = seed.Id;
		    }
	    }
    }
}
