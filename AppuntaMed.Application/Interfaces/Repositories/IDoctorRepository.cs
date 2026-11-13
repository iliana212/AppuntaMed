using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.Interfaces.Repositories
{
	public interface IDoctorRepository: IRepository<Doctor>
	{
		Task<IEnumerable<Doctor>> GetFilter(DoctorFilterDTO filter);

		Task<bool> ExistsByUserId(string userId);

		Task<Doctor?> GetByUserId(string userId);
	}
}
