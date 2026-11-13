using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CompleteAppointment
{
	public class CompleteAppointmentCommand : IRequest
	{
		public required Guid Id { get; set; }
	}
}
