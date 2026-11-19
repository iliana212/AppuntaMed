using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.Interfaces.Repositories
{
	public interface IPatientRepository : IRepository<Patient>
	{
		Task<IEnumerable<Patient>> GetFilter(PatientFilterDTO filter);

		Task<int> CountFilter(PatientFilterDTO filter);

		Task<bool> ExistsByUserId(string userId);

		Task<Patient?> GetByUserId(string userId);
	}
}
