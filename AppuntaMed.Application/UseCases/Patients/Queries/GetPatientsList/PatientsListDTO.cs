using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList
{
	public record PatientsListDTO(
		Guid Id,
		string FirstName,
		string LastName,
		DateTime DateOfBirth,
		string PhoneNumber,
		string Email
	);	
}
