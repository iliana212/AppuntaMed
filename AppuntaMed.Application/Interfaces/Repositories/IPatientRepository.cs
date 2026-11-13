using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using AppuntaMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Interfaces.Repositories
{
	public interface IPatientRepository : IRepository<Patient>
	{
		Task<IEnumerable<Patient>> GetFilter(PatientFilterDTO filter);

		Task<bool> ExistsByUserId(string userId);

		Task<Patient?> GetByUserId(string userId);
	}
}
