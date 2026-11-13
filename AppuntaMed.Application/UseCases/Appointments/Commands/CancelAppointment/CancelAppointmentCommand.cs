using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CancelAppointment
{
	public class CancelAppointmentCommand : IRequest
	{
		public required Guid Id { get; set; }
	}
}
