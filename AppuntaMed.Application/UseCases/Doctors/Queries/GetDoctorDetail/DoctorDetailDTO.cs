using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorDetail
{
	public record DoctorDetailDTO
	(
		Guid Id,
		string FirstName,
		string LastName,
		Guid ProfessionId,
		string Profession,
		Guid? SpecialityId,
		string? Speciality,
		string Email,
		string PhoneNumber,
		string Name
	);
}
