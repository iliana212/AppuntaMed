using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientDetail
{
	public record PatientDetailDTO
	(
		Guid Id,
		string FirstName,
		string LastName,
		DateTime DateOfBirth,
		string PhoneNumber,
		string Email,
		string Name
	);
}
