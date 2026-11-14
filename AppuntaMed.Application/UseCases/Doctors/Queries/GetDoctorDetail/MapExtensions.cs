using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorDetail
{
	public static class MapExtensions
	{
		public static DoctorDetailDTO ToDTO(this Doctor doctor)
		{
			var profession = doctor.Profession
               ?? throw new InvalidOperationException("Doctor.Profession was not loaded");
			
			return new DoctorDetailDTO(
				doctor.Id,
				doctor.FirstName,
				doctor.LastName,
				doctor.ProfessionId,
				profession.Name,
				doctor.SpecialityId,
				doctor.Speciality?.Name,
				doctor.Email.Value,
				doctor.PhoneNumber.Value,
				$"{doctor.FirstName} {doctor.LastName}"
			);
		}
	}
}
