using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.DeleteDoctor
{
	public class DeleteDoctorCommand : IRequest
	{
		public required Guid Id { get; set; }
	}
}
