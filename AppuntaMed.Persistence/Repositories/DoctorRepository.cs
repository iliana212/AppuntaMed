using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

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

		new public async Task<Doctor?> GetById(Guid id)
		{
			return await _context.Doctors
				.Include(x => x.Profession)
				.Include(x => x.Speciality)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		new public Task Update(Doctor doctor)
		{
			_context.Entry(doctor).State = EntityState.Modified;
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<Doctor>> GetFilter(DoctorFilterDTO filter)
		{
			return await ApplyFilter(filter)
               .Include(x => x.Profession)
               .Include(x => x.Speciality)
               .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
               .Pagination(filter.Page, filter.PageSize)
               .ToListAsync();
		}

		public async Task<int> CountFilter(DoctorFilterDTO filter)
		{
			return await ApplyFilter(filter).CountAsync();
		}

		private IQueryable<Doctor> ApplyFilter(DoctorFilterDTO filter)
		{
			var queryable = _context.Doctors.AsQueryable();

			if (!string.IsNullOrWhiteSpace(filter.DoctorName))
			{
				queryable = queryable.Where(x => x.FirstName.Contains(filter.DoctorName)
					|| x.LastName.Contains(filter.DoctorName));
			}

			if (filter.ProfessionId.HasValue)
				queryable = queryable.Where(x => x.ProfessionId == filter.ProfessionId.Value);
			
			if (filter.SpecialityId.HasValue)
				queryable = queryable.Where(x => x.SpecialityId == filter.SpecialityId.Value);
			
			return queryable;
		}
	}
}
