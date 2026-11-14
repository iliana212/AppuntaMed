using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.Interfaces.Repositories
{
	public interface IProfessionRepository
	{
		Task<IEnumerable<Profession>> GetAllWithSpecialities();
		Task<Profession?> GetByIdWithSpecialities(Guid id);
	}
}
