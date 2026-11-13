using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientDetail
{
	public static class MapExtensions
	{
		public static PatientDetailDTO ToDTO(this Patient patient)
		{
			var dto = new PatientDetailDTO(
				patient.Id, 
				patient.FirstName, 
				patient.LastName, 
				patient.DateOfBirth,
				patient.PhoneNumber.Value, 
				patient.Email.Value, 
				Name: $"{patient.FirstName} {patient.LastName}"
			);

			return dto;
		}
	}
}
