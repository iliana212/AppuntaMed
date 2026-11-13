using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AppuntaMed.Persistence.Repositories
{
	public class DoctorRepository : Repository<Doctor>, IDoctorRepository
	{
		private readonly AppuntaMedDbContext _context;

		public DoctorRepository(AppuntaMedDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<bool> ExistsByUserId(string userId)
		{
			return await _context.Doctors.AnyAsync(x => x.UserId == userId);
		}

		public async Task<Doctor?> GetByUserId(string userId)
		{
			return await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == userId);
		}

		public async Task<IEnumerable<Doctor>> GetFilter(DoctorFilterDTO filter)
		{
			var queryable = _context.Doctors.AsQueryable();

			if (!string.IsNullOrWhiteSpace(filter.DoctorName))
			{
				queryable = queryable.Where(x => x.FirstName.Contains(filter.DoctorName)
					|| x.LastName.Contains(filter.DoctorName));
			}

			return await queryable.OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
				.Pagination(filter.Page, filter.PageSize)
				.ToListAsync();
		}
	}
}
