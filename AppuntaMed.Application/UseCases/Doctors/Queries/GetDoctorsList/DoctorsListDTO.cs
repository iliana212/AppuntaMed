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
		Guid ProfessionId,
		string Profession,
		Guid? SpecialityId,
		string? Speciality,		
		string Email,
		string PhoneNumber
	);
}
