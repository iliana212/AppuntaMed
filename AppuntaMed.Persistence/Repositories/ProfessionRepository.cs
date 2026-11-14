using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppuntaMed.Persistence.Repositories
{
	public class ProfessionRepository(AppuntaMedDbContext context) : IProfessionRepository
	{
		public async Task<IEnumerable<Profession>> GetAllWithSpecialities()
		{
			return await context.Professions
				.AsNoTracking()
				.Include(p => p.Specialities.OrderBy(s => s.Name))
				.OrderBy(p => p.Name)
				.ToListAsync();
		}

		public async Task<Profession?> GetByIdWithSpecialities(Guid id)
		{
			return await context.Professions
				.AsNoTracking()
				.Include(p => p.Specialities)
				.FirstOrDefaultAsync(p => p.Id == id);
		}
    }
}
