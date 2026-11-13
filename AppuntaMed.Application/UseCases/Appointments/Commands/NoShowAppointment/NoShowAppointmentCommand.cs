using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.NoShowAppointment
{
	public class NoShowAppointmentCommand : IRequest
	{
		public required Guid Id { get; set; }
	}
}
