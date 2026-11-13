using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientDetail
{
	public class GetPatientDetailQuery : IRequest<PatientDetailDTO>
	{
		public Guid Id { get; set; }
	}
}
