using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList
{
	public static class MapExtensions
	{
		public static DoctorsListDTO ToDTO(this Doctor doctor)
		{
			var profession = doctor.Profession
                ?? throw new InvalidOperationException("Doctor.Profession was not loaded");

			return new DoctorsListDTO(doctor.Id, doctor.FirstName, doctor.LastName,
				doctor.ProfessionId, profession.Name, doctor.SpecialityId, doctor.Speciality?.Name,
				doctor.Email.Value, doctor.PhoneNumber.Value);
		}
	}
}
