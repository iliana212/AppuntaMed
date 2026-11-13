using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList
{
	public record DoctorsListDTO
	(
		Guid Id,
		string FirstName,
		string LastName,
		string Profession,
		string Speciality,		
		string Email,
		string PhoneNumber
	);
}
