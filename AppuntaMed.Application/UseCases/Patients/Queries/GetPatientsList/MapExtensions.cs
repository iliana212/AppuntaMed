using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList
{
	public static class MapExtensions
	{
		public static PatientsListDTO ToDTO(this Patient patient)
		{
			var dto = new PatientsListDTO(patient.Id, patient.FirstName, patient.LastName, 
				patient.DateOfBirth, patient.PhoneNumber.Value, patient.Email.Value);
		
			return dto;
		}
	}
}
