using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace AppuntaMed.Persistence.Repositories
{
	public class PatientRepository : Repository<Patient>, IPatientRepository
	{
		private readonly AppuntaMedDbContext _context;

		public PatientRepository(AppuntaMedDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<bool> ExistsByUserId(string userId)
		{
			return await _context.Patients.AnyAsync(x => x.UserId == userId);
		}

		public async Task<Patient?> GetByUserId(string userId)
		{
			return await _context.Patients.FirstOrDefaultAsync(x => x.UserId == userId);
		}

		public async Task<IEnumerable<Patient>> GetFilter(PatientFilterDTO filter)
		{
			return await ApplyFilter(filter)
               .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
               .Pagination(filter.Page, filter.PageSize)
               .ToListAsync();
		}

		public async Task<int> CountFilter(PatientFilterDTO filter)
		{
			return await ApplyFilter(filter).CountAsync();
		}

		private IQueryable<Patient> ApplyFilter(PatientFilterDTO filter)
		{

			var queryable = _context.Patients.AsQueryable();

			if (!string.IsNullOrWhiteSpace(filter.PatientName))
			{
				queryable = queryable.Where(x => x.FirstName.Contains(filter.PatientName)
					|| x.LastName.Contains(filter.PatientName));
			}

			return queryable;
		}
	}
}
