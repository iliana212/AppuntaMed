using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Patients.Commands.DeletePatient
{
	public class DeletePatientCommand : IRequest
	{
		public required Guid Id { get; set; }
	}
}
